using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Hands;

public class XRComponents : MonoBehaviour
{
    private static XRComponents _instance;

    public static XRComponents Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<XRComponents>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<XRComponents>();
                    singletonObject.name = typeof(XRComponents).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    [Header("XR Basics")]
    [SerializeField] private GameObject xrRig;
    public GameObject XRRig => xrRig;
    [SerializeField] private Camera xrCamera;
    public Camera XRCamera => xrCamera;
    public GameObject LeftController => leftController;
    [SerializeField] private GameObject leftController;
    [SerializeField] private GameObject rightController;
    public GameObject RightController => rightController;
    [SerializeField] private GameObject headGazeSphere;
    public GameObject HeadGazeSphere => headGazeSphere;
    [SerializeField] private GameObject headForwardDirection;
    public GameObject HeadForwardDirection => headForwardDirection;

    [Header("Hands")]
    public XRHandSubsystem handSubsystem;
    public List<XRHandSubsystem> handSubsystems = new List<XRHandSubsystem>();
    public GameObject leftHandObject;
    public GameObject wristL;
    public XRHand leftHand;
    public GameObject rightHandObject;
    public GameObject wristR;
    public XRHand rightHand;
    public GameObject CoinAnchor;
    public MeshRenderer CoinAnchorMeshRenderer;

    [Header("Gesture Objects")]
    public GameObject GestureVisualizationStartObject;
    public GameObject InteractionZones;
    public GameObject InteractionUpperBound;
    public GameObject InteractionLowerBound;

    [Header("Materials")]
    public Material CircularMask;
    public Material CircularMaskOutlined;
    public Material CircularMaskGrayscale;

    [Header("UI Components")]
    public GameObject StudyUIParent;
    public TextMeshProUGUI TechniqueTitle;
    public TextMeshProUGUI TechniqueDescription;
    public VideoPlayer TutotialVideoPlayer;

}
