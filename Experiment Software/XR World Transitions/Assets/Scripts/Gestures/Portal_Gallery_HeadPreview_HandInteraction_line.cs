using System.Collections;
using Flexalon;
using UnityEngine;


public class Portal_Gallery_HeadPreview_HandInteraction_line : InteractionHandler
{

    // [SerializeField] private float gestureScrollSpeed = 1f;
    // private Vector3 gestureStartPositionInLocalSpace = Vector3.zero;
    // private Vector3 handPositionInLocalSpace = Vector3.zero;
    // private Vector3 prevHandPositionInLocalSpace;
    // private bool firstScrollPass;

    // [SerializeField] private float velocityLowerBound = 0f;
    // [SerializeField] private float velocityUpperBound = 20f;
    // [SerializeField] private float multiplierLowerBound = 0.1f;
    // [SerializeField] private float multiplierUpperBound = 1f;
    // private TransitionInterface interfaceRef;

    // [SerializeField] private float lerpWeight = 0.5f;
    // [SerializeField] private float scrollSpeedLowerBound = 0.15f;
    // [SerializeField] private float handYVelocityUpperBound = 0.3f;

    // float scrollInterval;

    private bool isTransitioning = false;

    // private float targetScrollPos;
    // private float scrollStartX;
    // private float scrollEndX;

    // Start is called before the first frame update
    void Start()
    {
        // Initialization code here
        
    }

    // Update is called once per frame
    void Update()
    {
        // ProcessUpdate();
    }

    // public override void InitInteractionHandler() {
    //     // interfaceRef = TransitionUIManager.Instance.CurrentTransitionInterface;

        
    // }

    public override void HandleActivation()
    {
        // Implementation for handling activation
    }

    public override void HandleNavigation()
    {
        // Implementation for handling navigation
    }

    public override void HandleSelection()
    {
        // Implementation for handling selection
    }

    bool middlePinchGestureDetected;
    Vector3 tipPosition, basePosition;

    /// <summary>
    /// Check for gestures and handle them accordingly. Will not be reached if a gesture is already in progress.
    /// </summary>
    public override void CheckForGestures()
    {
        // handle gesture progress tracking
        // if (GestureInProgress) return;
        if (StudyConfigurationManager.Instance.UserDominantHand == StudyConfigurationManager.DominantHand.LeftHand) {
            (middlePinchGestureDetected, tipPosition, basePosition) = GestureDetected("L_MiddleThumb_Pinch");
        } else {
            (middlePinchGestureDetected, tipPosition, basePosition) = GestureDetected("R_MiddleThumb_Pinch");
        }

        // middle pinch is detected:
        if (middlePinchGestureDetected)
        {
            // Debug.Log("Middle finger and thumb are touching.");
            // GestureInProgress = true;
            // StartCoroutine(DelaySetGestureProgressToComplete());

            // if menu is closed, open it.
            if (CurrentState == GestureState.MenuClose) {

                TransitionToState(GestureState.MenuOpen);

                Vector3 worldmiddleTipPose = XRComponents.Instance.XRRig.transform.TransformPoint(tipPosition);

                HandPositionInLocalSpace = XRComponents.Instance.HeadGazeSphere.transform.InverseTransformPoint(worldmiddleTipPose);

                // set first scroll pass flag because this is a new gesture
                FirstScrollPass = true; 

                // Reset cursor position when menu is opened.
                // portalInterface.CursorObject.transform.localPosition = new Vector3(scrollStartX, 0, 0);
                interfaceRef.CursorObject.transform.localPosition = Vector3.zero;

            }
            // if menu is open, handle user scrolling
            else if (CurrentState == GestureState.MenuOpen) {
                HandleUserScrolling(tipPosition);
            }
            // else if (CurrentState == GestureState.SelectMenuItemForPreview)
            //     TransitionToState(GestureState.MenuClose);
        } 
        // middle Pinch NOT detected:
        else {
            // if menu is open, we detect end of gesture
            if (CurrentState == GestureState.MenuOpen) {
                
                // Debug.Log("Pinch released");

                FirstScrollPass = true;

                float handPositionYDiff = HandPositionInLocalSpace.y - GestureStartPositionInLocalSpace.y;

                StartCoroutine(SnapToTargetScrollPos());

                // if user releases pinch below threshold, close menu
                if (handPositionYDiff < -0.1f) {
                    Debug.Log("pinch released below gesture start position. Closing menu.");
                    TransitionToState(GestureState.MenuClose);
                    XRComponents.Instance.InteractionZones.SetActive(false);
                }

                else if (handPositionYDiff > 0.1f & !TransitionManager.Instance.Transitioning) {
                    Debug.Log("pinch released above gesture start position.");
                    isTransitioning = true;

                    HandleConfirmTransitionToPreviewWorld();
                    isTransitioning = false;
                    
                    // StartCoroutine(delayCompleteTransitionToSelectedWorld());
                    
                    XRComponents.Instance.InteractionZones.SetActive(false);
                    // HandleConfirmTransitionToPreviewWorld();
                }
            }
        }
    }

    private IEnumerator delayCompleteTransitionToSelectedWorld() {
        yield return new WaitForSeconds(0.25f);
        HandleConfirmTransitionToPreviewWorld();
        isTransitioning = false;
    }

    

    

    public override void HandleSelectMenuItemForPreview()
    {
        
        if (TransitionUIManager.Instance.HoveredMenuItem != null) {

            if (TransitionUIManager.Instance.HoveredMenuItem.CompareTag("ScreenshotObject")) {
                Debug.Log("Selecting menu item for preview.");
                TransitionUserInterface.UpdateSelectedMenuItem(TransitionUIManager.Instance.HoveredMenuItem);
            }

            // clear hovered menu item
            TransitionUIManager.Instance.HoveredMenuItem = null;
        }
    }

    public override void HandleConfirmTransitionToPreviewWorld()
    {
        Debug.Log("Confirmed preview menu object");

        TransitionUserInterface.SelectedWorldTargetMenuItem.ConfirmWorldTargetMenuItem();

    }
}
