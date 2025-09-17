using System.Collections.Generic;
using Flexalon;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public abstract class TransitionInterface : MonoBehaviour
{
    [Header("General Transition Interface Objects")]
    public GameObject InterfaceObject;
    public LayoutBase Layout;
    public GameObject MenuItemPrefab;
    public List<WorldTargetMenuItem> MenuItems = new List<WorldTargetMenuItem>();

    public WorldTargetMenuItem SelectedWorldTargetMenuItem;

    public WorldTargetMenuItem LastSelectedWorldTargetMenuItem;

    

    [Header("Scrolling Objects")]
    public GameObject CursorObject;
    public Transform CursorStartPoint;
    public Transform CursorEndPoint;
    
    /// <summary>
    /// Responsible for populating List<WorldTargetMenuItem> for the interface layout
    /// </summary>
    public abstract void PopulateLayout();

    // public abstract void AddWorldTargetMenuItemToLayout(WorldTarget worldTarget);

    public abstract void CheckSelectedMenuItem();

    // public abstract void UpdateSelectedMenuItem();

    public virtual void UpdateSelectedMenuItem(WorldTargetMenuItem selectedMenuItem) {

        if (selectedMenuItem == null || selectedMenuItem.WorldTargetRef == null) {
            return;
        }
        
        string logEvent = "SelectedMenuItem";
        string logEnv = selectedMenuItem.WorldTargetRef.Name;
        DataLogger.Instance.LogInteraction(logEvent, logEnv);

        // Debug.Break();

        if (selectedMenuItem.WorldTargetRef != WorldTargetManager.Instance.GetCurrentWorldTarget()) {
            // TODO: Consider: handle just in PaletteMenuObject?
            // selectedMenuItem.WorldPreviewImage.material = XRComponents.Instance.CircularMaskOutlined;
        }

        if (LastSelectedWorldTargetMenuItem != null)
        {
            if (LastSelectedWorldTargetMenuItem.WorldTargetRef != WorldTargetManager.Instance.GetCurrentWorldTarget()) {
                // TODO: Consider: handle just in PaletteMenuObject?
                // LastSelectedWorldTargetMenuItem.WorldPreviewImage.material = XRComponents.Instance.CircularMask;
            }
        }
        
    }

    public void AddWorldTargetMenuItemToLayout(WorldTarget worldTarget)
    {
        WorldTargetMenuItem wtItem = Instantiate(MenuItemPrefab, Layout.transform).GetComponent<WorldTargetMenuItem>();
        wtItem.WorldTargetRef = worldTarget;
        wtItem.MenuItemText.text = wtItem.WorldTargetRef.Name;
        AddMenuItemToList(wtItem);
        worldTarget.AssociatedWorldTargetMenuItems.Add(wtItem);
        if (wtItem.WorldPreviewStencilQuad != null)
        {
            wtItem.WorldPreviewStencilQuad.SetActive(false);
        }
        // wtItem.WorldPreviewStencilQuad.SetActive(false);
    }

    /// <summary>
    /// Adds a WorldTargetMenuItem to the specified list.
    /// </summary>
    /// <param name="menuItem">The WorldTargetMenuItem to add.</param>
    public void AddMenuItemToList(WorldTargetMenuItem menuItem)
    {
        MenuItems.Add(menuItem);
    }

    public virtual void DisablePortalPreview() {
        
    }

    public void EnableTransitionInterface(bool enable) {
        Layout.gameObject.SetActive(enable);
    }

    public void ToggleTransitionInterface() {
        Layout.gameObject.SetActive(!gameObject.activeSelf);
    }

    public virtual void EnableInterfaceObject(bool enable) {

        if (enable & InterfaceObject.activeSelf) {
            return;
        } else if (!enable & !InterfaceObject.activeSelf) {
            return;
        }
        
        string logStr = enable ? "Enable_Interface" : "Disable_Interface";
        if (!enable) {
            TransitionManager.Instance.Transitioning = false;
        }
        DataLogger.Instance.LogInteraction(logStr, "");

        InterfaceObject.SetActive(enable);
        if (SelectedWorldTargetMenuItem == null & MenuItems.Count > 0)
        {
            SelectedWorldTargetMenuItem = MenuItems[0];
        }
        // disable menu object that the user is currently at.
        foreach (WorldTargetMenuItem item in MenuItems)
        {
            if (item.WorldTargetRef == WorldTargetManager.Instance.GetCurrentWorldTarget())
            {
                // Debug.Log($"Setting grayscale on current world target. {item.WorldTargetRef.Name}");
                item.WorldPreviewImage.material = XRComponents.Instance.CircularMaskGrayscale;
                // make it non-interactable
                item.GetComponent<SphereCollider>().enabled = false;
                // for baseline interface, menu items have interactable component, not just sphere collider. We also set their collider isTrigger to false in editor.
                if (item.GetComponent<XRSimpleInteractable>() != null)
                {
                    item.GetComponent<XRSimpleInteractable>().enabled = false;
                }

            } else {
                item.WorldPreviewImage.material = XRComponents.Instance.CircularMask;
                item.GetComponent<SphereCollider>().enabled = true;
                if (item.GetComponent<XRSimpleInteractable>() != null)
                {
                    item.GetComponent<XRSimpleInteractable>().enabled = true;
                }
            }
        }
    }

    void Awake()
    {
        // MenuItems = new List<WorldTargetMenuItem>();
    }

    public virtual void Update()
    {
        // CheckSelectedMenuItem();
    }

}