using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BaselineGalleryMenuItem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnEnable()
    {
        var interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEntered);
            interactable.hoverExited.AddListener(OnHoverExited);
        }
    }

    void OnDisable()
    {
        var interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEntered);
            interactable.hoverExited.RemoveListener(OnHoverExited);
        }
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        Debug.Log("Hover entered");
        TransitionUIManager.Instance.UpdateHoveredMenuItem(GetComponent<WorldTargetMenuItem>());
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        Debug.Log("Hover exited");
        TransitionUIManager.Instance.HoveredMenuItem = null;
    }
}
