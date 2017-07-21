Shader "DynamicFog/Image Effect/Desktop Plus" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_NoiseTex ("Noise (RGB)", 2D) = "white" {}
		_Noise2Tex ("Noise2 (RGB)", 2D) = "white" {}
		_FogAlpha ("Alpha", Range (0, 1)) = 0.8
		_FogDistance ("Distance Params", Vector) = (0.1, 0.001, 1.0, 0.15)
		_FogHeightData ("Baseline Height", Vector) = (1,0,0,0.1)  // x = height, y = base height, z = clipping minimum height, w = height fall off
		_FogColor ("Color", Color) = (1,1,1,1)
		_FogColor2 ("Color 2", Color) = (1,1,1,1)
		_FogNoiseData ("Noise Data", Vector) = (0,0,0)
		_FogSpeed ("Speed", Range (0, 0.5)) = 0.1
		_FogOfWarCenter("Fog Of War Center", Vector) = (0,0,0)
		_FogOfWarSize("Fog Of War Size", Vector) = (1,1,1)
		_FogOfWar ("Fog of War Mask", 2D) = "white" {}
	}
	SubShader {
    ZTest Always Cull Off ZWrite Off
   	Fog { Mode Off }
	Pass {

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma fragmentoption ARB_precision_hint_fastest
	#pragma multi_compile __ FOG_OF_WAR_ON
	#pragma multi_compile __ DITHER_ON
	#pragma target 3.0

	#include "UnityCG.cginc"
	#define DYNAMIC_FOG_STEPS 5

	sampler2D _MainTex;
	sampler2D _NoiseTex;
	sampler2D _Noise2Tex;
	sampler2D_float _CameraDepthTexture;
	float4 _MainTex_ST;
	float4 _MainTex_TexelSize;
	float _FogAlpha;
	float4 _FogDistance; // x = min distance, y = min distance falloff, x = max distance, y = max distance fall off
	float4 _FogHeightData;
	float3 _FogNoiseData; // x = noise, y = turbulence, z = depth attenuation
	float4 _FogSkyData; // x = haze, y = speed, z = noise, w = alpha
	float _FogSpeed;
	fixed4 _FogColor, _FogColor2;

    #if FOG_OF_WAR_ON 
    sampler2D _FogOfWar;
    float3 _FogOfWarCenter;
    float3 _FogOfWarSize;
    float3 _FogOfWarCenterAdjusted;
    #endif

    float4x4 _ClipToWorld;
    
    struct appdata {
    	float4 vertex : POSITION;
		half2 texcoord : TEXCOORD0;
    };
    
    
	struct v2f {
	    float4 pos : SV_POSITION;
	    float2 uv: TEXCOORD0;
    	float2 depthUV : TEXCOORD1;
    	float3 cameraToFarPlane : TEXCOORD2;
	};

	v2f vert(appdata v) {
    	v2f o;
    	o.pos = UnityObjectToClipPos(v.vertex);
    	o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
    	o.depthUV = o.uv;
   	      
    	#if UNITY_UV_STARTS_AT_TOP
    	if (_MainTex_TexelSize.y < 0) {
	        // Depth texture is inverted WRT the main texture
    	    o.depthUV.y = 1 - o.depthUV.y;
    	}
    	#endif
               
    	// Clip space X and Y coords
    	float2 clipXY = o.pos.xy / o.pos.w;
               
    	// Position of the far plane in clip space
    	float4 farPlaneClip = float4(clipXY, 1, 1);
               
    	// Homogeneous world position on the far plane
    	farPlaneClip *= float4(1,_ProjectionParams.x,1,1);    	
    	float4 farPlaneWorld4 = mul(_ClipToWorld, farPlaneClip);
               
    	// World position on the far plane
    	float3 farPlaneWorld = farPlaneWorld4.xyz / farPlaneWorld4.w;
               
    	// Vector from the camera to the far plane
    	o.cameraToFarPlane = farPlaneWorld - _WorldSpaceCameraPos;
 
    	return o;
	}

	
	fixed4 computeSkyColor(fixed4 color, float3 worldPos) {
		float wpy = abs(worldPos.y) + 2.0;
		float2 np = worldPos.xz/wpy;
		float skyNoise = tex2D(_NoiseTex, np * 0.01 +_Time.x * _FogSkyData.y).g;
		fixed4 skyFogColor = lerp(_FogColor, _FogColor2, saturate(wpy / _FogHeightData.x));
		return lerp(color, skyFogColor, _FogSkyData.w * saturate((_FogSkyData.x / wpy)*(1-skyNoise*_FogSkyData.z)));
	}

	float noise3D(float3 x ) {
    	float3 f = frac(x);
    	float3 p = x - f;
		f = f*f*(3.0-2.0*f);
		float4 xy = float4(p.xy + float2(37.0,17.0)*p.z + f.xy, 0, 0);
		xy.xy = (xy.xy + 0.5) / 256.0;
		float2 zz = tex2Dlod(_Noise2Tex, xy).yx;
		return lerp( zz.x, zz.y, f.z );
	}
		
	float3 getWorldPos(v2f i, float depth01) {
    	// Reconstruct the world position of the pixel
     	_WorldSpaceCameraPos.y -= _FogHeightData.y;
    	float3 worldPos = (i.cameraToFarPlane * depth01) + _WorldSpaceCameraPos;
    	worldPos.y += 0.00001; // fixes artifacts when worldPos.y = _WorldSpaceCameraPos.y which is really rare but occurs at y = 0
    	return worldPos;
    }


	half4 getFogColor(float2 uv, float3 worldPos, float depth, fixed4 color) {
		
		// early exit if fog is not crossed
		if (_WorldSpaceCameraPos.y>_FogHeightData.x && worldPos.y>_FogHeightData.x) {
			return color;		
		}

		half voidAlpha = 1.0;

		// Determine "fog length" and initial ray position between object and camera, cutting by fog distance params
		float3 adir = worldPos - _WorldSpaceCameraPos;
		
		// ceiling cut
		float delta = length(adir.xz);
		float2 ndirxz = adir.xz / delta;
		delta /= adir.y;
		
		float h = min(_WorldSpaceCameraPos.y, _FogHeightData.x);
		float xh = delta * (_WorldSpaceCameraPos.y - h);
		float2 xz = _WorldSpaceCameraPos.xz - ndirxz * xh;
		float3 fogCeilingCut = float3(xz.x, h, xz.y);
		
		// does fog stars after pixel? If it does, exit now
		float adirLength = length(adir);
		float dist  = min(adirLength, _FogDistance.z);
		float distanceToFog = distance(fogCeilingCut, _WorldSpaceCameraPos);
		if (distanceToFog>=dist) return color;

		// floor cut
		float hf = 0;
		// edge cases
		if (delta>0 && worldPos.y > -0.5) {
			hf = _FogHeightData.x;
		} else if (delta<0 && worldPos.y < 0.5) {
			hf = worldPos.y;
		}
		float xf = delta * ( hf - _WorldSpaceCameraPos.y ); 
		float2 xzb = _WorldSpaceCameraPos.xz - ndirxz * xf;
		float3 fogFloorCut = float3(xzb.x, hf, xzb.y);

		// fog length is...
		float fogLength = distance(fogCeilingCut, fogFloorCut);
		fogLength = min(fogLength, dist - distanceToFog);
		if (fogLength<=0) return color;
		fogFloorCut = fogCeilingCut + (adir/adirLength) * fogLength;
		
		#if FOG_OF_WAR_ON
		if (depth<0.999) {
			float2 fogTexCoord = fogFloorCut.xz / _FogOfWarSize.xz - _FogOfWarCenterAdjusted.xz;
			voidAlpha = tex2D(_FogOfWar, fogTexCoord).a;
			if (voidAlpha <=0) return color;
		}
		#endif
		
		float3 st = (fogFloorCut - fogCeilingCut) / DYNAMIC_FOG_STEPS;
		float3 pos = fogCeilingCut;
		fixed4 fogColor = fixed4(0,0,0,0);
		float incDist = fogLength / DYNAMIC_FOG_STEPS;
		for (int k=DYNAMIC_FOG_STEPS;k>=0;k--, pos += st, distanceToFog += incDist) {
			float fh = (_FogHeightData.x - pos.y) / (_FogHeightData.x * _FogHeightData.w) - 0.1;
			float fl = (distanceToFog - _FogDistance.x) / _FogDistance.y;
			fh = min(fh, fl);
			float noise = noise3D(pos * 0.1 + _Time.www * _FogSpeed * 5.0);
			fixed4 col = lerp(_FogColor, _FogColor2, saturate( pos.y / _FogHeightData.x) );
			col.a *= saturate ( fh * (1.0 - noise * _FogNoiseData.x ));
			col.rgb *= col.a;
			fogColor += col * (1.0 - fogColor.a);
		}
		#if DITHER_ON
		float dither = dot(float2(2.4084507, 3.2535211), uv * _MainTex_TexelSize.zw);
		dither = frac(dither) - 0.4;
		fogColor *= 1.0 + dither * 0.01;
		#endif
		fogColor *= voidAlpha * _FogAlpha;
	 	return color * (1.0 - fogColor.a) + fogColor;
	}

	// Fragment Shader
	fixed4 frag (v2f i) : SV_Target {
   		fixed4 color = tex2D(_MainTex, i.uv);
		float depth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.depthUV)));
		float3 worldPos = getWorldPos(i, depth);
		
		if (depth>0.999) {
	    	color = computeSkyColor(color, worldPos);
		}
		return getFogColor(i.uv, worldPos, depth, color);
	}
	ENDCG
	}
}
FallBack Off

}