using Cysharp.Threading.Tasks;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;

namespace PartyQuiz.Gameplay.Sequences
{
    public sealed partial class SequenceController
    {
        private bool _wasAuctionPlayed;

        private void SubscribeAuction()
        {
            _gameController.OnAuctionCalled += AuctionCalledHandler;
            DC.AddDisposable(() => _gameController.OnAuctionCalled -= AuctionCalledHandler);

            _gameController.OnBidMade += OnBidMadeHandler;
            DC.AddDisposable(() => _gameController.OnBidMade -= OnBidMadeHandler);

            _gameController.OnPlayerPassesAuction += OnPlayerPassesAuctionHandler;
            DC.AddDisposable(() => _gameController.OnPlayerPassesAuction -= OnPlayerPassesAuctionHandler);

            _gameController.OnAuctionDecided += OnAuctionDecidedHandler;
            DC.AddDisposable(() => _gameController.OnAuctionDecided -= OnAuctionDecidedHandler);

            _gameController.OnAuctionImpossible += OnAuctionImpossibleHandler;
            DC.AddDisposable(() => _gameController.OnAuctionImpossible -= OnAuctionImpossibleHandler);

            _gameController.OnHostEndsAuction += OnHostEndsAuctionHandler;
            DC.AddDisposable(() => _gameController.OnHostEndsAuction -= OnHostEndsAuctionHandler);

            _gameController.OnAuctionFinished += OnAuctionFinishedHandler;
            DC.AddDisposable(() => _gameController.OnAuctionFinished -= OnAuctionFinishedHandler);
        }
        
        private async void AuctionCalledHandler(Question question, Theme theme, Player selector)
        {
            _gameController.SendSequenceStartedCommand();

            await Play(AuctionSequence);

            _gameController.ConfirmAuction(question);
            _wasAuctionPlayed = true;
        }

        private async UniTask AuctionSequence()
        {
            if (_wasAuctionPlayed == false)
            {
                var introduction = new Sequence().SetText("This is an auction question").SetCameraType(ECameraPointType.HostCloseUp);
                await Run(introduction, _hostSpeechBubble, _textReader);

                var explanation = new Sequence().SetText("You'll have to make bids to determine who will answer the question").SetCameraType(ECameraPointType.HostCloseUp);
                await Run(explanation, _hostSpeechBubble, _textReader);

                var overall = new Sequence().SetText("You can either pass or make a higher bid if you have enough points").SetCameraType(ECameraPointType.Overall);
                await Run(overall, _hostSpeechBubble, _textReader);

                var decision = new Sequence().SetText("Are you ready?").SetCameraType(ECameraPointType.CatInPoke).SetStopAfterDone(false);
                await Run(decision, _hostSpeechBubble, _textReader);
            }
            else
            {
                var explanation = new Sequence().SetText("Oh, another auction!").SetCameraType(ECameraPointType.HostCloseUp);
                await Run(explanation, _hostSpeechBubble, _textReader);

                var decision = new Sequence().SetText("You know the drill").SetCameraType(ECameraPointType.CatInPoke).SetStopAfterDone(false);
                await Run(decision, _hostSpeechBubble, _textReader);
            }
        }

        private void OnBidMadeHandler(Player player, int price)
        {
            var sequence = new Sequence().SetText($"{price}");
            Run(sequence, _hostSpeechBubble, _textReader).HandleExceptions();
        }

        private void OnPlayerPassesAuctionHandler(Player player)
        {
            var sequence = new Sequence().SetText($"{player.Name} passes");
            Run(sequence, _hostSpeechBubble, _textReader).HandleExceptions();
        }

        private async void OnAuctionDecidedHandler(Player highestBidder, int bid)
        {
            _gameController.SendSequenceStartedCommand();
            
            var noMoreBids = new Sequence().SetText("No more bids").SetCameraType(ECameraPointType.HostCloseUp);
            await Run(noMoreBids, _hostSpeechBubble, _textReader);

            var bidder = new Sequence().SetText($"{highestBidder.Name} will answer the question for {bid} points!").SetCameraType(ECameraPointType.QuestionCloseUp);
            await Run(bidder, _hostSpeechBubble, _textReader);
            
            _gameController.FinishAuction();
        }

        private async void OnAuctionImpossibleHandler(Player highestBidder, int bid)
        {
            _gameController.SendSequenceStartedCommand();
            
            var noPoints = new Sequence().SetText("No one has enough points for the auction, so we'll skip it.").SetCameraType(ECameraPointType.HostCloseUp);
            await Run(noPoints, _hostSpeechBubble, _textReader);

            var whoWillAnswer = new Sequence().SetText($"{highestBidder.Name} will answer the question for {bid} points!").SetCameraType(ECameraPointType.QuestionCloseUp);
            await Run(whoWillAnswer, _hostSpeechBubble, _textReader);

            _gameController.FinishAuction();
        }

        private async void OnHostEndsAuctionHandler(Player highestBidder, int bid)
        {
            _gameController.SendSequenceStartedCommand();

            var bullshit = new Sequence().SetText("I'm done with your bullshit").SetCameraType(ECameraPointType.HostCloseUp);
            await Run(bullshit, _hostSpeechBubble, _textReader);

            var decision = new Sequence().SetText($"{highestBidder.Name} will answer").SetCameraType(ECameraPointType.QuestionCloseUp).SetStopAfterDone(false);
            await Run(decision, _hostSpeechBubble, _textReader);

            _gameController.FinishAuction();
        }

        private void OnAuctionFinishedHandler(Player highestBidder, int bid)
        {
            _hostSpeechBubble.Stop();
        }
    }
}