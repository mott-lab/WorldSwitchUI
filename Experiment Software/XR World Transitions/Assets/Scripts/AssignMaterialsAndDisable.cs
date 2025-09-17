using UnityEngine;

public class AssignMaterialsBasedOnDistance : MonoBehaviour
{
    [Header("Materials to Assign")]
    public Material handMaskMaterial;      
    public Material[] handMaterials; // holds the materiasl from different previews   
    public Material defaultHandMaterial;   

    [Header("GameObject to Disable")]
    public GameObject testTextureRendered;

    [Header("Runtime Object Name")]
    public string targetObjectName = "LeftHand"; 

    [Header("L_Wrist Object Name")]
    public string wristObjectName = "L_Wrist"; 

    [Header("Distance Threshold (cm)")]
    public float proximityThreshold = 40f; 

    private GameObject targetObject;       // Runtimecreated hand object
    private GameObject wristObject;        // Wrist object for position reference
    private Transform hmdTransform;        // Reference to the user's HMD 
    private Renderer targetRenderer;       // Renderer of the target object

    private bool isPortalMaterialAssigned = false;


    public RenderTexture renderTexture;        // Render Texture for the portal

    private FistGestureDetection gestureDetectionScript; // here I Reference to the gesture detection script
    void Start()
    {

        if (testTextureRendered != null)
        {
            testTextureRendered.SetActive(false);
        }
        else
        {
            Debug.LogWarning("testTextureRendered is not assigned or missing in the scene.");
        }

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            hmdTransform = mainCamera.transform;
        }
        else
        {
            Debug.LogError("Main Camera not found! Ensure your XR Origin has a camera.");
            return;
        }

        wristObject = GameObject.Find(wristObjectName);
        if (wristObject == null)
        {
            Debug.Log(wristObjectName + " not found in the scene. Ensure it exists and is named correctly.");
        }


        GameObject xrOrigin = GameObject.Find("XR Origin (XR Rig)");
        gestureDetectionScript = xrOrigin.GetComponent<FistGestureDetection>();
        if (gestureDetectionScript == null)
        {
            Debug.LogError("FistGestureDetection script not found on XR Origin.");
        }

    }

    void Update()
    {

        if (GameObject.Find("XR Origin (XR Rig)").GetComponent<FistGestureDetection>().teleported == true)
        {
            return;
        }
        if (targetObject == null)
        {
            targetObject = GameObject.Find(targetObjectName);
            if (targetObject != null)
            {
                Debug.Log("Found " + targetObjectName + " in the scene.");
                targetRenderer = targetObject.GetComponent<Renderer>();
                if (targetRenderer == null)
                {
                    Debug.LogWarning("Renderer component not found on " + targetObjectName);
                }
            }
        }

        if (wristObject == null)
        {
            wristObject = GameObject.Find(wristObjectName);
            if (wristObject == null)
            {
                Debug.Log(wristObjectName + " not found in the scene. Ensure it exists and is named correctly.");
                return;
            }
        }

        if (wristObject != null && hmdTransform != null && targetRenderer != null && gestureDetectionScript != null)
        {
            float distance = Vector3.Distance(wristObject.transform.position, hmdTransform.position);

            if (distance * 100f <= proximityThreshold) 
            {
                if (!isPortalMaterialAssigned)
                {
                    int currentPreviewIndex = gestureDetectionScript.GetCurrentPreviewIndex();
                    if (currentPreviewIndex < 0) currentPreviewIndex = 0;
                    if (currentPreviewIndex >= handMaterials.Length) currentPreviewIndex = handMaterials.Length - 1;
                    if (GameObject.Find("XR Origin (XR Rig)").GetComponent<FistGestureDetection>().teleported == false
                        && currentPreviewIndex >= 0 && currentPreviewIndex < handMaterials.Length)
                    {
                        targetRenderer.materials = new Material[] { handMaskMaterial, handMaterials[currentPreviewIndex] };
                        Debug.Log(" the hand has the current preview material " + handMaterials[currentPreviewIndex]);

                    }
                    else
                    {
                        targetRenderer.material = defaultHandMaterial;
                        Debug.Log("Default material assigned to the hand.");
                    }

                    isPortalMaterialAssigned = true;

                }
            }
            else
            {
                if (isPortalMaterialAssigned)
                {
                    targetRenderer.material = new Material(defaultHandMaterial);
                    Debug.Log("Default material assigned to the hand.");
                    isPortalMaterialAssigned = false; 
                }
            }
        }
    }
}
