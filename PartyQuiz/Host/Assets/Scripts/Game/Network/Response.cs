namespace PartyQuiz.Network
{
    internal sealed class Message
    {
        public string id = null;
        public string text = null;
    }

    internal sealed class PlayerMessage
    {
        public SetInfoResponse SetInfo = null;
        public string ButtonPressed = null;
        public string SetRole = null;
        public string NextRound = null;
        public string ResumeGame = null;
        public string AnswerDecision = null;
        public SelectedQuestion SelectedQuestion = null;
        public string CatInPokeOwnerSelected = null;
        public string AllHere = null;
        public string PlayEmoji = null;
        public string SkipSequence = null;
        public MakeABid MakeABid = null;
        public bool Pass = false;
        public bool EndAuction = false;
    }

    internal sealed class PlayerState
    {
        public string id = null;
        public bool online = false;
    }

    internal sealed class Response
    {
        public string PlayerJoined = null;
        public string PlayerLeft = null;
        public string PlayerConnected = null;
        public string PlayerDisconnected = null;
        public Message PlayerMessage = null;
        public PlayerState[] CurrentState = null;
    }

    internal sealed class SelectedQuestion
    {
        public int ThemeId;
        public int QuestionId;
    }

    internal sealed class SetInfoResponse
    {
        public string Name;
        public string Avatar;
    }

    internal sealed class MakeABid
    {
        public int Bid;
        public int OldValue;
    }
}
