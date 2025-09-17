using System.Collections;
using System.Collections.Generic;
using Flexalon;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Baseline_Gallery_Interface : TransitionInterface
{


    public override void PopulateLayout()
    {    
        
        
    }
        
    public override void Update()
    {
        base.Update();
        
        
    }

    private void PositionGalleryInterfaceObject() {
        // Move the interface back toward the head gaze sphere smoothly
        Vector3 targetPosition = 
                XRComponents.Instance.HeadGazeSphere.transform.position + 
                (1f * XRComponents.Instance.HeadGazeSphere.transform.forward);

        // float distance = Vector3.Distance(InterfaceObject.transform.position, targetPosition);
        // if (distance > 0.01f) // threshold for being close enough
        // {
        //     InterfaceObject.transform.position = Vector3.Lerp(InterfaceObject.transform.position, targetPosition, 0.1f);
        // }
        InterfaceObject.transform.position = targetPosition;
        // Rotate the gallery interface to face the user
        InterfaceObject.transform.rotation = Quaternion.LookRotation(InterfaceObject.transform.position - XRComponents.Instance.XRCamera.transform.position);
    }

    public override void CheckSelectedMenuItem()
    {

    }

    public override void EnableInterfaceObject(bool enable)
    {
        base.EnableInterfaceObject(enable);

        if (enable) {

            PositionGalleryInterfaceObject();
            studyManager.Instance.GetComponent<XRInteractorSetup>().EnableBaselineInteractor(true);

            // if (studyManager.Instance.GetComponent<XRInteractorSetup>().rightHand != null) {
            //     studyManager.Instance.GetComponent<XRInteractorSetup>().RightNearFarInteractor.keepSelectedTargetValid = false;
            // }

        } else {


            studyManager.Instance.GetComponent<XRInteractorSetup>().EnableBaselineInteractor(false);
            
            // studyManager.Instance.GetComponent<XRInteractorSetup>().EnableRightNearFarInteractor(false);
            // studyManager.Instance.GetComponent<XRInteractorSetup>().EnableRightRayLineVisual = false;
            // if (studyManager.Instance.GetComponent<XRInteractorSetup>().rightHand != null) {
            //     studyManager.Instance.GetComponent<XRInteractorSetup>().RightNearFarInteractor.keepSelectedTargetValid = true;
            // }

        }
    }

    // private IEnumerator delayEnableRay() {
    //     yield return new WaitForSeconds(0.05f);
        
    // }

    public override void UpdateSelectedMenuItem(WorldTargetMenuItem selectedMenuItem)
    {
        base.UpdateSelectedMenuItem(selectedMenuItem);
        
        // Debug.Log($"New selected item: {selectedMenuItem.name}");

        if (LastSelectedWorldTargetMenuItem != null)
        {
            // // Debug.Log($"last selected: {LastSelectedWorldTargetMenuItem.WorldTargetRef.Name}");
            // LastSelectedWorldTargetMenuItem.WorldPreviewStencilQuad.SetActive(false);
            // LastSelectedWorldTargetMenuItem.WorldPreviewImage.gameObject.SetActive(true);

            // if previously selected menu item is not the current world target, remove it from view of the portal
            if (LastSelectedWorldTargetMenuItem.WorldTargetRef != WorldTargetManager.Instance.GetCurrentWorldTarget()) {

                TransitionManager.Instance.SetWorldTargetToPortalOutsideAndDeactivate(LastSelectedWorldTargetMenuItem.WorldTargetRef);
            }
        }

        SelectedWorldTargetMenuItem = selectedMenuItem;

        // if menu is not on the current world target, set the selected world target to the background for viewing through the portal
        if (SelectedWorldTargetMenuItem.WorldTargetRef != WorldTargetManager.Instance.GetCurrentWorldTarget()) {
            TransitionManager.Instance.SetWorldTargetToPortalInsideAndActivate(SelectedWorldTargetMenuItem.WorldTargetRef);
            // PortalObject.MenuItemText = SelectedWorldTargetMenuItem.MenuItemText;
        }

        LastSelectedWorldTargetMenuItem = SelectedWorldTargetMenuItem;
    }
}