using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessOutlineRenderer : PostProcessEffectRenderer<PostProcessOutline>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/PostProcessOutline"));
        Debug.Log("PostProcessOutlineRenderer.Render: sheet=" + sheet);
        sheet.properties.SetFloat("_OutlineThickness", settings.outlineThickness);
        sheet.properties.SetFloat("_DepthMin", settings.depthMin);
        sheet.properties.SetFloat("_DepthMax", settings.depthMax);
        sheet.properties.SetColor("_OutlineColor", settings.outlineColor);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}