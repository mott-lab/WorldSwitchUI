using UnityEngine;

public class CameraSync : MonoBehaviour
{
    [SerializeField] private Camera cameraToSyncTo;
    private Camera thisCamera;

    void Awake()
    {
        thisCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        thisCamera.fieldOfView = cameraToSyncTo.fieldOfView;
        thisCamera.transform.position = cameraToSyncTo.transform.position;
        thisCamera.transform.rotation = cameraToSyncTo.transform.rotation;
    }
}
