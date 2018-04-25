Shader "Hidden/P3D_TexturePreview"
{
	Properties
	{
		_Texture("Texture", 2D) = "white" {}
		_Tint("Tint", Color) = (1, 1, 1, 1)
		_Base("Base", Color) = (0, 0, 0, 0)
		_Opac("Opac", Color) = (0, 0, 0, 0)
	}
	SubShader
	{
		Tags
		{
			"Queue"           = "Overlay+10"
			"RenderType"      = "Transparent"
			"IgnoreProjector" = "True"
		}
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			Lighting Off
			ZWrite On
			Offset -1, -1
			
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				
				sampler2D _Texture;
				float4    _Texture_ST;
				float4x4  _Matrix;
				float4    _Tint;
				float4    _Base;
				float4    _Opac;
				
				struct a2v
				{
					float4 vertex    : POSITION;
					float2 texcoord0 : TEXCOORD0;
				};
				
				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float2 texcoord0 : TEXCOORD0;
				};
				
				struct f2g
				{
					float4 color : COLOR;
				};
				
				void Vert(a2v i, out v2f o)
				{
					float4x4 mat = mul(UNITY_MATRIX_VP, _Matrix);
					
					o.vertex    = mul(mat, i.vertex);
					o.texcoord0 = i.texcoord0 * _Texture_ST.xy + _Texture_ST.zw;
				}
				
				void Frag(v2f i, out f2g o)
				{
					float4 tex = tex2D(_Texture, i.texcoord0);
					
					o.color = _Base + _Tint * tex + _Opac * tex.a;
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader