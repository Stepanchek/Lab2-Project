namespace PartyQuiz.Utils.Inputs
{
    public static class CommandExtensions
    {
        public static bool IsCommandDown(this ECommand command, ECommand checkCommand)
        {
            return command == (checkCommand | ECommand.Press);
        }

        public static bool IsCommandHold(this ECommand command, ECommand checkCommand)
        {
            return command == (checkCommand | ECommand.Hold);
        }

        public static bool IsCommandUp(this ECommand command, ECommand checkCommand)
        {
            return command == (checkCommand | ECommand.Release);
        }

        public static bool IsMouseCommand(this ECommand command)
        {
            return command.IsCommandDown(ECommand.Mouse0) ||
                   command.IsCommandDown(ECommand.Mouse1) ||
                   command.IsCommandDown(ECommand.Mouse2);
        }
    }
}