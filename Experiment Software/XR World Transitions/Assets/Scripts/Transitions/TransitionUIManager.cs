using Flexalon;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;

[Serializable]
public enum TransitionUIType {
        None,
        Portal_Gallery,
        Portal_Palette_Hand,
        Portal_Palette_HeadHand,
        Portal_SteeringWheel,
        WIM_Palette_Hand,
        WIM_Palette_HeadHand,
        WIM_Gallery,
        WIM_SteeringWheel,
        Baseline
}

public class TransitionUIManager : MonoBehaviour
{

    private static TransitionUIManager _instance;

    public static TransitionUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindFirstObjectByType<TransitionUIManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<TransitionUIManager>();
                    singletonObject.name = typeof(TransitionUIManager).ToString() + " (Singleton)";
                }
            }
            return _instance;
        }
    }

    public void SetTransitionType(TransitionUIType newType)
    {
        worldTransitionUIType = newType;
    }

    public InteractionManager InteractionManager;

    [SerializeField] private GameObject transitionUI;
    [SerializeField] private GetSwipeTranslation swipeTranslator;
    [SerializeField] private TransitionUIType worldTransitionUIType;
    public TransitionUIType WorldTransitionUIType { get => worldTransitionUIType; set => worldTransitionUIType = value; }

    [SerializeField] private float galleryCurveLayoutScrollStart;
    public float GalleryCurveLayoutScrollStart { get => galleryCurveLayoutScrollStart; }


    public List<TransitionInterface> TransitionInterfaces;
    public TransitionInterface CurrentTransitionInterface;

    public TransitionCursorController TransitionCursorController;
    public GameObject WheelCursor;
    public WorldTargetMenuItem HoveredMenuItem;

    public GameObject WIMObjects;

    public CoinAnchorController CoinAnchorController;

    private void Reset()
    {

        InteractionManager = GetComponentInChildren<InteractionManager>();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        ActivateSelectedInterface();
    }

    public void ActivateSelectedInterface() {
        foreach (TransitionInterface transitionInterface in TransitionInterfaces)
        {
            if (transitionInterface != CurrentTransitionInterface)
            {
                transitionInterface.gameObject.SetActive(false);
            } else {
                transitionInterface.gameObject.SetActive(true);
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HandleWorldTransitionUITypeChanged();
    }
#endif

    public void HandleWorldTransitionUITypeChanged()
    {
        // TODO: turn off current interface
        CurrentTransitionInterface?.InterfaceObject.SetActive(false);

        // Add your logic here to handle the change in worldTransitionUIType
        Debug.Log("worldTransitionUIType has been changed to: " + worldTransitionUIType);
        if (worldTransitionUIType != TransitionUIType.Portal_Palette_Hand && worldTransitionUIType != TransitionUIType.Portal_Palette_HeadHand && worldTransitionUIType != TransitionUIType.WIM_Palette_Hand && worldTransitionUIType != TransitionUIType.WIM_Palette_HeadHand)
        {
            TransitionCursorController.gameObject.SetActive(false);
        }
        else
        {
            TransitionCursorController.gameObject.SetActive(true);
        }
        switch (worldTransitionUIType)
        {
            case TransitionUIType.None:
                CurrentTransitionInterface = null;
                InteractionManager.SelectedTransitionInteraction = null;
                break;
            case TransitionUIType.Baseline:
                CurrentTransitionInterface = TransitionInterfaces.Find(x => x is Baseline_Gallery_Interface);
                InteractionManager.SelectedTransitionInteraction = InteractionManager.TransitionInteractions.Find(x => x is Baseline_Gallery_HandInteraction);
                break;
            case TransitionUIType.Portal_Palette_Hand:
                CurrentTransitionInterface = TransitionInterfaces.Find(x => x is Palette_Interface);
                InteractionManager.SelectedTransitionInteraction = InteractionManager.TransitionInteractions.Find(x => x is Palette_HandPreview_HandInteraction);
                break;
            case TransitionUIType.Portal_Palette_HeadHand:
                CurrentTransitionInterface = TransitionInterfaces.Find(x => x is Palette_Interface);
                InteractionManager.SelectedTransitionInteraction = InteractionManager.TransitionInteractions.Find(x => x is Palette_HandPreview_HeadHandInteraction);
                break;
            case TransitionUIType.Portal_SteeringWheel:
                CurrentTransitionInterface = TransitionInterfaces.Find(x => x is Wheel_Interface);
                InteractionManager.SelectedTransitionInteraction = InteractionManager.TransitionInteractions.Find(x => x is Wheel_LRPreview_LRInteraction);
                break;
            case TransitionUIType.WIM_Palette_Hand:
                CurrentTransitionInterface = TransitionInterfaces.Find(x => x is PaletteWIM_Interface);
                InteractionManager.SelectedTransitionInteraction = InteractionManager.TransitionInteractions.Find(x => x is PaletteWIM_HandPreview_HandInteraction);
                break;
            case TransitionUIType.WIM_Gallery:
                CurrentTransitionInterface = TransitionInterfaces.Find(x => x is WIM_Gallery_Interface);
                InteractionManager.SelectedTransitionInteraction = InteractionManager.TransitionInteractions.Find(x => x is WIM_Gallery_BodyPreview_HandInteraction);
                break;
            case TransitionUIType.Portal_Gallery:
                CurrentTransitionInterface = TransitionInterfaces.Find(x => x is Portal_Gallery_Interface_line);
                InteractionManager.SelectedTransitionInteraction = InteractionManager.TransitionInteractions.Find(x => x is Portal_Gallery_HeadPreview_HandInteraction_line);
                break;
            case TransitionUIType.WIM_Palette_HeadHand:
                CurrentTransitionInterface = TransitionInterfaces.Find(x => x is PaletteWIM_Interface);
                InteractionManager.SelectedTransitionInteraction = InteractionManager.TransitionInteractions.Find(x => x is PaletteWIM_HandPreview_HeadHandInteraction);
                break;
            case TransitionUIType.WIM_SteeringWheel:
                CurrentTransitionInterface = TransitionInterfaces.Find(x => x is WIM_Wheel_Interface);
                InteractionManager.SelectedTransitionInteraction = InteractionManager.TransitionInteractions.Find(x => x is WIM_Wheel_LRPreview_LRInteraction);
                break;
            default:
                break;
        }

        ActivateSelectedInterface();
        UpdateTutorialVideo();
        InteractionManager.ActivateSelectedInteractionHandler();
        if (studyManager.Instance.studyUIManager != null)
        {
            studyManager.Instance.studyUIManager.SetUpTechniqueTrainingUI(worldTransitionUIType);
        }
    }

    public void UpdateTutorialVideo()
    {
        if (CurrentTransitionInterface != null)
        {
            VideoClip videoClip = InteractionManager.SelectedTransitionInteraction.TutorialVideoClip;
            if (videoClip != null)
            {
                // Add logic to play/display the video
                XRComponents.Instance.TutotialVideoPlayer.clip = videoClip;
                XRComponents.Instance.TutotialVideoPlayer.Play();
            }
        }
    }


    /// <summary>
    /// Adds a WorldTargetMenuItem to the specified list.
    /// </summary>
    /// <param name="menuItem">The WorldTargetMenuItem to add.</param>
    /// <param name="list">The list to which the menuItem will be added.</param>
    public void AddMenuItemToList(WorldTargetMenuItem menuItem, List<WorldTargetMenuItem> list)
    {
        list.Add(menuItem);
    }

    private void Update()
    {
        // checkSelectedMenuItem();
    }

    public void CompleteRightSwipe()
    {
        TransitionManager.Instance.SetTransitionMode(TransitionManager.TransitionMode.Swipe);
        TransitionManager.Instance.TransitionToWorldTarget(WorldTargetManager.Instance.GetPreviousWorldTarget());
        //if (TransitionManager.Instance.LastSwipeDirection == TransitionManager.SwipeDirection.Right)
        //{
        //}
    }

    public void ActivateWorldTargetWIM(WorldTarget worldTarget) {
        worldTarget.WIMObjects.SetActive(true);

        foreach(WorldTarget wt in WorldTargetManager.Instance.WorldTargets) {
            if (wt != worldTarget) {
                wt.WIMObjects.SetActive(false);
            }
        }
    }

    public void UpdateHoveredMenuItem(WorldTargetMenuItem menuItem)
    {
        Debug.Log("Updating hovered menu item");
        if (CurrentTransitionInterface == null) {
            HoveredMenuItem = menuItem;
            return;
        }

        if (!CurrentTransitionInterface.gameObject.activeSelf)
        {
            HoveredMenuItem = menuItem;
            // paletteInterface.UpdateSelectedMenuItem(GetComponent<WorldTargetMenuItem>());
        }
        else {
            Debug.Log("Updating selected menu item");
            CurrentTransitionInterface.UpdateSelectedMenuItem(menuItem);
            HoveredMenuItem = menuItem;
        }
    }

    public void ResetHoveredMenuItem() {
        if (CurrentTransitionInterface != null || !CurrentTransitionInterface.gameObject.activeSelf) {
            HoveredMenuItem = null;
        }
    }

    public void CompleteGallerySelect()
    {

    }

    private void PositionUIInFrontOfController()
    {
        if (XRComponents.Instance.LeftController != null)
        {
            // position UI in front of controller
            transitionUI.transform.position = XRComponents.Instance.LeftController.transform.position + XRComponents.Instance.LeftController.transform.forward;
            // rotate UI to face controller
            transitionUI.transform.rotation = Quaternion.LookRotation(XRComponents.Instance.LeftController.transform.forward, Vector3.up);
        }
    }
}
