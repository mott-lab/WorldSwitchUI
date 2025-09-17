using UnityEngine;

public class AvatarFollower : MonoBehaviour
{

    public Transform avatarHead;
    public Transform avatarBody;
    public Transform avatarLeftHand;
    public Transform avatarRightHand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TransitionUIManager.Instance.WIMObjects.activeSelf) {
            if (XRComponents.Instance.wristL == null) return;
            if (XRComponents.Instance.wristR == null) return;
            if (XRComponents.Instance.XRCamera == null) return;
            avatarHead.SetLocalPositionAndRotation(XRComponents.Instance.XRCamera.transform.localPosition, XRComponents.Instance.XRCamera.transform.localRotation);
            avatarLeftHand.SetLocalPositionAndRotation(XRComponents.Instance.wristL.transform.localPosition, XRComponents.Instance.wristL.transform.localRotation);
            avatarRightHand.SetLocalPositionAndRotation(XRComponents.Instance.wristR.transform.localPosition, XRComponents.Instance.wristR.transform.localRotation);
            Vector3 bodyPosition = XRComponents.Instance.XRCamera.transform.localPosition;
            bodyPosition.y = 0.6f;
            avatarBody.localPosition = bodyPosition;
        }
        
    }
}
