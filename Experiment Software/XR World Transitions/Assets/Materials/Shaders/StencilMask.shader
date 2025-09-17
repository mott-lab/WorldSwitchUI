Shader "Custom/StencilMask"
{
    Properties
    {
        [IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
        _Radius ("Circle Radius", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1" "RenderPipeline" = "UniversalPipeline"}

        Blend Zero One
        ZWrite Off

        Stencil
        {
            Ref [_StencilID]
            Comp Always
            Pass Replace
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Radius;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Convert UV to range [-1,1] where (0,0) is the center
                float2 centeredUV = i.uv * 2.0 - 1.0;
                float distanceFromCenter = length(centeredUV);

                // Discard pixels outside the circle
                if (distanceFromCenter > _Radius)
                    discard;

                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}