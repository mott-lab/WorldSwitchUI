using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class StudyUIManager : MonoBehaviour
{

    
    public Dictionary<TransitionUIType, (string title, string description)> transitionUIDictionary = new Dictionary<TransitionUIType, (string title, string description)>
    {
        {
            TransitionUIType.Baseline, 
            ("Baseline - static menu with ray-based selections", "Grid of world images with a ray-based selection method. \n\nTo activate: hold middle pinch gesture with your dominant hand. \n\nTo navigate: move your dominant hand to control the ray selector. \n\nTo select: release the pinch gesture. \n\nTo cancel: release the pinch gesture without selecting a world.")
        },
        { 
            TransitionUIType.Portal_Gallery, 
            ("Portal - head-referenced with linear menu", "Portal that follows your head direction with a linear selector to pick the world. \n\nTo activate: hold middle pinch gesture with you dominant hand.\n\nTo navigate: move your dominant hand left/right to scroll the selector.\n\nTo select: release the pinch gesture slightly ABOVE where you started the gesture.\n\nTo cancel: release the pinch gesture slightly BELOW where you started the gesture.\n\nNote: Releasing the pinch at the same height you started the pinch will leave the portal open, allowing you to look around the preview environment with a stabilized view.\n\nDuring training, the translucent green box that appears above your hand will show the threshold for confirming the transition, and the red box will show the cancel threshold.") 
        },
        { 
            TransitionUIType.Portal_Palette_Hand, 
            ("Portal - Palette with hand-based selections", "Portal attached to your hand with an arrangement of worlds to pick from. \n\nTo activate: hold middle pinch gesture with your non-dominant hand. \n\nTo navigate: touch one of the world images with your dominant hand. \n\nTo select: release the pinch gesture while touching the middle portal or one of the world images. \n\nTo cancel: release the pinch gesture without selecting a world.") 
        },
        { 
            TransitionUIType.Portal_Palette_HeadHand, 
            ("Portal - Palette with head-based selections", "Portal attached to your hand with an arrangement of worlds to pick from. \n\nTo activate: hold middle pinch gesture with your dominant hand. \n\nTo navigate: point your head at the world image you want to select. You will see a cursor appear on the image. \n\nTo select: release the pinch gesture while the cursor is on the image. \n\nTo cancel: release the pinch gesture without selecting a world.") 
        },
        { 
            TransitionUIType.Portal_SteeringWheel, 
            ("Portal - Steering Wheel", "Portal between your two hands with a steering wheel to pick the world. \n\nTo activate: With both hands in front of you, hold middle pinch gesture with your dominant hand. \n\nTo navigate: rotate your hands to rotate the wheel. The world under the cursor at the top of the wheel is set in the preview portal. \n\nTo select: release the pinch gesture while the selector is on the world you want to pick.") 
        },
        { 
            TransitionUIType.WIM_Gallery, 
            ("WIM - head-referenced with linear menu", "World-in-miniature that follows your head with a linear selector to pick the world. \n\nTo activate: hold middle pinch gesture with you dominant hand.\n\nTo navigate: move your dominant hand left/right to scroll the selector.\n\nTo select: release the pinch gesture slightly ABOVE where you started the gesture.\n\nTo cancel: release the pinch gesture slightly BELOW where you started the gesture.\n\nNote: Releasing the pinch at the same height you started the pinch will leave the portal open, allowing you to look around the preview environment with a stabilized view.\n\nDuring training, the translucent green box that appears above your hand will show the threshold for confirming the transition, and the red box will show the cancel threshold.") 
        },
        { 
            TransitionUIType.WIM_Palette_Hand, 
            ("WIM - Palette with hand-based selections", "World-in-miniature attached to your hand with an arrangement of worlds to pick from. \n\nTo activate: hold middle pinch gesture with your non-dominant hand. \n\nTo navigate: touch one of the world images with your dominant hand. \n\nTo select: release the pinch gesture while touching the middle portal or one of the world images. \n\nTo cancel: release the pinch gesture without selecting a world.") 
        },
        { 
            TransitionUIType.WIM_Palette_HeadHand, 
            ("WIM - Palette with head-based selections", "World-in-miniature attached to your hand with an arrangement of worlds to pick from. \n\nTo activate: hold middle pinch gesture with your dominant hand. \n\nTo navigate: point your head at the world image you want to select. You will see a cursor appear on the image. \n\nTo select: release the pinch gesture while the cursor is on the image. \n\nTo cancel: release the pinch gesture without selecting a world.") 
        },
        { 
            TransitionUIType.WIM_SteeringWheel, 
            ("WIM - Steering Wheel", "World-in-miniature between your two hands with a steering wheel to pick the world. \n\nTo activate: With both hands in front of you, hold middle pinch gesture with your dominant hand. \n\nTo navigate: rotate your hands to rotate the wheel. The world under the cursor at the top of the wheel is set in the preview portal. \n\nTo select: release the pinch gesture while the selector is on the world you want to pick.") 
        }
    };
    
    public void SetUpTechniqueTrainingUI(TransitionUIType transitionUIType) {
        
        XRComponents.Instance.StudyUIParent.SetActive(true);

        XRComponents.Instance.TechniqueTitle.text = transitionUIDictionary[transitionUIType].title;
        
        XRComponents.Instance.TechniqueDescription.text = transitionUIDictionary[transitionUIType].description;
    }
}
