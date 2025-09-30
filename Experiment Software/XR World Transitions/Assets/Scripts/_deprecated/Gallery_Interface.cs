using System;
using System.Collections;
using System.Collections.Generic;
using Flexalon;
using UnityEngine;

[Obsolete]
public class Gallery_Interface : TransitionInterface
{

    public override void PopulateLayout()
    {    
        
        
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
        
        // Move the interface back toward the head gaze sphere smoothly
        Vector3 targetPosition = XRComponents.Instance.HeadGazeSphere.transform.position + 
                 (XRComponents.Instance.HeadGazeSphere.transform.forward * -0.5f);
        float distance = Vector3.Distance(InterfaceObject.transform.position, targetPosition);
        if (distance > 0.01f) // threshold for being close enough
        {
            InterfaceObject.transform.position = Vector3.Lerp(InterfaceObject.transform.position, targetPosition, 0.1f);
        }
        // Rotate the gallery interface to face the user
        InterfaceObject.transform.rotation = Quaternion.LookRotation(InterfaceObject.transform.position - XRComponents.Instance.XRCamera.transform.position);
    }

    /// <summary>
    /// Checks and updates the selected world target menu item based on the minimum Z position.
    /// If the selected item changes, it updates the selected item.
    /// </summary>
    // TODO: Optimize. This method is called every frame. Should be event-driven.
    public override void CheckSelectedMenuItem()
    {
        float maxZ = float.MinValue;

        foreach (WorldTargetMenuItem item in MenuItems)
        {
            if (item.transform.localPosition.z > maxZ)
            {
                maxZ = item.transform.localPosition.z;
                SelectedWorldTargetMenuItem = item;
            }
        }
        if (SelectedWorldTargetMenuItem != LastSelectedWorldTargetMenuItem)
        {
            UpdateSelectedMenuItem(SelectedWorldTargetMenuItem);
        }
    }

    /// <summary>
    /// Updates the selected world target menu item, scales it up, and manages the visibility of the preview image and stencil quad.
    /// Also handles the transition of the world target to the portal.
    /// </summary>
    // public override void UpdateSelectedMenuItem()
    // {
       
    // }

    public override void UpdateSelectedMenuItem(WorldTargetMenuItem selectedMenuItem)
    {
        base.UpdateSelectedMenuItem(selectedMenuItem);
        
         Debug.Log($"New selected item: {SelectedWorldTargetMenuItem.name}");

        if (LastSelectedWorldTargetMenuItem != null)
        {
            Debug.Log($"last selected: {LastSelectedWorldTargetMenuItem.WorldTargetRef.Name}");
            // LastSelectedWorldTargetMenuItem.transform.localScale = new Vector3(1f, 1f, 1f);
            StartCoroutine(ScaleOverTime(LastSelectedWorldTargetMenuItem.GetComponent<FlexalonObject>(), new Vector3(1f, 1f, 1f), 0.25f));
            LastSelectedWorldTargetMenuItem.WorldPreviewStencilQuad.SetActive(false);
            LastSelectedWorldTargetMenuItem.WorldPreviewImage.gameObject.SetActive(true);

            // if previously selected menu item is not the current world target, remove it from view of the portal
            if (LastSelectedWorldTargetMenuItem.WorldTargetRef != WorldTargetManager.Instance.GetCurrentWorldTarget())
                TransitionManager.Instance.SetWorldTargetToPortalOutsideAndDeactivate(LastSelectedWorldTargetMenuItem.WorldTargetRef);
        }

        Debug.Log($"now selected: {SelectedWorldTargetMenuItem.WorldTargetRef.Name}");
        // SelectedWorldTargetMenuItem.GetComponent<FlexalonObject>().Scale = new Vector3(1.5f, 1.5f, 1.5f);
        StartCoroutine(ScaleOverTime(SelectedWorldTargetMenuItem.GetComponent<FlexalonObject>(), new Vector3(1.5f, 1.5f, 1.5f), 0.25f));
        SelectedWorldTargetMenuItem.WorldPreviewImage.gameObject.SetActive(false);
        SelectedWorldTargetMenuItem.WorldPreviewStencilQuad.SetActive(true);

        // if menu is not on the current world target, set the selected world target to the background for viewing through the portal
        if (SelectedWorldTargetMenuItem.WorldTargetRef != WorldTargetManager.Instance.GetCurrentWorldTarget())
            TransitionManager.Instance.SetWorldTargetToPortalInsideAndActivate(SelectedWorldTargetMenuItem.WorldTargetRef);

        // if menu is on the current world target, hide the stencil quad and show the preview image
        if (SelectedWorldTargetMenuItem.WorldTargetRef == WorldTargetManager.Instance.GetCurrentWorldTarget())
        {
            SelectedWorldTargetMenuItem.WorldPreviewImage.gameObject.SetActive(true);
            SelectedWorldTargetMenuItem.WorldPreviewStencilQuad.SetActive(false);
        }

        LastSelectedWorldTargetMenuItem = SelectedWorldTargetMenuItem;
    }
}