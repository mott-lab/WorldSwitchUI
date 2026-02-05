// using System;
// using System.Collections;
// using Flexalon;
// using UnityEngine;
// using UnityEngine.XR.Hands;

// [Obsolete]
// public class Gallery_HeadPreview_HandInteraction : InteractionHandler
// {
//     public override void HandleActivation()
//     {
//         (int middleFingerTouching, Vector3 middleTipPose, Vector3 middleThumbPose) = checkFingerTipsTouching(XRComponents.Instance.leftHand, XRHandJointID.MiddleTip, XRHandJointID.ThumbTip);
//         // Debug.Log($"middleFingerTouching: {middleFingerTouching}, middleTipPose: {middleTipPose}, middleThumbPose: {middleThumbPose}");
//         handleGalleryToggle(middleFingerTouching, middleTipPose, middleThumbPose);
//     }

//     public override void HandleNavigation()
//     {
//         (int indexFingerTouching, Vector3 indexTipPose, Vector3 indexThumbPose) = checkFingerTipsTouching(XRComponents.Instance.leftHand, XRHandJointID.IndexTip, XRHandJointID.ThumbTip);
//         handleGalleryScrolling(indexFingerTouching, indexTipPose, indexThumbPose);
//     }

//     public override void HandleSelection()
//     {
//         // TODO: for the moment, selection is handled in navigation.
//     }

    
//     [SerializeField] private float gestureScrollSpeed = 6f;
//     private float curveLayoutPositionAtScrollStart;
//     private WorldTargetMenuItem selectedWorldMenuItemAtStartOfGesture;
//     private bool isGalleryOpen = false;
//     private bool toggleMenuGestureInProgress = false;
//     private bool scrollGestureInProgress = false;
//     private Vector3 gestureStartPositionInLocalSpace = Vector3.zero;
//     [SerializeField] float distanceFromGestureStartPos;

//     private Vector3 handPositionInLocalSpace = Vector3.zero;
//     private Vector3 prevHandPositionInLocalSpace;
//     private bool firstScrollPass;

//     [SerializeField] private float velocityLowerBound = 0.1f;
//     [SerializeField] private float velocityUpperBound = 20f;
//     [SerializeField] private float multiplierLowerBound = 0.01f;
//     [SerializeField] private float multiplierUpperBound = 2f;

//     /// <summary>
//     /// Handles the toggling of the gallery menu based on the middle finger and thumb pinch gesture.
//     /// </summary>
//     /// <param name="middleFingerTouching">An integer indicating the touch status of the middle finger and thumb (1 for touching, 0 for close to touching, 2 for not touching).</param>
//     /// <param name="middleTipPose">The position of the middle fingertip joint.</param>
//     /// <param name="middleThumbPose">The position of the thumb joint.</param>
//     private void handleGalleryToggle(int middleFingerTouching, Vector3 middleTipPose, Vector3 middleThumbPose)
//     {
//         isGalleryOpen = TransitionUserInterface.Layout.gameObject.activeSelf;

//         // Toggle gallery menu with middle finger to thumb pinch
//         if (middleFingerTouching == 1 && !isGalleryOpen && !toggleMenuGestureInProgress)
//         {
//             Debug.Log("Middle finger and thumb are touching, opening gallery");

//             toggleMenuGestureInProgress = true;
//             // TransitionUIManager.Instance.ToggleGalleryUI();
//             TransitionUserInterface.EnableInterfaceObject(true);

//             // TODO: Figure out how to separate this from the TransitionUIManager, should be in the interface class itself.
//             // this works for now because gallery interaction will only use this kind of layout
//             TransitionUIManager.Instance.CurrentTransitionInterface.Layout.GetComponent<FlexalonCurveLayout>().StartAt = TransitionUIManager.Instance.GalleryCurveLayoutScrollStart; // first gallery object at front of gallery

//             //isGalleryOpen = true;
//             StartCoroutine(delaySetGestureProgressToComplete());
//         }
//         else if (middleFingerTouching == 1 && isGalleryOpen && !toggleMenuGestureInProgress)
//         {
//             Debug.Log("Middle finger and thumb are touching, closing gallery");

