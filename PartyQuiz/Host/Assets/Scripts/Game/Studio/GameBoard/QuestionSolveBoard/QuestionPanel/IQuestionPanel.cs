namespace PartyQuiz.Gameplay.Studio.Board
{
    internal interface IQuestionPanel<in T>
    {
        bool CanProceed { get; }
        
        void Show(T value);
        void SetPaused(bool isPaused);
    }
}