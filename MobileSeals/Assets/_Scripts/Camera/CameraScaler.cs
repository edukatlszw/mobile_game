using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    private float referenceAspect = 16f / 9f;
    [SerializeField] private float referenceOrthoSize = 5f;
    [SerializeField] Camera targetCamera;
    
    private void Start()
    {
        float currentAspect = (float)Screen.width / Screen.height;
        targetCamera.orthographicSize = referenceOrthoSize * (referenceAspect / currentAspect);
    }
}