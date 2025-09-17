using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum TransitionInteractionType {
    Gallery_HeadPreview_HandInteraction,
    Palette_HandPreview_HeadHandInteraction
}

[System.Serializable]
public class TransitionInteraction {
    public TransitionInteractionType InteractionType;
    public InteractionHandler InteractionHandler;
}

public class InteractionManager : MonoBehaviour {

    [SerializeField] private List<InteractionHandler> transitionInteractions;
    public List<InteractionHandler> TransitionInteractions { get => transitionInteractions; }
    public InteractionHandler SelectedTransitionInteraction;
    
    private bool handFound_L = false;
    // [SerializeField] private Vector3 lastHandPosition_L;

    private bool handFound_R = false;

    public bool HandsAlive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SubsystemManager.GetSubsystems(XRComponents.Instance.handSubsystems);
        if (XRComponents.Instance.handSubsystems.Count > 0)
        {
            XRComponents.Instance.handSubsystem = XRComponents.Instance.handSubsystems[0];
        }

        ActivateSelectedInteractionHandler();
    }

    public void ActivateSelectedInteractionHandler() {
        // turn off all interaction handlers except the selected one
        foreach (InteractionHandler interaction in transitionInteractions)
        {
            if (interaction != SelectedTransitionInteraction)
            {
                interaction.gameObject.SetActive(false);
            }
            else {
                interaction.gameObject.SetActive(true);
                interaction.InitInteractionHandler();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!handFound_L)
        {
            XRComponents.Instance.leftHandObject = GameObject.Find("LeftHand");
            XRComponents.Instance.wristL = GameObject.Find("L_Wrist");
            if (XRComponents.Instance.leftHandObject != null)
            {
                handFound_L = true;
                Debug.Log("Found LeftHand GameObject");
            }
            return;
        }
        else
        {

        }

        if (!handFound_R)
        {
            XRComponents.Instance.rightHandObject = GameObject.Find("RightHand");
            XRComponents.Instance.wristR = GameObject.Find("R_Wrist");
            if (XRComponents.Instance.rightHandObject != null)
            {
                handFound_R = true;
                Debug.Log("Found RightHand GameObject");
            }
            return;
        }
        else
        {

        }

        if (XRComponents.Instance.handSubsystem == null || !XRComponents.Instance.handSubsystem.running || (!XRComponents.Instance.leftHandObject.activeInHierarchy & !XRComponents.Instance.rightHandObject.activeInHierarchy)) return;

        XRComponents.Instance.leftHand = XRComponents.Instance.handSubsystem.leftHand;
        XRComponents.Instance.rightHand = XRComponents.Instance.handSubsystem.rightHand;

        if (!XRComponents.Instance.leftHand.isTracked & !XRComponents.Instance.rightHand.isTracked) return;

        HandsAlive = true;

        // TransitionUIManager.Instance.InteractionManager.SelectedTransitionUIInteraction.InteractionHandler.ProcessUpdate();
        TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction.ProcessUpdate();
    }

}