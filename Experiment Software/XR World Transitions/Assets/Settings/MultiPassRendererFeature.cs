using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using System.Collections.Generic;

public class MultiPassRendererFeature : ScriptableRendererFeature
{
    public List<string> lightModePasses;
    private MultiPassPass mainPass;

    /// <inheritdoc/>
    public override void Create()
    {
        mainPass = new MultiPassPass(lightModePasses);

        // Configures where the render pass should be injected.
        mainPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(mainPass);
    }
}
