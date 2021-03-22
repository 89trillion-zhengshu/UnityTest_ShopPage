Shader "OutlineEx/UI/OutlineExOpt"
{
    Properties
	{

		[PerRendererData]_MainTex("Main Texture", 2D) = "white" {}
	    _Color("Tint", Color) = (1, 1, 1, 1)
		//_OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
		//_OutlineWidth ("Outline Width", Int) = 1

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		Stencil
	{
		Ref[_Stencil]
		Comp[_StencilComp]
		Pass[_StencilOp]
		ReadMask[_StencilReadMask]
		WriteMask[_StencilWriteMask]
	}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
	{
		Name "OUTLINE"

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 3.0

		//Add for RectMask2D  
		#include "UnityUI.cginc"
		//End for RectMask2D 

		sampler2D _MainTex;
		fixed4 _Color;
		fixed4 _TextureSampleAdd;
		float4 _MainTex_TexelSize;

		//float4 _OutlineColor;
		//int _OutlineWidth;
		fixed4 outLineColor;

		//Add for RectMask2D  
		float4 _ClipRect;
		//End for RectMask2D  

	struct appdata
	{
		float4 vertex : POSITION;
		float4 tangent : TANGENT;
		float4 normal : NORMAL;
		float2 texcoord : TEXCOORD0;
		float2 uv1 : TEXCOORD1;
		float2 uv2 : TEXCOORD2;
		float2 uv3 : TEXCOORD3;
		fixed4 color : COLOR;
	};


	struct v2f
	{
		float4 vertex : SV_POSITION;
		float4 tangent : TANGENT;
		float4 normal : NORMAL;
		float2 texcoord : TEXCOORD0;
		float2 uv1 : TEXCOORD1;
		float2 uv2 : TEXCOORD2;
		float2 uv3 : TEXCOORD3;
		fixed4 color : COLOR;

		//Add for RectMask2D  
		float4 worldPosition : TEXCOORD4;
		//End for RectMask2D
	};

	v2f vert(appdata IN)
	{
		v2f o;
		//Add for RectMask2D  
		o.worldPosition = IN.vertex;
		//End for RectMask2D 

		o.vertex = UnityObjectToClipPos(IN.vertex);
		o.tangent = IN.tangent;
		o.texcoord = IN.texcoord;
		o.color = IN.color * _Color;
		o.uv1 = IN.uv1;
		o.uv2 = IN.uv2;
		o.uv3 = IN.uv3;
		o.normal = IN.normal;
		return o;
	}
	/*
	fixed IsInRect(float2 pPos, float4 pClipRect)
	{
	pPos = step(pClipRect.xy, pPos) * step(pPos, pClipRect.zw);
	return pPos.x * pPos.y;
	}
	*/
	fixed IsInRect(float2 pPos, float2 pClipRectMin, float2 pClipRectMax)
	{
		pPos = step(pClipRectMin, pPos) * step(pPos, pClipRectMax);
		return pPos.x * pPos.y;
	}

	fixed SampleAlpha(int pIndex, v2f IN)
	{
		const fixed sinArray[12] = { 0, 0.5, 0.866, 1, 0.866, 0.5, 0, -0.5, -0.866, -1, -0.866, -0.5 };
	    const fixed cosArray[12] = { 1, 0.866, 0.5, 0, -0.5, -0.866, -1, -0.866, -0.5, 0, 0.5, 0.866 };
		float2 pos = IN.texcoord + _MainTex_TexelSize.xy * float2(cosArray[pIndex], sinArray[pIndex]) * IN.uv3.y;
		return IsInRect(pos, IN.uv1, IN.uv2) * (tex2D(_MainTex, pos) + _TextureSampleAdd).w;// * outLineColor.w;
	}
	
	fixed4 GetOutLineColor(float num)
	{
	    int ori = trunc(num);
		if (ori > 198989898) {
		    return fixed4(1, 1, 1, 1);
		} else {
		    fixed a = (ori % 100) * 0.01 + 0.01;
            fixed b = (ori % 10000 / 100) * 0.01 + 0.01;
            fixed g = (ori % 1000000 / 10000) * 0.01 + 0.01;
            fixed r = (ori % 100000000 / 1000000) * 0.01 + 0.01;
            return fixed4(r, g, b, a);
		}
	}

	fixed4 frag(v2f IN) : SV_Target
	{
	    fixed4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
        if (IN.uv3.y > 0)
        {
            outLineColor = GetOutLineColor(IN.uv3.x);
            color.w *= IsInRect(IN.texcoord, IN.uv1, IN.uv2);
            half4 val = half4(outLineColor.x, outLineColor.y, outLineColor.z, 0);
            
            val.w += SampleAlpha(0, IN);
            val.w += SampleAlpha(1, IN);
            val.w += SampleAlpha(2, IN);
            val.w += SampleAlpha(3, IN);
            val.w += SampleAlpha(4, IN);
            val.w += SampleAlpha(5, IN);
            val.w += SampleAlpha(6, IN);
            val.w += SampleAlpha(7, IN);
            val.w += SampleAlpha(8, IN);
            val.w += SampleAlpha(9, IN);
            val.w += SampleAlpha(10, IN);
            val.w += SampleAlpha(11, IN);
    
            color = saturate(val * (1.0 - color.a) + color * color.a);
            color.a *= IN.color.a * IN.color.a * IN.color.a;	//字逐渐隐藏时，描边也要隐藏
        }

	    //Add for RectMask2D 
	    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

	    #ifdef UNITY_UI_ALPHACLIP
		    clip(color.a - 0.001);
	    #endif
	    //End for RectMask2D 

	return color;
	}

		ENDCG
	}
	}
	
	FallBack "UI/Default"
}