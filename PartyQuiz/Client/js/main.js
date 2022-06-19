const answeredColor = '#11aa55';

function CreateSocket()
{
  socket = new ReconnectingWebSocket("ws://partyquiz.club:3012");
  socket.debug = true;
  socket.timeoutInterval = 3000;
}

function InitPages()
{
  window.auction = {
    update(highestBidderId, highestBid, yourPoints, youPass)
    {
      this.highestBid = highestBid;
      this.youAreHighestBidder = window.playerId == highestBidderId;
      this.notEnoughPoints = yourPoints <= highestBid;
      this.youPass = youPass;
      this.canBid = !auction.youAreHighestBidder && !auction.notEnoughPoints && !auction.youPass;
    }
  };

  window.auctionPage = {
    highestBidder: document.getElementById('highest_bidder'),
    highestBid: document.getElementById('highest_bid'),
    yourBid: document.getElementById('your_bid'),
    buttonBid: document.getElementById('button_bid'),
    buttonPass: document.getElementById('button_pass'),
    youAreHighestBidder: document.getElementById('you_are_highest_bidder'),
    notEnoughPoints: document.getElementById('not_enough_points'),
    passing: document.getElementById('passing'),

    validateBid()
    {
      let bid = parseInt(this.yourBid.value);
      const min = parseInt(this.yourBid.min);
      const max = parseInt(this.yourBid.max);
  
      if (bid < min)
        this.yourBid.value = min;
      else if (bid > max)
        this.yourBid.value = max;
    },

    enableControls()
    {
      this.yourBid.disabled = false;
      this.buttonBid.disabled = false;
      this.buttonPass.disabled = false;
    },

    disableControls()
    {
      this.yourBid.disabled = true;
      this.buttonBid.disabled = true;
      this.buttonPass.disabled = true;
    },

    onBidClicked(auction)
    {
      this.validateBid();
      this.disableControls();
      MakeABid(this.yourBid.value, auction.highestBid);
    },

    onPassClicked()
    {
      this.disableControls();
      Pass();
    },

    update(auction, yourScore, highestBidderName)
    {
      this.enableControls();

      this.highestBidder.innerText = auction.youAreHighestBidder ? "YOU" : highestBidderName;
      this.highestBid.innerText = auction.highestBid;

      this.yourBid.min = auction.highestBid + 1;
      this.yourBid.max = yourScore;
      this.yourBid.value = auction.highestBid + 1;

      this.yourBid.hidden = !auction.canBid;
      this.buttonBid.hidden = !auction.canBid;
      this.buttonPass.hidden = !auction.canBid;

      this.youAreHighestBidder.hidden = !auction.youAreHighestBidder;
      this.notEnoughPoints.hidden = !auction.notEnoughPoints || auction.youPass || auction.youAreHighestBidder;
      this.passing.hidden = !auction.youPass;
    }
  };

  window.auctionHostPage = {
    buttonEndAuction: document.getElementById('button_end_auction'),

    enableControls()
    {
      this.buttonEndAuction.disabled = false;
    },

    disableControls()
    {
      this.buttonEndAuction.disabled = true;
    },

    onEndAuction()
    {
      this.disableControls();
      EndAuction();
    }
  };
}

