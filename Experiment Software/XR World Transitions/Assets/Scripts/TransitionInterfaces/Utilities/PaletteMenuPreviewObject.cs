using UnityEngine;

public class PaletteMenuPreviewObject : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;
    private Material instanceMaterial;
    [SerializeField] private GameObject backgroundObject;

    void OnEnable()
    {
        // create a new instance of the material to avoid affecting other objects
        instanceMaterial = new Material(outlineMaterial);
        backgroundObject.GetComponent<Renderer>().material = instanceMaterial;
    }
    private void OnTriggerEnter(Collider other)
    {

        // Disable cursor renderer if using head cursor interaction to avoid depth conflict
        if (TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction is Palette_HandPreview_HeadHandInteraction) {
            TransitionUIManager.Instance.TransitionCursorController.CursorRenderer.enabled = false;
        }

        SetOutlineWidth(0.003f);

        // only update hovered menu item if 
        TransitionUIManager.Instance.UpdateHoveredMenuItem(GetComponent<WorldTargetMenuItem>());
    }

    private void OnTriggerExit(Collider other)
    {
        TransitionUIManager.Instance.ResetHoveredMenuItem();
        ResetCursor();
    }

    public void ResetCursor() {
        TransitionUIManager.Instance.TransitionCursorController.CursorRenderer.enabled = true;

        // set outline material's property OutlineWidth to 0
        SetOutlineWidth(0.0f);

        TransitionUIManager.Instance.ResetHoveredMenuItem();
    }

    public void SetOutlineWidth(float width)
    {
        // Debug.Log("Setting outline width to " + width);
        instanceMaterial.SetFloat("_OutlineWidth", width);
    }
}