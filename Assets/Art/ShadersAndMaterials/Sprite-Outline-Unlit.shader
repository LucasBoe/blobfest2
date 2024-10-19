// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Texture-Outline"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_MainTex("MainTex", 2D) = "white" {}
		_OutlineColor("OutlineColor", Color) = (1,1,1,1)
		[Enum(OnlyOutline,0,AlsoSprite,1)]_OnlyOutline("OnlyOutline", Int) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}


		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform int _OnlyOutline;
			uniform float4 _OutlineColor;
			float4 _MainTex_TexelSize;

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				
				
				OUT.worldPosition.xyz +=  float3( 0, 0, 0 ) ;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 appendResult2_g61 = (float2(_MainTex_TexelSize.x , _MainTex_TexelSize.y));
				float2 appendResult2_g62 = (float2(_MainTex_TexelSize.x , _MainTex_TexelSize.y));
				float2 appendResult2_g63 = (float2(_MainTex_TexelSize.x , _MainTex_TexelSize.y));
				float2 appendResult2_g64 = (float2(_MainTex_TexelSize.x , _MainTex_TexelSize.y));
				float clampResult8_g59 = clamp( ( tex2D( _MainTex, (uv_MainTex*1.0 + ( appendResult2_g61 * float2( 1,0 ) )) ) + tex2D( _MainTex, (uv_MainTex*1.0 + ( appendResult2_g62 * float2( -1,0 ) )) ) + tex2D( _MainTex, (uv_MainTex*1.0 + ( appendResult2_g63 * float2( 0,1 ) )) ) + tex2D( _MainTex, (uv_MainTex*1.0 + ( appendResult2_g64 * float2( 0,-1 ) )) ) ).a , 0.0 , 1.0 );
				float2 appendResult2_g60 = (float2(_MainTex_TexelSize.x , _MainTex_TexelSize.y));
				float4 temp_output_37_0 = ( _OutlineColor * ( clampResult8_g59 - tex2D( _MainTex, (uv_MainTex*1.0 + ( appendResult2_g60 * float2( 0,0 ) )) ).a ) );
				float4 lerpResult40 = lerp( ( tex2D( _MainTex, uv_MainTex ) * _OnlyOutline ) , temp_output_37_0 , temp_output_37_0.a);
				
				half4 color = lerpResult40;
				
				#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18935
0;66;1728;946;828.9393;333.0881;1;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;5;-1194.806,-132.1904;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;0;False;0;False;5cb782f76053a412aaca9a0f83037bd7;5cb782f76053a412aaca9a0f83037bd7;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ColorNode;36;-536.2951,229.5857;Inherit;False;Property;_OutlineColor;OutlineColor;1;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;37;-303.382,144.5443;Inherit;True;SampleOutline;-1;;59;5a71788c9530442cab70e2373439963a;0;2;11;SAMPLER2D;0;False;12;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;6;-433.5104,-272.7231;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;42;-302.9393,4.911926;Inherit;False;Property;_OnlyOutline;OnlyOutline;2;1;[Enum];Create;True;0;2;OnlyOutline;0;AlsoSprite;1;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;38;-29.62698,286.3218;Inherit;True;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-96.93933,-40.08807;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;40;203.5973,74.18423;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;41;453.6851,20.23091;Float;False;True;-1;2;ASEMaterialInspector;0;6;Texture-Outline;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;True;0;True;-9;False;False;False;False;False;False;False;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;0;True;-11;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;37;11;5;0
WireConnection;37;12;36;0
WireConnection;6;0;5;0
WireConnection;38;0;37;0
WireConnection;43;0;6;0
WireConnection;43;1;42;0
WireConnection;40;0;43;0
WireConnection;40;1;37;0
WireConnection;40;2;38;3
WireConnection;41;0;40;0
ASEEND*/
//CHKSM=A8F1F3ACD851453BAB2D2B03FC3FEC4995C0DF8E