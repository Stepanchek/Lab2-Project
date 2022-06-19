using System.Collections.Generic;

namespace PartyQuiz.Utils.Inputs
{
    public abstract class UIInputNode : UIElement, IInputNode
    {
        private readonly InputTranslator _inputTranslator = new();

        //the order of polling key input in InputDispatcher
        public abstract int Order { get; }

        private void Awake()
        {
            InputDispatcher.Register(this);
            DC.AddDisposable(() => InputDispatcher.Unregister(this));
        }

        public void ReceiveInput(List<ECommand> commands)
        {
            _inputTranslator.Translate(this, commands);
        }

        public abstract EInputDispatchResult ReceiveCommand(ECommand command);
    }
}