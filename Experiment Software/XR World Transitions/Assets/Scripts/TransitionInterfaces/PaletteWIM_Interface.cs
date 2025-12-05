using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

public class PaletteWIM_Interface : TransitionInterface
{

    [Header("Palette Interface Objects")]
    public WorldTargetMenuItem MiddlePaletteMenuObject;
    public GameObject PaletteObjectsParent;
    private bool positionPaletteOnRightHand;

    [Header("Palette WIM Interface Objects")]
    public Transform WIMTransform;

    public override void Update() {
        base.Update();
        
        positionInterface();

        
    }

    private void positionInterface() {

        
        // Move the interface toward the left hand smoothly
        Vector3 targetPosition;
        GameObject activeHand;

        if (positionPaletteOnRightHand)
        {
            if (TransitionUIManager.Instance.InteractionManager.XRInteractorSetup.CurrentInteractionMode == XRInteractorSetup.InteractionMode.Hands)
            {
                if (XRComponents.Instance.wristR == null) return;
                activeHand = XRComponents.Instance.wristR;
                PaletteObjectsParent.transform.localPosition = new Vector3(-0.216f, -0.247f, 0.2f);
                PaletteObjectsParent.transform.localEulerAngles = new Vector3(10f, 32f, 90f);
            }
            else
            {
                activeHand = XRComponents.Instance.RightController;
                PaletteObjectsParent.transform.localPosition = new Vector3(-0.2782f, -.0095f, -0.0045f);
                PaletteObjectsParent.transform.localEulerAngles = new Vector3(-5.387f, 10f, 34f);
            }
            
        }
        else
        {
            if (TransitionUIManager.Instance.InteractionManager.XRInteractorSetup.CurrentInteractionMode == XRInteractorSetup.InteractionMode.Hands)
            {
                if (XRComponents.Instance.wristL == null) return;
                activeHand = XRComponents.Instance.wristL;
                PaletteObjectsParent.transform.localPosition = new Vector3(0.216f, -0.247f, 0.2f);
                PaletteObjectsParent.transform.localEulerAngles = new Vector3(10f, -32f, -90f);
            }
            else
            {
                activeHand = XRComponents.Instance.LeftController;
                PaletteObjectsParent.transform.localPosition = new Vector3(0.2782f, -.0095f, -0.0045f);
                PaletteObjectsParent.transform.localEulerAngles = new Vector3(-5.387f, -10f, -34f);
            }
            
        }

        targetPosition = activeHand.transform.position;

        float distance = Vector3.Distance(InterfaceObject.transform.position, targetPosition);
        if (distance > 0.005f) // threshold for being close enough
        {
            InterfaceObject.transform.position = Vector3.Lerp(InterfaceObject.transform.position, targetPosition, 0.5f);
        }
        // Rotate the interface to align with the user's wrist
        float angleDifference = Quaternion.Angle(InterfaceObject.transform.rotation, activeHand.transform.rotation); 
        if (angleDifference > 0.5f) // threshold for being close enough
        {
            InterfaceObject.transform.rotation = Quaternion.Slerp(InterfaceObject.transform.rotation, activeHand.transform.rotation, 0.1f);
        }
    }

    public override void PopulateLayout()
    {

    }

    public override void EnableInterfaceObject(bool enable)
    {
        base.EnableInterfaceObject(enable);
        if (enable) {
            TransitionUIManager.Instance.WIMObjects.transform.position = WIMTransform.position;
            TransitionUIManager.Instance.WIMObjects.transform.rotation = WIMTransform.rotation;
            TransitionUIManager.Instance.WIMObjects.transform.parent = WIMTransform;

            TransitionUIManager.Instance.TransitionCursorController.GetComponent<MeshRenderer>().enabled = true;

            // TODO: Consider putting this in an INIT function.
            bool isHeadHandInteraction = TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction is Palette_HandPreview_HeadHandInteraction || TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction is PaletteWIM_HandPreview_HeadHandInteraction;
        
            if (isHeadHandInteraction) {
                if (StudyConfigurationManager.Instance.UserDominantHand == StudyConfigurationManager.DominantHand.LeftHand) {
                    positionPaletteOnRightHand = false;
                } else {
                    positionPaletteOnRightHand = true;
                }
            } else {
                if (StudyConfigurationManager.Instance.UserDominantHand == StudyConfigurationManager.DominantHand.LeftHand) {
                    positionPaletteOnRightHand = true;
                } else {
                    positionPaletteOnRightHand = false;
                }
            }

        }
        else {
            TransitionUIManager.Instance.TransitionCursorController.GetComponent<MeshRenderer>().enabled = false;
        }
        TransitionUIManager.Instance.WIMObjects.SetActive(enable);
    }

    public override void CheckSelectedMenuItem()
    {
        // Implement logic to check the selected menu item
        // Example: Check if a menu item is clicked or selected
    }

    // public override void UpdateSelectedMenuItem()
    // {
    //     // Implement logic to update the selected menu item
    //     // Example: Update the UI or perform actions based on the selected item
    // }

    public override void UpdateSelectedMenuItem(WorldTargetMenuItem selectedMenuItem)
    {
        // Debug.Log($"New selected item: {selectedMenuItem.name}");
        base.UpdateSelectedMenuItem(selectedMenuItem);

        if (LastSelectedWorldTargetMenuItem != null)
        {
            // Debug.Log($"last selected: {LastSelectedWorldTargetMenuItem.WorldTargetRef.Name}");

            // if previously selected menu item is not the current world target, remove it from view of the portal
            if (LastSelectedWorldTargetMenuItem.WorldTargetRef != WorldTargetManager.Instance.GetCurrentWorldTarget()) {
                TransitionManager.Instance.SetWorldTargetToPortalOutsideAndDeactivate(LastSelectedWorldTargetMenuItem.WorldTargetRef);
            }
        }

        SelectedWorldTargetMenuItem = selectedMenuItem;

        Debug.Log($"user selected: {SelectedWorldTargetMenuItem.WorldTargetRef.Name}");

        MiddlePaletteMenuObject.WorldTargetRef = SelectedWorldTargetMenuItem.WorldTargetRef;
        MiddlePaletteMenuObject.MenuItemText.text = SelectedWorldTargetMenuItem.MenuItemText.text;

        // if menu IS NOT on the current world target, set the selected world target to the background for viewing through the portal
        if (SelectedWorldTargetMenuItem.WorldTargetRef != WorldTargetManager.Instance.GetCurrentWorldTarget()) {
            Debug.Log("New world target selected. Activating its WIM.");
            // TransitionManager.Instance.SetWorldTargetToPortalInsideAndActivate(SelectedWorldTargetMenuItem.WorldTargetRef);
            TransitionUIManager.Instance.ActivateWorldTargetWIM(SelectedWorldTargetMenuItem.WorldTargetRef);
        }

        // if menu IS on the current world target, hide the stencil quad and show the preview image
        if (SelectedWorldTargetMenuItem.WorldTargetRef == WorldTargetManager.Instance.GetCurrentWorldTarget())
        {
            Debug.Log("Selected world target is the current world target. Showing preview image and hiding stencil quad.");
            SelectedWorldTargetMenuItem.WorldPreviewImage.gameObject.SetActive(true);
            // SelectedWorldTargetMenuItem.WorldPreviewStencilQuad.SetActive(false);
        }

        LastSelectedWorldTargetMenuItem = SelectedWorldTargetMenuItem;
    }

}