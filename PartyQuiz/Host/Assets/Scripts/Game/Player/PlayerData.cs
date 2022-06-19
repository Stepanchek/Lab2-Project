namespace PartyQuiz.Gameplay.Players
{
    internal sealed class PlayerData
    {
        internal string Name = string.Empty;
        internal int Score;
        internal bool IsOnline;
        internal ERole Role = ERole.NotSet;
        internal bool IsSelector;

        internal string Id { get; }

        public PlayerData(string id, bool isOnline)
        {
            Id = id;
            IsOnline = isOnline;
        }

        public PlayerData(PlayerData playerData)
        {
            Id = playerData.Id;
            Name = playerData.Name;
            Score = playerData.Score;
            IsOnline = playerData.IsOnline;
            Role = playerData.Role;
            IsSelector = false;
        }

#if DEBUG
        public PlayerData(string name, int score, bool isOnline, ERole role, bool isSelector, string id)
        {
            Name = name;
            Score = score;
            IsOnline = isOnline;
            Role = role;
            IsSelector = isSelector;
            Id = id;
        }
#endif
    }
}