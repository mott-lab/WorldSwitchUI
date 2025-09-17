using System.Collections;
using System.Collections.Generic;
using Flexalon;
using UnityEngine;

public class Portal_Gallery_Interface_line : TransitionInterface
{
    [Header("Portal Gallery Interface Objects")]
    public WorldTargetMenuItem PortalObject;

    

    public override void PopulateLayout()
    {    
        
        
    }
        
    public override void Update()
    {
        base.Update();
        
        PositionGalleryInterfaceObject();
    }

    private void PositionGalleryInterfaceObject() {
        // Move the interface back toward the head gaze sphere smoothly
        Vector3 targetPositionHeadLevel = XRComponents.Instance.HeadGazeSphere.transform.position + 
                (0.8f * XRComponents.Instance.HeadGazeSphere.transform.forward);
        // XRComponents.Instance.HeadForwardDirection.transform.position;
        Vector3 targetPosition = targetPositionHeadLevel;
        targetPosition.y += 0.25f;

        float distance = Vector3.Distance(InterfaceObject.transform.position, targetPosition);
        if (distance > 0.01f) // threshold for being close enough
        {
            InterfaceObject.transform.position = Vector3.Lerp(InterfaceObject.transform.position, targetPosition, 0.1f);
        }
        // Rotate the gallery interface to face the user
        InterfaceObject.transform.rotation = Quaternion.LookRotation(targetPositionHeadLevel - XRComponents.Instance.XRCamera.transform.position);
    }

    /// <summary>
    /// Checks and updates the selected world target menu item based on the minimum Z position.
    /// If the selected item changes, it updates the selected item.
    /// </summary>
    // TODO: Optimize. This method is called every frame. Should be event-driven.
    public override void CheckSelectedMenuItem()
    {

    }


    public override void DisablePortalPreview() {
        PortalObject.StencilCanvas.SetActive(false);
    }

    public override void UpdateSelectedMenuItem(WorldTargetMenuItem selectedMenuItem)
    {
        base.UpdateSelectedMenuItem(selectedMenuItem);
        
        // Debug.Log($"New selected item: {selectedMenuItem.name}");

        if (LastSelectedWorldTargetMenuItem != null)
        {
            // Debug.Log($"last selected: {LastSelectedWorldTargetMenuItem.WorldTargetRef.Name}");
            LastSelectedWorldTargetMenuItem.WorldPreviewStencilQuad.SetActive(false);
            LastSelectedWorldTargetMenuItem.WorldPreviewImage.gameObject.SetActive(true);

            // if previously selected menu item is not the current world target, remove it from view of the portal
            if (LastSelectedWorldTargetMenuItem.WorldTargetRef != WorldTargetManager.Instance.GetCurrentWorldTarget()) {
                TransitionManager.Instance.SetWorldTargetToPortalOutsideAndDeactivate(LastSelectedWorldTargetMenuItem.WorldTargetRef);
            }
        }

        SelectedWorldTargetMenuItem = selectedMenuItem;

        // if menu is not on the current world target, set the selected world target to the background for viewing through the portal
        if (SelectedWorldTargetMenuItem.WorldTargetRef != WorldTargetManager.Instance.GetCurrentWorldTarget()) {
            // Debug.Log("Trying to select: " + SelectedWorldTargetMenuItem.WorldTargetRef.Name);
            PortalObject.StencilCanvas.SetActive(true);
            TransitionManager.Instance.SetWorldTargetToPortalInsideAndActivate(SelectedWorldTargetMenuItem.WorldTargetRef);
            PortalObject.MenuItemText.text = SelectedWorldTargetMenuItem.MenuItemText.text;
        } 
        // else if (SelectedWorldTargetMenuItem.WorldTargetRef == WorldTargetManager.Instance.GetCurrentWorldTarget()) {
        //     PortalObject.WorldPreviewStencilQuad.gameObject.SetActive(false);
        //     // SelectedWorldTargetMenuItem.WorldPreviewStencilQuad.SetActive(false);
        // }

        LastSelectedWorldTargetMenuItem = SelectedWorldTargetMenuItem;
    }
}