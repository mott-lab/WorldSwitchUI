using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoManager : MonoBehaviour
{

    private static DemoManager _instance;

    public static DemoManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<DemoManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<DemoManager>();
                    singletonObject.name = typeof(XRComponents).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    private string selectedPreviewPattern;
    public string SelectedPreviewPattern { get => selectedPreviewPattern;}
    private string selectedInteractionPattern;
    public string SelectedInteractionPattern { get => selectedInteractionPattern;}

    public List<Button> previewPatternButtons;
    public List<Button> interactionPatternButtons;
    public Button baselineButton;
    public List<Image> previewButtonBackgroundImages;
    public List<Image> interactionButtonBackgroundImages;
    public Image baselineButtonBackgroundImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {
        Debug.Log("DemoManager Init");
        // XRComponents.Instance.InterfaceSelectorDropdown.options = 
        //     TransitionUIManager.Instance.TransitionInterfaces
        //         .ConvertAll(ti => new TMPro.TMP_Dropdown.OptionData(ti.ToString()));
        selectedPreviewPattern = "NONE";
        selectedInteractionPattern = "NONE";
        XRComponents.Instance.XRRig.transform.eulerAngles = new Vector3(0, 90, 0);
    }

    public void SelectBaseline()
    {
        Debug.Log("Selected Baseline Interaction");
        selectedInteractionPattern = "Baseline";
        selectedPreviewPattern = "Baseline";
        TransitionUIManager.Instance.WorldTransitionUIType = TransitionUIType.Baseline;
        TransitionUIManager.Instance.HandleWorldTransitionUITypeChanged();

        // set all preview buttons to normal color
        foreach (var btn in previewButtonBackgroundImages)
        {
            // var colors = btn.colors;
            // colors.normalColor = Color.white;
            // btn.colors = colors;
            btn.color = new Color32(154, 154, 154, 128);
        }
        // set all interaction buttons to normal color
        foreach (var btn in interactionButtonBackgroundImages)
        {
            // var colors = btn.colors;
            // colors.normalColor = Color.white;
            // btn.colors = colors;
            btn.color = new Color32(154, 154, 154, 128);
        }
        // set the calling button's normal color to its selected color
        // var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<UnityEngine.UI.Button>();
        // if (button != null)
        // {
        //     // var colors = button.colors;
        //     // colors.normalColor = colors.selectedColor;
        //     // button.colors = colors;
        //     button.GetComponentInChildren<Image>().color = new Color32(0, 255, 0, 128);
        // }
        baselineButtonBackgroundImage.color = new Color32(0, 255, 0, 128);
    }

    public void SelectPreviewPattern(string previewPattern)
    {
        Debug.Log("Selected Preview Pattern: " + previewPattern);
        XRComponents.Instance.PreviewSelectionReminder.SetActive(false);

        selectedPreviewPattern = previewPattern;

        baselineButtonBackgroundImage.color = new Color32(154, 154, 154, 128);

        // var baseColors = baselineButton.colors;
        // baseColors.normalColor = Color.white;
        // baselineButton.colors = baseColors;

        // set all preview buttons to normal color
        foreach (var btn in previewButtonBackgroundImages)
        {
            // var colors = btn.colors;
            // colors.normalColor = Color.white;
            // btn.colors = colors;
            btn.color = new Color32(154, 154, 154, 128);
        }
        // set the calling button's normal color to its selected color
        switch (previewPattern)
        {
            case "Portal":
                previewButtonBackgroundImages[0].color = new Color32(0, 255, 0, 128);
                break;
            case "WIM":
                previewButtonBackgroundImages[1].color = new Color32(0, 255, 0, 128);
                break;
        }
        // var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<UnityEngine.UI.Button>();
        // if (button != null)
        // {
        //     Debug.Log("Setting selected preview button color");
        //     // var colors = button.colors;
        //     // colors.normalColor = colors.selectedColor;
        //     // button.colors = colors;
        //     button.GetComponentInChildren<Image>().color = new Color32(0, 255, 0, 128);
        // }

        CheckInteractionSelection();
    }

    public void CheckInteractionSelection()
    {
        // if interaction selection is NONE, remind user to select interaction and return
        if (selectedInteractionPattern == "NONE" || selectedInteractionPattern == "Baseline")
        {
            XRComponents.Instance.InteractionSelectionReminder.SetActive(true);
            return;
        }

        TransitionUIType transitionUIType = 0;
        if (selectedPreviewPattern == "Portal")
        {
            switch (selectedInteractionPattern)
            {
                case "WorldWheel":
                    transitionUIType = TransitionUIType.Portal_SteeringWheel;
                    break;
                case "Gallery":
                    transitionUIType = TransitionUIType.Portal_Gallery;
                    break;
                case "HandPalette":
                    transitionUIType = TransitionUIType.Portal_Palette_Hand;
                    break;
                case "HeadPalette":
                    transitionUIType = TransitionUIType.Portal_Palette_HeadHand;
                    break;
            }
        }
        else if (selectedPreviewPattern == "WIM")
        {
            switch (selectedInteractionPattern)
            {
                case "WorldWheel":
                    transitionUIType = TransitionUIType.WIM_SteeringWheel;
                    break;
                case "Gallery":
                    transitionUIType = TransitionUIType.WIM_Gallery;
                    break;
                case "HandPalette":
                    transitionUIType = TransitionUIType.WIM_Palette_Hand;
                    break;
                case "HeadPalette":
                    transitionUIType = TransitionUIType.WIM_Palette_HeadHand;
                    break;
            }
        }

        TransitionUIManager.Instance.WorldTransitionUIType = transitionUIType;
        TransitionUIManager.Instance.HandleWorldTransitionUITypeChanged();
    }

    public void SelectInteractionPattern(string interactionPattern)
    {
        Debug.Log("Selected Interaction Pattern: " + interactionPattern);
        XRComponents.Instance.InteractionSelectionReminder.SetActive(false);

        selectedInteractionPattern = interactionPattern;

        baselineButtonBackgroundImage.color = new Color32(154, 154, 154, 128);

        // var baseColors = baselineButton.colors;
        // baseColors.normalColor = Color.white;
        // baselineButton.colors = baseColors;

        // set all interaction buttons to normal color
        foreach (var btn in interactionButtonBackgroundImages)
        {
            // var colors = btn.colors;
            // colors.normalColor = Color.white;
            // btn.colors = colors;
            btn.color = new Color32(154, 154, 154, 128);
        }
        // set the calling button's normal color to its selected color
        // var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<UnityEngine.UI.Button>();
        // if (button != null)
        // {
        //     Debug.Log("Setting selected interaction button color");
        //     // var colors = button.colors;
        //     // colors.normalColor = colors.selectedColor;
        //     // button.colors = colors;
        //     button.GetComponentInChildren<Image>().color = new Color32(0, 255, 0, 128);
        // }

        switch(interactionPattern)
        {
            case "Gallery":
                interactionButtonBackgroundImages[0].color = new Color32(0, 255, 0, 128);
                break;
            case "WorldWheel":
                interactionButtonBackgroundImages[1].color = new Color32(0, 255, 0, 128);
                break;
            case "HandPalette":
                interactionButtonBackgroundImages[2].color = new Color32(0, 255, 0, 128);
                break;
            case "HeadPalette":
                interactionButtonBackgroundImages[3].color = new Color32(0, 255, 0, 128);
                break;
        }

        CheckPreviewSelection();
    }

    public void CheckPreviewSelection()
    {
        // if preview selection is NONE, remind user to select preview and return
        if (selectedPreviewPattern == "NONE" || selectedPreviewPattern == "Baseline")
        {
            XRComponents.Instance.PreviewSelectionReminder.SetActive(true);
            return;
        }
        TransitionUIType transitionUIType = 0;
        if (selectedPreviewPattern == "Portal")
        {
            switch (selectedInteractionPattern)
            {
                case "WorldWheel":
                    transitionUIType = TransitionUIType.Portal_SteeringWheel;
                    break;
                case "Gallery":
                    transitionUIType = TransitionUIType.Portal_Gallery;
                    break;
                case "HandPalette":
                    transitionUIType = TransitionUIType.Portal_Palette_Hand;
                    break;
                case "HeadPalette":
                    transitionUIType = TransitionUIType.Portal_Palette_HeadHand;
                    break;
            }
        }
        else if (selectedPreviewPattern == "WIM")
        {
            switch (selectedInteractionPattern)
            {
                case "WorldWheel":
                    transitionUIType = TransitionUIType.WIM_SteeringWheel;
                    break;
                case "Gallery":
                    transitionUIType = TransitionUIType.WIM_Gallery;
                    break;
                case "HandPalette":
                    transitionUIType = TransitionUIType.WIM_Palette_Hand;
                    break;
                case "HeadPalette":
                    transitionUIType = TransitionUIType.WIM_Palette_HeadHand;
                    break;
            }
        }
        TransitionUIManager.Instance.WorldTransitionUIType = transitionUIType;
        TransitionUIManager.Instance.HandleWorldTransitionUITypeChanged();
    }

    public void UpdatePreviewSelection(int index)
    {
        if (index == 0) // if preview selection is 0, remind user to select preview and return
        {
            XRComponents.Instance.PreviewSelectionReminder.SetActive(true);
            return;
        }
        else
        {
            XRComponents.Instance.PreviewSelectionReminder.SetActive(false);
            CheckInteractionSelection(index);
        }
    }

    public void CheckInteractionSelection(int previewIndex)
    {
        // if interaction selection is 0, remind user to select interaction and return
        if (XRComponents.Instance.InteractionSelectorDropdown.value == 0)
        {
            XRComponents.Instance.InteractionSelectionReminder.SetActive(true);
            return;
        }

        TransitionUIType transitionUIType = 0;
        // if previewIndex is 1, it is Portal
        if (previewIndex == 1)
        {
            switch (XRComponents.Instance.InteractionSelectorDropdown.value)
            {
                case 1:
                    transitionUIType = TransitionUIType.Portal_SteeringWheel;
                    break;
                case 2:
                    transitionUIType = TransitionUIType.Portal_Gallery;
                    break;
                case 3:
                    transitionUIType = TransitionUIType.Portal_Palette_Hand;
                    break;
                case 4:
                    transitionUIType = TransitionUIType.Portal_Palette_HeadHand;
                    break;
                case 5:
                    transitionUIType = TransitionUIType.Baseline;
                    break;
            }
        }
        // if previewIndex is 2, it is WIM
        else if (previewIndex == 2)
        {
            switch (XRComponents.Instance.InteractionSelectorDropdown.value)
            {
                case 1:
                    transitionUIType = TransitionUIType.WIM_SteeringWheel;
                    break;
                case 2:
                    transitionUIType = TransitionUIType.WIM_Gallery;
                    break;
                case 3:
                    transitionUIType = TransitionUIType.WIM_Palette_Hand;
                    break;
                case 4:
                    transitionUIType = TransitionUIType.WIM_Palette_HeadHand;
                    break;
                case 5:
                    transitionUIType = TransitionUIType.Baseline;
                    break;
            }
        }
        // if previewIndex is 3, it is Baseline
        else if (previewIndex == 3)
        {
            XRComponents.Instance.InteractionSelectorDropdown.value = 5; // set interaction to baseline
            transitionUIType = TransitionUIType.Baseline;
        }

        TransitionUIManager.Instance.WorldTransitionUIType = transitionUIType;
        TransitionUIManager.Instance.HandleWorldTransitionUITypeChanged();
    }

    public void UpdateInteractionSelection(int index)
    {
        if (index == 0)
        {
            XRComponents.Instance.InteractionSelectionReminder.SetActive(true);
            return;
        }
        else
        {
            XRComponents.Instance.InteractionSelectionReminder.SetActive(false);
            CheckPreviewSelection(index);
        }
    }

    public void CheckPreviewSelection(int interactionIndex)
    {
        // if preview selection is 0, remind user to select preview and return
        if (XRComponents.Instance.PreviewSelectorDropdown.value == 0)
        {
            XRComponents.Instance.PreviewSelectionReminder.SetActive(true);
            return;
        }


        TransitionUIType transitionUIType = 0;
        // if interactionIndex is 5, it is Baseline
        if (interactionIndex == 5)
        {
            XRComponents.Instance.PreviewSelectorDropdown.value = 3; // set preview to baseline
            transitionUIType = TransitionUIType.Baseline;
        }
        // if previewIndex is 1, it is Portal
        else if (XRComponents.Instance.PreviewSelectorDropdown.value == 1)
        {
            switch (interactionIndex)
            {
                case 1:
                    transitionUIType = TransitionUIType.Portal_SteeringWheel;
                    break;
                case 2:
                    transitionUIType = TransitionUIType.Portal_Gallery;
                    break;
                case 3:
                    transitionUIType = TransitionUIType.Portal_Palette_Hand;
                    break;
                case 4:
                    transitionUIType = TransitionUIType.Portal_Palette_HeadHand;
                    break;
                case 5:
                    transitionUIType = TransitionUIType.Baseline;
                    break;
            }
        }
        // if previewIndex is 2, it is WIM
        else if (XRComponents.Instance.PreviewSelectorDropdown.value == 2)
        {
            switch (interactionIndex)
            {
                case 1:
                    transitionUIType = TransitionUIType.WIM_SteeringWheel;
                    break;
                case 2:
                    transitionUIType = TransitionUIType.WIM_Gallery;
                    break;
                case 3:
                    transitionUIType = TransitionUIType.WIM_Palette_Hand;
                    break;
                case 4:
                    transitionUIType = TransitionUIType.WIM_Palette_HeadHand;
                    break;
                case 5:
                    transitionUIType = TransitionUIType.Baseline;
                    break;
            }
        }

        TransitionUIManager.Instance.WorldTransitionUIType = transitionUIType;
        TransitionUIManager.Instance.HandleWorldTransitionUITypeChanged();
    }

}
