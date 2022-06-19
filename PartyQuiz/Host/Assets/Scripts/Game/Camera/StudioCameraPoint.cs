using System;

namespace PartyQuiz.Gameplay
{
    internal sealed class StudioCameraPoint : CameraPoint
    {
        internal event Action<bool> OnStateChanged;

        internal new void SetState(bool value)
        {
            OnStateChanged?.Invoke(value);
            
            base.SetState(value);
        }
    }
}