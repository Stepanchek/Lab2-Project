namespace PartyQuiz.Gameplay.Sequences
{
    internal sealed class Sequence
    {
        internal string Text = string.Empty;
        internal object Key = null;
        internal ECameraPointType CameraPoint = ECameraPointType.None;
        internal bool StopAfterDone = true;
        internal float Delay = 0.5f;
        
        internal Sequence SetText(string text)
        {
            Text = text;
            return this;
        }
        
        internal Sequence SetKey(object key)
        {
            Key = key;
            return this;
        }
        
        internal Sequence SetCameraType(ECameraPointType cameraType)
        {
            CameraPoint = cameraType;
            return this;
        }
        
        internal Sequence SetStopAfterDone(bool stopAfterDone)
        {
            StopAfterDone = stopAfterDone;
            return this;
        }
        
        internal Sequence SetDelay(float additionalDelay)
        {
            Delay = additionalDelay;
            return this;
        }
    }
}