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
            Transform leftHand;
            Transform rightHand;
            if (TransitionUIManager.Instance.InteractionManager.XRInteractorSetup.CurrentInteractionMode == XRInteractorSetup.InteractionMode.Hands)
            {
                if (XRComponents.Instance.wristL == null) return;
                if (XRComponents.Instance.wristR == null) return;
                leftHand = XRComponents.Instance.wristL.transform;
                rightHand = XRComponents.Instance.wristR.transform;
            }
            else
            {
                leftHand = XRComponents.Instance.LeftController.transform;
                rightHand = XRComponents.Instance.RightController.transform;
            }
            if (XRComponents.Instance.XRCamera == null) return;
            avatarHead.SetLocalPositionAndRotation(XRComponents.Instance.XRCamera.transform.localPosition, XRComponents.Instance.XRCamera.transform.localRotation);
            avatarLeftHand.SetLocalPositionAndRotation(leftHand.localPosition, leftHand.localRotation);
            avatarRightHand.SetLocalPositionAndRotation(rightHand.localPosition, rightHand.localRotation);
            Vector3 bodyPosition = XRComponents.Instance.XRCamera.transform.localPosition;
            bodyPosition.y = 0.6f;
            avatarBody.localPosition = bodyPosition;
        }
        
    }
}
