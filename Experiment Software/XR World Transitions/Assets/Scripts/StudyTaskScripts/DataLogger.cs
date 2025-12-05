using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
// using VERA;

public class DataLogger : MonoBehaviour
{
    public static DataLogger Instance;

    [Header("Participant Info")]
    // [Tooltip("Enter the Participant ID")]
    private string participantID = ""; // todo edit from the study manager

    // [Tooltip("Current Task (set by studyManager)")]
    private string taskType = "";

    // [Tooltip("Current Transition Technique used")]
    private string techniquename = "";

    [Header("Log File Settings")]
    [Tooltip("Name of the CSV file to create")]
    public string fileName = "_.csv"; // todo edit from the study manager

    private string folderPath;
    private string filePath;
    private string trackingFilePath;
    private StreamWriter writer;
    private StreamWriter trackingWriter; 
    private StreamWriter interactionWriter;
    // private XRComponents xrComponents;


    private Dictionary<int, float> spawnTimes = new Dictionary<int, float>();
    private Dictionary<int, float> collectedTimes = new Dictionary<int, float>();

    // log hmd and hand positions and rotations
    // private XRComponents xrComponents;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // SetupLogFile();
        }
        else
        {
            Destroy(gameObject);
        }

        participantID = studyManager.Instance.participantID;
        taskType = studyManager.Instance.currentTask.ToString();
        techniquename = studyManager.Instance.currentTechnique.ToString();
    }

    private string GetTransformData(Transform t)
    {
        if (t == null)
            return "N/A,N/A,N/A,N/A,N/A,N/A,N/A";  // Prevent logging errors when reference is missing

        Vector3 pos = t.localPosition;
        Quaternion rot = t.localRotation;

        return string.Format("{0:F4},{1:F4},{2:F4},{3:F4},{4:F4},{5:F4},{6:F3}",
                             pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, rot.w);
    }

    // private Transform FindPalm(GameObject handRoot)
    // {
    //     if (handRoot == null) return null;

    //     foreach (Transform child in handRoot.GetComponentsInChildren<Transform>())
    //     {
    //         if (child.name.ToLower().Contains("palm"))
    //         {
    //             return child;  // Return the first palm joint found
    //         }
    //     }
    //     return null; // Return null if no palm joint is found
    // }

    private void Update()
    {
        if (trackingWriter == null) return;
        // if (StudyConfigurationManager.Instance.CurrentOperationMode == StudyConfigurationManager.OperationMode.Demo) return;

        // GameObject hmdObject = GameObject.Find("Main Camera");
        // GameObject leftHandRoot = GameObject.Find("LeftHandDebugDrawJoints");
        // GameObject rightHandRoot = GameObject.Find("RightHandDebugDrawJoints");

        // Find the palm joints for both hands
        // Transform leftPalm = FindPalm(leftHandRoot);
        // Transform rightPalm = FindPalm(rightHandRoot);

        GameObject hmdObject = XRComponents.Instance.XRCamera?.gameObject;
        Transform leftWrist = XRComponents.Instance.wristL != null ? XRComponents.Instance.wristL.transform : null;
        Transform rightWrist = XRComponents.Instance.wristR != null ? XRComponents.Instance.wristR.transform : null;

        // string currentTimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string currentTimeStamp = Time.time.ToString("F4");
        string hmdData = GetTransformData(hmdObject?.transform);
        string leftHandData = "";
        string rightHandData = "";
        if (TransitionUIManager.Instance.InteractionManager.XRInteractorSetup.CurrentInteractionMode == XRInteractorSetup.InteractionMode.Hands)
        {
            leftHandData = GetTransformData(leftWrist);
            rightHandData = GetTransformData(rightWrist);
        }
        else
        {
            leftHandData = GetTransformData(XRComponents.Instance.LeftController.transform);
            rightHandData = GetTransformData(XRComponents.Instance.RightController.transform);
        }

        // Format the tracking log line
        string trackingLogLine = string.Format("{0},{1},{2},{3},{4}",
                                                currentTimeStamp, hmdData, leftHandData, rightHandData, TransitionUIManager.Instance.InteractionManager.XRInteractorSetup.CurrentInteractionMode);

        trackingWriter.WriteLine(trackingLogLine);
        trackingWriter.Flush();
    }


    // private void SetupTrackingReferences()
    // {
    //     xrComponents = XRComponents.Instance;

    //     while (xrComponents == null)
    //     {

    //         xrComponents = XRComponents.Instance;
    //         Debug.LogWarning("XRComponents instance not found! HMD and Hand tracking will not be logged.");
    //     }
    // }

    public void SetupLogFile()
    {
        fileName = $"{participantID}_{techniquename}_{taskType}.csv";
        fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fileName;

        folderPath = Path.Combine(Application.dataPath, "Participants", participantID);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Prevent overwriting the experiment data file
        string baseFileName = Path.GetFileNameWithoutExtension(fileName);
        string extension = Path.GetExtension(fileName);
        if (string.IsNullOrEmpty(extension))
        {
            extension = ".csv";
        }

        string finalFileName = baseFileName + extension;
        string finalFilePath = Path.Combine(folderPath, finalFileName);

        filePath = finalFilePath;
        writer = new StreamWriter(filePath, false);
        writer.WriteLine("Timestamp,ParticipantID,TaskType,Technique,TaskBlock,Trial,CoinName,Event,Duration");
        writer.Flush();

        // ----------------- Tracking Data -----------------
        string trackingBaseFileName = $"{participantID}_tracking_data";
        string trackingExtension = ".csv";
        string finalTrackingFileName = trackingBaseFileName + trackingExtension;
        finalTrackingFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + finalTrackingFileName;
        string finalTrackingFilePath = Path.Combine(folderPath, finalTrackingFileName);

        trackingWriter = new StreamWriter(finalTrackingFilePath, false);
        trackingWriter.WriteLine("Timestamp,HMD_PosX,HMD_PosY,HMD_PosZ,HMD_RotX,HMD_RotY,HMD_RotZ,HMD_RotW,LeftHand_PosX,LeftHand_PosY,LeftHand_PosZ,LeftHand_RotX,LeftHand_RotY,LeftHand_RotZ,LeftHand_RotW,RightHand_PosX,RightHand_PosY,RightHand_PosZ,RightHand_RotX,RightHand_RotY,RightHand_RotZ,RightHand_RotW,CurrentInteractionMode");
        trackingWriter.Flush();

        // ----------------- Interaction Data -----------------
        string interactionBaseFileName = $"{participantID}_interactions";
        string interactionExtension = ".csv";
        string finalInteractionFilePath = interactionBaseFileName + interactionExtension;
        finalInteractionFilePath = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + finalInteractionFilePath;
        finalInteractionFilePath = Path.Combine(folderPath, finalInteractionFilePath);

        interactionWriter = new StreamWriter(finalInteractionFilePath, false);
        interactionWriter.WriteLine("Timestamp,ParticipantID,TaskType,Technique,Trial,Event,CurrentInteractionMode");
        interactionWriter.Flush();

        Debug.Log($"DataLogger initialized. Logging experiment data to: {filePath}");
        Debug.Log($"DataLogger initialized. Logging tracking data to: {finalTrackingFilePath}");
        Debug.Log($"DataLogger initialized. Logging interaction data to: {finalInteractionFilePath}");
    }

    private void WriteLogLine(float timestamp, string coinName, string eventType, float duration)
    {
        // if (StudyConfigurationManager.Instance.CurrentOperationMode == StudyConfigurationManager.OperationMode.Demo) return;
        
        // Convert duration from seconds to min
        // float durationMinutes = duration / 60f;
        // Get the current system time as a formatted string
        string currentTimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        int trialIndex = 0;
        if (studyManager.Instance != null)
        {
            trialIndex = studyManager.Instance.currentTrial; 
        }

        string logLine = string.Format("{0:F4},{1},{2},{3},{4},{5},{6},{7},{8:F2},{9}",
                            timestamp, participantID, taskType, TransitionUIManager.Instance.WorldTransitionUIType, studyManager.Instance.CurrentBlock, trialIndex + 1, coinName, eventType, duration, TransitionUIManager.Instance.InteractionManager.XRInteractorSetup.CurrentInteractionMode);
        writer.WriteLine(logLine);
        writer.Flush();
        Debug.Log("Logged: " + logLine);
    }

    private void WriteInteractionLogLine(float timestamp, string eventType, string field1)
    {
       
        int trialIndex = 0;
        if (studyManager.Instance != null)
        {
            trialIndex = studyManager.Instance.currentTrial; 
        }

        // Format: Timestamp,ParticipantID,TaskType,Technique,Trial,CoinName,Event,Duration
        string logLine = string.Format("{0:F4},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                            timestamp, participantID, taskType, TransitionUIManager.Instance.WorldTransitionUIType, studyManager.Instance.CurrentBlock, trialIndex + 1, "", eventType, field1, "", TransitionUIManager.Instance.InteractionManager.XRInteractorSetup.CurrentInteractionMode);
        // interactionWriter.WriteLine(logLine);
        // interactionWriter.Flush();
        writer.WriteLine(logLine);
        writer.Flush();
        Debug.Log("Logged: " + logLine);

        int eventInt = 0;
        if (!string.IsNullOrEmpty(eventType))
        {
            foreach (char c in eventType)
            {
                eventInt += (int)c;
            }
        }

        // VERAFile_InteractionLog.CreateCsvEntry(eventInt, field1);
    }

    public void LogInteraction(string eventType, string field1)
    {
        WriteInteractionLogLine(Time.time, eventType, field1);
    }

    /// <summary>
    /// Log when a coin/disk is spawned.
    /// </summary>
    public void LogSpawn(GameObject coin)
    {
        int id = coin.GetInstanceID();
        float timeNow = Time.time;
        spawnTimes[id] = timeNow;
        WriteLogLine(timeNow, coin.name, "SpawnedCoin", 0f);
    }

    /// <summary>
    /// Log when a coin/disk is collected (for ObjectCollection, when the coin is grabbed).
    /// </summary>
    ///

    //public void LogGrabbedCoin(GameObject coin)
    //{
    //    int id = coin.GetInstanceID();
    //    float timeNow = Time.time;

    //    if (spawnTimes.ContainsKey(id))
    //    {
    //        float duration = timeNow - spawnTimes[id];
    //        collectedTimes[id] = timeNow;
    //        timeStampOfGrab = timeNow;
    //        WriteLogLine(timeNow, coin.name, "GrabbedCoin", duration);
    //    }
    //}

    public float timeStampOfGrab;
    /// <summary>
    /// Log when a coin is deposited in the chest (for ObjectCollection).
    /// </summary>
    //public void LogDepositedInChest(GameObject coin)
    //{
    //    Debug.Log("called this crap");
    //    int id = coin.GetInstanceID();
    //    float timeNow = Time.time;
    //    float durationSeconds = timeNow - timeStampOfGrab;

    //    //// Try to get the coin's collection time
    //    //if (collectedTimes.TryGetValue(id, out float collectedTime))
    //    //{
    //    //    duration = timeNow - collectedTime;
    //    //}
    //    //else if (spawnTimes.TryGetValue(id, out float spawnTime))
    //    //{
    //    //    // Fall back to using the spawn time if it wasn't logged as collected
    //    //    duration = timeNow - spawnTime;
    //    //}
    //    //else
    //    //{
    //    //    Debug.LogWarning("Coin instance not found in collectedTimes or spawnTimes for deposit logging.");
    //    //}

    //    WriteLogLine(timeNow, coin.name, "DepositedInChest", (float)durationSeconds);
    //}

    //public void LogDepositedInChest(GameObject coin)
    //{
    //    int id = coin.GetInstanceID();
    //    float timeNow = Time.time;
    //    float duration = 0f;

    //    // Try to get the coin's grab time from the dictionary.
    //    if (collectedTimes.TryGetValue(id, out float grabTime))
    //    {
    //        duration = timeNow - grabTime;
    //        // Optionally remove the coin's grab time after logging to prevent reuse.
    //        collectedTimes.Remove(id);
    //    }
    //    else if (spawnTimes.TryGetValue(id, out float spawnTime))
    //    {
    //        // Fall back to using the spawn time if the coin wasn't logged as grabbed.
    //        duration = timeNow - spawnTime;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Coin instance not found in collectedTimes or spawnTimes for deposit logging.");
    //    }

    //    WriteLogLine(timeNow, coin.name, "DepositedInChest", duration);
    //}

    /// <summary>
    /// Log when a coin/disk is collected (for ObjectCollection, when the coin is grabbed).
    /// </summary>
    public void LogGrabbedCoin(GameObject coin)
    {
        if (coin == null) return;

        int id = coin.GetInstanceID();
        float timeNow = Time.time;

        // Store the grab time in the dictionary
        collectedTimes[id] = timeNow;

        // Calculate duration from spawn to grab
        float duration = 0f;
        if (spawnTimes.ContainsKey(id))
        {
            duration = timeNow - spawnTimes[id];
        }
        timeStampOfGrab = timeNow;
        WriteLogLine(timeNow, coin.name, "GrabbedCoin", duration);
        Debug.Log($"Grabbed coin with ID: {id}, at time: {timeNow}");
    }

    /// <summary>
    /// Log when a coin is deposited in the chest (for ObjectCollection).
    /// </summary>
    public void LogDepositedInChest(GameObject coin)
    {
        if (coin == null) return;

        int id = coin.GetInstanceID();
        float timeNow = Time.time;
        float duration = 0f;

        Debug.Log($"Depositing coin with ID: {id}, at time: {timeNow}");

        // Try to get the coin's grab time from the dictionary
        if (collectedTimes.TryGetValue(id, out float grabTime))
        {
            duration = timeNow - timeStampOfGrab;
            Debug.Log($"Found grab time: {grabTime}, duration: {duration}");
        }
        else
        {
            // If grab time not found, fall back to spawn time
            if (spawnTimes.TryGetValue(id, out float spawnTime))
            {
                duration = timeNow - timeStampOfGrab;
                Debug.Log($"No grab time found, using spawn time: {spawnTime}, duration: {duration}");
            }
            else
            {
                Debug.LogWarning($"Coin instance {id} ({coin.name}) not found in collectedTimes or spawnTimes for deposit logging.");
            }
        }

        WriteLogLine(timeNow, coin.name, "DepositedInChest", duration);

        spawnTimes.Remove(id);
        collectedTimes.Remove(id);
    }

    /// <summary>
    /// Log when a coin/disk is selected (for Monitoring; time from spawn to selection).
    /// </summary>
    public void LogSelectedFlyingDisk(GameObject coin)
    {
        int id = coin.GetInstanceID();
        float timeNow = Time.time;
        if (spawnTimes.ContainsKey(id))
        {
            float duration = timeNow - spawnTimes[id];
            WriteLogLine(timeNow, coin.name, "DiskSelected", duration);
        }
    }

    private void OnDestroy()
    {
        if (writer != null)
        {
            writer.Close();
        }
        if (trackingWriter != null)
        {
            trackingWriter.Close();
        }
        if (interactionWriter != null)
        {
            interactionWriter.Close();
        }
    }
}