using UnityEngine;

namespace PartyQuiz.Utils
{
    internal sealed class FaceCamera : MonoBehaviour
    {
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            var dir = transform.position - _camera.transform.position;
            dir.y = 0;
            
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}