using Unity.VisualScripting;
using UnityEngine;

public class CoinAnchorController : MonoBehaviour
{

    public void UpdateCoinAnchorPosition(Transform coinPosition) {

        Vector3 xrCameraPosition = XRComponents.Instance.XRCamera.transform.position;
        Vector3 direction = (coinPosition.position - xrCameraPosition).normalized;
        Vector3 halfwayPosition = xrCameraPosition + direction * 0.1f;
        halfwayPosition.y = 1f;

        Quaternion xrCameraRotation = XRComponents.Instance.XRCamera.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, xrCameraRotation.eulerAngles.y, 0);
        
        transform.SetPositionAndRotation(halfwayPosition, targetRotation);
    }
}
