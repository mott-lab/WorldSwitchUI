using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Hands;

[Obsolete]
public class WheelBarController : MonoBehaviour {
    void Update() {
        // PositionBar();
    }

    public void PositionBar() {

        if (TransitionUIManager.Instance.InteractionManager.HandsAlive){
        
            // XRComponents.Instance.rightHand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var tipPose);

            // Vector3 midPointBetweenHands = (XRComponents.Instance.wristL.transform.position + XRComponents.Instance.wristR.transform.position) / 2;

            // midPointBetweenHands += Vector3.up * 0.1f;
            // Vector3 targetPosition = midPointBetweenHands + XRComponents.Instance.XRCamera.transform.forward * 0.1f;

            // transform.position = targetPosition;

            // // set rotation to Y rotation of XRCamera
            // transform.rotation = Quaternion.Euler(0, XRComponents.Instance.XRCamera.transform.rotation.eulerAngles.y, 0);
        }
    }
}