using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Hands;

public abstract class InteractionHandler : MonoBehaviour
{
    public TransitionInterface TransitionUserInterface;

    public enum GestureState
    {
        MenuOpen,
        MenuClose,
        SelectMenuItemForPreview,
        ConfirmTransitionToPreviousWorld
    }

    public VideoClip TutorialVideoClip;

    public GestureState CurrentState;
    public bool GestureInProgress = false;

    public TransitionInterface interfaceRef;

    public Vector3 GestureStartPositionInLocalSpace = Vector3.zero;
    public Vector3 HandPositionInLocalSpace = Vector3.zero;
    private Vector3 prevHandPositionInLocalSpace;
    public bool FirstScrollPass;

    public bool DominantHandActivation = true; // if true, dominant hand does activation gesture. if false, non-dominant hand does activation gesture.

    [Header("Scrolling Interface Settings")]
    [SerializeField] private float gestureScrollSpeed = 1f;
    [SerializeField] private float velocityLowerBound = 0f;
    [SerializeField] private float velocityUpperBound = 20f;
    [SerializeField] private float multiplierLowerBound = 0.1f;
    [SerializeField] private float multiplierUpperBound = 1f;
    [SerializeField] private float lerpWeight = 0.5f;
    [SerializeField] private float scrollSpeedLowerBound = 0.05f;
    [SerializeField] private float handYVelocityUpperBound = 0.3f;
    private float targetScrollPos;
    private float scrollStartX;
    private float scrollEndX;
    
    float scrollInterval;

    // Abstract methods for handling user interactions
    public abstract void HandleActivation();
    public abstract void HandleNavigation();
    public abstract void HandleSelection();

    

    public void ProcessUpdate() {
        HandleActivation();
        HandleNavigation();
        HandleSelection();

        CheckForGestures();
    }

    void OnEnable()
    {
        CurrentState = GestureState.MenuClose; // Default start state
        Debug.Log("Starting State: " + CurrentState);
        OnStateEnter(CurrentState);
    }

    public virtual void InitInteractionHandler()
    {
        interfaceRef = TransitionUIManager.Instance.CurrentTransitionInterface;
        CurrentState = GestureState.MenuClose; // Default start state
        Debug.Log("Starting State: " + CurrentState);
        OnStateEnter(CurrentState);
    }

    public void SetScrollingVariables(float velocityLowerBound, float velocityUpperBound, float multiplierLowerBound, float multiplierUpperBound, float scrollSpeedLowerBound, float handYVelocityUpperBound)
    {
        // this.gestureScrollSpeed = gestureScrollSpeed;
        this.velocityLowerBound = velocityLowerBound;
        this.velocityUpperBound = velocityUpperBound;
        this.multiplierLowerBound = multiplierLowerBound;
        this.multiplierUpperBound = multiplierUpperBound;
        // this.lerpWeight = lerpWeight;
        this.scrollSpeedLowerBound = scrollSpeedLowerBound;
        this.handYVelocityUpperBound = handYVelocityUpperBound;
    }

    /// <summary>
    /// Checks if the specified finger joints are touching or close to touching.
    /// </summary>
    /// <param name="hand">The XRHand object representing the hand.</param>
    /// <param name="tipJoint">The XRHandJointID of the fingertip joint to check.</param>
    /// <param name="baseJoint">The XRHandJointID of the base joint to check.</param>
    /// <returns>
    /// A tuple containing:
    /// - touchStatus: An integer indicating the touch status (1 for touching, 0 for close to touching, 2 for not touching, -1 for error).
    /// - tipPosition: The position of the fingertip joint.
    /// - basePosition: The position of the base joint.
    /// </returns>
    public (int touchStatus, Vector3 tipPosition, Vector3 basePosition) checkFingerTipsTouching(XRHand hand, XRHandJointID tipJoint, XRHandJointID baseJoint)
    {
        if (!hand.GetJoint(tipJoint).TryGetPose(out var tipPose) ||
            !hand.GetJoint(baseJoint).TryGetPose(out var basePos))
            return (-1, Vector3.zero, Vector3.zero);

        float distance = Vector3.Distance(tipPose.position, basePos.position);

        int touchStatus;
        if (distance < 0.03f)
        {
            touchStatus = 1; // touching
        }
        else if (distance < 0.06f)
        {
            touchStatus = 0; // close to touching, buffer zone
        }
        else
        {
            touchStatus = 2; // not touching
        }

        return (touchStatus, tipPose.position, basePos.position);
    }

    public void TransitionToState(GestureState newState)
    {
        Debug.Log($"Transitioning from {CurrentState} to {newState}");
        CurrentState = newState;
        OnStateEnter(newState);
    }

    public void OnStateEnter(GestureState state)
    {

        switch (state)
        {
            case GestureState.MenuOpen:
                HandleMenuOpen();
                break;
            case GestureState.MenuClose:
                HandleMenuClose();
                break;
            // case GestureState.SelectMenuItemForPreview:
            //     HandleSelectMenuItemForPreview();
            //     break;
            // case GestureState.ConfirmTransitionToPreviousWorld:
            //     HandleConfirmTransitionToPreviousWorld();
            //     break;
        }
    }

