using UnityEngine;
using System.IO;
using UnityEngine.Rendering;

public class envScreenshots : MonoBehaviour
{
    private string baseFolderPath = Path.Combine(Application.dataPath, "Screenshots");

    public float screenshotInterval = 1.0f;
    // private float timer = 0f;
    [SerializeField] private Camera screenshotCamera;

    void Start()
    {
        if (!Directory.Exists(baseFolderPath))
        {
            Directory.CreateDirectory(baseFolderPath);
        }
        if (screenshotCamera == null) {
            screenshotCamera = GetComponent<Camera>();
        }
    }

    void Update()
    {
        //timer += Time.deltaTime;

        //if (timer >= screenshotInterval)
        //{
        //    TakeScreenshot();
        //    timer = 0f;
        //}
    }

    public void TakeScreenshot()
    {
        screenshotCamera.gameObject.SetActive(true);
        //string fileName = "RecentScreenshot.png";
        //string fullPath = Path.Combine(baseFolderPath, fileName);

        // turn off all menu items during screenshot. 
        // TODO: Ideally, this can just work by properly setting layers.
        // However, the stencil shader is not working as expected, so we need to manually disable the menu items.
        TransitionUIManager.Instance.CurrentTransitionInterface.EnableInterfaceObject(false);
        foreach (WorldTargetMenuItem item in TransitionUIManager.Instance.CurrentTransitionInterface.MenuItems)
        {
            item.gameObject.SetActive(false);
        }

        Vector3 worldTargetRotation = WorldTargetManager.Instance.GetCurrentWorldTarget().transform.rotation.eulerAngles;
        if (WorldTargetManager.Instance.GetCurrentWorldTarget().Name != "Home") {
            screenshotCamera.transform.eulerAngles = new Vector3(worldTargetRotation.x, -45f, worldTargetRotation.z);
        }


        RenderTexture renderTexture = screenshotCamera.targetTexture;
        RenderTexture.active = renderTexture;
        screenshotCamera.Render();

        Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA64, false, true);
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();

        RenderTexture.active = null;

        Debug.Log("Setting screenshot to menu items in each interface");
        // foreach (TransitionInterface transitionInterface in TransitionUIManager.Instance.TransitionInterfaces)
        // {
        //     transitionInterface.SelectedWorldTargetMenuItem = ;
        //     transitionInterface.SelectedWorldTargetMenuItem.WorldPreviewImage.texture = screenshot;
        // }
        // TransitionUIManager.Instance.CurrentTransitionInterface.SelectedWorldTargetMenuItem.WorldPreviewImage.texture = screenshot;
        foreach (WorldTargetMenuItem menuItem in WorldTargetManager.Instance.GetCurrentWorldTarget().AssociatedWorldTargetMenuItems)
        {
            menuItem.WorldPreviewImage.texture = screenshot;
        }

        // foreach (WorldTargetMenuItem item in TransitionUIManager.Instance.CurrentTransitionInterface.MenuItems)
        // {
        //     item.WorldPreviewImage.texture = screenshot;
        // }

        // WorldTargetManager.Instance.GetCurrentWorldTarget().CircleMenuItem.WorldPreviewImage.texture = screenshot;
        // WorldTargetManager.Instance.GetCurrentWorldTarget().CurveMenuItem.WorldPreviewImage.texture = screenshot;

        //byte[] bytes = screenshot.EncodeToPNG();
        //File.WriteAllBytes(fullPath, bytes);
        //Debug.Log($"Screenshot saved at: {fullPath}");

        screenshotCamera.gameObject.SetActive(false);

        // turn the menu items back on after taking the screenshot (again, should be handled with layers)
        TransitionUIManager.Instance.CurrentTransitionInterface.EnableInterfaceObject(false);
        foreach (WorldTargetMenuItem item in TransitionUIManager.Instance.CurrentTransitionInterface.MenuItems)
        {
            item.gameObject.SetActive(true);
        }

        //ScreenCapture.CaptureScreenshot(fullPath);
        //Debug.Log($"Screenshot saved at: {fullPath}");
    }
}
