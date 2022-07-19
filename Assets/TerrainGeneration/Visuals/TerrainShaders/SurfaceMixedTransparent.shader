Shader "Custom/SurfaceMixedTransparent"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_ColorMode("ColorMode", Range(0,1)) = 0.0
		_AlphaTex("Transparent Tex (Greyscale)", 2D) = "white" {}
	}
		SubShader
	{
		Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types including transparency
		#pragma surface surf Standard alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _AlphaTex;

		struct Input
		{
			float2 uv_MainTex;
			fixed4 color : COLOR;
			//float2 secondUV : TEXCOORD1;

		};

		half _Glossiness;
		half _Metallic;
		half _ColorMode;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 alpha = tex2D(_AlphaTex, IN.uv_MainTex);

			c = _ColorMode * c * IN.color + (1 - _ColorMode) * (c + IN.color);

			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = alpha.r;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
