using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRInteractorSetup : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] public InputActionReference grabAction;   // e.g., "SelectGrab"

    [SerializeField] public InputActionReference pointAction;  // e.g., "PointSelect"

    // Optionally assign your hand objects (e.g., your right-hand object) here,
    // or let the script search for GameObjects tagged "Controller".
    public GameObject[] handObjects;

    private studyManager manager;

    public bool foundRightHand = false;

    public GameObject rightHand;
    public bool oneLookup = false;

    public bool EnableRightRayLineVisual = false;
    public bool EnableLeftRayLineVisual = false;
    private Transform rightRayLineVisual;
    private Transform leftRayLineVisual;
    private Transform leftHand;
    public NearFarInteractor LeftNearFarInteractor;
    public GameObject LeftAimPose;
    public GameObject RightAimPose;

    private Transform nearFar;
    private Transform pinchGrabPose;
    public NearFarInteractor RightNearFarInteractor;

    private void Start()
    {
        // Find the studyManager to get the current task type.
        manager = FindFirstObjectByType<studyManager>();
        if (manager == null)
        {
            Debug.LogError("No studyManager found in the scene. Please ensure a studyManager exists.");
            return;
        }
    }

    public void EnableBaselineInteractor(bool enable) {
        
        if (StudyConfigurationManager.Instance.UserDominantHand == StudyConfigurationManager.DominantHand.RightHand) {

            if (rightHand != null) {
                if (enable) { // do not disable the NFInteractor if false, still needed to interact with the coins.
                    RightNearFarInteractor.gameObject.SetActive(true);
                }
                EnableRightRayLineVisual = enable;
                // studyManager.Instance.GetComponent<XRInteractorSetup>().EnableBaselineRayLineVisual = enable;
                rightRayLineVisual.gameObject.SetActive(enable);
                RightNearFarInteractor.keepSelectedTargetValid = !enable;
                LeftAimPose.SetActive(!enable);
            }
        } else {
            if (leftHand != null) {

                if (enable) { // do not disable the NFInteractor if false, still needed to interact with the coins.
                    LeftNearFarInteractor.gameObject.SetActive(true);
                }
                EnableLeftRayLineVisual = enable;
                // studyManager.Instance.GetComponent<XRInteractorSetup>().EnableBaselineRayLineVisual = enable;
                leftRayLineVisual.gameObject.SetActive(enable);
                LeftNearFarInteractor.keepSelectedTargetValid = !enable;
                RightAimPose.SetActive(!enable);
            }
        }
    }

    public void EnableRightNearFarInteractor(bool enable)
    {
        if (rightHand != null) {
            RightNearFarInteractor.gameObject.SetActive(enable);
        }
    }

    public void EnableLeftNearFarInteractor(bool enable)
    {
        if (leftHand != null) {
            LeftNearFarInteractor.gameObject.SetActive(enable);
        }
    }

    public void EnablePinchGrabPose(bool enable)
    {
        if (rightHand != null) {
            pinchGrabPose.gameObject.SetActive(enable);
        }
    }

    private void Update()
    {
        //if (oneLookup)
        if (!foundRightHand)
        {
            rightHand = GameObject.Find("Right Hand");
            if (rightHand != null)
            {
                foundRightHand = true;
                Debug.Log("[XRInteractorSetup] Found Right Hand object.");
                nearFar = rightHand.transform.Find("Near-Far Interactor");
                pinchGrabPose = rightHand.transform.Find("Pinch Grab Pose");
                rightRayLineVisual = nearFar.transform.Find("LineVisual");
                RightNearFarInteractor = rightHand.GetComponentInChildren<NearFarInteractor>();
                RightAimPose = rightHand.transform.Find("Aim Pose").gameObject;
            }
        }

        if (leftHand == null)
        {
            leftHand = GameObject.Find("Left Hand")?.transform;
            if (leftHand != null)
            {
                Debug.Log("[XRInteractorSetup] Found Left Hand object.");
                leftRayLineVisual = leftHand.Find("Near-Far Interactor/LineVisual");
                LeftNearFarInteractor = leftHand.GetComponentInChildren<NearFarInteractor>();
                LeftAimPose = leftHand.Find("Aim Pose").gameObject;
            }
        }

        if (foundRightHand && manager != null)
        {
            // If the current task is ObjectCollection, disable "Near-Far Interactor"
            if (manager.currentTask == TaskType.ObjectCollection)
            {
                if (nearFar != null)
                {
                    rightRayLineVisual.gameObject.SetActive(EnableRightRayLineVisual);
                    leftRayLineVisual.gameObject.SetActive(EnableLeftRayLineVisual);
                    // Debug.Log("[XRInteractorSetup] Disabled Near-Far Interactor for ObjectCollection task.");
                }
                else
                {
                    Debug.LogWarning("[XRInteractorSetup] 'Near-Far Interactor' child not found under Right Hand.");
                }
            }
            else // If Monitoring, disable "Pinch Grab Pose" so wer remove the grabbing
            {
                if (pinchGrabPose != null)
                {
                    pinchGrabPose.gameObject.SetActive(false);
                    Debug.Log("[XRInteractorSetup] Disabled Pinch Grab Pose for Monitoring task.");
                }
                else
                {
                    Debug.LogWarning("[XRInteractorSetup] 'Pinch Grab Pose' child not found under Right Hand.");
                }
            }

            oneLookup = true;
        }
    }
}