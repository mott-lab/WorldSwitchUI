using UnityEngine;

public class FollowLeftHand : MonoBehaviour
{
    private Transform wristTransform; // Transform of the L_Wrist

    void Update()
    {
        // Dynamically find the L_Wrist object at runtime
        if (wristTransform == null)
        {
            GameObject wrist = GameObject.Find("L_Wrist");
            if (wrist != null)
            {
                wristTransform = wrist.transform;

                // Make testTextureRendered a child of L_Wrist
                transform.SetParent(wristTransform);

                // Apply the exact transform values for Position, Rotation, and Scale
                transform.localPosition = new Vector3(-0.01f, -0.054f, 0.099f); // Position relative to L_Wrist
                transform.localRotation = Quaternion.Euler(-80.516f, 57.147f, 119.669f); // Rotation in degrees
                transform.localScale = new Vector3(0.22f, 0.25f, 0.1f); // Scale across all axes
            }
        }
    }
}
