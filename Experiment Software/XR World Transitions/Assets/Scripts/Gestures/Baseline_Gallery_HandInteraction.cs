using System.Collections;
using Flexalon;
using UnityEngine;
using UnityEngine.XR.Hands;

public class Baseline_Gallery_HandInteraction : InteractionHandler
{

    // Start is called before the first frame update
    void Start()
    {
        // Initialization code here
        
    }

    void OnEnable()
    {
        DominantHandActivation = true;
    }

    // Update is called once per frame
    void Update()
    {
        // ProcessUpdate();
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

    // bool pinchDetected;
    /// <summary>
    /// Check for gestures and handle them accordingly. Will not be reached if a gesture is already in progress.
    /// </summary>
    // public override void CheckForGestures()
    // {
    //     // // // handle gesture progress tracking
    //     // // if (GestureInProgress) return;

    //     // pinchDetected = CheckActivationPinch(true);

    //     // // middle pinch detected
    //     // if (pinchDetected)
    //     // {
    //     //     if (CurrentState == GestureState.MenuClose) {
    //     //         TransitionToState(GestureState.MenuOpen);
    //     //     } 
    //     // }
    //     // // middle pinch NOT detected
    //     // else {
    //     //     // if menu is open, we detect end of gesture
    //     //     if (CurrentState == GestureState.MenuOpen) {

    //     //         if (TransitionUIManager.Instance.HoveredMenuItem != null) {
    //     //             TransitionUIManager.Instance.HoveredMenuItem.ConfirmWorldTargetMenuItem();
    //     //         }
    //     //         TransitionToState(GestureState.MenuClose); 
    //     //     }
    //     // }
    // }


    public override void HandleSelectMenuItemForPreview()
    {

    }

    public override void HandleConfirmTransitionToPreviewWorld()
    {


    }
}