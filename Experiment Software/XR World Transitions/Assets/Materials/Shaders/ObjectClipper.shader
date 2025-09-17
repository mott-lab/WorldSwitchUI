Shader "Unlit/ObjectClipper"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SpherePosition ("Sphere Position", Vector) = (-0.238, 1.17, 1.083)
        _SphereRadius ("Sphere Radius", Float) = 0.5
        _Cutoff ("Alpha Clip Threshold", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="AlphaTest"}
        LOD 100

        Pass
        {
            Tags { "LightMode" = "ClipPassOne"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float3 _SpherePosition;
            float _SphereRadius;
            float _Cutoff;

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // clip if inside sphere
                float3 spherePosition = _SpherePosition.xyz;
                float sphereRadius = _SphereRadius;
                // float distance = i.worldPos - _SpherePosition;
                float3 worldPos = i.worldPos;
                float distance = length(worldPos - _SpherePosition);
                float alpha = step(distance, _SphereRadius);
                col.a = alpha;
                clip(alpha - _Cutoff);
                // // distance = distance / sphereRadius;
                // if (distance > sphereRadius)
                // {
                //     clip(-1);
                // }

                // col = float4(distance, distance, distance, 1);
                return col;
            }
            ENDCG
        }
    }
}
