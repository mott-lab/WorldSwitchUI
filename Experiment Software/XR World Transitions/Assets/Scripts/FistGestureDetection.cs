using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;
using UnityEngine.UIElements;
using TMPro;
using System.ComponentModel;

public class FistGestureDetection : MonoBehaviour
{
    private XRHandSubsystem handSubsystem;
    private List<XRHandSubsystem> handSubsystems = new List<XRHandSubsystem>();
    private GameObject leftHandObject;
    private bool handFound = false;
    private bool wasFist = false;
    private int fistCount = 0;

    [SerializeField]
    private GameObject[] destinationCameras;

    [SerializeField]
    private GameObject xrOrigin;

    [SerializeField]
    private Material defaultHandMaterial;

    [SerializeField]
    private Material[] handMaterials; // rendered texture materials from all worlds that we have

    public bool teleported = false;

    private int currentPreviewIndex = -1; // first no preview

    private Vector3 lastHandPosition;

    void Start()
    {

        teleported = false;
        if (destinationCameras.Length == 0)
        {
            Debug.LogError("Please assign destination camera in inspector!");
            return;
        }

        if (xrOrigin == null)
        {
            xrOrigin = GameObject.Find("XR Origin");
            if (xrOrigin == null)
            {
                Debug.LogError("Please assign XR Origin in inspector!");
                return;
            }
        }

        SubsystemManager.GetSubsystems(handSubsystems);
        if (handSubsystems.Count > 0)
        {
            handSubsystem = handSubsystems[0];
        }
    }

    bool once = false;
    void Update()
    {
        if (teleported) return;

        if (once == false)
        {
            once = true;
            for (int i = 0; i < destinationCameras.Length; i++)
            {
                print("Destination Camera: " + destinationCameras[i]);
                print("Hand Material: " + handMaterials[i]);
            }
        }

        if (!handFound)
        {
            leftHandObject = GameObject.Find("LeftHand");
            GameObject wristL = GameObject.Find("L_Wrist");
            if (leftHandObject != null)
            {
                handFound = true;
                lastHandPosition = leftHandObject.transform.position;
                Debug.Log("Found LeftHand GameObject and recoreded position");
            }
            return;
        }
        else
        {
            GameObject wristL = GameObject.Find("L_Wrist");
            lastHandPosition = leftHandObject.transform.position;
        }

        if (handSubsystem == null || !handSubsystem.running || !leftHandObject.activeInHierarchy) return;

        XRHand leftHand = handSubsystem.leftHand;
        if (!leftHand.isTracked) return;

        bool changedPreview = DetectSlideGesture(leftHand);
        if (changedPreview == true)
        {
            fistCount = 0;
        }

        bool isFist = CheckForFist(leftHand);

        print("Is Fist: " + isFist);

        if (wasFist && !isFist)
        {
            fistCount++;
            Debug.Log($"Fist Count: {fistCount}");

            // Check if we've reached 2 fists
            if (fistCount == 2 && teleported == false)
            {
                Teleport();
                fistCount = 0;
            }
        }

        wasFist = isFist;
    }


    bool wasDetected = false;

    private bool DetectSlideGesture(XRHand leftHand)
    {
        if (wasDetected && !CheckFingerCurled(leftHand, XRHandJointID.IndexTip, XRHandJointID.IndexProximal) &&
            !CheckFingerCurled(leftHand, XRHandJointID.RingTip, XRHandJointID.RingProximal) &&
            !CheckFingerCurled(leftHand, XRHandJointID.LittleTip, XRHandJointID.LittleProximal))
        {
            wasDetected = false;
        }

        bool isIndexCurled = CheckFingerCurled(leftHand, XRHandJointID.IndexTip, XRHandJointID.IndexProximal);
        bool isRingAndPinkyCurled = CheckFingerCurled(leftHand, XRHandJointID.RingTip, XRHandJointID.RingProximal) &&
                                   CheckFingerCurled(leftHand, XRHandJointID.LittleTip, XRHandJointID.LittleProximal);

        // Only process if we haven't detected a gesture yet
        if (!wasDetected)
        {
            if (isIndexCurled && !isRingAndPinkyCurled) // Added check to prevent conflicts
            {
                Debug.Log("Index curled - displaying previous environment");
                PreviousPreview();
                wasDetected = true;
                return true;
            }
            else if (isRingAndPinkyCurled && !isIndexCurled) // Added check to prevent conflicts
            {
                Debug.Log("Ring and Pinky curled - displaying next environment");
                NextPreview();
                wasDetected = true;
                return true;
            }
        }

        return wasDetected;
    }
    private bool CheckForFist(XRHand hand)
    {
        bool allFingersCurled =
            CheckFingerCurled(hand, XRHandJointID.IndexTip, XRHandJointID.IndexProximal) &&
            CheckFingerCurled(hand, XRHandJointID.MiddleTip, XRHandJointID.MiddleProximal) &&
            CheckFingerCurled(hand, XRHandJointID.RingTip, XRHandJointID.RingProximal) &&
            CheckFingerCurled(hand, XRHandJointID.LittleTip, XRHandJointID.LittleProximal);
        // apparently including the thumb screws up the detection of the fist gesture
        //&&
        //CheckFingerCurled(hand, XRHandJointID.ThumbTip, XRHandJointID.ThumbProximal);

        if (allFingersCurled)
        {
            Debug.Log("Fist detected - all fingers are curled");
        }

        return allFingersCurled;
    }

