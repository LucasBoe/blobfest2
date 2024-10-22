// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Sprite-Progressbar"
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
		_progress("progress", Float) = 0.5
		_barFillColor("barFillColor", Color) = (1,0.8761824,0,1)
		_barBackgroundColor("barBackgroundColor", Color) = (0.5201139,0.5201139,0.5201139,1)

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
			float4 _MainTex_TexelSize;
			uniform float4 _barBackgroundColor;
			uniform float4 _barFillColor;
			uniform float4 _MainTex_ST;
			uniform float _progress;

			
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

				float2 appendResult10_g16 = (float2(( 1.0 - ( _MainTex_TexelSize.x * 2.0 ) ) , _MainTex_TexelSize.y));
				float2 temp_output_11_0_g16 = ( abs( (IN.texcoord.xy*2.0 + -1.0) ) - appendResult10_g16 );
				float2 break16_g16 = ( 1.0 - ( temp_output_11_0_g16 / fwidth( temp_output_11_0_g16 ) ) );
				float temp_output_10_0 = saturate( min( break16_g16.x , break16_g16.y ) );
				float barFullMask15 = temp_output_10_0;
				float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float temp_output_31_0 = step( (0.0 + (uv_MainTex.x - _MainTex_TexelSize.x) * (1.0 - 0.0) / (( 1.0 - _MainTex_TexelSize.x ) - _MainTex_TexelSize.x)) , _progress );
				float4 lerpResult33 = lerp( _barBackgroundColor , _barFillColor , temp_output_31_0);
				float2 appendResult10_g15 = (float2(1.0 , ( _MainTex_TexelSize.y * 3.0 )));
				float2 temp_output_11_0_g15 = ( abs( (IN.texcoord.xy*2.0 + -1.0) ) - appendResult10_g15 );
				float2 break16_g15 = ( 1.0 - ( temp_output_11_0_g15 / fwidth( temp_output_11_0_g15 ) ) );
				float barOutlineMask16 = ( saturate( min( break16_g15.x , break16_g15.y ) ) - temp_output_10_0 );
				
				half4 color = ( ( barFullMask15 * lerpResult33 ) + barOutlineMask16 );
				
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
12.67924;79.69811;2048.604;1048.585;47.34558;493.504;1;True;True
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;4;-743.2135,3.834606;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexelSizeNode;9;-468.4469,-352.9105;Inherit;False;-1;1;0;SAMPLER2D;;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;27;70.758,17.20325;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-377.651,-26.90601;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-277.3526,-432.6085;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-167.7678,-221.5886;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;400.7217,209.3062;Inherit;False;Property;_progress;progress;0;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;12;-117.0507,-472.4576;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;26;292.6448,-24.45721;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;8;90.76959,-233.907;Inherit;False;Rectangle;-1;;15;6b23e0c975270fb4084c354b2c83366a;0;3;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT;0.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;34;735.1451,-200.9758;Inherit;False;Property;_barBackgroundColor;barBackgroundColor;2;0;Create;True;0;0;0;False;0;False;0.5201139,0.5201139,0.5201139,1;0.5201139,0.5201139,0.5201139,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;31;695.967,161.3061;Inherit;True;2;0;FLOAT;1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;35;717.032,-20.74933;Inherit;False;Property;_barFillColor;barFillColor;1;0;Create;True;0;0;0;False;0;False;1,0.8761824,0,1;0.5201139,0.5201139,0.5201139,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;10;84.00592,-363.7782;Inherit;False;Rectangle;-1;;16;6b23e0c975270fb4084c354b2c83366a;0;3;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT;0.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;33;1007.749,-63.31543;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;15;529.5909,-367.605;Inherit;False;barFullMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;17;361.6554,-267.3745;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;1124.58,-356.7493;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;525.5799,-275.5255;Inherit;False;barOutlineMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;36;1306.617,-296.9757;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;1293.611,28.43785;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-166.2984,348.7363;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;5;-444.8059,508.4497;Inherit;False;SampleOutline;-1;;1;5a71788c9530442cab70e2373439963a;0;2;11;SAMPLER2D;0;False;12;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;6;-454.6743,295.9257;Inherit;False;SampleTextureWithOffset;-1;;14;5d3d9b54a24424438b781666315f3a7b;0;2;8;SAMPLER2D;0;False;9;FLOAT2;0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1554.231,-31.24762;Float;False;True;-1;2;ASEMaterialInspector;0;6;Sprite-Progressbar;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;True;0;True;-9;False;False;False;False;False;False;False;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;0;True;-11;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;9;0;4;0
WireConnection;27;0;9;1
WireConnection;24;2;4;0
WireConnection;13;0;9;1
WireConnection;14;0;9;2
WireConnection;12;1;13;0
WireConnection;26;0;24;1
WireConnection;26;1;9;1
WireConnection;26;2;27;0
WireConnection;8;3;14;0
WireConnection;31;0;26;0
WireConnection;31;1;30;0
WireConnection;10;2;12;0
WireConnection;10;3;9;2
WireConnection;33;0;34;0
WireConnection;33;1;35;0
WireConnection;33;2;31;0
WireConnection;15;0;10;0
WireConnection;17;0;8;0
WireConnection;17;1;10;0
WireConnection;32;0;15;0
WireConnection;32;1;33;0
WireConnection;16;0;17;0
WireConnection;36;0;32;0
WireConnection;36;1;16;0
WireConnection;22;0;31;0
WireConnection;7;0;6;0
WireConnection;7;1;5;0
WireConnection;5;11;4;0
WireConnection;6;8;4;0
WireConnection;0;0;36;0
ASEEND*/
//CHKSM=8B9791655E984AEBAD1755A22F3F3FC819A43C59