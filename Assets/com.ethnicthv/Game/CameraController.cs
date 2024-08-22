using UnityEngine;

namespace com.ethnicthv.Game
{
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Properties")]
        [SerializeField] private float cameraDistance = -10;
        [SerializeField] private int upperBoundDistance = -3;
        [SerializeField] private float maxCameraDistance = -5;
        [SerializeField] private float minCameraDistance = -20;
        
        [Header("Setup")]
        public Camera mainCamera;
        public Transform cameraRoot;
        
        [Header("Debug")]
        public bool cameraShiftedX;
        public bool cameraShiftedY;
        public bool cameraShiftedZ;

        public bool cameraShiftX
        {
            set
            {
                cameraShiftedX = value;
                cameraRoot.transform.localPosition = new Vector3(cameraShiftedX ? -0.5f : 0, cameraShiftedY ? -0.5f : 0, cameraShiftedZ ? -0.5f : 0);
            }
        }
        
        public bool cameraShiftY
        {
            set
            {
                cameraShiftedY = value;
                cameraRoot.transform.localPosition = new Vector3(cameraShiftedX ? -0.5f : 0, cameraShiftedY ? -0.5f : 0, cameraShiftedZ ? -0.5f : 0);
            }
        }
        
        public bool cameraShiftZ
        {
            set
            {
                cameraShiftedZ = value;
                cameraRoot.transform.localPosition = new Vector3(cameraShiftedX ? -0.5f : 0, cameraShiftedY ? -0.5f : 0, cameraShiftedZ ? -0.5f : 0);
            }
        }

        public float cameraDist
        {
            get => cameraDistance;
            set
            {
                cameraDistance = Mathf.Clamp(value, minCameraDistance, maxCameraDistance);
                mainCamera.transform.localPosition = new Vector3(0, 0, cameraDistance);
            }
        }

        public float maxCameraDist
        {
            get => maxCameraDistance;
            set => maxCameraDistance = Mathf.Min(value, upperBoundDistance);
        }
        
        public float minCameraDist
        {
            get => minCameraDistance;
            set => minCameraDistance = value;
        }

        private void Start()
        {
            mainCamera.transform.localPosition = new Vector3(0, 0, cameraDistance);
            cameraRoot.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}