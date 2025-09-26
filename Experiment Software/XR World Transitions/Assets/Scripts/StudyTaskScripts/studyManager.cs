using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum TaskType
{
    ObjectCollection,
    Monitoring
}

[System.Serializable]
public class StudyItem
{
    public GameObject item;     // coin/disc
    public GameObject wimItem; // associated WIM object
    public int environmentIndex; // environment this item belongs to
    public WorldTarget worldTarget;
    public Transform originalParent; // original parent of the item
    public Vector3 originalPosition; // original position of the item
    public Vector3 originalRotation; // original rotation of the item
}

// public enum Technique
// {
//     Portal_Gallery,
//     Portal_Palette_Hand,
//     Portal_Palette_HeadHand,
//     Portal_SteeringWheel,
//     WIM_Palette_Hand,
//     WIM_Gallery,
//     Portal_Gallery_line,
//     Technique8,
//     Technique9
// }

public class studyManager : MonoBehaviour
{

    private static studyManager _instance;

    public static studyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<studyManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<studyManager>();
                    singletonObject.name = typeof(TransitionManager).ToString() + " (Singleton)";
                }
            }
            return _instance;
        }
    }

    private Dictionary<string, int> environmentMap = new Dictionary<string, int>()
    {
        { "snowy", 0 },
        { "city",      1 },
        { "funland",   2 },
        { "medieval",  3 },
        { "castle",    4 }
    };

    [Header("Log the participant ID")]
    [SerializeField]
    public string participantID = "";

    [SerializeField] private bool breakBeforeSettingTechniqueInstructions = false;

    private LatinSquareUtil latinSquareUtil;
    public StudyUIManager studyUIManager;
    // [SerializeField] private TransitionUIType currentTechnique;

    // [Header("Select Task Type")]
    public TaskType currentTask = TaskType.ObjectCollection;

    public StudyItem trialBlockStartItem;

    [Header("Items for Object Collection (15 items)")]
    public StudyItem[] objectCollectionItems = new StudyItem[15];

    // [Header("Items for Monitoring (15 items)")]
    private StudyItem[] monitoringItems = new StudyItem[15];

    [Header("Chest Reference")]
    public Chest chest;

    [Header("Environments list")]
    public GameObject[] enviroments = new GameObject[5];

    private StudyItem[] activeItems;  // The set (coins or discs) to use in the study
    private bool[] usedItems;         // Tracks which items have been used
    private int lastEnvironment = -1; // Last trial�s environment index
    // private int totalCollected = 0;   // Total number of items collected
    public StudyItem CurrentItem;

    // ----- TEST MODE ----
    [Header("Test Mode")]
    [Tooltip("If true, only the test coins are enabled. Camera placement within the test zone is updated. No regular trial logic runs.")]
    public bool isTestMode = false;

    // -- assign the technique
    [Header("Transition Technique name")]
    public TransitionUIType currentTechnique = TransitionUIType.Portal_Gallery;

    // log the current trial
    public int currentTrial = 0;
    public int CurrentBlock = 0;

    public void AssignCoinsAtStart()
    {
        var allCoins = GameObject.FindGameObjectsWithTag("Collectable");

        List<StudyItem> collectionList = new List<StudyItem>();
        List<StudyItem> monitoringList = new List<StudyItem>();

        foreach (var coinGO in allCoins)
        {
            string coinName = coinGO.name;


            // split the name to get the info, basically as example:
            // parts[0] = "goldCoin"
            // parts[1] = "city"
            // parts[2] = "point" or "grab"
            string[] parts = coinName.Split('_');
            if (parts.Length < 3)
            {
                Debug.LogWarning($"this {coinName}, name format not recognized -- check.");
                // throw an erroor
                Debug.LogWarning("a coin was not recognized");
                continue;
            }

            string environmentName = parts[1]; // e.g., "city"
            string coinType = parts[2];        // e.g., "point" or "grab"

            if (!environmentMap.ContainsKey(environmentName))
            {
                Debug.LogWarning($"Environment '{environmentName}' not found in environmentMap. Skipping.");
                continue;
            }
            int envIndex = environmentMap[environmentName];

            StudyItem newItem = new StudyItem();
            newItem.item = coinGO;
            newItem.environmentIndex = envIndex;
            newItem.originalParent = coinGO.transform.parent;
            newItem.originalPosition = coinGO.transform.localPosition;
            newItem.originalRotation = coinGO.transform.localRotation.eulerAngles;
            newItem.worldTarget = enviroments[envIndex].GetComponentInChildren<WorldTarget>();
            
            GameObject wimPartnerObject = GameObject.Find(coinName + "_wim");
            if (wimPartnerObject != null)
            {
                newItem.wimItem = wimPartnerObject;
            }

            if (coinType == "point" || coinType == "point ")
            {
                monitoringList.Add(newItem);
            }
            else if (coinType == "grab" || coinType == "grab ")
            {
                collectionList.Add(newItem);
            }
            else
            {
                Debug.Log($"Coin type '{coinType}' not recognized. Skipping {coinName}.");
                Debug.LogWarning($"the current coin does not have a task type attribution '{coinType}'");
            }
        }

        // ensure that we only have 15
        objectCollectionItems = collectionList.Take(15).ToArray();
        monitoringItems = monitoringList.Take(15).ToArray();

        Debug.Log($"Assigned {objectCollectionItems.Length} items to objectCollectionItems.");
        Debug.Log($"Assigned {monitoringItems.Length} items to monitoringItems.");
    }

    private void TurnOnEnvironments() {
        for (int i = 0; i < enviroments.Length; i++)
        {
            enviroments[i].SetActive(true);
        }
    }

    private void TurnOffEnvironments() {
        for (int i = 0; i < enviroments.Length; i++)
        {
            enviroments[i].SetActive(false);
        }
    }

    private void resetAllItems() {
        for (int i = 0; i < activeItems.Length; i++)
        {
            if (activeItems[i].item != null) {
                activeItems[i].item.transform.SetParent(activeItems[i].originalParent);
                activeItems[i].item.transform.localPosition = activeItems[i].originalPosition;
                activeItems[i].item.transform.localRotation = Quaternion.Euler(activeItems[i].originalRotation);
                activeItems[i].item.SetActive(false);
                activeItems[i].item.GetComponent<CollectableItem>().HasLoggedGrab = false;
            }
        }
    }

    private void SetUpStudyBlock() {

        TurnOnEnvironments();

        // DisableAllTestCoins();
        // AssignCoinsAtStart();
        resetAllItems();

        // setup the current task to alway be object collection
        currentTask = TaskType.ObjectCollection;

        activeItems = (currentTask == TaskType.ObjectCollection) ? objectCollectionItems : monitoringItems;

        // all items start disabled.
        for (int i = 0; i < activeItems.Length; i++)
        {
            if (activeItems[i].item != null)
                activeItems[i].item.SetActive(false);
        }

        // for the other task that is not the active one we need to disable the items
        StudyItem[] otherItems = (currentTask == TaskType.ObjectCollection) ? monitoringItems : objectCollectionItems;
        for (int i = 0; i < otherItems.Length; i++)
        {
            if (otherItems[i].item != null)
                otherItems[i].item.SetActive(false);
        }

        usedItems = new bool[activeItems.Length];


        // WorldTargetManager.Instance.TurnOffWIMs();
        // TransitionUIManager.Instance.WIMObjects.SetActive(false);

        WorldTargetManager.Instance.StartAtFirstWorldTarget();

        currentTechnique = (TransitionUIType)latinSquareUtil.Conditions[CurrentBlock].UIType;

        TransitionUIManager.Instance.SetTransitionType(currentTechnique);
        TransitionUIManager.Instance.HandleWorldTransitionUITypeChanged();
        Debug.Log($"Transition UI Type set to match the selected technique: {currentTechnique}");

        SetUpTraining();
    }

    private void SetUpTestMode() {
        // ------ TEST Mode content only -----
        // MOVE CHEST AND XR ORIGIN TO TEST SPPOT

        // Move the XR Origin to (-8.354, 0, -17.221)
        GameObject xrOrigin = GameObject.Find("XR Origin Hands (XR Rig)");
        if (xrOrigin != null)
        {
            xrOrigin.transform.position = new Vector3(-8.354f, 0f, -17.221f);
            xrOrigin.transform.rotation = Quaternion.identity; // (0, 0, 0)
                                                                    
        }
        else
        {
            Debug.LogWarning("XR Origin Hands (XR Rig) not found in scene for test mode positioning.");
        }

        // Move the Chest to (-10.765, 1.152, -13.452) with rotation (0, 135.14, 0)
        if (chest != null)
        {
            chest.transform.position = new Vector3(-10.765f, 1.152f, -13.452f);
            chest.transform.rotation = Quaternion.Euler(0f, 135.14f, 0f);
            chest.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else
        {
            Debug.LogWarning("Chest reference not set in studyManager for test mode positioning.");
        }


        EnableTestCoinsOnly();
        // Still attach the chest event so deposit counting works if you want to test that
        if (chest != null)
        {
            chest.OnChestCountChanged += OnChestCoinCountChanged;
        }
        return;
        // -----------------------------------
    }

    private IEnumerator SetUpExperimentFirstPass() {
        AssignCoinsAtStart();

        // setup the current task to alway be object collection
        currentTask = TaskType.ObjectCollection;

        activeItems = (currentTask == TaskType.ObjectCollection) ? objectCollectionItems : monitoringItems;

        // all items start disabled.
        for (int i = 0; i < activeItems.Length; i++)
        {
            // NOTE: DISABLED FOR GETTING SCREENSHOTS
            if (activeItems[i].item != null) {
                activeItems[i].item.SetActive(false);
            }
        }

        // for the other task that is not the active one we need to disable the items
        StudyItem[] otherItems = (currentTask == TaskType.ObjectCollection) ? monitoringItems : objectCollectionItems;
        for (int i = 0; i < otherItems.Length; i++)
        {
            if (otherItems[i].item != null)
                otherItems[i].item.SetActive(false);
        }

        usedItems = new bool[activeItems.Length];

        // DATA LOG SETUP
        if (DataLogger.Instance != null)
        {
            DataLogger.Instance.SetupLogFile();
        }

        if (chest != null)
        {
            chest.OnChestCountChanged += OnChestCoinCountChanged;
        }
        else
        {
            Debug.Log("chest reference not set in the manager there is an issue registering the event of when the coin is put in the chest.");
        }


        WorldTargetManager.Instance.TurnOffWIMs();
        TransitionUIManager.Instance.WIMObjects.SetActive(false);

        WorldTargetManager.Instance.InitWorldTargets();

        latinSquareUtil.InitConditionsLatinSquare();
        latinSquareUtil.SetConditionOrderForPID(int.Parse(participantID));
        CurrentBlock = 0;
        if (breakBeforeSettingTechniqueInstructions) {
            Debug.Break();
        }
        yield return null;
        yield return null;
        yield return null;
        currentTechnique = (TransitionUIType)latinSquareUtil.Conditions[CurrentBlock].UIType;
        
        // write the technique chosen to the transition Ui manager
        TransitionUIManager.Instance.SetTransitionType(currentTechnique);
        TransitionUIManager.Instance.HandleWorldTransitionUITypeChanged();
        Debug.Log($"Transition UI Type set to match the selected technique: {currentTechnique}");

        SetUpTraining();
    }

    public void SetUpTraining() {
        XRComponents.Instance.CoinAnchorMeshRenderer.enabled = false;

        if (currentTechnique == TransitionUIType.Portal_Gallery || currentTechnique == TransitionUIType.WIM_Gallery) {
            StudyConfigurationManager.Instance.GalleryTrainingComplete = false;
            XRComponents.Instance.InteractionLowerBound.GetComponent<MeshRenderer>().enabled = true;
            XRComponents.Instance.InteractionUpperBound.GetComponent<MeshRenderer>().enabled = true;
        } else {
            StudyConfigurationManager.Instance.GalleryTrainingComplete = true;
            XRComponents.Instance.InteractionLowerBound.GetComponent<MeshRenderer>().enabled = false;
            XRComponents.Instance.InteractionUpperBound.GetComponent<MeshRenderer>().enabled = false;
        }

        if (!StudyConfigurationManager.Instance.TechniqueTrainingComplete) {
            studyUIManager.SetUpTechniqueTrainingUI(currentTechnique);
            TransitionManager.Instance.SetLayerRecursively(XRComponents.Instance.StudyUIParent, LayerMask.NameToLayer("Default"));
            TransitionManager.Instance.SetLayerRecursively(trialBlockStartItem.item, LayerMask.NameToLayer("Default"));
            TransitionManager.Instance.SetLayerRecursively(chest.gameObject, LayerMask.NameToLayer("Default"));
        } else {
            XRComponents.Instance.StudyUIParent.SetActive(false);
            StudyConfigurationManager.Instance.GalleryTrainingComplete = true;
            NextTrial();
        }

        trialBlockStartItem.item.SetActive(true);
        trialBlockStartItem.item.transform.SetParent(trialBlockStartItem.originalParent);
        trialBlockStartItem.item.transform.localPosition = trialBlockStartItem.originalPosition;
        trialBlockStartItem.item.transform.localRotation = Quaternion.Euler(trialBlockStartItem.originalRotation);

        TransitionUIManager.Instance.gameObject.SetActive(true);
    }

    public void StartTrialBlock() {
        Debug.Log("Starting trial block.");
        StudyConfigurationManager.Instance.TechniqueTrainingComplete = true;
        StudyConfigurationManager.Instance.GalleryTrainingComplete = true;
        XRComponents.Instance.InteractionLowerBound.GetComponent<MeshRenderer>().enabled = false;
        XRComponents.Instance.InteractionUpperBound.GetComponent<MeshRenderer>().enabled = false;
        XRComponents.Instance.StudyUIParent.SetActive(false);
        XRComponents.Instance.CoinAnchorMeshRenderer.enabled = true;
        // XRComponents.Instance.CoinAnchor.GetComponentInChildren<Collider>().enabled = true;
        NextTrial();
    }

    private void Start()
    {

        if (StudyConfigurationManager.Instance.CurrentOperationMode == StudyConfigurationManager.OperationMode.Demo)
        {
            Debug.Log("=== DEMO ===");
            DemoManager.Instance.Init();
        }

        latinSquareUtil = GetComponent<LatinSquareUtil>();
        studyUIManager = GetComponent<StudyUIManager>();

        trialBlockStartItem = new StudyItem();
        trialBlockStartItem.item = GameObject.Find("TrialBlockStartItem");
        trialBlockStartItem.environmentIndex = -1;
        trialBlockStartItem.originalParent = trialBlockStartItem.item.transform.parent;
        trialBlockStartItem.originalPosition = trialBlockStartItem.item.transform.localPosition;
        trialBlockStartItem.originalRotation = trialBlockStartItem.item.transform.localRotation.eulerAngles;

        for (int i = 0; i < enviroments.Length; i++)
        {
            if (enviroments[i].activeSelf == false)
                enviroments[i].SetActive(true);
        }

        if (isTestMode) {
            SetUpTestMode();
            return;
        }

        DisableAllTestCoins();
        
        StartCoroutine(SetUpExperimentFirstPass());
    }

    /// <summary>
    /// Finds all "Collectable" coins that have "_test_" in their name, basically shiow them,
    /// and disables everything else
    /// </summary>
    private void EnableTestCoinsOnly()
    {
        var allCoins = GameObject.FindGameObjectsWithTag("Collectable");
        int enabledCount = 0;
        int disabledCount = 0;

        foreach (var coin in allCoins)
        {
            if (coin.name.Contains("_test_"))
            {
                coin.SetActive(true);
                enabledCount++;
            }
            else
            {
                coin.SetActive(false);
                disabledCount++;
            }
        }
        Debug.Log($"[Test Mode] Enabled {enabledCount} test coins. Disabled {disabledCount} non-test coins.");
    }

    private void DisableAllTestCoins()
    {
        var allCoins = GameObject.FindGameObjectsWithTag("Collectable");
        int disabledCount = 0;

        foreach (var coin in allCoins)
        {
            if (coin.name.Contains("_test_"))
            {
                coin.SetActive(false);
                disabledCount++;
            }
        }
        Debug.Log($"[NONE - Test Mode] Disabled {disabledCount} test coins.");
    }

    public GameObject currentCollectedItem = null;

    public void OnItemCollected(GameObject collectedItem)
    {
        // ---- test mode code -----
        if (isTestMode) return;
        // -------------------------


        for (int i = 0; i < activeItems.Length; i++)
        {
            if (activeItems[i].item == collectedItem)
            {
                //currentCollectedItem = collectedItem;
                usedItems[i] = true;
                // totalCollected++;
                Debug.Log($"Collected item #{chest.coinCount} from environment {activeItems[i].environmentIndex}");

                //if (DataLogger.Instance != null)
                //    DataLogger.Instance.LogGrabbedCoin(collectedItem);

                DataLogger.Instance.LogDepositedInChest(collectedItem);

                // arguably this is not needed anymore this if statement below
                if (DataLogger.Instance != null)
                {
                    if (currentTask == TaskType.ObjectCollection)
                    {
                        //DataLogger.Instance.LogGrabbedCoin(collectedItem);
                    }
                    else if (currentTask == TaskType.Monitoring)
                    {
                        DataLogger.Instance.LogSelectedFlyingDisk(collectedItem);
                    }
                }

                // if (currentTask == TaskType.ObjectCollection)
                //     continue;

                //currentTrial++; the below is jut for the monitoring task 
                // MG: NOTE: COMMENTED OUT TO DEBUG...
                if (chest.coinCount < 15)
                {
                    currentTrial++;
                    NextTrial();
                }
                else
                {
                    EndTask();
                }
                return;
            } 
            else {
                Debug.Log("Not active item");
            }
        }
    }

    private void OnChestCoinCountChanged(int newCount, GameObject depositedCoin)
    {
        //if (DataLogger.Instance != null && chest != null)
        //{
        //    GameObject lastDepositedCoin = chest.GetLastDepositedCoin(); // Assume chest has a method to get the last coin
        //    if (lastDepositedCoin != null)
        //    {
        //        DataLogger.Instance.LogDepositedInChest(lastDepositedCoin);
        //    }
        //}

        //if (DataLogger.Instance != null && depositedCoin != null)
        //{
        // DataLogger.Instance.LogDepositedInChest(depositedCoin);
        //}
        // // MG: NOTE: Commented out to debug...

        // if (chest.coinCount < 15)
        // {
        //     currentTrial++;
        //     NextTrial();
        // }
        // else
        // {
        //     EndTask();
        // }
        // // MG: NOTE: Commented out to debug...
        // if (newCount >= 15)
        // {
        //     EndTask();
        // }
    }

    public void NextTrial()
    {

        // Find candidates that are not used and from a different environment than last trial.
        List<int> candidateIndices = new List<int>();
        for (int i = 0; i < activeItems.Length; i++)
        {
            if (!usedItems[i] && activeItems[i].environmentIndex != lastEnvironment)
            {
                candidateIndices.Add(i);
            }
        }

        // If no candidate remains with a different environment, allow any unused item.
        if (candidateIndices.Count == 0)
        {
            for (int i = 0; i < activeItems.Length; i++)
            {
                if (!usedItems[i])
                    candidateIndices.Add(i);
            }
        }

        if (candidateIndices.Count == 0)
        {
            Debug.LogWarning("No available items for the next trial. Ending task.");
            EndTask();
            return;
        }

        int chosenIndex = candidateIndices[UnityEngine.Random.Range(0, candidateIndices.Count)];
        lastEnvironment = activeItems[chosenIndex].environmentIndex;

        if (activeItems[chosenIndex].item != null)
        {
            activeItems[chosenIndex].item.SetActive(true);
            if (activeItems[chosenIndex].wimItem != null) {
                activeItems[chosenIndex].wimItem.SetActive(true);
            }

            if (DataLogger.Instance != null)
            {
                DataLogger.Instance.LogSpawn(activeItems[chosenIndex].item);  // Log when coin is spawned
            }

            for (int i = 0; i < activeItems.Length; i++)
            {
                if (activeItems[i].item != activeItems[chosenIndex].item) {
                    activeItems[i].item.SetActive(false);
                    if (activeItems[i].wimItem != null) {
                        activeItems[i].wimItem.SetActive(false);
                    }
                }
            }
            CurrentItem = activeItems[chosenIndex];
            CurrentItem.item.gameObject.GetComponent<CollectableItem>().placedOnSphere = false;
            Debug.Log($"NextTrial -> Activating item from environment {lastEnvironment}");
        }
    }

    private void EndTask()
    {
        Debug.Log($"Task ended after collecting {chest.coinCount} items.");
        chest.PlayTaskBlockCompleteAudio();
        CurrentBlock++;
        currentTechnique = (TransitionUIType)latinSquareUtil.Conditions[CurrentBlock].UIType;

        // totalCollected = 0;
        currentTrial = 0;
        chest.ResetChest();

        StudyConfigurationManager.Instance.TechniqueTrainingComplete = false;
        XRComponents.Instance.StudyUIParent.SetActive(true);

        SetUpStudyBlock();
    }
}