#![feature(drain_filter)]
extern crate ws;
extern crate uuid;
extern crate serde_json;
#[macro_use]
extern crate serde_derive;

use std::io::{self, Write, Read};
use std::vec::Vec;
use std::collections::HashMap;
use std::cell::RefCell;
use std::borrow::BorrowMut;

#[cfg(windows)]
use std::os::windows::io::AsRawSocket;
#[cfg(unix)]
use std::os::unix::io::AsRawFd;

use uuid::Uuid;

#[derive(Deserialize)]
pub enum ClientRequest {
    Register,
    Login(PlayerLogin),
    Join(JoinGameRequest),
    Message(PlayerRequest),
    Leave,
}

#[derive(Deserialize)]
pub struct PlayerLogin {
    id: Uuid
}

#[derive(Deserialize)]
pub struct JoinGameRequest {
    name: String
}

#[derive(Deserialize)]
pub struct PlayerRequest {
    data: String
}

#[derive(Serialize)]
pub enum ClientResponse<'a> {
    BadRequest,
    MatchMakerError(MatchMakerError),
    ClientStateChanged(ClientState<'a>),
    Message(&'a str)
}

#[derive(Copy, Clone)]
pub enum ConnectionType {
    Client,
    Server
}

pub struct WebSocketHandler {
    channel_sender: std::sync::mpsc::Sender<SocketEvent>,
    socket: ws::Sender,
    connection_type: ConnectionType,
    id: usize
}

#[derive(Serialize)]
pub struct ClientState<'a> {
    player_id: Uuid,
    state: String,
    game_id: Option<& 'a str>
}

#[derive(Deserialize)]
pub enum ServerRequest {
    StartGame(String),
    EndGame(String),
    Message(ServerMessage),
    KickPlayer(Uuid)
}

#[derive(Deserialize)]
pub struct ServerMessage {
    target: MessageTarget,
    message: String
}

#[derive(Deserialize)]
pub enum MessageTarget {
    Id(Uuid),
    Ids(Vec<Uuid>),
    Broadcast
}

#[derive(Serialize)]
pub enum ServerResponse {
    BadRequest,
    MatchMakerError(MatchMakerError),
    CurrentState(Vec<PlayerState>),
    PlayerConnected(Uuid),
    PlayerDisconnected(Uuid),
    PlayerJoined(Uuid),
    PlayerLeft(Uuid),
    PlayerMessage { id: Uuid, text: String }
}

#[derive(Serialize)]
pub struct PlayerState {
    id: Uuid,
    online: bool
}

pub enum Error {
    MatchmakerError(MatchMakerError),
    WebSocketError(ws::Error),
    SerdeError(serde_json::Error),
    IOError(std::io::Error)
}

#[derive(Debug, Serialize)]
pub struct MatchMakerError {
    text: & 'static str
}

impl std::convert::From<ws::Error> for Error {
    fn from(error: ws::Error) -> Self {
        Error::WebSocketError(error)
    }
}

impl std::convert::From<serde_json::Error> for Error {
    fn from(error: serde_json::Error) -> Self {
        Error::SerdeError(error)
    }
}

impl std::convert::From<std::io::Error> for Error {
    fn from(error: std::io::Error) -> Self {
        Error::IOError(error)
    }
}

impl std::convert::From<MatchMakerError> for Error {
    fn from(error: MatchMakerError) -> Self {
        Error::MatchmakerError(error)
    }
}

impl std::convert::From<&'static str> for MatchMakerError {
    fn from(text: &'static str) -> Self { MatchMakerError { text } }
}

pub struct SocketEvent(ws::Sender, ConnectionType, WebSocketEvent, usize);

pub enum WebSocketEvent {
    Open(std::net::SocketAddr),
    Close(String),
    Message(ws::Message)
}

impl std::fmt::Debug for SocketEvent {
    fn fmt(&self, f: &mut std::fmt::Formatter) -> Result<(), std::fmt::Error> {
        match self {
            &SocketEvent(_, _, ref tcp_socket_event, _) => {
                f.write_str("SocketEvent ");
                tcp_socket_event.fmt(f);
            }
        };

        Ok(())
    }
}