    private bool CheckFingerCurled(XRHand hand, XRHandJointID tipJoint, XRHandJointID baseJoint)
    {
        if (!hand.GetJoint(tipJoint).TryGetPose(out var tipPose) ||
            !hand.GetJoint(baseJoint).TryGetPose(out var basePos))
            return false;

        float distance = Vector3.Distance(tipPose.position, basePos.position);
        bool isCurled = distance < 0.06f; 

        if (tipJoint == XRHandJointID.IndexTip && isCurled)
        {
            Debug.Log($"Index finger curl detected. Distance: {distance}");
        }

        return isCurled;
    }

    private void NextPreview()
    {
        currentPreviewIndex++;
        if (currentPreviewIndex >= destinationCameras.Length)
        {
            currentPreviewIndex -= 1;
            AssignHandMaterial(handMaterials[currentPreviewIndex]);
            Debug.Log("No more environments to preview");
        }
        else
        {
            AssignHandMaterial(handMaterials[currentPreviewIndex]);
            Debug.Log($"Previewing the next environment {destinationCameras[currentPreviewIndex].name}");
        }
    }

    private void PreviousPreview()
    {
        currentPreviewIndex--;
        if (currentPreviewIndex < 0)
        {
            currentPreviewIndex = 0;
            AssignHandMaterial(handMaterials[0]);
            Debug.Log("no more previous environments.");
        }
        else
        {
            AssignHandMaterial(handMaterials[currentPreviewIndex]);
            Debug.Log($"Previewing environment {destinationCameras[currentPreviewIndex].name}");
        }
    }

    public Material handMaskMaterial;
    private void AssignHandMaterial(Material material)
    {
        Renderer targetRenderer = leftHandObject.GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            targetRenderer.materials = new Material[] { handMaskMaterial, handMaterials[currentPreviewIndex] };
        }
    }


    private void Teleport()
    {
        if (teleported) return; // the second teleported bool needs to be removed dependent on user study setup
        print("###### Teleporting ######");
        GameObject destinationCamera = destinationCameras[currentPreviewIndex];

        if (destinationCamera == null)
        {
            Debug.LogError("Destination Camera at the current index is not assigned -> " + currentPreviewIndex);
            return;
        }

        //Vector3 targetPosition = destinationCamera.transform.position;
        if (destinationCamera != null)
        {

            Vector3 userPosition = xrOrigin.transform.position;
            Vector3 targetPosition = destinationCamera.transform.position;
            Vector3 offset = new Vector3(
            userPosition.x - targetPosition.x, // Horizontal X offset
                0,                                // Ignore Y to avoid floating
                userPosition.z - targetPosition.z // Horizontal Z offset
            );

            // find the parent of the destination camera
            GameObject destinationWorld = destinationCamera.transform.parent.gameObject;
            destinationWorld.transform.position += offset;

            // adjust the height based on the main camera position
            GameObject mainCamera = GameObject.Find("Main Camera");
            if (mainCamera == null)
            {
                Debug.LogError("Main camera not found...");
                return;
            }
            float cameraToFloorOffset = Mathf.Abs(mainCamera.transform.position.y - destinationWorld.transform.position.y);
            destinationWorld.transform.position = new Vector3(
                destinationWorld.transform.position.x,
                destinationWorld.transform.position.y + cameraToFloorOffset,
                destinationWorld.transform.position.z
            );
            xrOrigin.transform.position = new Vector3(
                xrOrigin.transform.position.x,
                targetPosition.y, // Set XR Origin's Y position to the ground level of the destination
                xrOrigin.transform.position.z
            );

            // Align XR Origin's Y-axis rotation to match wherever we teleporting
            Vector3 targetEulerAngles = new Vector3(
                0,
                destinationCamera.transform.eulerAngles.y,
                0
            );
            xrOrigin.transform.rotation = Quaternion.Euler(targetEulerAngles);

            GameObject originWorld = GameObject.Find("City Environment"); // I have a hierarchy idea to use later, but for testing I hardcoded for now as the starting point needs to change
            if (originWorld != null && destinationCameras[currentPreviewIndex].transform.parent.name != "City Environment")
            {
                originWorld.SetActive(false);
            }


            Renderer targetRenderer = null;
            string targetObjectName = "LeftHand";
            GameObject targetObject = GameObject.Find(targetObjectName);
            if (targetObject != null)
            {
                Debug.Log("Found " + targetObjectName + " in the scene.");
                targetRenderer = targetObject.GetComponent<Renderer>();
                if (targetRenderer == null)
                {
                    Debug.LogWarning("Renderer component not found on " + targetObjectName);
                }
            }

            targetRenderer.material = new Material(defaultHandMaterial);
            Debug.Log("Default material assigned to the hand.");

            teleported = true;
        }
        else
        {
            Debug.LogError("Destination Camera is not assigned.");
        }
    }

    void OnGUI()
    {
        string parentName = "None";

        if (currentPreviewIndex >= 0 && currentPreviewIndex < destinationCameras.Length)
        {
            GameObject parent = destinationCameras[currentPreviewIndex].transform.parent?.gameObject;
            if (parent != null)
            {
                parentName = parent.name;
            }
        }

        GUI.Label(new Rect(10, 10, 300, 20), $"Previewing Environment: {parentName}");
        GUI.Label(new Rect(10, 40, 200, 20), $"Fist Count: {fistCount}");
    }

    public int GetCurrentPreviewIndex()
    {
        return currentPreviewIndex;
    }
}