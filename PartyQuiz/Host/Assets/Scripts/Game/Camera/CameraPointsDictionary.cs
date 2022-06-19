using System;
using PartyQuiz.Utils;

namespace PartyQuiz.Gameplay
{
    [Serializable]
    internal sealed class CameraPointsDictionary : SerializedDictionary<ECameraPointType, StudioCameraPoint>
    {
    }
}