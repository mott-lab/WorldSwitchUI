//using UnityEngine;

//public class HandTrackingSmoothing : MonoBehaviour
//{
//    [Header("Smoothing Settings")]
//    public float positionSmoothing = 10f;  // Higher values = smoother position
//    public float rotationSmoothing = 10f;  // Higher values = smoother rotation
//    public string handObjectName = "LeftHand"; // Name of the hand object to find at runtime

//    private Transform handTarget;          // The real-time tracked hand position
//    private Vector3 smoothedPosition;
//    private Quaternion smoothedRotation;

//    void Start()
//    {
//        // Initialize smoothed values to prevent snapping
//        smoothedPosition = Vector3.zero;
//        smoothedRotation = Quaternion.identity;
//    }

//    void Update()
//    {
//        // Dynamically find the hand object if it hasn't been found yet
//        if (handTarget == null)
//        {
//            GameObject handObject = GameObject.Find(handObjectName);
//            if (handObject != null)
//            {
//                handTarget = handObject.transform;
//                Debug.Log("LeftHand object found and assigned for smoothing.");
//                smoothedPosition = handTarget.position;
//                smoothedRotation = handTarget.rotation;
//            }
//            else
//            {
//                Debug.LogWarning("LeftHand object not yet created in the scene.");
//                return; // Exit the update early if the hand is not available
//            }
//        }

//        // Smooth position and rotation
//        smoothedPosition = Vector3.Lerp(smoothedPosition, handTarget.position, Time.deltaTime * positionSmoothing);
//        smoothedRotation = Quaternion.Slerp(smoothedRotation, handTarget.rotation, Time.deltaTime * rotationSmoothing);

//        // Apply smoothed position and rotation
//        transform.position = smoothedPosition;
//        transform.rotation = smoothedRotation;
//    }
//}

using UnityEngine;

public class FollowHandSmoothly : MonoBehaviour
{
    [Tooltip("Filter this Transform's position and rotation and apply it to the target")]
    [SerializeField] private Transform handToFollow;

    [Tooltip("Filter param: how much confidence in the new value? More = slower updates, but less jitter")]
    [Range(0, 1)]
    [SerializeField] private float newValConfidence = 0.3f;

    [Tooltip("Apply position and rotation smoothing?")]
    [SerializeField] private bool applyFilter = true;

    [Tooltip("Name of the hand GameObject to follow")]
    [SerializeField] private string handObjectName = "LeftHand";

    private Vector3 localOffsetInTracked, lastFilteredPosition;
    private Quaternion localQuatInTracked, lastFilteredQuaternion;

    private void Start()
    {
        // Attempt to find the hand at the start (it may not yet exist)
        FindHandObject();

        // Initialize filter values
        lastFilteredPosition = transform.position;
        lastFilteredQuaternion = transform.rotation;
    }

    private void Update()
    {
        // Dynamically find the hand if it hasn't been found yet
        if (handToFollow == null)
        {
            FindHandObject();
            if (handToFollow == null) return; // Early exit if still not found
        }

        if (applyFilter)
        {
            ApplySmoother();
        }
        else
        {
            // Directly follow the hand without smoothing
            transform.position = handToFollow.position;
            transform.rotation = handToFollow.rotation;
        }
    }

    private void FindHandObject()
    {
        GameObject handObject = GameObject.Find(handObjectName);
        if (handObject != null)
        {
            handToFollow = handObject.transform;
            Debug.Log($"Found hand object: {handObjectName}");

            // Calculate local offsets relative to the tracked hand
            localOffsetInTracked = handToFollow.InverseTransformVector(transform.position - handToFollow.position);
            localQuatInTracked = Quaternion.Inverse(handToFollow.rotation) * transform.rotation;
        }
        else
        {
            Debug.LogWarning($"Hand object '{handObjectName}' not found in the scene yet.");
        }
    }

    private void ApplySmoother()
    {
        // Smooth position
        Vector3 targetPosition = handToFollow.TransformPoint(localOffsetInTracked);
        transform.position = Vector3.Lerp(lastFilteredPosition, targetPosition, newValConfidence);
        lastFilteredPosition = transform.position;

        // Smooth rotation
        Quaternion targetRotation = handToFollow.rotation * localQuatInTracked;
        transform.rotation = Quaternion.Lerp(lastFilteredQuaternion, targetRotation, newValConfidence);
        lastFilteredQuaternion = transform.rotation;
    }
}
