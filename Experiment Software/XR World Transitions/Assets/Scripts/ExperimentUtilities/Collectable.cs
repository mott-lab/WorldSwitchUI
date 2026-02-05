using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CollectableItem : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    private bool isHeld = false;
    public bool IsHeld => isHeld;

    // Cache the Rigidbody for resetting velocity on release.
    private Rigidbody rb;


    // get teh transform of where the coin sshould be placed on the body of the user
    private Transform coinAnchorTransform;


    public bool placedOnSphere = false;

    public bool HasLoggedGrab = false;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        // Disable throwing on detach to prevent coins from rolling away accidentally.
        throwOnDetach = false;


        // Find the coinAnchorOnHand
        GameObject coinAnchor = GameObject.Find("coinAnchorOnHand");
        if (coinAnchor != null)
        {
            coinAnchorTransform = coinAnchor.transform;
        }
        else
        {
            Debug.LogWarning("coinAnchorOnHand not found! Ensure it exists in the scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (placedOnSphere) return;
        if (CompareTag("TrialBlockStartItem")) {
            return;
        }

        // Check if the coin collides with CoinSnapSphere
        if (other.gameObject.name == "Sphere_anchor")
        {
            rb.isKinematic = false;
            // Debug.Log("Coin collided with snap sphere!");

            // add outline effect to coin
            GetComponent<Outline>().enabled = true;

            // Snap the coin to the CoinSnapSphere position
            if (coinAnchorTransform != null)
            {
                transform.SetParent(coinAnchorTransform);
                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = new Vector3(90f, 0, 0);
                rb.isKinematic = true; // Stop physics
                if (!placedOnSphere & !HasLoggedGrab) {
                    Debug.Log("Coin placed on sphere. Re-enabling transition ui manager.");
                    DataLogger.Instance.LogGrabbedCoin(this.gameObject);
                    HasLoggedGrab = true;
                    TransitionUIManager.Instance.gameObject.SetActive(true);
                    studyManager.Instance.GetComponent<XRInteractorSetup>().EnableRightNearFarInteractor(false);
                    studyManager.Instance.GetComponent<XRInteractorSetup>().EnableLeftNearFarInteractor(false);
                }
                placedOnSphere = true;
            }
            else
            {
                Debug.LogError("CoinSnapSphere transform is missing!");
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        // Debug.Log("on trigger exit");
        GetComponent<Outline>().enabled = false;
        if (other.gameObject.name == "Sphere_anchor" && placedOnSphere) {
            // Debug.Log("Exiting sphere holder");
            placedOnSphere = false;
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // if it is on the sphere, do not allow it to be selected until user is in home environment
        // if (placedOnSphere) {
        //     if (WorldTargetManager.Instance.GetCurrentWorldTarget().Name != "Home") {
        //         Debug.LogWarning("Coin is on sphere. Cannot be selected until user is in Home.");
        //         return;
        //     }
        // }

        Debug.Log("in selected entered now");

        TransitionUIManager.Instance.CoinAnchorController.UpdateCoinAnchorPosition(gameObject.transform);

        base.OnSelectEntered(args);
        rb.isKinematic = false;
        isHeld = true;

        // coin is held, disable transition ui manager so user cannot accidentally trigger a transition
        TransitionUIManager.Instance.gameObject.SetActive(false);

        if (coinAnchorTransform != null)
        {
            transform.SetParent(null);
            transform.SetParent(coinAnchorTransform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else {
            Debug.Log("Could not place on the side of the user after selection");

        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log("select exited now");
        base.OnSelectExited(args);
        // Reset velocity so that if the coin is accidentally dropped, it remains in place.
        if (rb != null)
        {
            // rb.velocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        isHeld = false;
        rb.isKinematic = true;

        studyManager.Instance.chest.IsCollectableIntersecting();

        // coin is released, enable transition ui manager.
        // only do this if the coin has already been placed on the sphere and logged, 
        // or if the technique training is not complete, i.e., it is the starttrialblock coin
        if (HasLoggedGrab) {
            TransitionUIManager.Instance.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Called by the Chest when the coin is dropped into it.
    /// Notifies the studyManager and disables the coin.
    /// </summary>
    public void Collect()
    {
        // studyManager manager = FindAnyObjectByType<studyManager>();
        // if (manager != null)
        // {
        //     manager.OnItemCollected(gameObject);
        // }
        // else
        // {
        //     Debug.LogWarning("StudyManager not found in the scene.");
        // }
        studyManager.Instance.OnItemCollected(gameObject);
        gameObject.SetActive(false);
    }
}

// using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit;

// public class CollectableItem : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
// {
//     private bool isHeld = false;
//     public bool IsHeld => isHeld;

//     // Cache the Rigidbody for resetting velocity on release.
//     private Rigidbody rb;


//     // get teh transform of where the coin sshould be placed on the body of the user
//     private Transform coinAnchorTransform;


//     public bool placedOnSphere = false;

//     public bool HasLoggedGrab = false;

//     public Vector3 coinPosition;
//     public Quaternion coinRotation;

//     protected override void Awake()
//     {
//         coinPosition = transform.position;
//         coinRotation = transform.rotation;
//         base.Awake();
//         rb = GetComponent<Rigidbody>();
//         rb.isKinematic = true;
//         // Disable throwing on detach to prevent coins from rolling away accidentally.
//         throwOnDetach = false;


//         // Find the coinAnchorOnHand
//         GameObject coinAnchor = GameObject.Find("coinAnchorOnHand");
//         if (coinAnchor != null)
//         {
//             coinAnchorTransform = coinAnchor.transform;
//         }
//         else
//         {
//             Debug.LogWarning("coinAnchorOnHand not found! Ensure it exists in the scene.");
//         }
//     }

//     private void OnEnable()
//     {
//         transform.position = coinPosition;
//         transform.rotation = coinRotation;
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         // if (placedOnSphere) return;
//         if (CompareTag("TrialBlockStartItem")) {
//             return;
//         }
        
//         // Check if the coin collides with CoinSnapSphere
//         if (other.gameObject.name == "Sphere_anchor")
//         {
//             rb.isKinematic = false;
//             // Debug.Log("Coin collided with snap sphere!");

//             // add outline effect to coin
//             GetComponent<Outline>().enabled = true;

//             // Snap the coin to the CoinSnapSphere position
//             if (coinAnchorTransform != null)
//             {
//                 transform.SetParent(coinAnchorTransform);
//                 transform.localPosition = Vector3.zero;
//                 transform.localEulerAngles = new Vector3(90f, 0, 0);
//                 rb.isKinematic = true; // Stop physics
//                 if (!placedOnSphere & !HasLoggedGrab) {
//                     Debug.Log("Coin placed on sphere. Re-enabling transition ui manager.");
//                     DataLogger.Instance.LogGrabbedCoin(this.gameObject);
//                     HasLoggedGrab = true;
//                     TransitionUIManager.Instance.gameObject.SetActive(true);
//                     studyManager.Instance.GetComponent<XRInteractorSetup>().EnableRightNearFarInteractor(false);
//                     studyManager.Instance.GetComponent<XRInteractorSetup>().EnableLeftNearFarInteractor(false);
//                 }
//                 placedOnSphere = true;
//             }
//             else
//             {
//                 Debug.LogError("CoinSnapSphere transform is missing!");
//             }
//         }
//     }

//     private void OnTriggerExit(Collider other) {
//         // Debug.Log("on trigger exit");
//         GetComponent<Outline>().enabled = false;
//         if (other.gameObject.name == "Sphere_anchor" && placedOnSphere) {
//             // Debug.Log("Exiting sphere holder");
//             placedOnSphere = false;
//         }
//     }

//     protected override void OnSelectEntered(SelectEnterEventArgs args)
//     {
//         // if it is on the sphere, do not allow it to be selected until user is in home environment
//         // if (placedOnSphere) {
//         //     if (WorldTargetManager.Instance.GetCurrentWorldTarget().Name != "Home") {
//         //         Debug.LogWarning("Coin is on sphere. Cannot be selected until user is in Home.");
//         //         return;
//         //     }
//         // }

//         Debug.Log("in selected entered now");
        
//         TransitionUIManager.Instance.CoinAnchorController.UpdateCoinAnchorPosition(gameObject.transform);

//         base.OnSelectEntered(args);
//         rb.isKinematic = false;
//         isHeld = true;

//         // coin is held, disable transition ui manager so user cannot accidentally trigger a transition
//         TransitionUIManager.Instance.gameObject.SetActive(false);

//         if (coinAnchorTransform != null)
//         {
//             transform.SetParent(null);
//             transform.SetParent(coinAnchorTransform);
//             transform.localPosition = Vector3.zero;
//             transform.localRotation = Quaternion.identity;
//         }
//         else {
//             Debug.Log("Could not place on the side of the user after selection");

//         }
//     }

//     protected override void OnSelectExited(SelectExitEventArgs args)
//     {
//         Debug.Log("select exited now");
//         base.OnSelectExited(args);
//         // Reset velocity so that if the coin is accidentally dropped, it remains in place.
//         if (rb != null)
//         {
//             // rb.velocity = Vector3.zero;
//             rb.linearVelocity = Vector3.zero;
//             rb.angularVelocity = Vector3.zero;
//         }
//         isHeld = false;
//         rb.isKinematic = true;

//         studyManager.Instance.chest.IsCollectableIntersecting();

//         // coin is released, enable transition ui manager.
//         // only do this if the coin has already been placed on the sphere and logged, 
//         // or if the technique training is not complete, i.e., it is the starttrialblock coin
//         if (HasLoggedGrab) {
//             TransitionUIManager.Instance.gameObject.SetActive(true);
//         }
//     }

//     /// <summary>
//     /// Called by the Chest when the coin is dropped into it.
//     /// Notifies the studyManager and disables the coin.
//     /// </summary>
//     public void Collect()
//     {
//         // studyManager manager = FindAnyObjectByType<studyManager>();
//         // if (manager != null)
//         // {
//         //     manager.OnItemCollected(gameObject);
//         // }
//         // else
//         // {
//         //     Debug.LogWarning("StudyManager not found in the scene.");
//         // }
//         studyManager.Instance.OnItemCollected(gameObject);
//         gameObject.SetActive(false);
//     }
// }