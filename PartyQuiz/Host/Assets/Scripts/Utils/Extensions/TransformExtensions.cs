using UnityEngine;

namespace PartyQuiz.Utils
{
    public static class TransformExtensions
    {
        public static void SetParentAndReset(this Transform transform, Transform parent, 
            bool resetPosition = true, bool resetRotation = true)
        {
            transform.SetParent(parent);
            
            if (resetPosition)
                transform.localPosition = Vector3.zero;
            
            if (resetRotation)
                transform.localRotation = Quaternion.identity;
        }
    }
}