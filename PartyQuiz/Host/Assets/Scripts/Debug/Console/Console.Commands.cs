using System.Linq;
using PartyQuiz.Gameplay;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Gameplay.Sequences;
using PartyQuiz.Gameplay.VFX;
using PartyQuiz.Utils;

namespace PartyQuiz.Debug
{
    internal sealed partial class Console
    {
        private GameController _gameController;
        private AvatarFactory _avatarFactory;
        private SequenceController _sequenceController;

        public override int Order => 1;

        private void AddCommands()
        {
            _commands.Add(new("help", _ =>
            {
                var orderedCommands = _commands.OrderBy(x => x.Regular).Reverse();

                foreach (var consoleCommand in orderedCommands)
                    AddLog(consoleCommand.Regular, EConsoleLogType.Regular);
            }));
            
            _commands.Add(new(@"addfakeplayers (\d+)", match =>
            {
                if (int.TryParse(match.Groups[1].Value, out var count) == false)
                    return;

                for (int i = 0; i < count; i++)
                {
                    var playerName = RandomIdGenerator.GenerateRandomId();
                    var playerId = RandomIdGenerator.GenerateRandomId();

                    var isHost = _gameController.Players.Any(x => x.Value.Role == ERole.Host) == false;
                    var role = isHost ? ERole.Host : ERole.Player;
                    
                    var isSelector = _gameController.Selector == null;
                    var player = new Player(playerName, 0, true, role, isSelector, playerId, _gameController);

                    var avatar = _avatarFactory.RequestRandom(player);
                    player.SetAvatar(avatar);
                    
                    _gameController.AddPlayer(player);
                }
            }));
            
            _commands.Add(new(@"questionselect (\d+) (\d+)", match =>
            {
                if (int.TryParse(match.Groups[1].Value, out var theme) == false)
                    return;
                
                if (int.TryParse(match.Groups[2].Value, out var question) == false)
                    return;

                _gameController.NotifyQuestionSelected(theme, question);
            }));
            
            _commands.Add(new(@"playeranswer (\d+)", match =>
            {
                if (int.TryParse(match.Groups[1].Value, out var playerIndex) == false)
                    return;

                var player = _gameController.Players.ElementAt(playerIndex).Value;
                player.ImitateButtonPressed();
            }));
            
            _commands.Add(new(@"setscore (\d+) (\d+)", match =>
            {
                if (int.TryParse(match.Groups[1].Value, out var playerIndex) == false)
                    return;
                
                if (int.TryParse(match.Groups[2].Value, out var score) == false)
                    return;

                var player = _gameController.Players.ElementAt(playerIndex).Value;
                player.Score = score;
            }));
            
            _commands.Add(new(@"playercorrect (\d+)", match =>
            {
                if (int.TryParse(match.Groups[1].Value, out var answer) == false)
                    return;
                
                _gameController.NotifyAnswerDecision(answer > 0);
            }));
            
            _commands.Add(new(@"nextround", _ =>
            {
                if (_gameController.CurrentRoundIndex <= 0)
                    _gameController.NotifyAllHere();
                else
                    _gameController.RequestNewRoundOrEndGame();
            }));
            
            _commands.Add(new(@"skipsq", _ =>
            {
                _sequenceController.Skip();
            }));
            
            _commands.Add(new(@"emoji (\d+) (\d+)", match =>
            {
                if (int.TryParse(match.Groups[1].Value, out var playerIndex) == false)
                    return;
                
                if (int.TryParse(match.Groups[2].Value, out var emoji) == false)
                    return;

                var player = _gameController.Players.ElementAt(playerIndex).Value;
                var emojiType = ((EPointParticleType)emoji).ToString();
                
                player.PlayEmoji(emojiType);
            }));
        }
    }
}