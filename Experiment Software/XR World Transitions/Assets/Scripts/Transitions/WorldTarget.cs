using System.Collections.Generic;
using UnityEngine;

public class WorldTarget : MonoBehaviour
{

    public string Name;
    public string Abbreviation;
    public Vector3 StartPosition;
    public Quaternion StartRotation;

    public List<WorldTargetMenuItem> AssociatedWorldTargetMenuItems = new List<WorldTargetMenuItem>();
    
    public Renderer[] Renderers;

    public GameObject WIMObjects;
    private Renderer[] WIMRenderers;

    public void FindAllRenderers()
    {
        Renderers = gameObject.transform.parent.GetComponentsInChildren<Renderer>();
        WIMRenderers = WIMObjects.GetComponentsInChildren<Renderer>();
    }

    public void TurnOffWIMObjects () {
        WIMObjects.SetActive(false);
    }

    public void TurnOffWIMShadows() {
        foreach (Renderer renderer in WIMRenderers)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    public void TurnOffShadows()
    {
        foreach (Renderer renderer in Renderers)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    public void TurnOnShadows()
    {
        foreach (Renderer renderer in Renderers)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }

}
