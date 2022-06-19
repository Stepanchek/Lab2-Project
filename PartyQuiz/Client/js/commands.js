function TryJoinGame()
{
  localStorage.setItem('started_new_game', "true");

  let gameId = document.getElementById('game_id').value;
  socket.send(JSON.stringify({ 'Join': { 'name': gameId } }));
}

function ResumeGame()
{
  SendMessage({ 'ResumeGame': 'true '});
}

function LeaveGame()
{
  localStorage.setItem('state', null);
  socket.send(JSON.stringify({ 'Leave': null }));
}

function SetHostRole()
{
  SendMessage({ 'SetRole': 'host' });
}

function SetPlayerRole()
{
  SendMessage({ 'SetRole': 'player' });
}

function SetInfo()
{
  let playerName = document.getElementById('player_name').value;

  localStorage.setItem('name', playerName);

  document.getElementById('all_here_player_name').textContent = playerName;
  document.getElementById('select_question_player_group_name').textContent = playerName;

  SendMessage({ 'SetInfo':
        {
          'Name' : playerName,
          'Avatar' : currentAvatarId
        }});
}

function CallAllHere()
{
  SendMessage({ 'AllHere': 'true' });
}

function PlayEmoji(emojiType)
{
  SendMessage({ 'PlayEmoji': emojiType });
}

function CallNextRound()
{
  SendMessage({ 'NextRound': 'true' });
}

function AnswerButtonPressed()
{
  SendMessage({ 'ButtonPressed': 'answer' });
}

function ReportAnswerDecision(isCorrect)
{
  SendMessage({ 'AnswerDecision': isCorrect });
}

function SelectCatInPokeOwner(playerId)
{
  SendMessage({ 'CatInPokeOwnerSelected' : playerId })
}

function SelectQuestion(theme, question)
{
  SendMessage({ 'SelectedQuestion': { 'ThemeId' : theme, 'QuestionId' : question }});
}

function SkipSequence()
{
  SendMessage({ 'SkipSequence': 'true' });
}

function MakeABid(bid, oldValue)
{
  SendMessage({ 'MakeABid': { 'Bid': bid, 'OldValue': oldValue } });
}

function Pass()
{
  SendMessage({ 'Pass': true });
}

function EndAuction()
{
  SendMessage({ 'EndAuction': true });
}
