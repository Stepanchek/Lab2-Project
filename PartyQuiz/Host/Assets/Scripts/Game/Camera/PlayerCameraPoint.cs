using System;

namespace PartyQuiz.Gameplay
{
    internal sealed class PlayerCameraPoint : CameraPoint
    {
        internal event Action<bool, bool> OnStateChanged;
        
        internal void SetState(bool value, bool playAnimation = false)
        {
            OnStateChanged?.Invoke(value, playAnimation);
            
            base.SetState(value);
        }        
    }
}