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
    
    void OnEnable()
    {
        DominantHandActivation = false;
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
        TransitionUIManager.Instance.CurrentTransitionInterface.GetComponent<Palette_Interface>().MiddlePaletteMenuObject.GetComponent<PaletteMenuPreviewObject>().ResetCursor();
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
