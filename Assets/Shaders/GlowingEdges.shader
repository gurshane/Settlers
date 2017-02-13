Shader "Test/GlowingEdges" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_RimEffectStrength("Rim Effect Strength", Range(-1,1)) = 0
		_EdgeGlowColor("Edge Glow Color", Color) = (1,1,1,1)
		_EdgeGlowStrength("Edge Glow Strength", Range(1,10)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldNormal;
			float3 viewDir;
		};
		
		fixed4 _EdgeGlowColor;
		float _RimEffectStrength;
		float _EdgeGlowStrength;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color

			float edgeValue = 1 - (abs(dot(IN.worldNormal, IN.viewDir)));
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

			if (edgeValue >= 0.75)
			{
				o.Albedo = _EdgeGlowColor.rgb;
				o.Emission = pow(_EdgeGlowColor, _EdgeGlowStrength);
			}
			else
			{
				o.Albedo = c.rgb;
			}

			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
