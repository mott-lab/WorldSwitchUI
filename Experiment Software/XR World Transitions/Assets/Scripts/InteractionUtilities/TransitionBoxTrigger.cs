using UnityEngine;

public class TransitionBoxTrigger : MonoBehaviour
{

    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private TransitionManager.SwipeDirection direction;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Entered: {direction}");
        TransitionManager.Instance.LastSwipeDirection = direction;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Exited: {direction}");
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"Stayed: {direction}");
    }

    
}
