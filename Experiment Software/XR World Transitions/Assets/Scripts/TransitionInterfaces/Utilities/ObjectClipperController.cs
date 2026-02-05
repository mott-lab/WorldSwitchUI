using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ObjectClipperController : MonoBehaviour
{
    public Material objectClipperMaterial;
    public Transform sphereTransform;
    [Range(0.0f, 0.5f)]
    public float sphereRadius;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if (objectClipperMaterial == null)
        // {
        //     Debug.LogError("ObjectClipper material is not assigned.");
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (sphereTransform != null)
        {
            // objectClipperMaterial.SetVector("_SpherePosition", sphereTransform.position);
            // objectClipperMaterial.SetFloat("_SphereRadius", sphereRadius);
            Shader.SetGlobalVector("_SpherePosition", sphereTransform.position);
            Shader.SetGlobalFloat("_SphereRadius", sphereRadius);
        }
    }

    // This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only)
    void OnValidate()
    {
        Update();
    }
}