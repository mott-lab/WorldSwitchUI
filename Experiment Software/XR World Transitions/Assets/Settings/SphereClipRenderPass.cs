using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class SphereClipRenderPass : ScriptableRenderPass
{
    private const string k_mainTextureName = "_MainTex";
    private const string k_sphereClipPassName = "SphereClipRenderPass";
    private Material material;
    private RenderTextureDescriptor sphereClipTextureDescriptor;
    public SphereClipRenderPass(Material material) {
        this.material = material;
        sphereClipTextureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        // UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        // UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

        // // The following line ensures that the render pass doesn't blit
        // // from the back buffer.
        // if (resourceData.isActiveTargetBackBuffer)
        //     return;

        // // // Get the current object's material
        // Material objectMaterial = cameraData.camera.GetComponent<Renderer>().material;

        // // // Combine the object's material with the pass material
        // material.CopyPropertiesFromMaterial(objectMaterial);

        // TextureHandle srcCamColor = resourceData.activeColorTexture;
        // TextureHandle dst = UniversalRenderer.CreateRenderGraphTexture(renderGraph, sphereClipTextureDescriptor,
        //     k_mainTextureName, false);

        // // // // Update the blur settings in the material
        // // // UpdateBlurSettings();

        // // // This check is to avoid an error from the material preview in the scene
        // if (!srcCamColor.IsValid() || !dst.IsValid())
        //     return;
        
        // // // The AddBlitPass method adds a vertical blur render graph pass that blits from the source texture (camera color in this case) to the destination texture using the first shader pass (the shader pass is defined in the last parameter).
        // RenderGraphUtils.BlitMaterialParameters paraVertical = new(srcCamColor, dst, material, 0);
        // renderGraph.AddBlitPass(paraVertical, k_sphereClipPassName);
        
    }

}