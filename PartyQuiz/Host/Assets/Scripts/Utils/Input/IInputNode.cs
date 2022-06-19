using System.Collections.Generic;

namespace PartyQuiz.Utils.Inputs
{
    public interface IInputNode
    {
        int Order { get; }
        
        void ReceiveInput(List<ECommand> commands);
        EInputDispatchResult ReceiveCommand(ECommand command);
    }
}