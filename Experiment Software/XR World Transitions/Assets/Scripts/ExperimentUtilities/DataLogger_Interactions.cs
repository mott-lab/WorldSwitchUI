using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataLogger_Interactions : MonoBehaviour
{
    public static DataLogger_Interactions Instance;

    [Header("Participant Info")]
    [Tooltip("Enter the Participant ID")]
    public string participantID = ""; // todo edit from the study manager

    [Tooltip("Current Task (set by studyManager)")]
    public string taskType = "";

    [Tooltip("Current Transition Technique used")]
    public string techniquename = "";

    [Header("Log File Settings")]
    [Tooltip("Name of the CSV file to create")]
    public string fileName = "_.csv"; // todo edit from the study manager

    private string folderPath;
    private string filePath;
    private StreamWriter writer;

    private Dictionary<int, float> spawnTimes = new Dictionary<int, float>();
    private Dictionary<int, float> collectedTimes = new Dictionary<int, float>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupLogFile();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupLogFile()
    {
        folderPath = Path.Combine(Application.dataPath, "Participants", participantID);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        fileName = System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fileName;
        filePath = Path.Combine(folderPath, fileName);
        writer = new StreamWriter(filePath, false);
        writer.WriteLine("Timestamp,ParticipantID,TaskType,Technique,Trial,Event");
        writer.Flush();
        Debug.Log("DataLogger initialized. Logging to: " + filePath);
    }

    private void WriteLogLine(float timestamp, string eventType, string field1)
    {
       
        int trialIndex = 0;
        studyManager sm = FindAnyObjectByType<studyManager>();
        if (sm != null)
        {
            trialIndex = sm.currentTrial; 
        }

        // Format: Timestamp,ParticipantID,TaskType,Technique,Trial,CoinName,Event,Duration
        string logLine = string.Format("{0:F2},{1},{2},{3},{4},{5},{6}",
                            timestamp, participantID, taskType, techniquename, trialIndex, eventType, field1);
        writer.WriteLine(logLine);
        writer.Flush();
        Debug.Log("Logged: " + logLine);
    }

    public void LogInteraction(string eventType, string field1)
    {
        WriteLogLine(Time.time, eventType, field1);
    }

    private void OnDestroy()
    {
        if (writer != null)
        {
            writer.Close();
        }
    }
}