window.onload = () =>
{
  preventSelection(document);
  localStorage.setItem('started_new_game', "false");

  InitPages();
  CreateSocket();
  InitGameIdInput();

  SendMessage = message =>
  {
    let result = JSON.stringify({ 'Message': { 'data': JSON.stringify(message) } });

    console.log("out: " + result);

    socket.send(result);
  };

  socket.onopen = () =>
  {
    console.log("Socket.onopen");

    let state = JSON.parse(localStorage.getItem('state'));

    if (state == null || state.id == null)
    {
      socket.send(JSON.stringify({ "Register": null }));
      console.log("Register new player");

      LoadEnterIdPage();
    }
    else
    {
      socket.send(JSON.stringify({ 'Login': { 'id': state.id } }));
      console.log("Login: " + state.id);

      LoadResumeGamePage();
    }

    console.log("Current state is " + localStorage.getItem('state'));
  };

  socket.onmessage = event =>
  {
    console.log("in: " + event.data);
    let response = JSON.parse(event.data);

    let role = localStorage.getItem('role');
    let isHost = role == 'host';
    let isPlayer = role == 'player';

    let gameNotFoundLabel = document.getElementById('error_label');

    if (response.MatchMakerError)
    {
      LoadEnterIdPage();
      gameNotFoundLabel.textContent = response.MatchMakerError.text;
    }
    else
      gameNotFoundLabel.textContent = "";

    if (response == 'BadRequest' || (response.MatchMakerError && response.MatchMakerError.text == 'Player with id not found'))
    {
      socket.send(JSON.stringify({'Register': null}));
    }
    else
    {
      if (response.ClientStateChanged)
      {
        let playerId = response.ClientStateChanged.player_id;
        window.playerId = playerId;

        console.log("Saving player id " + playerId);

        let gameId = response.ClientStateChanged.game_id;
        let inGame = gameId != null;

        let startedNewGame = localStorage.getItem('started_new_game');
        let hasStartedNewGame = startedNewGame == "true";

        if (inGame)
        {
          if (hasStartedNewGame){
            localStorage.setItem('state', JSON.stringify({'id': playerId}));
            LoadRoleSelectionPage();
          }
          else
            LoadResumeGamePage();
        }
        else
        {
          LoadEnterIdPage();
        }
      }
      else if (response.Message)
      {
        let message = JSON.parse(response.Message);

        if (message != null)
        {
          let command = message.Command;

          if (command == "Error")
          {
            console.error(message.Message);
            alert(message.Message);
          }
          else if (command == "SetRoleResult")
          {
            if (message.IsRoleSet)
            {
              localStorage.setItem('role', message.Role);
              LoadSetInfoPage();
            }
            else
            {
              document.getElementById('role_set_error').textContent = message.Error;
            }
          }
          else if (command == "InfoSetResult")
          {
            if (message.IsInfoSet)
            {
              if (isHost)
                LoadAllHereHostPage();
              else if (isPlayer)
                LoadAllHerePlayerPage();
            }
            else
            {
              document.getElementById('info_set_error').textContent = message.Error;
            }
          }
          else if (command == "CanAnswer")
          {
            if (isHost)
              ResumeQuestionSelectedPage();
            else if (isPlayer)
              LoadCanAnswerPlayerPage();
          }
          else if (command == "ContinueGame")
          {
            if (isHost)
              LoadSelectQuestionHostPage();
            else if (isPlayer)
              LoadSelectQuestionPlayerPage();
          }
          else if (command == "QuestionSelected")
          {
            LoadQuestionSelectedPage(message);

            document.getElementById('right_answer').textContent = message.Answer;
          }
          else if (command == "ScoreChanged")
          {
            document.getElementById('select_question_player_group_score').textContent = message.Score;
          }
          else if (command == "StartGame")
          {
            if (isHost)
            {
              LoadSelectQuestionHostPage();
              CreateQuestionsFromJson(message.Themes);
            }
            else if (isPlayer)
            {
              localStorage.setItem('name', message.Name);

              document.getElementById('select_question_player_group_score').textContent = message.Score;
              document.getElementById('select_question_player_group_name').textContent = message.Name;

              LoadSelectQuestionPlayerPage();
            }
          }
          else if (command == "PlayerStartedAnswering")
          {
            if (isHost)
              LoadPlayerAnswersHostPage(message.AnsweringPlayer);
            else if (isPlayer && message.IsYou)
              LoadYouAnswerPage();
          }
          else if (command == "PlayerStoppedAnswering")
          {
            if (isHost)
              ResumeQuestionSelectedPage();
            else if (isPlayer)
              LoadCanAnswerPlayerPage(message.TriedToAnswer);
          }
          else if (command == "StartCatInPoke")
          {
            if (isHost)
              LoadCatInPokeHostPage(message.Players);
            else if (isPlayer)
              LoadCatInPokePlayerPage();
          }
          else if (command == "StartAuction")
          {
            if (role == 'host')
              LoadAuctionHostPage();
            else if (role == 'player')
              LoadAuctionPlayerPage(message);
          }
          else if (command == "HighestBid")
          {
            if (role == 'host')
              ;
            else if (role == 'player')
              LoadAuctionPlayerPage(message);
          }
          else if (command == "AllQuestionsAnswered")
          {
            if (isHost)
              LoadAllQuestionsAnsweredHostPage();
            else if (isPlayer)
              LoadAllQuestionsAnsweredPlayerPage();
          }
          else if (command == "SequenceStarted")
          {
            if (isHost)
              LoadSequenceStartedHostPage();
            else if (isPlayer)
              LoadSequenceStartedPlayerPage();
          }
          else
          {
            console.log("Unknown command: " + message.Command)
          }
        }
        else
        {
          console.log("Unknown message: " + message)
        }
      }
    }
  }

  // ENTER ID
  function LoadEnterIdPage()
  {
    HideAll();

    document.getElementById('title').textContent = "ENTER GAME ID";
    document.getElementById('enter_id_group').hidden = false;
  }

  // RESUME GAME
  function LoadResumeGamePage()
  {
    HideAll();

    document.getElementById('title').textContent = "RESUME GAME";
    document.getElementById('resume_game_group').hidden = false;
  }

  // SET ROLE
  function LoadRoleSelectionPage()
  {
    HideAll();

    document.getElementById('title').textContent = "CHOOSE YOUR ROLE";
    document.getElementById('role_selection_group').hidden = false;
  }

  // SET INFO
  function LoadSetInfoPage()
  {
    HideAll();

    document.getElementById('title').textContent = "SET YOUR INFO";
    document.getElementById('set_info_group').hidden = false;
    document.getElementById('gender_group').hidden = false;
  }

  // ALL HERE
  // HOST
  function LoadAllHereHostPage()
  {
    HideAll();
    ShowEmojiGroup();

    document.getElementById('title').textContent = "WAITING FOR PLAYERS";
    document.getElementById('all_here_host_group').hidden = false;
  }

  // PLAYER
  function LoadAllHerePlayerPage()
  {
    HideAll();
    ShowEmojiGroup();

    document.getElementById('title').textContent = "WAITING FOR PLAYERS";
    document.getElementById('all_here_player_group').hidden = false;
  }

  // SELECT QUESTION
  // HOST
  function LoadSelectQuestionHostPage()
  {
    HideAll();
    ShowEmojiGroup();

    document.getElementById('title').textContent = "SELECT THE QUESTION";
    document.getElementById('select_question_host_group').hidden = false;
  }

  // PLAYER
  function LoadSelectQuestionPlayerPage()
  {
    HideAll();
    ShowEmojiGroup();

    document.getElementById('title').textContent = "HOST IS SELECTING THE QUESTION";
    document.getElementById('select_question_player_group').hidden = false;
  }

  // QUESTION SELECTED
  function ResumeQuestionSelectedPage()
  {
    HideAll();
    ShowEmojiGroup();

    document.getElementById('title').textContent = "QUESTION SELECTED";
    document.getElementById('question_selected_group').hidden = false;

    let role = localStorage.getItem('role');

    if (role == 'host')
      document.getElementById('right_answer_group').hidden = false;
  }

  // CAN ANSWER (BUTTON)
  function LoadCanAnswerPlayerPage(triedToAnswer)
  {
    HideAll();
    ShowEmojiGroup();

    document.getElementById('can_answer_player_group').hidden = false;

    if (triedToAnswer)
    {
      document.getElementById('title').textContent = "YOU CAN'T ANSWER NOW :(";
      document.getElementById('answer_button').hidden = true;
    }
    else
    {
      document.getElementById('title').textContent = "PRESS IF YOU KNOW THE ANSWER!";
      document.getElementById('answer_button').hidden = false;
    }
  }

  // PLAYER ANSWERS
  // HOST
  function LoadPlayerAnswersHostPage(player)
  {
    HideAll();
    ShowEmojiGroup();

    document.getElementById('title').textContent = "";

    document.getElementById('answering_player').textContent = player;
    document.getElementById('player_answers_host_group').hidden = false;
    document.getElementById('right_answer_group').hidden = false;
  }

  function LoadYouAnswerPage()
  {
    HideAll();
    ShowEmojiGroup();

    document.getElementById('title').textContent = "YOU ANSWER!";
    document.getElementById('you_answer_group').hidden = false;
  }

  // ALL QUESTIONS ANSWERED
  // HOST
  function LoadAllQuestionsAnsweredHostPage()
  {
    HideAll();
    ShowEmojiGroup();

    document.getElementById('all_questions_answered_host_group').hidden = false;
  }

  // PLAYER
  function LoadAllQuestionsAnsweredPlayerPage()
  {
    HideAll();
    ShowEmojiGroup();

    document.getElementById('all_questions_answered_player_group').hidden = false;
  }

  function LoadQuestionSelectedPage(message)
  {
    ResumeQuestionSelectedPage();
    ShowEmojiGroup();

    document.getElementById('question_selected_theme').textContent = message.Theme;
    document.getElementById('question_selected_price').textContent = message.Price;
  }

  function LoadCatInPokeHostPage(message)
  {
    HideAll();
    ShowEmojiGroup();

    CreatePlayersForCatInPoke(message);

    document.getElementById('title').textContent = "WHO GETS THE CAT?";
    document.getElementById('cat_in_poke_host_group').hidden = false;
  }

  function LoadCatInPokePlayerPage()
  {
    HideAll();
    ShowEmojiGroup();

    document.getElementById('title').textContent = "CAT IN A POKE SELECTION";
    document.getElementById('cat_in_poke_player_group').hidden = false;
  }

  // SEQUENCE STARTED
  // PLAYER
  function LoadSequenceStartedPlayerPage()
  {
    HideAll();
    ShowEmojiGroup();

    document.getElementById('title').textContent = "LOOK AT THE SCREEN";
  }

  // HOST
  function LoadSequenceStartedHostPage()
  {
    HideAll();
    ShowEmojiGroup();

    document.getElementById('title').textContent = "LOOK AT THE SCREEN";
    document.getElementById('skip_sequence_group').hidden = false;
  }

  function LoadAuctionHostPage()
  {
    HideAll();

    document.getElementById('title').textContent = "AUCTION";
    document.getElementById('auction_host_group').hidden = false;
  }

  function LoadAuctionPlayerPage(message)
  {
    HideAll();

    document.getElementById('title').textContent = "AUCTION";
    document.getElementById('auction_player_group').hidden = false;

    UpdateAuctionPlayerPage(message.BidderId, message.BidderName, message.Bid, message.YourScore, message.YouPass);
  }

  function UpdateAuctionPlayerPage(highestBidderId, highestBidderName, highestBid, yourPoints, youPass)
  {
    console.log("UpdateAuctionPlayerPage yourId=" + window.playerId + " highestBidderId=" + highestBidderId + " highestBidderName=" + highestBidderName + " highestBid=" + highestBid + " yourPoints=" + yourPoints + " youPass=" + youPass);

    window.auction.update(highestBidderId, highestBid, yourPoints, youPass);
    window.yourPoints = yourPoints;
    window.auctionPage.update(window.auction, yourPoints, highestBidderName);
  }

  function CreateQuestionsFromJson(json)
  {
    ClearQuestionTable();

    var table = document.getElementById("questions_table");

    for (var theme of json) 
    {
      var tr = document.createElement("tr");
      var th = document.createElement("th");

      var themeText = document.createTextNode(theme.theme);
      var themeId = theme.id;

      th.appendChild(themeText)
      tr.appendChild(th);

      for (var question of theme.questions)
      {
        var questionId = question.id;
        var price = document.createTextNode(question.price);

        var td = document.createElement("td");

        if (question.wasAnswered)
        {
          td.style.background = answeredColor;
        }
        else
        {
          td.onclick = function(themeId, questionId)
          {
            return function()
            {
              SelectQuestion(themeId, questionId);
  
              this.style.background = answeredColor;
              this.onclick = null;
            }
          }
          (themeId, questionId);
        }        
      
        td.appendChild(price);
        tr.appendChild(td);
      }

      table.appendChild(tr);
    }
  }
  
  function CreatePlayersForCatInPoke(players)
  {
    ClearCatInPokeTable();

    var table = document.getElementById("cat_in_poke_host_group");

    for (var player of players)
    {
      var tr = document.createElement("tr");
      var th = document.createElement("th");

      var playerName = document.createTextNode(player.name);
      var playerId = player.id;

      th.appendChild(playerName)
      tr.appendChild(th);

      tr.onclick = function(playerId)
      {
        return function()
        {
          SelectCatInPokeOwner(playerId);

          this.style.background = answeredColor;
          this.onclick = null;
        }
      }
      (playerId);

      table.appendChild(tr);
    }
  }

  function HideAll()
  {
    document.getElementById('title').textContent = "";
    document.getElementById('emoji_group').hidden = true;

    document.getElementById('enter_id_group').hidden = true;
    document.getElementById('resume_game_group').hidden = true;
    document.getElementById('role_selection_group').hidden = true;
    document.getElementById('set_info_group').hidden = true;        
    document.getElementById('all_here_host_group').hidden = true;
    document.getElementById('all_here_player_group').hidden = true;
    document.getElementById('select_question_player_group').hidden = true;
    document.getElementById('can_answer_player_group').hidden = true;
    document.getElementById('question_selected_group').hidden = true;
    document.getElementById('select_question_host_group').hidden = true;
    document.getElementById('player_answers_host_group').hidden = true;
    document.getElementById('cat_in_poke_host_group').hidden = true;
    document.getElementById('cat_in_poke_player_group').hidden = true;
    document.getElementById('auction_host_group').hidden = true;
    document.getElementById('auction_player_group').hidden = true;
    document.getElementById('all_questions_answered_host_group').hidden = true;
    document.getElementById('all_questions_answered_player_group').hidden = true;
    document.getElementById('avatar_group').hidden = true;
    document.getElementById('gender_group').hidden = true;
    document.getElementById('you_answer_group').hidden = true;
    document.getElementById('right_answer_group').hidden = true;
    document.getElementById('skip_sequence_group').hidden = true;
  }    

  function ShowEmojiGroup()
  {
    document.getElementById('emoji_group').hidden = false;
  }

  function ClearQuestionTable()
  {
    var node = document.getElementById("questions_table");
    while (node.hasChildNodes()) 
    {
        node.removeChild(node.lastChild);
    }
  }

  function ClearCatInPokeTable()
  {
    var node = document.getElementById("cat_in_poke_host_group");
    while (node.hasChildNodes())
    {
      node.removeChild(node.lastChild);
    }
  }

  function InitGameIdInput()
  {
    var input = document.getElementById("game_id");

    input.addEventListener("keyup", function(event)
    {
      if (event.keyCode === 13)
      {
        event.preventDefault();
        document.getElementById("join_game_button").click();
      }
    });
  }
}