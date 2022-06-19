using System.Collections.Generic;

namespace PartyQuiz.Network
{
    public struct HostTheme
    {
        public int id;
        public string theme;
        public List<HostQuestion> questions;

        public HostTheme(int id, string theme, List<HostQuestion> questions)
        {
            this.id = id;
            this.theme = theme;
            this.questions = questions;
        }
    }
}