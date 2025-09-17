using UnityEngine;
using UnityEngine.XR.Hands;

public class TransitionCursorController : MonoBehaviour
{
    [SerializeField] private float maxRayDistance = 100f;
    [SerializeField] private LayerMask collisionLayerMask;
    public Renderer CursorRenderer;

    Camera mainCamera;
    TransitionCursorController transitionCursorController;

    private void Update()
    {
        PositionCursor();
    }

    void Awake()
    {
        mainCamera = XRComponents.Instance.XRCamera;
        transitionCursorController = TransitionUIManager.Instance.TransitionCursorController;
    }


    private void PositionCursor()
    {
        if (mainCamera == null || transitionCursorController == null)
        {
            Debug.LogWarning("Main Camera or cursor is not assigned.");
            return;
        }

        // For HeadHandInteraction
        if (TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction is Palette_HandPreview_HeadHandInteraction || 
                TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction is PaletteWIM_HandPreview_HeadHandInteraction) {

            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRayDistance, collisionLayerMask))
            {
                transitionCursorController.transform.position = hit.point;
            }
            else
            {
                transitionCursorController.transform.position = ray.GetPoint(maxRayDistance);
            }
        }
        // For HandInteraction
        else if (
                TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction is Palette_HandPreview_HandInteraction ||
                TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction is PaletteWIM_HandPreview_HandInteraction
                )
        {
            if (TransitionUIManager.Instance.InteractionManager.HandsAlive)
            {
                if (StudyConfigurationManager.Instance.UserDominantHand == StudyConfigurationManager.DominantHand.LeftHand) {
                    XRComponents.Instance.leftHand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var tipPose);
                    transitionCursorController.transform.localPosition = tipPose.position;
                } else {
                    XRComponents.Instance.rightHand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var tipPose);
                    transitionCursorController.transform.localPosition = tipPose.position;
                }

            }
        }


    }
}