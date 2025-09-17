using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GetSwipeTranslation : MonoBehaviour
{
    public GameObject BoundsObject;
    public float BoundsRadius;
    private TransitionManager transitionManager;
    private WorldTargetManager worldTargetManager;
    private SwipeActions swipeAction;
    
    private void DisplayBounds() 
    {
        BoundsObject.SetActive(true);
    }

    private void HideBounds()
    {
        BoundsObject.SetActive(false);
    }

    public void SetStartLocation()
    {
        Debug.Log("Start swipe");
        DisplayBounds();
        BoundsObject.transform.localScale = Vector3.one * (2 * BoundsRadius);
        BoundsObject.transform.parent = null;
    }

    public void ResetVisualization()
    {
        BoundsObject.transform.parent = transform;
        BoundsObject.transform.localPosition = Vector3.zero;
        BoundsObject.transform.localRotation = Quaternion.identity;
        HideBounds();
    }

    public void CheckSwipeComplete()
    {
        //float distance = Vector3.Project(BoundsObject.transform.position - transform.position, BoundsObject.transform.right).magnitude;

        Vector3 distanceVec = transform.position - BoundsObject.transform.position;

        if (distanceVec.magnitude < BoundsRadius)
        {
            Debug.Log("Swipe Incomplete");
            ResetVisualization();
            return;
        }
            
        Debug.Log("Swipe Complete");

        Vector3 direction = distanceVec.normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y) && Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            if (direction.x > 0)
            {
                Debug.Log("right swipe");
                TransitionUIManager.Instance.CompleteRightSwipe();
            }
            else
            {
                Debug.Log("left swipe");
                TransitionUIManager.Instance.CompleteRightSwipe();
            }
        }
        else if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x) && Mathf.Abs(direction.y) > Mathf.Abs(direction.z))
        {
            if (direction.y > 0)
            {
                Debug.Log("up swipe");
                TransitionUIManager.Instance.CurrentTransitionInterface.ToggleTransitionInterface();
            }
            else
            {
                Debug.Log("down swipe");
            }
        }
        else
        {
            Debug.Log("forward swipe");
        }

        ResetVisualization();
    }

    private void Awake()
    {
        //swipeAction = new SwipeActions();
        //swipeAction.@default.StartSwipe.performed += ctx => SetStartLocation();
        //swipeAction.@default.EndSwipe.performed += ctx => CheckSwipeComplete();
        
        worldTargetManager = WorldTargetManager.Instance;
        transitionManager = TransitionManager.Instance;
    }

    private void OnEnable()
    {
        //swipeAction.Enable();
    }

    private void OnDisable()
    {
        //swipeAction.Disable();
    }
}
