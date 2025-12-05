# WorldSwitchUI

Unity files are in the `Experiment Software` folder.

Experiment data and analysis scripts are in the `Data and Analysis` folder.

## Updates to the repo during R&R cycle

We added support for trying our techniques with controllers. The interaction process is consistent with controllers, the activation gesture (middle-finger--thumb pinch) is simply matched to the `primary button` on the controller. E.g., if the interaction technique calls for holding a left (or right) middle--thumb pinch, the user can hold down the left controller's `X button` (or the right controller's `A button`).

We also made a demo scene `WorldSwitch_Demo` which includes a UI for the user to switch between the different techniques. Rather than running through the experiment procedure (and manually changing the active interface in the Unity Editor) the user can push the buttons on the world-switching interface selection UI. We recommend users do this unless they want to experience the entire experiment.

## Dependencies

This project has several dependencies that should be placed in a folder called `ThirdPartyAssets`. Unfortunately, the first two in this list are paid assets. The Polygon Sampler Pack is available for free with the [Unity Student Plan](https://unity.com/products/unity-student). I do not see any way out of purchasing the Flexalon Pro asset. However, I find it quite useful for developing 3DUI quickly, so 3DUI researchers may be interested in having this asset as well.
- [Polygon - Sampler Pack](https://assetstore.unity.com/packages/3d/environments/polygon-sampler-pack-207048)
- [Flexalon Pro: 3D & UI Layouts](https://assetstore.unity.com/packages/tools/utilities/flexalon-pro-3d-ui-layouts-230509)
- [Tresure Set - Free Chest](https://assetstore.unity.com/packages/3d/props/interior/treasure-set-free-chest-72345)
- [Quick Outline](https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488)
- [Snaps Prototype | Office](https://assetstore.unity.com/packages/3d/environments/snaps-prototype-office-137490)

It is not required for any functionality, but I use the [Pretty Hierarchy](https://github.com/NCEEGEE/PrettyHierarchy/tree/main) package to add colored backgrounds to empty gameobjects in the scene. If you don't want to install this package, just remove those missing scripts from the label gameobjects (Managers, User Interaction, Interfaces, Scene, Environments).

This project also uses some assets that are available from the Unity registry.
- XR Interaction Toolkit 3.0.8
- XR Plugin Management
  - Sample: Example XR Management implementation
  - XR Hands: add via package name: com.unity.xr.hands
- Post Processing
- ProBuilder

## Running the Project
Connect your Meta Quest headset to your computer using a Meta Link cable and start Oculus Link.
If you want to run through the entire experiment, open the `XRWorldSwitcher_IntegratedStudy` scene --- it is the experiment scene.
If you just want to try out the interface, open the `WorldSwitch_Demo` scene.
Press Play.

All interfaces are activated by a middle-finger--thumb pinch.
The interfaces remain active as long as the pinch is held.
In the `Study Manager` object, set your `Dominant Hand` preference. It is set to right hand by default.
Most interfaces are activated with a pinch on the non-dominant hand, but a couple are activated with a pinch on the dominant hand.
The experiment environment provides instructions for activating and selecting worlds with interfaces.

### Trying the experiment
If you would like to complete the entire experiment as participants did, the only in-Unity setup you need to follow are given by the steps above. 
Follow the instructions on the UI in the Home environment (the environment with the treasure chest) to learn how to use the assigned interface. 
Practice switching to a couple different environments using the interface. 
When you are ready, return to the Home environment, find the blue coin, pick it up, and put it in the treasure chest.
Then, a coin will spawn in one of the environments.
Use the interface to find it, transition to its environment, and pick it up using an index--thumb pinch with either hand.
Pull the coin toward you to snap the coin into the yellow-orange inventory sphere.
Then, use the interface to transition back to the Home environment.
Place the coin in the treasure chest.

The experiment involves collecting 15 coins, each one spawning in a random environmnent.
After you deposit the 15th coin in the chest, the instructions for the next interface will appear in the Home environment.

### Trying different interfaces
If you do not want to complete all the trials for every interface, we recommend using the `WorldSwitch_Demo` scene.
In the Home environment for this scene, there is a push-button UI for selecting different world-switching interfaces.
In this version, the experiment task will continue to run and not break for different trial blocks.

If you are using the `XRWorldSwitcher_IntegratedStudy` scene, you can switch between different interfaces whenever you want through the `Transition UI Manager` gameobject.
In the Inspector, click the dropdown for `World Transition UI Type`.
Select an interface and begin using it to transition to different environments.

## Extending or Creating Interfaces

Each interface is implemented with the intention of decoupling the interface's display from its interaction.
Display features are included in the `Scripts/TransitionInterfaces` folder.
Interactions are in the `Scripts/Gestures` folder.

For examples of how the interfaces in this project are set up, see the gameobjects under the `Interfaces` gameobject in the Unity hierarchy.

If you create a new interface in this pattern, you can add it to the set of interfaces that can be selected in the scene with the following procedure.
1. From the `Transition UI Manager` object: add an element to the `Transition Interfaces` list on the `Transition UI Manager` script. 
2. From the same object, add an element to the `Transition Interactions` list on the `Interaction Manager` script.
3. On the `Interfaces` child object, create a new gameobject and add the script you created for the interface display components.
4. On the `Interaction Handlers` child object, create a new gameobject and add the script you created for handling user interaction/gestures.
5. On the `Transition UI Manager` object, drag the new interface object and the new interaction object to their respective lists.