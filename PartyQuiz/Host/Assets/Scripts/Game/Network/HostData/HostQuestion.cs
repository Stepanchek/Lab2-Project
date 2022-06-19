namespace PartyQuiz.Network
{
    public struct HostQuestion
    {
        public int id;
        public int price;
        public bool wasAnswered;

        public HostQuestion(int id, int price, bool wasAnswered)
        {
            this.id = id;
            this.price = price;
            this.wasAnswered = wasAnswered;
        }
    }
}