impl std::fmt::Debug for WebSocketEvent {
    fn fmt(&self, f: &mut std::fmt::Formatter) -> Result<(), std::fmt::Error> {
        let m = match self {
            &WebSocketEvent::Open(_) => "Open",
            &WebSocketEvent::Close(_) => "Close",
            &WebSocketEvent::Message(_) => "Message"
        };

        f.write_str(m);
        Ok(())
    }
}

impl ws::Handler for WebSocketHandler {
    fn on_open(&mut self, shake: ws::Handshake) -> ws::Result<()> {
        self.channel_sender.send(SocketEvent(self.socket.clone(), self.connection_type, WebSocketEvent::Open(shake.peer_addr.unwrap()), self.id));
        Ok(())
    }

    fn on_message(&mut self, msg: ws::Message) -> ws::Result<()> {
        self.channel_sender.send(SocketEvent(self.socket.clone(), self.connection_type, WebSocketEvent::Message(msg), self.id));
        Ok(())
    }

    fn on_close(&mut self, _code: ws::CloseCode, reason: &str) {
        self.channel_sender.send(SocketEvent(self.socket.clone(), self.connection_type, WebSocketEvent::Close(reason.to_owned()), self.id));
    }
}

pub struct Player {
    socket: Option<ws::Sender>,
    id: Uuid
}

pub struct Game {
    socket: Option<ws::Sender>,
    id: usize,
    players: Vec<Player>,
    name: String
}

pub struct State {
    free_players: Vec<Player>,
    games: HashMap<String, Game>
}

impl State {
    fn new() -> State {
        State {
            free_players: Vec::new(),
            games: HashMap::new()
        }
    }

    fn find_player_mut(&mut self, socket: ws::Sender) -> Option<&mut Player> {
        self.all_players_mut()
            .find(|player| player.socket.as_ref() == Some(&socket))
    }

    fn find_player(&self, socket: ws::Sender) -> Option<&Player> {
        self.all_players()
            .find(|player| player.socket.as_ref() == Some(&socket))
    }

    fn find_or_create_player(&mut self, socket: ws::Sender) -> Uuid {
        let player_id = self.all_players()
            .find(|player| player.socket.as_ref() == Some(&socket))
            .map(|player| player.id);

        match player_id {
            Some(player_id) => player_id,
            None => {
                let id = Uuid::new_v4();

                self.free_players.push(Player { socket: Some(socket), id });

                id
            }
        }
    }

    fn all_players(&self) -> impl Iterator<Item = &Player> {
        (&self.free_players)
            .iter()
            .chain(self.games
                .values()
                .flat_map(|game| &game.players))
    }

    fn all_players_mut(&mut self) -> impl Iterator<Item = &mut Player> {
        (&mut self.free_players)
            .iter_mut()
            .chain(self.games
                .values_mut()
                .flat_map(|game| &mut game.players))
    }

    fn login(&mut self, id: Uuid, socket: ws::Sender) -> Result<(), Error> {
        let player = self.all_players_mut()
            .find(|player| player.id == id)
            .ok_or(Error::MatchmakerError(MatchMakerError { text: "Player with id not found" }))?;

        if let Some(ref old_socket) = player.socket {
            old_socket.close(ws::CloseCode::Normal)?;
        }

        player.socket = Some(socket);
        Ok(())
    }

//    fn find_player_by_id(&self, id: i32) -> Option<&Player> {
//        (&self.games)
//            .iter()
//            .flat_map(|game| &game.players)
//            .find(|x| x.id == id)
//    }

//    fn get_free_player(&mut self, socket: Rc<RefCell<ws::Sender>>) -> Option<Player> {
//
//    }