    public IEnumerator DelaySetGestureProgressToComplete()
    {
        yield return new WaitForSeconds(0.5f);
        GestureInProgress = false;
    }

    public void HandleMenuOpen()
    {
        if (!TransitionUserInterface.InterfaceObject.activeSelf)
        {
            Debug.Log("Menu Opened.");
            TransitionUserInterface.EnableInterfaceObject(true);
        }
    }

    public void HandleMenuClose()
    {
        if (TransitionUserInterface.InterfaceObject.activeSelf)
        {
            Debug.Log("Menu Closed.");
            TransitionUserInterface.EnableInterfaceObject(false);
        }
    }

    public abstract void HandleSelectMenuItemForPreview();

    public abstract void HandleConfirmTransitionToPreviewWorld();

    
    public (bool, Vector3 fingerPose, Vector3 thumbPose) GestureDetected(string gestureName)
    {
        // if hands not yet found, do nothing.
        if (!TransitionUIManager.Instance.InteractionManager.HandsAlive) {
            return (false, Vector3.zero, Vector3.zero);
        }

        int fingersTouchingID;
        Vector3 tipPose;
        Vector3 thumbPose;

        switch (gestureName)
        {
            case "L_MiddleThumb_Pinch":

                (fingersTouchingID, tipPose,thumbPose) = checkFingerTipsTouching(XRComponents.Instance.leftHand, XRHandJointID.MiddleTip, XRHandJointID.ThumbTip);
                return (fingersTouchingID == 1, tipPose, thumbPose);

            case "R_MiddleThumb_Pinch":
                (fingersTouchingID, tipPose,thumbPose) = checkFingerTipsTouching(XRComponents.Instance.rightHand, XRHandJointID.MiddleTip, XRHandJointID.ThumbTip);
                return (fingersTouchingID == 1, tipPose, thumbPose);

            case "L_IndexThumb_Pinch":
                (fingersTouchingID, tipPose,thumbPose) = checkFingerTipsTouching(XRComponents.Instance.leftHand, XRHandJointID.IndexTip, XRHandJointID.ThumbTip);
                return (fingersTouchingID == 1, tipPose, thumbPose);

            case "R_IndexThumb_Pinch":
               (fingersTouchingID, tipPose,thumbPose) = checkFingerTipsTouching(XRComponents.Instance.rightHand, XRHandJointID.IndexTip, XRHandJointID.ThumbTip);
                return (fingersTouchingID == 1, tipPose, thumbPose);

            default:
                return (false, Vector3.zero, Vector3.zero);
        }
        
    }

    /// <summary>
    /// Checks for activation gesture based on settings for user's dominant hand and interface's dominant hand setting.
    /// </summary>
    /// <param name="dominantHandActivation">True if the dominant hand is used for activation, false otherwise.</param>
    /// <returns>True if the activation gesture is detected, false otherwise.</returns>
    public (bool, Vector3) CheckActivationPinch(bool dominantHandActivation)
    {
        // handle gesture progress tracking
        if (GestureInProgress) return (false, Vector3.zero);

        if (StudyConfigurationManager.Instance.UserDominantHand == StudyConfigurationManager.DominantHand.RightHand)
        {
            if (dominantHandActivation)
            {
                return (GestureDetected("R_MiddleThumb_Pinch").Item1, GestureDetected("R_MiddleThumb_Pinch").Item2);
            }
            else
            {
                return (GestureDetected("L_MiddleThumb_Pinch").Item1, GestureDetected("L_MiddleThumb_Pinch").Item2);
            }
        }
        else
        {
            if (dominantHandActivation)
            {
                return (GestureDetected("L_MiddleThumb_Pinch").Item1, GestureDetected("L_MiddleThumb_Pinch").Item2);
            }
            else
            {
                return (GestureDetected("R_MiddleThumb_Pinch").Item1, GestureDetected("R_MiddleThumb_Pinch").Item2);
            }
        }
    }

    

    

    /// <summary>
    /// Checks for gestures and transitions between states based on the detected gestures.
    /// </summary>
    public void CheckForGestures()
    {
        (bool pinchDetected, Vector3 tipPosition) = CheckActivationPinch(DominantHandActivation);

        // middle pinch detected
        if (pinchDetected)
        {
            HandlePinchDetected(tipPosition);
        }
        // middle pinch NOT detected
        else
        {
            HandlePinchNotDetected();
        }
    }

    public virtual void HandlePinchDetected(Vector3 tipPosition)
    {
        Debug.Log("InteractionHandler: Pinch Detected.");
        if (CurrentState == GestureState.MenuClose)
        {
            ActivationGestureDetected();
        }
    }

    public virtual void HandlePinchNotDetected()
    {
        if (CurrentState == GestureState.MenuOpen)
        {
            DeactivationGestureDetected();
        }
    }

