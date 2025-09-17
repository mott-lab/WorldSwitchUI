using Flexalon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages the world targets in the game, including their activation and deactivation.
/// </summary>
public class WorldTargetManager : MonoBehaviour
{
    private static WorldTargetManager _instance;

    public static WorldTargetManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindFirstObjectByType<WorldTargetManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<WorldTargetManager>();
                    singletonObject.name = typeof(WorldTargetManager).ToString() + " (Singleton)";
                }
            }
            return _instance;
        }
    }

    [SerializeField] private WorldTarget[] worldTargets;
    public WorldTarget[] WorldTargets { get => worldTargets; }
    [SerializeField] private WorldTarget currentWorldTarget;
    [SerializeField] private WorldTarget previousWorldTarget;

    /// <summary>
    /// At Awake(), initializes the singleton instance and ensures it persists across scenes.
    /// </summary>
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

    /// <summary>
    /// At Start(), initializes the world targets and sets the first one as the current target.
    /// TODO: This should probably be done from a more coordinated Init() process.
    /// </summary>
    public void InitWorldTargets()
    {
        //StartCoroutine(InitializeWorldTargets());
        for (int i = 0; i < worldTargets.Length; i++)
        {
            // get parent object of world target
            GameObject parentObject = worldTargets[i].transform.parent.gameObject;
            // get all children of parent object
            Collider[] childColliders = parentObject.GetComponentsInChildren<Collider>();
            // disable all colliders on children
            foreach (Collider collider in childColliders)
            {
                if (!collider.gameObject.CompareTag("Collectable") & !collider.gameObject.CompareTag("Chest") & !collider.gameObject.CompareTag("TrialBlockStartItem")) {
                    collider.enabled = false;
                }
            }
            
            NavMeshObstacle[] navMeshObstacles = parentObject.GetComponentsInChildren<NavMeshObstacle>();
            // disable all nav mesh obstacles on children
            foreach (NavMeshObstacle navMeshObstacle in navMeshObstacles)
            {
                navMeshObstacle.enabled = false;
            }

            // populate list of renderers for each world target. These are used for shadow de/activation.
            worldTargets[i].FindAllRenderers();
            // turn off all WIM shadows
            worldTargets[i].TurnOffWIMShadows();

            Debug.Log($"Index: {i}, WorldTarget Name: {worldTargets[i].Name}, WorldTarget Position: {worldTargets[i].transform.position}");

            foreach (TransitionInterface transitionInterface in TransitionUIManager.Instance.TransitionInterfaces)
            {
                transitionInterface.AddWorldTargetMenuItemToLayout(worldTargets[i]);
                
            }

            // Activate the world target...
            DeactivateCurrentWorldTarget();
            SetCurrentWorldTarget(worldTargets[i]);
            ActivateCurrentWorldTarget();
            TransitionManager.Instance.MoveWorldTargetToUser(currentWorldTarget);
            // take initial screenshot of it
            XRComponents.Instance.XRCamera.GetComponent<envScreenshots>().TakeScreenshot();
        }

        // Start at first one
        StartAtFirstWorldTarget();
    }

    public void StartAtFirstWorldTarget()
    {
        DeactivateAllWorlds();
        SetCurrentWorldTarget(worldTargets[0]);
        ActivateCurrentWorldTarget();
        TransitionManager.Instance.MoveWorldTargetToUser(currentWorldTarget);
        TransitionManager.Instance.SetLayerRecursively(currentWorldTarget.transform.parent.gameObject, LayerMask.NameToLayer("PortalOutside"));
        TransitionManager.Instance.SetLayerRecursively(XRComponents.Instance.StudyUIParent, LayerMask.NameToLayer("Default"));
    }

    public void TurnOffWIMs() {
        foreach (WorldTarget worldTarget in worldTargets)
        {
            worldTarget.TurnOffWIMObjects();
        }
    }

    /// <summary>
    /// When user clicks Reset on script in the Unity Editor, refresh the world target references and menu item prefab references.
    /// </summary>
    private void Reset()
    {
        worldTargets = GameObject.FindObjectsByType<WorldTarget>(FindObjectsSortMode.None);
        
    }

    /// <summary>
    /// Gets the current world target.
    /// </summary>
    /// <returns>The current world target.</returns>
    public WorldTarget GetCurrentWorldTarget()
    {
        return currentWorldTarget;
    }

    /// <summary>
    /// Sets the current world target.
    /// </summary>
    /// <param name="worldTarget">The world target to set as current.</param>
    public void SetCurrentWorldTarget(WorldTarget worldTarget)
    {
        previousWorldTarget = currentWorldTarget;
        currentWorldTarget = worldTarget;
    }

    /// <summary>
    /// Gets the previous world target.
    /// </summary>
    /// <returns>The previous world target.</returns>
    public WorldTarget GetPreviousWorldTarget()
    {
        return previousWorldTarget;
    }

    ///// <summary>
    ///// Sets the previous world target.
    ///// </summary>
    ///// <param name="worldTarget">The world target to set as previous.</param>
    //public void SetPreviousWorldTarget(WorldTarget worldTarget)
    //{
    //    previousWorldTarget = worldTarget;
    //}

    /// <summary>
    /// Deactivates all world targets' objects.
    /// </summary>
    public void DeactivateAllWorlds()
    {
        foreach (WorldTarget worldTarget in worldTargets)
        {
            worldTarget.transform.parent.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Activates the current world target.
    /// </summary>
    public void ActivateCurrentWorldTarget()
    {
        currentWorldTarget.transform.parent.gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivates the current world target.
    /// </summary>
    public void DeactivateCurrentWorldTarget()
    {
        if (currentWorldTarget != null)
            currentWorldTarget.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// Activates the previous world target.
    /// </summary>
    public void ActivatePreviousWorldTarget()
    {
        previousWorldTarget.transform.parent.gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivates the previous world target.
    /// </summary>
    public void DeactivatePreviousWorldTarget()
    {
        if (previousWorldTarget != null)
        {
            // Debug.Log("Deactivating previous world...");
            previousWorldTarget.transform.parent.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    void Update()
    {

    }
}
