Shader "Test/Silhoutte" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Tint("Tint", Color) = (1,1,1,1)
		_DotProduct("Rim effect", Range(-1, 1)) = 0.25
	}
		SubShader{
		Tags{ 
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Cull Off
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Tint;
		float _DotProduct;


	struct Input {
		float2 uv_MainTex;
		float3 worldNormal;
		float3 viewDir;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		
		float4 c = tex2D(_MainTex, IN.uv_MainTex) * _Tint;
		o.Albedo = c.rgb;

		//the dot product determines how close to the edge of a model we are. The closer we are the closer we are to -1 the more we want to make dissapear
		//1-theDotProduct gives us a fade to the edges as opposed to the facing polygons being visible and the edges missing
		//alpha gives us the ability to move between just the edges being visible to a transparent but not dissapearing model
		float border = 1 - (abs(dot(IN.viewDir, IN.worldNormal))); //fade to edges
		float alpha = (border * (1 - _DotProduct) + _DotProduct);//increase fade effect or decrease it
		o.Alpha = c.a * alpha;

	}
	ENDCG
	}
		FallBack "Diffuse"
}
