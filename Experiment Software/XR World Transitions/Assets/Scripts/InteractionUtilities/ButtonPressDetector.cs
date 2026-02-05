using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonPressDetector : MonoBehaviour
{
    public InputActionReference rightPrimaryButtonAction; // Assign your action in the Inspector
    public InputActionReference leftPrimaryButtonAction; // Assign your action in the Inspector

    private void OnEnable()
    {
        rightPrimaryButtonAction.action.started += OnPrimaryButtonPressedRight;
        rightPrimaryButtonAction.action.canceled += OnPrimaryButtonReleasedRight;
        leftPrimaryButtonAction.action.started += OnPrimaryButtonPressedLeft;
        leftPrimaryButtonAction.action.canceled += OnPrimaryButtonReleasedLeft;
    }

    private void OnDisable()
    {
        rightPrimaryButtonAction.action.started -= OnPrimaryButtonPressedRight;
        rightPrimaryButtonAction.action.canceled -= OnPrimaryButtonReleasedRight;
        leftPrimaryButtonAction.action.started -= OnPrimaryButtonPressedLeft;
        leftPrimaryButtonAction.action.canceled -= OnPrimaryButtonReleasedLeft;
    }

    private void OnPrimaryButtonPressedRight(InputAction.CallbackContext context)
    {
        Debug.Log("Right Primary button pressed!");
        // Add your desired functionality here
        TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction.ControllerButtonPressed(true);
    }

    private void OnPrimaryButtonPressedLeft(InputAction.CallbackContext context)
    {
        Debug.Log("Left Primary button pressed!");
        // Add your desired functionality here
        TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction.ControllerButtonPressed(false);
    }

    private void OnPrimaryButtonReleasedRight(InputAction.CallbackContext context)
    {
        Debug.Log("Right Primary button released!");
        // Add your desired functionality here
        TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction.ControllerButtonReleased(true);
    }

    private void OnPrimaryButtonReleasedLeft(InputAction.CallbackContext context)
    {
        Debug.Log("Left Primary button released!");
        // Add your desired functionality here
        TransitionUIManager.Instance.InteractionManager.SelectedTransitionInteraction.ControllerButtonReleased(false);
    }
}