using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class WorldTargetMenuItem : MonoBehaviour
{
    public TextMeshProUGUI MenuItemText;
    public Vector3 WorldPosition;
    public int WorldIndex;
    public WorldTarget WorldTargetRef;
    public RawImage WorldPreviewImage;
    public GameObject WorldPreviewStencilQuad;
    public GameObject TextCanvas;
    public GameObject StencilCanvas;
    public GameObject ScreenshotCanvas;
    public GameObject ItemCube;

    void OnEnable()
    {
        
    }


    /// <summary>
    /// Selects the world target menu item and initiates a transition to the associated world target.
    /// </summary>
    public void ConfirmWorldTargetMenuItem()
    {
        if (TransitionManager.Instance.Transitioning) return;
        TransitionManager.Instance.Transitioning = true;
        
        Debug.Log($"confirmed world target: {WorldTargetRef.Name}");
        // TransitionManager.Instance.SetTransitionMode(TransitionManager.TransitionMode.GrowOutward);
        TransitionManager.Instance.TransitionToWorldTarget(WorldTargetRef);
    }
}