//             toggleMenuGestureInProgress = true;
//             // TransitionUIManager.Instance.ToggleGalleryUI();
//             TransitionUserInterface.EnableInterfaceObject(false);
//             //isGalleryOpen = false;
//             StartCoroutine(delaySetGestureProgressToComplete());
//         }
//     }

//     /// <summary>
//     /// Handles the scrolling of the gallery menu based on the index finger and thumb pinch gesture.
//     /// </summary>
//     /// <param name="indexFingerTouching">An integer indicating the touch status of the index finger and thumb (1 for touching, 0 for close to touching, 2 for not touching).</param>
//     /// <param name="indexTipPose">The position of the index fingertip joint.</param>
//     /// <param name="indexThumbPose">The position of the thumb joint.</param>
//     private void handleGalleryScrolling(int indexFingerTouching, Vector3 indexTipPose, Vector3 indexThumbPose)
//     {
//         isGalleryOpen = TransitionUIManager.Instance.CurrentTransitionInterface.Layout.gameObject.activeSelf;

//         // Need to break up the scrolling into multiple steps
//         // 1. Start scrolling when index and thumb pinch
//         if (indexFingerTouching == 1 && isGalleryOpen && !scrollGestureInProgress)
//         {
//             scrollGestureInProgress = true;
//             XRComponents.Instance.GestureVisualizationStartObject.transform.position = indexTipPose;
//             gestureStartPositionInLocalSpace = XRComponents.Instance.XRCamera.transform.InverseTransformPoint(indexTipPose);
//             curveLayoutPositionAtScrollStart = TransitionUserInterface.Layout.GetComponent<FlexalonCurveLayout>().StartAt;
//             // selectedWorldMenuItemAtStartOfGesture = TransitionUIManager.Instance.SelectedWorldTargetMenuItem;
//             selectedWorldMenuItemAtStartOfGesture = TransitionUserInterface.SelectedWorldTargetMenuItem;
            
//             // handPositionInLocalSpace = Vector3.zero;
//             firstScrollPass = true;
//             Debug.Log("Index finger and thumb are touching, starting scroll");
//         }

//         // 2. Scroll gallery with index-thumb pinch + drag
//         else if (indexFingerTouching == 1 && isGalleryOpen && scrollGestureInProgress)
//         {
//             prevHandPositionInLocalSpace = handPositionInLocalSpace;


//             Vector3 worldIndexTipPose = XRComponents.Instance.XRRig.transform.TransformPoint(indexTipPose);


//             handPositionInLocalSpace = XRComponents.Instance.HeadGazeSphere.transform.InverseTransformPoint(worldIndexTipPose);

//             if (firstScrollPass) {
//                 firstScrollPass = false;
//                 return;
//             }

//             float handXVelocity = (handPositionInLocalSpace.x - prevHandPositionInLocalSpace.x) / Time.deltaTime;
//             if (Mathf.Abs(handXVelocity) < 0.005f) {
//                 handXVelocity = 0f;
//             }

//             Debug.Log($"handXVelocity: {handXVelocity}");

//             float handVelocity = Mathf.Abs(handXVelocity);

//             float scrollMultiplier = Mathf.Lerp(multiplierLowerBound, multiplierUpperBound, Mathf.InverseLerp(velocityLowerBound, velocityUpperBound, handVelocity));
//             if (handVelocity < velocityLowerBound) {
//                 scrollMultiplier = multiplierLowerBound;
//             } else if (handVelocity > velocityUpperBound) {
//                 scrollMultiplier = multiplierUpperBound;
//             }

//             // if (handVelocity.x > 0.1f) {
//             //     gestureScrollSpeed = 5f;
//             // // } else if (handVelocity.x > 0.2f) {
//             // //     gestureScrollSpeed = 8f;
//             // // } else if (handVelocity.x > 0.3f) {
//             // //     gestureScrollSpeed = 10f;
//             // } else if (handVelocity.x > 0.4f) {
//             //     gestureScrollSpeed = 12f;
//             // }

//             distanceFromGestureStartPos = gestureStartPositionInLocalSpace.x - handPositionInLocalSpace.x;