    fn join_player(&mut self, game_name: &str, socket: ws::Sender) -> Result<(), Error> {
        let game = self.games
            .get_mut(game_name)
            .ok_or(MatchMakerError { text: "Game not found" })?;

        let player = self.free_players
            .drain_filter(|player| player.socket.as_ref() == Some(&socket))
            .find(|_| true)
            .ok_or(MatchMakerError { text: "Player not found" })?;

        let player_id = player.id;
        game.players.push(player);

        if let Some(ref socket) = game.socket {
            socket.send(serde_json::to_string(&ServerResponse::PlayerJoined(player_id))?)?;
        }

        Ok(())
    }

    fn kick_player(&mut self, socket_id: usize, player_id: Uuid) -> Result<(), Error> {
        let game = self.games
            .iter_mut()
            .map(|(_, game)| game)
            .find(|game| game.id == socket_id)
            .ok_or(MatchMakerError { text: "not logged in" })?;

        let player = game
            .players
            .drain_filter(|player| player.id == player_id)
            .find(|player| true)
            .ok_or(MatchMakerError { text: "player not found" })?;

        let player_id = player.id;

        if let Some(ref socket) = player.socket {
            send_response(socket, ClientResponse::ClientStateChanged(ClientState {
                player_id,
                state: "leave".to_string(),
                game_id: None
            }));
        }

        self.free_players.push(player);

        if let Some(ref socket) = game.socket {
            socket.send(serde_json::to_string(&ServerResponse::PlayerLeft(player_id))?)?;
        }

        Ok(())
    }

    fn disconnect_player(&mut self, socket: ws::Sender) -> Result<(), Error> {
        let mut player_id = Uuid::new_v4();

        {
            let player = self
                .find_player_mut(socket)
                .ok_or(MatchMakerError { text: "Player not found" })?;

            player_id = player.id;

            player.socket = None;
        }

        let game = self
            .find_game_by_player_id(player_id)
            .ok_or(MatchMakerError { text: "Game not found" })?;

        if let Some(ref socket) = &game.socket {
            socket.send(serde_json::to_string(&ServerResponse::PlayerDisconnected(player_id)).unwrap())?;
        }

        Ok(())
    }
//
//    fn find_game_by_name(&mut self, game_name: &str) -> Option<&mut Game> {
//
//    }

    fn find_game_by_player_id(&self, id: Uuid) -> Option<&Game> {
        self.games
            .values()
            .find(|game| game.players
                .iter()
                .find(|player| player.id == id)
                .is_some())
    }
}

fn send_response(socket: &ws::Sender, response: ClientResponse) -> Result<(), Error> {
    socket.send(serde_json::to_string(&response).unwrap()).map_err(Error::WebSocketError)
}

fn send_message<'a, I>(message: String, players: I)
    where I: Iterator<Item = Option<&'a Player>>
{
    for result in players
        .map(|player| {
            player
                .ok_or(MatchMakerError { text: "player not found" })?
                .socket
                .as_ref()
                .ok_or(MatchMakerError { text: "player is offline" })?
                .send(serde_json::to_string(&ClientResponse::Message(&message))?)?;

            Ok::<(), Error>(())
        }) {
        match result {
            Err(Error::MatchmakerError(error)) => println!("{}", error.text),
            Err(Error::WebSocketError(error)) => println!("{}", error.to_string()),
            _ => ()
        }
    }
}

