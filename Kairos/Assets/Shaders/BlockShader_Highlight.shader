Shader "Custom/BlockShader_Highlight"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

		_Color("Main Color", Color) = (0.5,0.5,0.5,1)

		_OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineWidth("Outline Width", Range(0, 5)) = 2
        
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 0

    }

	CGINCLUDE
	#include "UnityCG.cginc"
 


	uniform sampler2D _MainTex;
	uniform float4 _Color;
	uniform float _Angle;

    uniform bool _EnableOutline;

	ENDCG

	SubShader{
        Pass {
            Name "Mask"
            Cull Off
            ZTest [_ZTest]
            ZWrite Off
            ColorMask 0

            Stencil {
              Ref 1
              Pass Replace
            }
        }

        Pass {
            Name "Fill"
            Cull Off
            ZTest [_ZTest]
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
        
            Stencil {
              Ref 1
              Comp NotEqual
            }
        
            CGPROGRAM
            #include "UnityCG.cginc"
        
            #pragma vertex vert
            #pragma fragment frag
        
            struct appdata {
              float4 vertex : POSITION;
              float3 normal : NORMAL;
              float3 smoothNormal : TEXCOORD3;
              UNITY_VERTEX_INPUT_INSTANCE_ID
            };
        
            struct v2f {
              float4 position : SV_POSITION;
              fixed4 color : COLOR;
              UNITY_VERTEX_OUTPUT_STEREO
            };
        
            uniform fixed4 _OutlineColor;
            uniform float _OutlineWidth;
        
            v2f vert(appdata input) {
              v2f output;
        
              UNITY_SETUP_INSTANCE_ID(input);
              UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        
              float3 normal = any(input.smoothNormal) ? input.smoothNormal : input.normal;
              float3 viewPosition = UnityObjectToViewPos(input.vertex);
              float3 viewNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, normal));
        
              output.position = UnityViewToClipPos(viewPosition + viewNormal * -viewPosition.z * _OutlineWidth / 1000.0);
              output.color = _OutlineColor;
        
              return output;
            }
        
            fixed4 frag(v2f input) : SV_Target {
              return input.color;
            }
            ENDCG
         }


        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0


        struct Input
        {
            float4 color : COLOR;
        };

        half _Glossiness;
        half _Metallic;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = IN.color * _Color;
            
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
