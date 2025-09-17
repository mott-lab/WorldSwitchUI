using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessOutlineRenderer), PostProcessEvent.AfterStack, "Custom/PostProcessOutline")]
public sealed class PostProcessOutline : PostProcessEffectSettings
{

    [Range(0f, 1f), Tooltip("Outline effect thickness.")]
    public FloatParameter outlineThickness = new FloatParameter { value = 0.5f };
    public FloatParameter depthMin = new FloatParameter { value = 0.0f };
    public FloatParameter depthMax = new FloatParameter { value = 1.0f };
    [ColorUsage(false, true), Tooltip("Outline effect color.")]
    public ColorParameter outlineColor = new ColorParameter { value = Color.white };
}