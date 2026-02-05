using UnityEngine;

public class FollowOriginalCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject originalCamera;
    public GameObject destinationCamera;

    private Vector3 lastOriginalPosition;
    private Quaternion lastOriginalRotation;

    void Start()
    {
        if (originalCamera == null)
        {
            Debug.LogError("Original camera reference is missing! Please assign it in the inspector.");
            enabled = false;
            return;
        }

        // Store initial positions and rotations
        lastOriginalPosition = originalCamera.transform.position;
        lastOriginalRotation = originalCamera.transform.rotation;
    }

    void Update()
    {
        // Calculate the change in position and rotation since last frame
        Vector3 positionDelta = originalCamera.transform.position - lastOriginalPosition;
        Quaternion rotationDelta = originalCamera.transform.rotation * Quaternion.Inverse(lastOriginalRotation);

        // Apply the same change to this camera
        destinationCamera.transform.position += positionDelta;
        destinationCamera.transform.rotation = rotationDelta * transform.rotation;

        // Update last known positions and rotations for next frame
        lastOriginalPosition = originalCamera.transform.position;
        lastOriginalRotation = originalCamera.transform.rotation;
    }
}
