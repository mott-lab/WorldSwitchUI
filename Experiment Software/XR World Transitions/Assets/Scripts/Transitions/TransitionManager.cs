using Flexalon;
using System.Collections;
using System.Drawing;
using Unity.XR.CoreUtils;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    private static TransitionManager _instance;

    public static TransitionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<TransitionManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<TransitionManager>();
                    singletonObject.name = typeof(TransitionManager).ToString() + " (Singleton)";
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

    public enum TransitionMode
    {
        FadeToBlack,
        Instantaneous,
        Swipe,
        GrowOutward
    }

    public enum SwipeDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    [SerializeField] private Renderer fadeQuadRenderer;
    [SerializeField] private GameObject swipePortal;
    [SerializeField] private TransitionMode transitionMode;
    public SwipeDirection LastSwipeDirection;
    [SerializeField] private bool transitioning;
    public bool Transitioning
    {
        get => transitioning;
        set => transitioning = value;
    }

    public void SetTransitionMode(TransitionMode mode)
    {
        transitionMode = mode;
    }

    /// <summary>
    /// Moves the specified world target to the user's position.
    /// </summary>
    /// <param name="worldTarget">The world target to move.</param>
    public void MoveWorldTargetToUser(WorldTarget worldTarget)
    {
        Vector3 diffFromWorldTargetPosToUserPos = XRComponents.Instance.XRRig.transform.position - worldTarget.transform.position;
        worldTarget.transform.parent.transform.position += diffFromWorldTargetPosToUserPos;

        XRComponents.Instance.XRRig.transform.rotation = worldTarget.transform.rotation;
    }

    /// <summary>
    /// Transitions to the specified world target using the defined fade mode.
    /// </summary>
    /// <param name="worldTarget">The target world object to transition to.</param>
    public void TransitionToWorldTarget(WorldTarget worldTarget)
    {
        if (worldTarget == WorldTargetManager.Instance.GetCurrentWorldTarget())
        {
            Debug.Log("Already at target world.");
            TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction.TransitionToState(InteractionHandler.GestureState.MenuClose);
            // TransitionUIManager.Instance.CurrentTransitionInterface.EnableInterfaceObject(false);
            return;
        }

        string logStr = "Confirm_Transition";
        string logEnv = worldTarget.name;
        DataLogger.Instance.LogInteraction(logStr, logEnv);

        // Update screenshot for world target before starting a transition
        // XRComponents.Instance.XRCamera.GetComponent<envScreenshots>().TakeScreenshot();

        switch (transitionMode)
        {
            case TransitionMode.FadeToBlack:
                StartCoroutine(FadeTransition(worldTarget));
                break;

            case TransitionMode.Instantaneous:
                WorldTargetManager.Instance.DeactivateCurrentWorldTarget();
                SwitchToWorldTarget(worldTarget);
                break;
            case TransitionMode.Swipe:
                StartCoroutine(SwipeTransition(worldTarget));
                break;
            case TransitionMode.GrowOutward:
                StartCoroutine(GrowOutwardTransition(worldTarget));
                break;
        }

        
    }

    /// <summary>
    /// Switches to the specified world target by deactivating the current world target,
    /// setting the new world target, and activating it. Also moves the world target to the user's position.
    /// </summary>
    /// <param name="worldTarget">The target world object to switch to.</param>
    public void SwitchToWorldTarget(WorldTarget worldTarget)
    {
        WorldTargetManager.Instance.SetCurrentWorldTarget(worldTarget);
        WorldTargetManager.Instance.ActivateCurrentWorldTarget();
        MoveWorldTargetToUser(worldTarget);
        worldTarget.TurnOnShadows();
    }

    /// <summary>
    /// Coroutine to handle the fade transition effect. This coroutine fades a quad's alpha value from 0 to 1,
    /// moves the world target to the user's position, and then fades the quad's alpha value back to 0.
    /// </summary>
    /// <param name="worldTarget">The target world object to transition to.</param>
    /// <returns>IEnumerator for coroutine.</returns>
    public IEnumerator FadeTransition(WorldTarget worldTarget)
    {
        
        // Fade a quad alpha value from 0 to 1
        yield return FadeToBlack(0.5f);

        SwapLayers(worldTarget);

        if (studyManager.Instance.CurrentItem.item != null) {
            studyManager.Instance.CurrentItem.item.layer = LayerMask.NameToLayer("Default");
        }

        // in home world
        if (worldTarget == WorldTargetManager.Instance.WorldTargets[0])
        {
            SetLayerRecursively(studyManager.Instance.chest.gameObject, LayerMask.NameToLayer("Default"));
            SetLayerRecursively(XRComponents.Instance.StudyUIParent, LayerMask.NameToLayer("Default"));
            SetLayerRecursively(studyManager.Instance.trialBlockStartItem.item, LayerMask.NameToLayer("Default"));
            studyManager.Instance.GetComponent<XRInteractorSetup>().EnableRightNearFarInteractor(true);
            studyManager.Instance.GetComponent<XRInteractorSetup>().EnableLeftNearFarInteractor(true);

            // TODO: idea: turn off the coin anchor in the Home env. turn it on when the task starts.
            if (!studyManager.Instance.GetComponent<StudyConfigurationManager>().TechniqueTrainingComplete)
            {
                XRComponents.Instance.CoinAnchorMeshRenderer.enabled = false; // enable when the task starts.
                // XRComponents.Instance.CoinAnchor.GetComponentInChildren<Collider>().enabled = false;
            }
            
            XRComponents.Instance.DemoUIParent.SetLayerRecursively(LayerMask.NameToLayer("UI"));
        }

        // disable transition interface when user arrives in env where the coin is. this is re-enabled when coin is collected.
        if (worldTarget == studyManager.Instance.CurrentItem.worldTarget && 
                !studyManager.Instance.CurrentItem.item.GetComponent<CollectableItem>().HasLoggedGrab &&
                studyManager.Instance.GetComponent<StudyConfigurationManager>().TechniqueTrainingComplete) {
            Debug.Log("In the environment with the coin. Disabling transition interface.");
            TransitionUIManager.Instance.gameObject.SetActive(false);
        }

        // Deactivate current world target
        WorldTargetManager.Instance.DeactivatePreviousWorldTarget();
        TransitionUIManager.Instance.CurrentTransitionInterface.EnableInterfaceObject(false);
        TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction.CurrentState = InteractionHandler.GestureState.MenuClose;

        TransitionUIManager.Instance.CurrentTransitionInterface.DisablePortalPreview();


        // Fade quad alpha back to 0
        yield return FadeToTransparent(0.5f);

        Transitioning = false;
    }

    public IEnumerator FadeToBlack(float duration)
    {
        //float duration = 0.3f;
        float elapsedTime = 0f;
        UnityEngine.Color color = fadeQuadRenderer.material.color;
        // delay to let the preview portal reach the user's position
        //yield return new WaitForSeconds(delayDuration - (duration));

        // Fade to black
        while (elapsedTime < duration)
        {
            color.a = Mathf.Lerp(0, 1, elapsedTime / duration);
            fadeQuadRenderer.material.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        color.a = 1;
        fadeQuadRenderer.material.color = color;

    }

    public IEnumerator FadeToTransparent(float duration)
    {
        //float duration = 0.5f;
        float elapsedTime = 0f;
        UnityEngine.Color color = fadeQuadRenderer.material.color;
        // Reset elapsed time

        // Fade back to transparent
        while (elapsedTime < duration)
        {
            color.a = Mathf.Lerp(1, 0, elapsedTime / duration);
            fadeQuadRenderer.material.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        color.a = 0;
        fadeQuadRenderer.material.color = color;
    }

    public void SetWorldTargetToPortalInsideAndActivate(WorldTarget worldTarget)
    {
        // if (worldTarget == null) {
        //     return;
        // }
        // Add the parent of the next world target to the layer "PortalInside"
        worldTarget.transform.parent.gameObject.SetActive(true);

        // Prevent shadows cast by worldTarget from showing
        worldTarget.TurnOffShadows();

        MoveWorldTargetToUser(worldTarget);
        SetLayerRecursively(worldTarget.transform.parent.gameObject, LayerMask.NameToLayer("PortalInside"));
    }

    public void SetWorldTargetToPortalOutsideAndDeactivate(WorldTarget worldTarget)
    {
        // if (worldTarget == null) return;
        SetLayerRecursively(worldTarget.transform.parent.gameObject, LayerMask.NameToLayer("PortalOutside"));
        worldTarget.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// Coroutine to handle the swipe transition effect. This coroutine activates the swipe portal,
    /// moves it to the user's position, switches the world target, and then deactivates the swipe portal.
    /// </summary>
    /// <param name="worldTarget">The target world object to transition to.</param>
    /// <returns>IEnumerator for coroutine.</returns>
    public IEnumerator SwipeTransition(WorldTarget worldTarget)
    {
        // Activate the swipe portal
        swipePortal.SetActive(true);
        //MovePortalToUser(); [Commented as I could not find this based on the commits in main]
        // Add the parent of the next world target to the layer "PortalInside"
        worldTarget.transform.parent.gameObject.SetActive(true);
        MoveWorldTargetToUser(worldTarget);
        SetLayerRecursively(worldTarget.transform.parent.gameObject, LayerMask.NameToLayer("PortalInside"));

        // Add the parent of the current world target to the layer "PortalOutside"
        SetLayerRecursively(WorldTargetManager.Instance.GetCurrentWorldTarget().transform.parent.gameObject, LayerMask.NameToLayer("PortalOutside"));

        // Move the swipe portal 2.5 meters to the left of the user's position
        swipePortal.transform.position = XRComponents.Instance.XRRig.transform.position + Vector3.left * 2.5f;

        // Rotate the swipe portal so that it is facing the user
        swipePortal.transform.LookAt(XRComponents.Instance.XRRig.transform);

        // Move the swipe portal to the user's position over 2 seconds
        Transform player = XRComponents.Instance.XRRig.transform;
        swipePortal.transform.LookAt(swipePortal.transform.position + player.right, Vector3.up);
        // swipePortal.transform.position = player.position - player.right * 2.5f;
        yield return MovePortal(swipePortal,
            player.position - player.right * distanceToSwipePortal,
            player.position + player.right * distanceToSwipePortal,
            2.0f,
            worldTarget);
        
        
        // yield return MoveObjectToPosition(swipePortal, XRComponents.Instance.XRRig.transform.position, 2.0f);
        //
        // // When the swipe portal reaches the user's camera position, and if the user has not already switched worlds:
        // if (WorldTargetManager.Instance.GetCurrentWorldTarget() != worldTarget)
        // {
        //
        //     // Add the parent of the next world target to the layer "PortalOutside"
        //     SetLayerRecursively(worldTarget.transform.parent.gameObject, LayerMask.NameToLayer("PortalOutside"));
        //
        //     // Add the parent of the current world target to the layer "PortalInside"
        //     SetLayerRecursively(WorldTargetManager.Instance.GetCurrentWorldTarget().transform.parent.gameObject, LayerMask.NameToLayer("PortalInside"));
        //
        //     // Switch to the next world target
        //     SwitchToWorldTarget(worldTarget);
        //
        //     // Rotate the swipe portal 180 degrees so that it is facing the user
        //     // swipePortal.transform.Rotate(0, 180, 0);
        //
        //     // Move the swipe portal to the right of the user's position over 2 seconds
        //     yield return MoveObjectToPosition(swipePortal, player.position + Vector3.right * 2.5f, 2.0f);
        // }

        // Deactivate the swipe portal
        swipePortal.SetActive(false);

        // Deactivate current world target
        WorldTargetManager.Instance.DeactivatePreviousWorldTarget();
    }

    /// <summary>
    /// Performs a grow outward transition for the specified world target, 
    /// where the target's menu item scales up and moves toward the user.
    /// </summary>
    /// <param name="worldTarget">The world target to transition to.</param>
    /// <returns>An IEnumerator for coroutine handling.</returns>
    public IEnumerator GrowOutwardTransition(WorldTarget worldTarget)
    {
        WorldTargetMenuItem selectedWorldTargetMenuItem = TransitionUIManager.Instance.CurrentTransitionInterface.SelectedWorldTargetMenuItem;
        // turn off other world target menu items
        foreach (WorldTargetMenuItem item in TransitionUIManager.Instance.CurrentTransitionInterface.MenuItems)
        {
            if (item != selectedWorldTargetMenuItem)
            {
                item.TextCanvas.SetActive(false);
                item.StencilCanvas.SetActive(false);
                item.ScreenshotCanvas.SetActive(false);
                item.ItemCube.SetActive(false);
            }
        }

        // scale up world target preview menu item to 4.2
        StartCoroutine(ScaleMenuItemOverTime(selectedWorldTargetMenuItem.GetComponent<FlexalonObject>(), new Vector3(4.2f, 4.2f, 4.2f), 1.0f));

        // Move the preview portal to the user's position over 2 seconds
        Vector3 targetPos = XRComponents.Instance.XRCamera.transform.position - XRComponents.Instance.XRCamera.transform.forward * 0.02f;
        float moveDuration = 2.0f;
        //StartCoroutine(FadeToBlack(moveDuration));
        yield return MoveObjectToPosition(selectedWorldTargetMenuItem.gameObject, targetPos, moveDuration);

        SwapLayers(worldTarget);

        // Deactivate current world target
        WorldTargetManager.Instance.DeactivatePreviousWorldTarget();
        TransitionUIManager.Instance.CurrentTransitionInterface.EnableInterfaceObject(false);

        selectedWorldTargetMenuItem.GetComponent<FlexalonObject>().Scale = new Vector3(1f, 1f, 1f);

        foreach (WorldTargetMenuItem item in TransitionUIManager.Instance.CurrentTransitionInterface.MenuItems)
        {
            if (item != selectedWorldTargetMenuItem)
            {
                item.TextCanvas.SetActive(true);
                item.StencilCanvas.SetActive(true);
                item.ScreenshotCanvas.SetActive(true);
                item.ItemCube.SetActive(true);
            }
        }
    }


    private IEnumerator ScaleMenuItemOverTime(FlexalonObject obj, Vector3 targetScale, float duration)
    {
        Vector3 initialScale = obj.Scale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            obj.Scale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.Scale = targetScale;
    }

    /// <summary>
    /// Recursively sets the layer of the specified game object and all its children.
    /// </summary>
    /// <param name="obj">The game object to set the layer for.</param>
    /// <param name="newLayer">The new layer to set.</param>
    public void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public float distanceToSwipePortal = 5.0f;
    public AnimationCurve curve;
    /// <summary>
    /// Coroutine to move a game object to a target position over a specified duration.
    /// </summary>
    /// <param name="obj">The game object to move.</param>
    /// <param name="targetPosition">The target position to move the object to.</param>
    /// <param name="duration">The duration over which to move the object.</param>
    /// <returns>IEnumerator for coroutine.</returns>
    private IEnumerator MoveObjectToPosition(GameObject obj, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = obj.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = targetPosition;
    }

    private IEnumerator MovePortal(GameObject portal, Vector3 startPosition, Vector3 targetPosition, float duration, WorldTarget worldTarget)
    {
        Debug.Log("target position: " + targetPosition.ToString("F2"));
        float elapsedTime = 0f;
        if (curve != null)
        {
            duration = curve[curve.length - 1].time;
        }

        bool reachedHalf = false;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            portal.transform.position = Vector3.Lerp(startPosition, targetPosition, curve?.Evaluate(t) ?? t);
            elapsedTime += Time.deltaTime;
            if (t >= 0.5f && !reachedHalf)
            {
                reachedHalf = true;
                SwapLayers(worldTarget);
            }
            yield return null;
        }

        portal.transform.position = targetPosition;
    }

    private void MovePortalToUser()
    {
        swipePortal.transform.position = XRComponents.Instance.XRRig.transform.position;
        swipePortal.transform.LookAt(XRComponents.Instance.XRRig.transform);
    }

    private void SwapLayers(WorldTarget worldTarget)
    {
        if (WorldTargetManager.Instance.GetCurrentWorldTarget() != worldTarget)
        {

            // Add the parent of the next world target to the layer "PortalOutside"
            SetLayerRecursively(worldTarget.transform.parent.gameObject, LayerMask.NameToLayer("PortalOutside"));

            // Add the parent of the current world target to the layer "PortalInside"
            SetLayerRecursively(WorldTargetManager.Instance.GetCurrentWorldTarget().transform.parent.gameObject, LayerMask.NameToLayer("PortalInside"));

            // Switch to the next world target
            SwitchToWorldTarget(worldTarget);
        }
    }
    
}