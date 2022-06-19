using TMPro;
using UnityEngine;

namespace PartyQuiz.Gameplay.Players
{
    [CreateAssetMenu(menuName = "Scriptable objects/New avatar data")]
    internal sealed class AvatarData : ScriptableObject
    {
        [SerializeField] internal GameObject EmojiPoint;
        [SerializeField] internal PlayerCameraPoint CameraPoint;
        [SerializeField] internal TextMeshPro SpeechBubble;

        [SerializeField] internal RuntimeAnimatorController PlayerAnimator;
        [SerializeField] internal RuntimeAnimatorController HostAnimator;

        [SerializeField] internal Color BidColor;
        [SerializeField] internal Color PassColor;
        [SerializeField] internal Color UndecidedColor;
    }
}