fn main() {
    let mut state = State::new();
    let (event_sender, event_receiver) = std::sync::mpsc::channel::<SocketEvent>();

    let mut socket_id = std::sync::Arc::new(std::sync::atomic::AtomicUsize::new(0));

    let event_sender_clone = event_sender.clone();
    let socket_id_clone = socket_id.clone();

    std::thread::spawn(move || {
        println!("Listening for clients on ws:[::]:3012");

        ws::listen(":::3012", |socket_sender| WebSocketHandler {
            channel_sender: event_sender_clone.clone(),
            socket: socket_sender,
            connection_type: ConnectionType::Client,
            id: socket_id_clone.fetch_add(1, std::sync::atomic::Ordering::SeqCst)
        }).unwrap();
    });

    let event_sender_clone = event_sender.clone();
    let socket_id_clone = socket_id.clone();

    std::thread::spawn(move || {
        println!("Listening for servers on ws:[::]:3013");

        ws::listen("0.0.0.0:3013", |socket_sender| WebSocketHandler {
            channel_sender: event_sender_clone.clone(),
            socket: socket_sender,
            connection_type: ConnectionType::Server,
            id: socket_id_clone.fetch_add(1, std::sync::atomic::Ordering::SeqCst)
        }).unwrap();
    });

    loop {
        match event_receiver.recv() {
            Ok(event) => {
                println!("Event received: {:?}", event);

                match event {
                    SocketEvent(socket, ConnectionType::Client, event, socket_id) => {
                        match match || -> Result<(), Error> {
                            match event {
                                WebSocketEvent::Open(address) => {
                                    println!("A new client socket connection from {} with id {}", address, socket_id);
                                    Ok(())
                                },
                                WebSocketEvent::Close(reason) => {
                                    state.disconnect_player(socket.clone());

                                    println!("Client connection closed. Reason: {}", reason);
                                    Ok(())
                                },
                                WebSocketEvent::Message(client_message) => {
                                    let client_message_str = client_message.as_text()?;
                                    println!("Client message {}", client_message_str);
                                    let client_message = serde_json::from_str(client_message_str)?;

                                    let socket = socket.clone();

                                    match client_message {
                                        ClientRequest::Register => {
                                            let player_id = state.find_or_create_player(socket.clone());
                                            let game = state.find_game_by_player_id(player_id);

                                            if let Some(game) = game {
                                                if let Some(ref socket) = &game.socket {
                                                    socket.send(serde_json::to_string(&ServerResponse::PlayerConnected(player_id)).unwrap());
                                                }
                                            }

                                            send_response(&socket, ClientResponse::ClientStateChanged(ClientState {
                                                player_id,
                                                state: "register".to_string(),
                                                game_id: game
                                                    .map(|game| game.name.as_ref())
                                            }))
                                        },
                                        ClientRequest::Login(login) => {
                                            let player_id = login.id;

                                            state.login(player_id, socket.clone())?;

                                            let game = state.find_game_by_player_id(player_id);

                                            if let Some(game) = game {
                                                if let Some(ref socket) = &game.socket {
                                                    socket.send(serde_json::to_string(&ServerResponse::PlayerConnected(player_id)).unwrap());
                                                }
                                            }

                                            send_response(&socket, ClientResponse::ClientStateChanged(ClientState {
                                                player_id,
                                                state: "login".to_string(),
                                                game_id: game
                                                    .map(|game| game.name.as_ref())
                                            }))
                                        },
                                        ClientRequest::Join(join) => {
                                            state.join_player(join.name.as_str(), socket.clone())?;

                                            let player_id = state.find_player(socket.clone()).ok_or(Error::MatchmakerError(MatchMakerError { text: "Player not found" }))?.id;

                                            send_response(&socket, ClientResponse::ClientStateChanged(ClientState {
                                                player_id,
                                                state: "join".to_string(),
                                                game_id: state
                                                    .find_game_by_player_id(player_id)
                                                    .map(|game| game.name.as_ref())
                                            }))
                                        },
                                        ClientRequest::Message(message) => {
                                            let player = state.find_player(socket)
                                                .ok_or(MatchMakerError { text: "Player not found" })?;

                                            let game = state.find_game_by_player_id(player.id)
                                                .ok_or(MatchMakerError { text: "Game not found" })?;

                                            let json = serde_json::to_string(&ServerResponse::PlayerMessage {
                                                id: player.id,
                                                text: message.data
                                            })?;

                                            if let Some(ref socket) = game.socket {
                                                socket.send(json.as_bytes())?;
                                            }

                                            Ok(())
                                        },
                                        ClientRequest::Leave => {
                                            let player_id = state.find_player(socket)
                                                .ok_or(MatchMakerError { text: "Player not found" })?
                                                .id;

                                            let game_id = state.find_game_by_player_id(player_id)
                                                .ok_or(MatchMakerError { text: "Game not found" })?
                                                .id;

                                            state.kick_player(game_id, player_id);

                                            Ok(())
                                        }
                                    }
                                }
                            }
                        }() {
                            Err(Error::WebSocketError(web_socket_error)) => Err(Error::WebSocketError(web_socket_error)),
                            Err(Error::MatchmakerError(matchmaker_error)) => {
                                send_response(&socket, ClientResponse::MatchMakerError(matchmaker_error))
                            },
                            Err(Error::SerdeError(serde_error)) => {
                                println!("Error: {:?}", serde_error);
                                send_response(&socket, ClientResponse::BadRequest)
                            },
                            _ => Ok(())
                        } {
                            Err(Error::WebSocketError(error)) => println!("{}", error.to_string()),
                            _ => ()
                        }
                    },
                    SocketEvent(socket, ConnectionType::Server, event, socket_id) =>
                        match || -> Result<(), Error> {
                            match event {
                                WebSocketEvent::Open(address) => {
                                    println!("A new server socket connection from {} with id {}", address, socket_id);
                                    Ok(())
                                },
                                WebSocketEvent::Close(reason) => {
                                    println!("Server connection closed. Reason: {}", reason);
                                    Ok(())
                                },
                                WebSocketEvent::Message(server_message) => {
                                    let server_message_str = server_message.as_text()?;
                                    println!("Server message {}", server_message_str);
                                    let server_message = serde_json::from_str::<ServerRequest>(server_message_str)?;

                                    match server_message {
                                        ServerRequest::StartGame(game_id) => {
                                            let existing_game = state.games
                                                .entry(game_id.clone())
                                                .and_modify(|existing_game| {
                                                    existing_game.socket = Some(socket.clone());
                                                    existing_game.id = socket_id;
                                                })
                                                .or_insert(Game {
                                                    socket: Some(socket.clone()),
                                                    id: socket_id,
                                                    players: Vec::new(),
                                                    name: game_id
                                                });

                                            let ids = existing_game
                                                .players.iter().map(|player| PlayerState {
                                                    id: player.id,
                                                    online: player.socket.is_some()
                                                })
                                                .collect();

                                            socket.send(serde_json::to_string(&ServerResponse::CurrentState(ids))?)?;

                                            Ok(())
                                        },
                                        ServerRequest::EndGame(game_id) => {
                                            let game = state.games
                                                .remove(&game_id)
                                                .ok_or(MatchMakerError { text: "Game not found" })?;
                                            
                                            for player in game.players {
                                                if let Some(socket) = player.socket {
                                                    socket.close_with_reason(ws::CloseCode::Normal, "Host has ended the game")?
                                                };
                                            }

                                            Ok(())
                                        },
                                        ServerRequest::Message(ServerMessage { target, message }) => {
                                            let players = &state.games.values()
                                                .find(|game| game.id == socket_id)
                                                .ok_or(MatchMakerError { text: "not logged in" })?
                                                .players;

                                            match target {
                                                MessageTarget::Id(id) => send_message(message, std::iter::once(players.iter().find(|player| player.id == id))),
                                                MessageTarget::Ids(ids) => send_message(message, ids.into_iter().map(|id| players.iter().find(|player| player.id == id))),
                                                MessageTarget::Broadcast => send_message(message, players.iter().map(Some))
                                            };
                                            Ok(())
                                        },
                                        ServerRequest::KickPlayer(player_id) => {
                                            state.kick_player(socket_id, player_id)
                                        }
                                    }
                                },
                            }
                        }() {
                            Err(Error::MatchmakerError(matchmaker_error)) => println!("Oops error {}", matchmaker_error.text),
                            Err(Error::SerdeError(serde_error)) => println!("Deserialization error {}", serde_error.to_string()),
                            _ => {}
                        }
                }
            },
            Err(error) => { let _ = io::stderr().write(error.to_string().as_bytes()); }
        }
    }
}