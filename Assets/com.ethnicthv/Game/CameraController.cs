using UnityEngine;

namespace com.ethnicthv.Game
{
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Properties")]
        [SerializeField] private float cameraDistance = -10;
        public float maxCameraDistance = -5;
        public float minCameraDistance = -20;
        
        [Header("Setup")]
        public Camera mainCamera;
        public Transform cameraRoot;
        
        [Header("Debug")]
        public  bool cameraShifted;

        public bool cameraShift
        {
            set
            {
                cameraShifted = value;
                mainCamera.transform.localPosition = new Vector3(cameraShifted ? -0.5f : 0, 0, cameraDistance);
            }
        }

        public float cameraDist
        {
            get => cameraDistance;
            set
            {
                cameraDistance = Mathf.Clamp(value, minCameraDistance, maxCameraDistance);
                mainCamera.transform.localPosition = new Vector3(cameraShifted ? -0.5f : 0, 0, cameraDistance);
            }
        }

        private void Start()
        {
            mainCamera.transform.localPosition = new Vector3(0, 0, cameraDistance);
            cameraRoot.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}