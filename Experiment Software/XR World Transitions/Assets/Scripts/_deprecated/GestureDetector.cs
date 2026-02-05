using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Hands;

[System.Obsolete("GestureDetector is deprecated and will be removed in future versions.")]
public class GestureDetector : MonoBehaviour
{
    
    private bool handFound = false;
    [SerializeField] private Vector3 lastHandPosition;
    // [SerializeField] float distanceFromGestureStartPos;
    // [SerializeField] private float gestureScrollMultiplier;

    // [SerializeField] private GameObject gestureVisualizationObject;
    // [SerializeField] private GameObject gestureVisualizationStartObject;
    // [SerializeField] private GameObject gestureVisualizationEndObject;
    // [SerializeField] private GameObject gestureIntermediateObj;
    private float galleryGestureEndPosX;
    //[SerializeField] private Curve

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SubsystemManager.GetSubsystems(XRComponents.Instance.handSubsystems);
        if (XRComponents.Instance.handSubsystems.Count > 0)
        {
            XRComponents.Instance.handSubsystem = XRComponents.Instance.handSubsystems[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!handFound)
        {
            XRComponents.Instance.leftHandObject = GameObject.Find("LeftHand");
            XRComponents.Instance.wristL = GameObject.Find("L_Wrist");
            if (XRComponents.Instance.leftHandObject != null)
            {
                handFound = true;
                lastHandPosition = XRComponents.Instance.leftHandObject.transform.position;
                Debug.Log("Found LeftHand GameObject and recorded its position");
            }
            return;
        }
        else
        {
            XRComponents.Instance.wristL = GameObject.Find("L_Wrist");
            lastHandPosition = XRComponents.Instance.wristL.transform.position; // last position of wrist (previously was left hand object)
        }

        if (XRComponents.Instance.handSubsystem == null || !XRComponents.Instance.handSubsystem.running || !XRComponents.Instance.leftHandObject.activeInHierarchy) return;

        XRComponents.Instance.leftHand = XRComponents.Instance.handSubsystem.leftHand;
        if (!XRComponents.Instance.leftHand.isTracked) return;

        // TransitionUIManager.Instance.InteractionManager.SelectedTransitionUIInteraction.InteractionHandler.ProcessUpdate();
        TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction.ProcessUpdate();

    }
}
