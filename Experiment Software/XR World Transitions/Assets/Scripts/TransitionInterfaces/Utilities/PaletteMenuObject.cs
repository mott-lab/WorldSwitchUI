using UnityEngine;

public class PaletteMenuObject : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;
    private Material instanceMaterial;

    [SerializeField] private GameObject backgroundObject;

    void OnEnable()
    {
        // create a new instance of the material to avoid affecting other objects
        instanceMaterial = new Material(outlineMaterial);
        backgroundObject.GetComponent<Renderer>().material = instanceMaterial;
        backgroundObject.GetComponent<Renderer>().material.shader = outlineMaterial.shader;
    }

    private void OnTriggerEnter(Collider other)
    {
        // set outline material's property OutlineWidth to 0.003f
        instanceMaterial.SetFloat("_OutlineWidth", 0.003f);

        GetComponent<WorldTargetMenuItem>().WorldPreviewImage.material = XRComponents.Instance.CircularMaskOutlined;

        TransitionUIManager.Instance.UpdateHoveredMenuItem(GetComponent<WorldTargetMenuItem>());

    }

    private void OnTriggerExit(Collider other)
    {
        // set outline material's property OutlineWidth to 0
        instanceMaterial.SetFloat("_OutlineWidth", 0.0f);

        GetComponent<WorldTargetMenuItem>().WorldPreviewImage.material = XRComponents.Instance.CircularMask;

        TransitionUIManager.Instance.ResetHoveredMenuItem();
    }
}