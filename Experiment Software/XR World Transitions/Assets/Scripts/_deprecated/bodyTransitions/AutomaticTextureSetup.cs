using UnityEngine;

public class ApplyMaterialToLeftHand : MonoBehaviour
{
    public Transform xrOrigin; // Reference to XR Origin
    public string materialPath = "Assets/texturesRender/Materials/destinationCameraView 1.mat"; // Path to the material
    private Material destinationMaterial; // The material to be applied
    private GameObject leftHand; // Reference to the dynamically created LeftHand GameObject

    void Start()
    {
        // Load the material from the specified path
        destinationMaterial = (Material)Resources.Load(materialPath, typeof(Material));

        if (destinationMaterial == null)
        {
            Debug.LogError($"Material not found at path: {materialPath}");
            return;
        }

        // Look for the LeftHand GameObject as a child of XR Origin
        leftHand = FindLeftHand();

        if (leftHand != null)
        {
            // Apply the material to the LeftHand's Renderer using sharedMaterial
            ApplyMaterialToHand(leftHand);
        }
        else
        {
            Debug.LogWarning("LeftHand GameObject not found as a child of XR Origin.");
        }
    }

    GameObject FindLeftHand()
    {
        if (xrOrigin == null)
        {
            Debug.LogError("XR Origin is not assigned.");
            return null;
        }

        // Search for "LeftHand" among the children of XR Origin
        foreach (Transform child in xrOrigin.GetComponentsInChildren<Transform>())
        {
            if (child.name == "LeftHand")
            {
                return child.gameObject;
            }
        }

        return null; // Return null if not found
    }

    void ApplyMaterialToHand(GameObject hand)
    {
        Renderer handRenderer = hand.GetComponent<Renderer>();
        if (handRenderer != null)
        {
            // Assign the shared material to ensure updates to the material are reflected
            handRenderer.sharedMaterial = destinationMaterial;
            Debug.Log("Material applied to LeftHand successfully.");
        }
        else
        {
            Debug.LogError("Renderer component not found on LeftHand GameObject.");
        }
    }
}
