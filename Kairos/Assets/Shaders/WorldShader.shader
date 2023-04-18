Shader "Custom/WorldShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SecondTex("Corruption (RGB)", 2D) = "white" {}
        _CorruptionColorOver("Corruption Color Percent", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        #pragma vertex vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0



        sampler2D _MainTex;
        sampler2D _SecondTex;
        float _CorruptionColorOver;


        struct appdata
        {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
            float2 texcoord3 : TEXCOORD3;
            float3 normal : NORMAL;
            float4 tangent: TANGENT;
        };

        struct Input
        {
            float2 uv_MainTex;
            float2 corr;
        };


        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.corr = v.texcoord3;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Merge
            float4 tex1 = tex2D(_MainTex, IN.uv_MainTex);
            float4 tex2 = tex2D(_SecondTex, float2(0.0f, 1.0f));
            fixed4 c = lerp(tex1, tex2, IN.corr.x * _CorruptionColorOver);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
