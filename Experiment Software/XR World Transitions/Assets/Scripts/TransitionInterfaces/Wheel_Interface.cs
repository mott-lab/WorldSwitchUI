using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

public class Wheel_Interface : TransitionInterface
{

    [Header("Wheel Interface Objects")]
    public WorldTargetMenuItem MiddlePaletteMenuObject;
    public GameObject WheelObjectsToRotate;

    private float angle;
    public float distanceMultiplier = 1f;

    public override void Update() {
        base.Update();
        
        positionInterface();
    }

    private void positionInterface() {
        if (XRComponents.Instance.wristL == null || XRComponents.Instance.wristR == null) return;

        Vector3 midPointBetweenHands = (XRComponents.Instance.wristL.transform.position + XRComponents.Instance.wristR.transform.position) / 2;

        Vector3 targetPosition = midPointBetweenHands + 
                (Vector3.up * 0.15f) + 
                (XRComponents.Instance.XRCamera.transform.forward * 0.15f);

        // Move the whole interface toward the target position smoothly
        float distance = Vector3.Distance(InterfaceObject.transform.position, targetPosition);
        if (distance > 0.01f) // threshold for being close enough
        {
            InterfaceObject.transform.position = Vector3.Lerp(InterfaceObject.transform.position, targetPosition, 0.15f);
        }

        // convert wristR position to local space from camera
        Vector3 wristRLocal = XRComponents.Instance.HeadForwardDirection.transform.InverseTransformPoint(XRComponents.Instance.wristR.transform.position);

        Vector3 wristLLocal = XRComponents.Instance.HeadForwardDirection.transform.InverseTransformPoint(XRComponents.Instance.wristL.transform.position);

        // Rotate the interface to align with user's view
        InterfaceObject.transform.rotation = XRComponents.Instance.XRCamera.transform.rotation;
        Vector3 currentRotation = InterfaceObject.transform.eulerAngles;

        Vector2 wristRXZ = new Vector2(wristRLocal.x, wristRLocal.z);
        Vector2 wristLXZ = new Vector2(wristLLocal.x, wristLLocal.z);

        angle = -1f * Vector2.SignedAngle(Vector2.up, wristRXZ - wristLXZ);
        float currentCameraRotationY = XRComponents.Instance.XRCamera.transform.eulerAngles.y;
        angle += currentCameraRotationY - 90f;
        InterfaceObject.transform.eulerAngles = new Vector3(currentRotation.x, angle, currentRotation.z);

        // convert midpoint position to local space from camera
        Vector3 midPointLocal = XRComponents.Instance.HeadForwardDirection.transform.InverseTransformPoint(midPointBetweenHands);
        // get angle between wristR and midpoint
        angle = Vector2.SignedAngle(Vector2.up, wristRLocal - midPointLocal);
        angle *= 2f;

        // Vector3 relativeWristRPos = wristRLocal - midPointLocal;
        // angle2 = Mathf.Atan2(relativeWristRPos.y, relativeWristRPos.x) * Mathf.Rad2Deg;

        Vector3 prevAngles = WheelObjectsToRotate.transform.localEulerAngles;
        // apply the angle difference between hands to the interface object's Z rotation to spin it
        WheelObjectsToRotate.transform.localEulerAngles = new Vector3(prevAngles.x, prevAngles.y, angle);

        // get distance between hands
        float distanceBetweenHands = Vector3.Distance(XRComponents.Instance.wristL.transform.position, XRComponents.Instance.wristR.transform.position);
        // scale the interface based on the distance between hands
        InterfaceObject.transform.localScale = distanceMultiplier * new Vector3(distanceBetweenHands, distanceBetweenHands, distanceBetweenHands);

        // Rotate the menu items to face up (counter the rotation of the wheel itself)
        foreach (WorldTargetMenuItem item in MenuItems)
        {
            item.transform.localEulerAngles = new Vector3(0, angle, 0);
        }
    }
    public override void PopulateLayout()
    {

    }

    public override void DisablePortalPreview() {
        MiddlePaletteMenuObject.StencilCanvas.SetActive(false);
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
        base.UpdateSelectedMenuItem(selectedMenuItem);
        
        Debug.Log($"New selected item: {selectedMenuItem.name}");

        if (LastSelectedWorldTargetMenuItem != null)
        {
            Debug.Log($"last selected: {LastSelectedWorldTargetMenuItem.WorldTargetRef.Name}");

            // if previously selected menu item is not the current world target, remove it from view of the portal
            if (LastSelectedWorldTargetMenuItem.WorldTargetRef != WorldTargetManager.Instance.GetCurrentWorldTarget())
                TransitionManager.Instance.SetWorldTargetToPortalOutsideAndDeactivate(LastSelectedWorldTargetMenuItem.WorldTargetRef);
        }

        SelectedWorldTargetMenuItem = selectedMenuItem;

        Debug.Log($"now selected: {SelectedWorldTargetMenuItem.WorldTargetRef.Name}");

        MiddlePaletteMenuObject.WorldTargetRef = SelectedWorldTargetMenuItem.WorldTargetRef;
        MiddlePaletteMenuObject.MenuItemText.text = SelectedWorldTargetMenuItem.MenuItemText.text;

        // if menu is not on the current world target, set the selected world target to the background for viewing through the portal
        if (SelectedWorldTargetMenuItem.WorldTargetRef != WorldTargetManager.Instance.GetCurrentWorldTarget()) {
            MiddlePaletteMenuObject.StencilCanvas.SetActive(true);
            TransitionManager.Instance.SetWorldTargetToPortalInsideAndActivate(SelectedWorldTargetMenuItem.WorldTargetRef);
        }

        // if menu is on the current world target, hide the stencil quad and show the preview image
        if (SelectedWorldTargetMenuItem.WorldTargetRef == WorldTargetManager.Instance.GetCurrentWorldTarget())
        {
            SelectedWorldTargetMenuItem.WorldPreviewImage.gameObject.SetActive(true);
            // SelectedWorldTargetMenuItem.WorldPreviewStencilQuad.SetActive(false);
        }

        LastSelectedWorldTargetMenuItem = SelectedWorldTargetMenuItem;
    }

}