//             // TODO: figure out if there is a better way to access layout-specific properties
//             float currentStartAt = TransitionUserInterface.Layout.GetComponent<FlexalonCurveLayout>().StartAt;
//             float diffStartAt = handXVelocity * gestureScrollSpeed * scrollMultiplier;
//             Debug.Log($"currentStartAt: {currentStartAt}, diffStartAt: {diffStartAt}");
//             float endStartAt = currentStartAt + diffStartAt;

//             endStartAt = Mathf.Clamp(endStartAt, -3.8f, TransitionUIManager.Instance.GalleryCurveLayoutScrollStart);
//             TransitionUserInterface.Layout.GetComponent<FlexalonCurveLayout>().StartAt = endStartAt;

//             TransitionUserInterface.CheckSelectedMenuItem();
//         }

//         // Detect user release scroll gesture
//         else if (indexFingerTouching == 2 && isGalleryOpen && scrollGestureInProgress)
//         {
//             firstScrollPass = true;
            
//             Debug.Log("Index finger and thumb are not touching, releasing scroll gesture");

//             // detect SELECTION: if current world target menu item is same as the one at the start of the scroll gesture
//             if (TransitionUserInterface.SelectedWorldTargetMenuItem == selectedWorldMenuItemAtStartOfGesture)
//             {
//                 Debug.Log("index finger released on same menu item as start of gesture. Selecting.");
//                 // TransitionUIManager.Instance.SelectedWorldTargetMenuItem.SelectWorldTargetMenuItem();
//                 TransitionUserInterface.SelectedWorldTargetMenuItem.ConfirmWorldTargetMenuItem();
//                 scrollGestureInProgress = false;
//                 //isGalleryOpen = false;
//                 return;
//             }

//             float spacing = TransitionUserInterface.Layout.GetComponent<FlexalonCurveLayout>().Spacing;
//             float initialStartAt = TransitionUIManager.Instance.GalleryCurveLayoutScrollStart;
//             float snapOffset =  initialStartAt - spacing;
//             Debug.Log($"snapIntegral: {snapOffset}");

//             float temp = TransitionUserInterface.Layout.GetComponent<FlexalonCurveLayout>().StartAt;
//             float targetScrollPos = RoundToNearestInterval(temp, spacing, initialStartAt);
//             Debug.Log($"targetScrollPos: {targetScrollPos}");

//             StartCoroutine(finishScrollGestureRoutine(TransitionUIManager.Instance.CurrentTransitionInterface.Layout.GetComponent<FlexalonCurveLayout>().StartAt, targetScrollPos));
//         }
//     }

//     private float RoundToNearestInterval(float value, float interval, float minValue)
//     {
//         float adjustedValue = value - minValue;
//         float roundedValue = Mathf.Round(adjustedValue / interval) * interval;
//         return roundedValue + minValue;
//     }

//     private IEnumerator delaySetGestureProgressToComplete()
//     {
//         yield return new WaitForSeconds(0.5f);
//         toggleMenuGestureInProgress = false;
//     }

//     private IEnumerator finishScrollGestureRoutine(float startScrollPos, float targetScrollPos)
//     {
//         // snap scroll position to nearest menu item
//         float duration = 0.25f;
//         float t = 0;
//         while (t < duration)
//         {
//             t += Time.deltaTime;
//             //float lerpValue = Mathf.Pow(t / duration, 3);
//             float lerpValue = t / duration;
//             // TransitionUIManager.Instance.GalleryCurveLayout.StartAt = Mathf.Lerp(startScrollPos, targetScrollPos, lerpValue);
//             TransitionUserInterface.Layout.GetComponent<FlexalonCurveLayout>().StartAt = Mathf.Lerp(startScrollPos, targetScrollPos, lerpValue);
//             yield return null;
//         }
//         // finish scroll gesture
//         scrollGestureInProgress = false;
//     }

//     public override void CheckForGestures()
//     {
        
//     }

//     public override void HandleSelectMenuItemForPreview()
//     {

//     }

//     public override void HandleConfirmTransitionToPreviewWorld()
//     {

//     }

//     public override void InitInteractionHandler()
//     {

//     }
// }