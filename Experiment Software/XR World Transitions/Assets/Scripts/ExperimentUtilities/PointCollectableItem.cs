using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PointCollectableItem : XRBaseInteractable
{
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        // Dynamically load the coin audio
        AudioClip clip = Resources.Load<AudioClip>("coinCollectionAudio");
        if (clip != null)
        {
            // Play the clip at the coin position
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
        else
        {
            Debug.LogWarning("Audio clip 'coinCollectionAudio' not found in Resources.");
        }

        studyManager manager = FindAnyObjectByType<studyManager>();
        if (manager != null)
        {

            //if (DataLogger.Instance != null)
            //{
            //    DataLogger.Instance.LogSelectedFlyingDisk(gameObject);
            //}

            manager.OnItemCollected(gameObject);
        }
        else
        {
            Debug.LogWarning("StudyManager not found in the scene.");
        }
        gameObject.SetActive(false);
    }
}
