using System.Collections;
using UnityEngine;


public class WIM_Wheel_LRPreview_LRInteraction : InteractionHandler
{
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

    void OnEnable()
    {
        DominantHandActivation = true;
    }

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

    public override void ActivationGestureDetected()
    {
        base.ActivationGestureDetected();
        TransitionUIManager.Instance.CurrentTransitionInterface.GetComponent<WIM_Wheel_Interface>().MiddlePaletteMenuObject.GetComponent<PaletteMenuPreviewObject>().ResetCursor();
    }

    // public override void DeactivationGestureDetected()
    // {
    //     if (TransitionUIManager.Instance.HoveredMenuItem != null)
    //     {
    //         TransitionUIManager.Instance.HoveredMenuItem.ConfirmWorldTargetMenuItem();
    //         StartCoroutine(delayConfirmTransitionToPreviewWorld());
    //     }
    // }

    // bool pinchDetected;
    // /// <summary>
    // /// Check for gestures and handle them accordingly. Will not be reached if a gesture is already in progress.
    // /// </summary>
    // public override void CheckForGestures()
    // {
    //     // handle gesture progress tracking
    //     if (GestureInProgress) return;

    //     if (StudyConfigurationManager.Instance.UserDominantHand == StudyConfigurationManager.DominantHand.LeftHand) {
    //         pinchDetected = GestureDetected("L_MiddleThumb_Pinch").Item1;
    //     } else {
    //         pinchDetected = GestureDetected("R_MiddleThumb_Pinch").Item1;
    //     }

    //     // middle pinch detected
    //     if (pinchDetected)
    //     {
    //         // Debug.Log("Middle finger and thumb are touching.");

    //         if (CurrentState == GestureState.MenuClose) {
    //             TransitionToState(GestureState.MenuOpen);
    //             TransitionUIManager.Instance.CurrentTransitionInterface.GetComponent<WIM_Wheel_Interface>().MiddlePaletteMenuObject.GetComponent<PaletteMenuPreviewObject>().ResetCursor();
    //         } 
    //     }
    //     // middle pinch NOT detected
    //     else {
    //         // if menu is open, we detect end of gesture
    //         if (CurrentState == GestureState.MenuOpen) {

    //             // if right index finger is hovering over menu item, select it
    //             if (TransitionUIManager.Instance.HoveredMenuItem != null) {

    //                 TransitionUserInterface.SelectedWorldTargetMenuItem.ConfirmWorldTargetMenuItem();

    //                 StartCoroutine(delayConfirmTransitionToPreviewWorld());
    //             } else {
    //                 TransitionToState(GestureState.MenuClose); 
    //             }
    //         }
    //     }
    // }

    private IEnumerator delayConfirmTransitionToPreviewWorld()
    {
        yield return new WaitForSeconds(0.5f);
        TransitionToState(GestureState.MenuClose);
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
        Debug.Log("Selected preview menu object");

        TransitionUserInterface.SelectedWorldTargetMenuItem.ConfirmWorldTargetMenuItem();

    }
}
