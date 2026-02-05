//using UnityEngine;
//using System;

//public class Chest : MonoBehaviour
//{
//    public int coinCount = 0;
//    public event Action<int> onChestCountChanged;

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Collectable"))
//        {
//            CollectableItem item = other.GetComponent<CollectableItem>();

//            if (item != null && !item.IsHeld)
//            {
//                coinCount++;
//                Debug.Log("Coin dropped in chest. Total count: " + coinCount);

//                // Notify any listeners 
//                onChestCountChanged?.Invoke(coinCount);

//                item.Collect();
//            }
//        }
//    }
//}

using UnityEngine;
using System;

public class Chest : MonoBehaviour
{
    public int coinCount = 0;
    public event Action<int, GameObject> OnChestCountChanged;

    [Header("Audio Feedback")]
    [Tooltip("Audio clip to play when a coin is deposited in the chest.")]
    public AudioClip coinCollectionAudio;
    public AudioClip trialBlockStartAudioClip;
    public AudioClip trialBlockCompleteAudioClip;
    private AudioSource audioSource;

    private Outline outline;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        outline = this.GetComponent<Outline>();
        if (outline == null)
        {
            outline = this.gameObject.AddComponent<Outline>();
        }
    }

    public void PlayTaskBlockCompleteAudio()
    {
        if (trialBlockCompleteAudioClip != null)
        {
            audioSource.PlayOneShot(trialBlockCompleteAudioClip);
        }
    }

    private bool colliderTest(Collider hitCollider) {
        if (hitCollider.CompareTag("Collectable"))
        {

            CollectableItem item = hitCollider.GetComponent<CollectableItem>();

            // return if the item is placed on the sphere holder
            if (item.placedOnSphere) return false;

            outline.enabled = true;
            Debug.Log("Chest entered");

            if (item != null && !item.IsHeld)
            {
                coinCount++;
                Debug.Log("Coin dropped in chest. Total count: " + coinCount);

                // Play audio feedback 
                if (coinCollectionAudio != null)
                {
                    audioSource.PlayOneShot(coinCollectionAudio);
                }
                // ------------------

                outline.enabled = false;

                studyManager.Instance.currentCollectedItem = hitCollider.gameObject;
                // DataLogger.Instance.LogDepositedInChest(other.gameObject); // Log deposit

                // Notify any listeners 
                // MG: Leave out? we can do everything when item is collected()
                OnChestCountChanged?.Invoke(coinCount, hitCollider.gameObject);

                item.Collect();
                TransitionManager.Instance.SetLayerRecursively(studyManager.Instance.chest.gameObject, LayerMask.NameToLayer("PortalOutside"));
            }
            return true;

        } 
        else if (hitCollider.CompareTag("TrialBlockStartItem")) {

            CollectableItem item = hitCollider.GetComponent<CollectableItem>();

            outline.enabled = true;

            if (item != null && !item.IsHeld)
            {
                outline.enabled = false;
                if (trialBlockStartAudioClip != null)
                {
                    audioSource.PlayOneShot(trialBlockStartAudioClip);
                }

                item.gameObject.SetActive(false);

                TransitionManager.Instance.SetLayerRecursively(studyManager.Instance.chest.gameObject, LayerMask.NameToLayer("PortalOutside"));
                TransitionUIManager.Instance.gameObject.SetActive(true);
                studyManager.Instance.StartTrialBlock();
            }
            return true;
        }
        return false;
    }

    public bool IsCollectableIntersecting()
    {
        if (WorldTargetManager.Instance.GetCurrentWorldTarget().Name != "Home") {
            Debug.LogWarning("Chest is not in Home. Cannot deposit coin.");
            return false;
        }
        Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity);
        bool isIntersecting = false;
        foreach (var hitCollider in hitColliders)
        {
            isIntersecting = colliderTest(hitCollider);
            if (isIntersecting) return true;
        }
        return false;
    }


    private void OnTriggerEnter(Collider other)
    {

        if (WorldTargetManager.Instance.GetCurrentWorldTarget().Name != "Home") {
            Debug.LogWarning("Chest is not in Home. Cannot deposit coin.");
            return;
        }

        colliderTest(other);
    }

    public void ResetChest()
    {
        coinCount = 0;
    }

    void OnTriggerExit(Collider other)
    {
        outline.enabled = false;
    }
}
