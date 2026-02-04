using System.Collections;
using System.Collections.Generic;
using Flexalon;
using UnityEngine;

public class WIM_Gallery_Interface : TransitionInterface
{

    [Header("WIM Interface Objects")]
    public Transform WIMTransform;
    // public float GalleryCurveLayoutScrollStart;
    

    void Awake()
    {
        // GalleryCurveLayoutScrollStart = Layout.GetComponent<FlexalonCurveLayout>().StartAt;   
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
        }
        TransitionUIManager.Instance.WIMObjects.SetActive(enable);
        TransitionUIManager.Instance.WIMObjects.transform.localScale = new Vector3(1.0582f, 1.0582f, 1.0582f);
        TransitionUIManager.Instance.WIMObjectClipperController.sphereRadius = 0.104f;
    }

    private IEnumerator ScaleOverTime(FlexalonObject flexalonObject, Vector3 targetScale, float duration)
        {
            Vector3 initialScale = flexalonObject.Scale;
            float elapsedTime = 0f;
    
            while (elapsedTime < duration)
            {
                flexalonObject.Scale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
    
            flexalonObject.Scale = targetScale;
        }
        
    public override void Update()
    {
        base.Update();
        
        positionInterface();
    }

    private void positionInterface() {
        // Move interface directly below the user's head, 0.5 meters forward in the XZ plane
        Vector3 cameraPosition = XRComponents.Instance.XRCamera.transform.position;
        Vector3 forwardXZ = new Vector3(XRComponents.Instance.XRCamera.transform.forward.x, 0, XRComponents.Instance.XRCamera.transform.forward.z).normalized;
        Vector3 targetPosition = cameraPosition + forwardXZ * 0.6f;
        Vector3 targetPositionHeadLevel = XRComponents.Instance.HeadGazeSphere.transform.position + 
                (0.8f * XRComponents.Instance.HeadGazeSphere.transform.forward);
        // XRComponents.Instance.HeadForwardDirection.transform.position;
        targetPosition = targetPositionHeadLevel;
        // targetPosition.y += 0.25f;
        targetPosition.y = cameraPosition.y - 0.5f;
        float distance = Vector3.Distance(InterfaceObject.transform.position, targetPosition);
        if (distance > 0.01f) // threshold for being close enough
        {
            InterfaceObject.transform.position = Vector3.Lerp(InterfaceObject.transform.position, targetPosition, 0.1f);
        }

        // Rotate the gallery interface to face the user
        Vector3 direction = InterfaceObject.transform.position - XRComponents.Instance.XRCamera.transform.position;
        direction.y = 0; // Keep the rotation only on the y-axis
        InterfaceObject.transform.rotation = Quaternion.LookRotation(direction);
    }

    /// <summary>
    /// Checks and updates the selected world target menu item based on the minimum Z position.
    /// If the selected item changes, it updates the selected item.
    /// </summary>
    // TODO: Optimize. This method is called every frame. Should be event-driven.
    public override void CheckSelectedMenuItem()
    {

    }

    public override void UpdateSelectedMenuItem(WorldTargetMenuItem selectedMenuItem)
    {

        base.UpdateSelectedMenuItem(selectedMenuItem);

        // LastSelectedWorldTargetMenuItem = SelectedWorldTargetMenuItem;
        Debug.Log($"New selected item: {selectedMenuItem.name}");

        if (LastSelectedWorldTargetMenuItem != null)
        {
            // Debug.Log($"last selected: {LastSelectedWorldTargetMenuItem.WorldTargetRef.Name}");
            LastSelectedWorldTargetMenuItem.WorldPreviewStencilQuad.SetActive(false);
            LastSelectedWorldTargetMenuItem.WorldPreviewImage.gameObject.SetActive(true);

            // if previously selected menu item is not the current world target, remove it from view of the portal
            if (LastSelectedWorldTargetMenuItem.WorldTargetRef != WorldTargetManager.Instance.GetCurrentWorldTarget())
                TransitionManager.Instance.SetWorldTargetToPortalOutsideAndDeactivate(LastSelectedWorldTargetMenuItem.WorldTargetRef);
        }

        SelectedWorldTargetMenuItem = selectedMenuItem;

        // if menu is not on the current world target, set the selected world target to the background for viewing through the portal
        if (SelectedWorldTargetMenuItem.WorldTargetRef != WorldTargetManager.Instance.GetCurrentWorldTarget()) {
            TransitionUIManager.Instance.ActivateWorldTargetWIM(SelectedWorldTargetMenuItem.WorldTargetRef);
        }

        LastSelectedWorldTargetMenuItem = SelectedWorldTargetMenuItem;
    }
}