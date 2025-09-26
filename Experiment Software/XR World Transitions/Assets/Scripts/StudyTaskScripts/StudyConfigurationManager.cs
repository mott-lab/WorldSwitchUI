using UnityEngine;

public class StudyConfigurationManager : MonoBehaviour
{

    private static StudyConfigurationManager _instance;

    public static StudyConfigurationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<StudyConfigurationManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<StudyConfigurationManager>();
                    singletonObject.name = typeof(XRComponents).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }
    public enum DominantHand
    {
        LeftHand,
        RightHand
    }

    public enum SubordinateHand
    {
        LeftHand,
        RightHand
    }

    public DominantHand UserDominantHand;
    public SubordinateHand UserSubordinateHand;

    public enum OperationMode
    {
        Study,
        Demo
    }
    [SerializeField] private OperationMode operationMode;
    public OperationMode CurrentOperationMode => operationMode;

    [SerializeField] public bool GalleryTrainingComplete;
    public bool TechniqueTrainingComplete;

    [Header("Set the scroll speed and multiplier for the gallery interfaces")]
    [SerializeField] private float scrollSpeedLowerBound; // default: 0
    [SerializeField] private float scrollSpeedUpperBound; // default: 20
    [SerializeField] private float scrollMultiplierLowerBound; // default: 0.1
    [SerializeField] private float scrollMultiplierUpperBound; // default: 1
    [SerializeField] private float handVelocityLowerBoundForSnapping; // default: 0.15
    [SerializeField] private float handVelocityUpperBound; // default: .3


    /// <summary>
    /// 
    /// scroll speed multiplier is computed the following way:
    /// interpolate between scrollSpeedLowerBound and scrollSpeedUpperBound, using the hand X velocity as the interpolation parameter. 
    // this hand X velocity parameter is first computed as the point between scrollMultiplierLowerBound and scrollMultiplierUpperBound (it is the Inverse Lerp). this value is then clamped between scrollMultiplierLowerBound and scrollMultiplierUpperBound.

    /// handVelocityLowerBoundForSnapping is the lower bound for the hand X velocity. if the hand X velocity is below this value, the scroll speed multiplier is set to 0, i.e., the cursor does not move.
    /// 
    /// handVelocityUpperBound is the upper bound for the hand X velocity. if the hand X velocity is above this value, the scroll speed multiplier is set to 1, i.e., the cursor moves at the maximum speed.
    /// 
    /// slow, direct:
    /// 0
    /// 1
    /// 0.05
    /// 0.05
    /// 0.05
    /// 0.2
    /// 
    /// medium:
    /// 0
    /// 20
    /// .05
    /// .1
    /// .05
    /// .5
    /// 
    /// pretty fast:
    /// 0
    /// 20
    /// .1
    /// 1
    /// .05
    /// .3
    /// </summary>

    void OnValidate()
    {
        if (UserDominantHand == DominantHand.LeftHand)
        {
            UserSubordinateHand = SubordinateHand.RightHand;
        }
        else
        {
            UserSubordinateHand = SubordinateHand.LeftHand;
        }

        XRComponents.Instance.InteractionLowerBound.GetComponent<MeshRenderer>().enabled = !GalleryTrainingComplete;
        XRComponents.Instance.InteractionUpperBound.GetComponent<MeshRenderer>().enabled = !GalleryTrainingComplete;
    

        if (TransitionUIManager.Instance.gameObject.activeSelf)
        {
            foreach (InteractionHandler interaction in TransitionUIManager.Instance.InteractionManager.TransitionInteractions)
            {
                interaction.SetScrollingVariables(scrollSpeedLowerBound, scrollSpeedUpperBound, scrollMultiplierLowerBound, scrollMultiplierUpperBound, handVelocityLowerBoundForSnapping, handVelocityUpperBound);
            }
            // TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction.SetScrollingVariables(scrollSpeedLowerBound, scrollSpeedUpperBound, scrollMultiplierLowerBound, scrollMultiplierUpperBound, handVelocityLowerBoundForSnapping, handVelocityUpperBound);
        }
    }

    public void CompletedTechniqueTraining() {
        TechniqueTrainingComplete = true;
        XRComponents.Instance.StudyUIParent.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
