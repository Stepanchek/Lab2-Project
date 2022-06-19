using DG.Tweening;
using UnityEngine;

namespace PartyQuiz.Utils
{
    public static class CanvasGroupExtensions
    {
        /// <summary>
        /// Sets to half of the full alpha (0.5f)
        /// </summary>
        public static void SetUnlockStatus(this CanvasGroup group, bool value, bool setRaycast = true, float time = 0.1f)
        {
            group.DOFade(value ? 1 : .5f, time);
            group.interactable = value;

            if (setRaycast)
                group.blocksRaycasts = value;
        }

        /// <summary>
        /// Completely disables/enables the alpha
        /// </summary>
        public static Tweener SetActiveStatus(this CanvasGroup group, bool value, float time = 0.1f)
        {
            var fadeValue = value ? 1 : 0f;
            return Fade(group, fadeValue, time);
        }

        public static Tweener Fade(this CanvasGroup group, float endValue, float time = 0.1f)
        {
            return group.DOFade(endValue, time);
        }
    }
}