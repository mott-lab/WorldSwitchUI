using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Rendering.Universal;

public class WIMRendererFeature : ScriptableRendererFeature
{

    [SerializeField] private Shader shader;
    private Material material;
    private SphereClipRenderPass sphereClipRenderPass;

    public override void Create()
    {
        if (shader == null)
        {
            return;
        }
        material = new Material(shader);
        sphereClipRenderPass = new SphereClipRenderPass(material);
        sphereClipRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(sphereClipRenderPass);
    }

    protected override void Dispose(bool disposing)
    {
        if (Application.isPlaying)
        {
            Destroy(material);
        }
        else
        {
            DestroyImmediate(material);
        }
    }

}
