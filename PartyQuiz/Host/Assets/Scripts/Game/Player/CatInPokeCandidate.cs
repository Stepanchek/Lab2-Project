namespace PartyQuiz.Gameplay.Players
{
    internal struct CatInPokeCandidate
    {
        public string id;
        public string name;

        internal CatInPokeCandidate(Player player)
        {
            id = player.Id;
            name = player.Name;
        }
    }
}