using Cinemachine;
using UnityEngine;

namespace PartyQuiz.Gameplay
{
    internal abstract class CameraPoint : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _camera;

        protected void SetState(bool value)
        {
            _camera.gameObject.SetActive(value);
        }
    }
}