    public virtual void ActivationGestureDetected()
    {
        TransitionToState(GestureState.MenuOpen);
        Debug.Log("InteractionHandler: Activation Gesture Detected.");
    }

    private IEnumerator delayConfirmTransitionToPreviewWorld()
    {
        yield return new WaitForSeconds(0.5f);
        TransitionToState(GestureState.MenuClose);
    }


    public virtual void DeactivationGestureDetected()
    {
        if (TransitionUIManager.Instance.HoveredMenuItem != null)
        {
            TransitionUIManager.Instance.HoveredMenuItem.ConfirmWorldTargetMenuItem();
            // StartCoroutine(delayConfirmTransitionToPreviewWorld());
        } else {
            TransitionToState(GestureState.MenuClose); 
        }

    }

    public void HandleUserScrolling(Vector3 middleTipPose)
    {

        prevHandPositionInLocalSpace = HandPositionInLocalSpace;

        Vector3 worldmiddleTipPose = XRComponents.Instance.XRRig.transform.TransformPoint(middleTipPose);

        HandPositionInLocalSpace = XRComponents.Instance.HeadGazeSphere.transform.InverseTransformPoint(worldmiddleTipPose);

        // do another pass if this is the first scroll pass on a new gesture. Otherwise, will compute the difference in hand position between current position, and where the last gesture completed.
        if (FirstScrollPass)
        {
            GestureStartPositionInLocalSpace = HandPositionInLocalSpace;
            XRComponents.Instance.InteractionZones.SetActive(true);
            XRComponents.Instance.InteractionZones.transform.localPosition = middleTipPose; // handVisualization and InteractionZones objects are both children of XR Origin
            FirstScrollPass = false;
            return;
        }

        float handXVelocity = (HandPositionInLocalSpace.x - prevHandPositionInLocalSpace.x) / Time.deltaTime;
        float handYVelocity = (HandPositionInLocalSpace.y - prevHandPositionInLocalSpace.y) / Time.deltaTime;

        float handXVelocityAbs = Mathf.Abs(handXVelocity);
        float handYVelocityAbs = Mathf.Abs(handYVelocity);

        // if hand moves more up than left/right, do not scroll
        if (handYVelocityAbs > handXVelocityAbs || handYVelocityAbs > handYVelocityUpperBound)
        {
            return;
        }

        // Debug.Log($"handXVelocity: {handXVelocity}");
        scrollStartX = interfaceRef.CursorStartPoint.localPosition.x;
        scrollEndX = interfaceRef.CursorEndPoint.localPosition.x;
        scrollInterval = (Mathf.Abs(scrollStartX) + Mathf.Abs(scrollEndX)) / (interfaceRef.MenuItems.Count - 1.0f);

        float scrollMultiplier = Mathf.Lerp(multiplierLowerBound, multiplierUpperBound, Mathf.InverseLerp(velocityLowerBound, velocityUpperBound, handXVelocityAbs));
        scrollMultiplier = Mathf.Clamp(scrollMultiplier, multiplierLowerBound, multiplierUpperBound);

        float currentCursorX = interfaceRef.CursorObject.transform.localPosition.x;
        float diffCursorX = handXVelocity * gestureScrollSpeed * scrollMultiplier;
        float endCursorX = currentCursorX + diffCursorX;
        endCursorX = Mathf.Clamp(endCursorX, scrollStartX, scrollEndX);
        Vector3 newCursorPos = new Vector3(endCursorX, 0, 0);

        // find nearest snap point, lerp to it
        targetScrollPos = RoundToNearestInterval(endCursorX, scrollInterval, scrollStartX);
        // if hand moves more up than left/right, lerp to nearest snap point
        if (handXVelocityAbs < scrollSpeedLowerBound)
        {
            newCursorPos.x = Mathf.Lerp(endCursorX, targetScrollPos, lerpWeight);
        }

        // newCursorPos.x = Mathf.Lerp(endCursorX, targetScrollPos, lerpWeight);
        interfaceRef.CursorObject.transform.localPosition = newCursorPos;
    }

    private float RoundToNearestInterval(float value, float interval, float minValue)
    {
        float adjustedValue = value - minValue;
        float roundedValue = Mathf.Round(adjustedValue / interval) * interval;
        return roundedValue + minValue;
    }

    public IEnumerator SnapToTargetScrollPos()
    {
        // snap scroll position to nearest menu item
        float startScrollPos = interfaceRef.CursorObject.transform.localPosition.x;
        float duration = 0.25f;
        float t = 0;
        Vector3 cursorPos = interfaceRef.CursorObject.transform.localPosition;
        while (t < duration)
        {
            t += Time.deltaTime;
            //float lerpValue = Mathf.Pow(t / duration, 3);
            float lerpValue = t / duration;
            cursorPos.x = Mathf.Lerp(startScrollPos, targetScrollPos, lerpValue);
            interfaceRef.CursorObject.transform.localPosition = cursorPos;
            yield return null;
        }
    }
}