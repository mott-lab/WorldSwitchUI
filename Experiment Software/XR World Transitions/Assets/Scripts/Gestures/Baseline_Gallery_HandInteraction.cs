using System.Collections;
using Flexalon;
using UnityEngine;
using UnityEngine.XR.Hands;

public class Baseline_Gallery_HandInteraction : InteractionHandler
{

    // Start is called before the first frame update
    void Start()
    {
        // Initialization code here
        
    }

    void OnEnable()
    {
        DominantHandActivation = true;
    }

    // Update is called once per frame
    void Update()
    {
        // ProcessUpdate();
    }

    public override void HandleActivation()
    {
        // Implementation for handling activation
    }

    public override void HandleNavigation()
    {
        // Implementation for handling navigation
    }

    public override void HandleSelection()
    {
        // Implementation for handling selection
    }


    public override void HandleSelectMenuItemForPreview()
    {

    }

    public override void HandleConfirmTransitionToPreviewWorld()
    {


    }
}