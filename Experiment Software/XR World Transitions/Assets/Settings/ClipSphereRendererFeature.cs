using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// public class ClipBySphereRendererFeature : ScriptableRendererFeature
// {
//     [System.Serializable]
//     public class ClipBySphereSettings
//     {
//         public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
//         public Material clipMaterial = null;
//         public Vector3 sphereCenter = Vector3.zero;
//         public float sphereRadius = 5.0f;
//     }

//     public ClipBySphereSettings settings = new ClipBySphereSettings();
//     private ClipBySphereRenderPass clipPass;

//     public override void Create()
//     {
//         // Instantiate the render pass and set its event.
//         clipPass = new ClipBySphereRenderPass(settings);
//         clipPass.renderPassEvent = settings.renderPassEvent;
//     }

//     // In URP 6000 using the latest API, use the cameraColorTargetHandle.
//     public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
//     {
//         clipPass.Setup(renderer.cameraColorTargetHandle);
//         renderer.EnqueuePass(clipPass);
//     }

//     private class ClipBySphereRenderPass : ScriptableRenderPass
//     {
//         private Material clipMaterial;
//         private RenderTargetHandle sourceHandle;
//         private RenderTargetHandle temporaryColorTexture;
//         private Vector3 sphereCenter;
//         private float sphereRadius;

//         public ClipBySphereRenderPass(ClipBySphereSettings settings)
//         {
//             clipMaterial = settings.clipMaterial;
//             sphereCenter = settings.sphereCenter;
//             sphereRadius = settings.sphereRadius;
//             temporaryColorTexture.Init("_TemporaryColorTexture");
//         }

//         // This method receives the current camera color target.
//         public void Setup(RenderTargetHandle source)
//         {
//             sourceHandle = source;
//         }

//         // Updated Execute method signature using 'in RenderingData'
//         public override void Execute(ScriptableRenderContext context, in RenderingData renderingData)
//         {
//             if (clipMaterial == null)
//             {
//                 Debug.LogWarning("ClipBySphereRenderPass: Missing clip material.");
//                 return;
//             }

//             Camera camera = renderingData.cameraData.camera;
//             // Get the camera-to-world and inverse projection matrices.
//             Matrix4x4 cameraToWorld = camera.cameraToWorldMatrix;
//             Matrix4x4 inverseProjection = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true).inverse;

//             // Pass matrices and sphere parameters to the clip shader.
//             clipMaterial.SetMatrix("_CameraToWorld", cameraToWorld);
//             clipMaterial.SetMatrix("_InverseProjection", inverseProjection);
//             clipMaterial.SetVector("_SphereCenter", new Vector4(sphereCenter.x, sphereCenter.y, sphereCenter.z, 1f));
//             clipMaterial.SetFloat("_SphereRadius", sphereRadius);

//             CommandBuffer cmd = CommandBufferPool.Get("ClipBySpherePass");

//             // Allocate a temporary render texture using the camera's target descriptor.
//             RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
//             cmd.GetTemporaryRT(temporaryColorTexture.id, descriptor, FilterMode.Bilinear);

//             // First, copy the scene color from the current target to the temporary RT.
//             cmd.Blit(sourceHandle.Identifier(), temporaryColorTexture.Identifier());
//             // Then blit back to the source while applying the clip shader.
//             cmd.Blit(temporaryColorTexture.Identifier(), sourceHandle.Identifier(), clipMaterial);

//             context.ExecuteCommandBuffer(cmd);
//             CommandBufferPool.Release(cmd);
//         }

//         // Clean up the temporary render texture.
//         public override void FrameCleanup(CommandBuffer cmd)
//         {
//             if (cmd == null)
//                 return;
//             cmd.ReleaseTemporaryRT(temporaryColorTexture.id);
//         }
//     }
// }
