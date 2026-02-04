Shader "URP/OutlineShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0.00, 0.1)) = 0.02
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }
        LOD 100

        // Pass for the Outline (Rendered First)
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="SRPDefaultUnlit" }
            
            Cull Front // Render back faces to create an outline
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            Varyings vert (Attributes v)
            {
                Varyings o;
                float3 normalWS = TransformObjectToWorldNormal(v.normalOS);
                float3 positionWS = TransformObjectToWorld(v.positionOS.xyz);
                
                positionWS += normalWS * _OutlineWidth; // Expand the model along its normals
                o.positionCS = TransformWorldToHClip(positionWS);
                
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return _OutlineColor; // Render solid outline color
            }
            ENDHLSL
        }

        // Pass for the Main Object (Rendered on Top)
        Pass
        {
            Name "MainObject"
            Tags { "LightMode"="UniversalForward" }

            Cull Back // Render the object normally

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            float4 _BaseColor;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return _BaseColor;
            }
            ENDHLSL
        }
    }
}
