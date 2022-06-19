using System;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Structure.Runtime;

namespace PartyQuiz.Gameplay
{
    /// <summary>
    /// Auction handling
    /// </summary>
    public sealed partial class GameController
    {
        internal event Action<Question, Theme, Player> OnAuctionCalled;
        internal event Action<Player, int> OnBidMade;
        internal event Action<Player> OnPlayerPassesAuction;
        internal event Action<Player, int> OnAuctionDecided;
        internal event Action<Player, int> OnAuctionImpossible;
        internal event Action<Player, int> OnAuctionFinished;
        internal event Action<Player, int> OnHostEndsAuction;

        private bool _auctionInProgress;
        private int _highestBid;
        private Player _highestBidder;
        
        internal void NotifyAuction(Question question, Theme theme)
        {
            OnAuctionCalled?.Invoke(question, theme, Selector);
        }

        internal void ConfirmAuction(Question question)
        {
            _auctionInProgress = true;
            
            foreach (var player in Players.Values)
            {
                player.Bid = null;
                player.Pass = false;
            }

            _highestBid = question.Price.Value;
            _highestBidder = Selector;
            _highestBidder.Bid = question.Price.Value;

            if (!SomeoneCanBid())
            {
                //await _sequenceController.AuctionImpossible();
                OnAuctionImpossible?.Invoke(_highestBidder, _highestBid);
                return;
            }

            foreach (var (id, player) in Players)
            {
                _messageWriter.SendObjectWithTargetPlayer(id, new
                {
                    Command = "StartAuction",
                    BidderId = Selector.Id,
                    BidderName = Selector.Name,
                    Bid = question.Price.Value,
                    YourScore = player.Score,
                    YouPass = player.Pass,
                });
            }
        }

        internal void NotifyMakeABid(Player bidder, int bid, int oldValue)
        {
            if (!_auctionInProgress)
                return;

            if (bidder.Role != ERole.Player)
                _messageWriter.SendError(bidder.Id, $"You are not a player. Only players can bid.");
            else if (bidder.Pass)
                _messageWriter.SendError(bidder.Id, $"You have already passed on this auction. You can no longer bid.");
            else if (bidder == _highestBidder)
                _messageWriter.SendError(bidder.Id, $"You are already the highest bidder, you cannot bid twice in a row.");
            else if (oldValue != _highestBid)
                return;
            else if (bidder.Score < bid)
                _messageWriter.SendError(bidder.Id, $"Your bid ({bid}) is too high. You only have {bidder.Score} points.");
            else if (bid <= _highestBid)
                _messageWriter.SendError(bidder.Id, $"Your bid ({bid}) is too low. Should be at least {_highestBid + 1}.");
            else
            {
                _highestBid = bid;
                _highestBidder = bidder;
                bidder.Bid = bid;

                foreach (var (id, player) in Players)
                {
                    _messageWriter.SendObjectWithTargetPlayer(id, new
                    {
                        Command = "HighestBid",
                        BidderId = bidder.Id,
                        BidderName = bidder.Name,
                        Bid = bid,
                        YourScore = player.Score,
                        YouPass = player.Pass
                    });
                }

                OnBidMade?.Invoke(bidder, bid);

                CheckAuctionDecided();
            }
        }

        internal void NotifyAuctionPass(Player player)
        {
            if (!_auctionInProgress)
                return;

            if (player.Role != ERole.Player)
                _messageWriter.SendError(player.Id, $"You are not a player. Only players can pass.");
            else if (player.Pass)
                _messageWriter.SendError(player.Id, "You have already passed on this auction. You cannot pass twice.");
            else if (player == _highestBidder)
                _messageWriter.SendError(player.Id, "You cannot pass when you are the highest bidder.");
            else 
            {
                player.Pass = true;
                OnPlayerPassesAuction?.Invoke(player);

                CheckAuctionDecided();
            }
        }

        internal void NotifyEndAuction(Player player)
        {
            if (!_auctionInProgress)
                return;

            if (player.Role != ERole.Host)
                _messageWriter.SendError(player.Id, $"You are not a host. Only hosts can end auctions.");
            else
            {
                _auctionInProgress = false;
                OnHostEndsAuction?.Invoke(_highestBidder, _highestBid);
            }
        }

        internal void FinishAuction()
        {
            OnAuctionFinished?.Invoke(_highestBidder, _highestBid);
        }

        private void CheckAuctionDecided()
        {
            if (SomeoneCanBid())
                return;
            
            _auctionInProgress = false;
            OnAuctionDecided?.Invoke(_highestBidder, _highestBid);
        }

        private bool SomeoneCanBid()
        {
            foreach (var (_, player) in Players)
                if (player != _highestBidder && !player.Pass && player.Score > _highestBid)
                    return true;
            
            return false;
        }
    }
}