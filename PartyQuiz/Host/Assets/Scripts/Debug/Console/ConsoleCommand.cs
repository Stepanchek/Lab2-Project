using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PartyQuiz.Debug
{
    internal sealed class ConsoleCommand
    {
        private readonly Action<Match> _onMatch;

        internal string Regular { get; }
        private Regex Regex { get; }

        public ConsoleCommand(string regular, Action<Match> match)
        {
            Regular = regular.Split(' ').First();

            Regex = new Regex("^" + regular + "$");
            _onMatch = match;
        }

        public bool TryExecute(string input)
        {
            var isMatch = Regex.IsMatch(input);

            if (isMatch)
                _onMatch(Regex.Match(input));

            return isMatch;
        }
    }
}