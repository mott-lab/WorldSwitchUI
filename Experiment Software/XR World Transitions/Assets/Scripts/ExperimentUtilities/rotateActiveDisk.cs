using UnityEngine;

public class rotateActiveDisk : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 50f; 

    void Update()
    {
        // Only rotate if is active
        if (gameObject.activeInHierarchy)
        {
            // Rotate around the Y-axis. 
            // Use negative speed to make it rotate clockwise
            transform.Rotate(0f, -rotationSpeed * Time.deltaTime, 0f);
        }
    }
}

