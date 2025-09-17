using System.Collections;
using UnityEngine;


public class PaletteWIM_HandPreview_HandInteraction : InteractionHandler
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

    public override void InitInteractionHandler()
    {
        
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

    bool pinchDetected;
    /// <summary>
    /// Check for gestures and handle them accordingly. Will not be reached if a gesture is already in progress.
    /// </summary>
    public override void CheckForGestures()
    {
  
        if (GestureInProgress) return;

        if (StudyConfigurationManager.Instance.UserDominantHand == StudyConfigurationManager.DominantHand.RightHand) {
            pinchDetected = GestureDetected("L_MiddleThumb_Pinch").Item1;
        } else {
            pinchDetected = GestureDetected("R_MiddleThumb_Pinch").Item1;
        }

        // middle pinch detected
        if (pinchDetected)
        {
            // Debug.Log("Middle finger and thumb are touching.");

            if (CurrentState == GestureState.MenuClose) {
                TransitionToState(GestureState.MenuOpen);
                // TransitionUIManager.Instance.CurrentTransitionInterface.GetComponent<Palette_Interface>().MiddlePaletteMenuObject.GetComponent<PaletteMenuPreviewObject>().ResetCursor();
            } 
        }
        // middle pinch NOT detected
        else {
            // if menu is open, we detect end of gesture
            if (CurrentState == GestureState.MenuOpen) {

                // if right index finger is hovering over menu item, select it
                if (TransitionUIManager.Instance.HoveredMenuItem != null) {
                    
                    TransitionUserInterface.SelectedWorldTargetMenuItem.ConfirmWorldTargetMenuItem();

                    StartCoroutine(delayConfirmTransitionToPreviewWorld());
                } else {
                    TransitionToState(GestureState.MenuClose); 
                }
            }
        }
    }

    private IEnumerator delayConfirmTransitionToPreviewWorld()
    {
        yield return new WaitForSeconds(0.5f);
        TransitionToState(GestureState.MenuClose);
    }

    public override void HandleSelectMenuItemForPreview()
    {
        
        if (TransitionUIManager.Instance.HoveredMenuItem != null) {
            // if hovered menu item is the middle palette menu object, then select it
            if (TransitionUIManager.Instance.HoveredMenuItem.CompareTag("PreviewObject") & TransitionUserInterface.SelectedWorldTargetMenuItem != null) {

                Debug.Log("Selected preview menu object");

                TransitionUserInterface.SelectedWorldTargetMenuItem.ConfirmWorldTargetMenuItem();
                
            }
            // if hovered menu item is a screenshot object, then update the selected menu item
            else if (TransitionUIManager.Instance.HoveredMenuItem.CompareTag("ScreenshotObject")) {
                Debug.Log("Selecting menu item for preview.");
                TransitionUserInterface.UpdateSelectedMenuItem(TransitionUIManager.Instance.HoveredMenuItem);
            }

            // clear hovered menu item
            TransitionUIManager.Instance.HoveredMenuItem = null;
        }
    }

    public override void HandleConfirmTransitionToPreviewWorld()
    {

    }
}
