using System.Collections.Generic;

namespace PartyQuiz.Utils.Inputs
{
    internal sealed class InputTranslator
    {
        internal void Translate(IInputNode inputNode, List<ECommand> commands)
        {
            var j = 0;

            while (j < commands.Count)
            {
                var translateResult = inputNode.ReceiveCommand(commands[j]);

                if (translateResult == EInputDispatchResult.Block)
                {
                    commands.RemoveAt(j);
                }
                else if (translateResult == EInputDispatchResult.BlockAll)
                {
                    commands.Clear();
                    break;
                }
                else
                {
                    j++;
                }
            }
        }
    }
}