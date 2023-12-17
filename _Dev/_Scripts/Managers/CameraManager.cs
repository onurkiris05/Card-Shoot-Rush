using Cinemachine;
using UnityEngine;

namespace Game.Managers
{
    public class CameraManager : StaticInstance<CameraManager>
    {
        [Header("Components")]
        [SerializeField] private CinemachineVirtualCamera runningCam;
        // [SerializeField] private CinemachineVirtualCamera closeLookUpCam;

        public void SetCamera(CameraType state)
        {
            runningCam.Priority = state == CameraType.Running ? 10 : 0;
            // closeLookUpCam.Priority = state == CameraType.CloseLookUp ? 10 : 0;
        }
    }

    public enum CameraType
    {
        Running,
        // CloseLookUp,
    }
}