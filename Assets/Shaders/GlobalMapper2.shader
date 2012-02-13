Shader "Global-Mapper2" {
Properties {
    _RimColor ("Rim Color", Color) = (0.26, 0.19, 0.16, 0.0)
    _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
    _PeakTex ("PeakTexture", 2D) = "white" {}
    _PeakLevel ("PeakLevel", Float) = 300
    _Level5Tex ("Level5Texture", 2D) = "white" {}
    _Level5 ("Level5", Float) = 400
    _Level4Tex ("Level4Texture", 2D) = "white" {}
    _Level4 ("Level4", Float) = 300
    _Level3Tex ("Level3Texture", 2D) = "white" {}
    _Level3 ("Level3", Float) = 200
    _Level2Tex ("Level2Texture", 2D) = "white" {}
    _Level2 ("Level2", Float) = 100
    _Level1Tex ("Level1Texture", 2D) = "white" {}
    _WaterLevel ("WaterLevel", Float) = 0
    _WaterTex ("WaterTexture", 2D) = "white" {}
    _Slope ("Slope Fader", Range (0,1)) = 0
}
SubShader {
    Tags { "RenderType" = "Opaque" }
    Fog { Mode Off }
    CGPROGRAM
    #pragma surface surf Lambert 
    #pragma target 3.0
    struct Input {
        float3 customColor;
        float3 worldPos;
	//float2 uv_PeakTex;
	//float2 uv_Level5Tex;
	//float2 uv_Level4Tex;
	//float2 uv_Level3Tex;
	//float2 uv_Level2Tex;
	//float2 uv_Level1Tex;
	float2 uv_WaterTex;
	float3 viewDir;
    };
    void vert (inout appdata_full v, out Input o) {
        o.customColor = abs(v.normal.y);
    }
    float4 _RimColor;
    float _RimPower;
    float _PeakLevel;
    sampler2D _PeakTex;
    float _Level5;
    sampler2D _Level5Tex;
    float _Level4;
    sampler2D _Level4Tex;
    float _Level3;
    sampler2D _Level3Tex;
    float _Level2;
    sampler2D _Level2Tex;
    float _Level1;
    sampler2D _Level1Tex;
    float _WaterLevel;
    sampler2D _WaterTex;
    float _Slope;
    void surf (Input IN, inout SurfaceOutput o) {
        if (IN.worldPos.y >= _PeakLevel) {
	    float4 peakColor = tex2D(_PeakTex, IN.uv_WaterTex);
	    o.Albedo = peakColor;
	}
        if (IN.worldPos.y <= _PeakLevel) {
	    float4 peakColor = tex2D(_PeakTex, IN.uv_WaterTex);
	    float4 level5Color = tex2D(_Level5Tex, IN.uv_WaterTex);
            o.Albedo = lerp(level5Color, peakColor, (IN.worldPos.y - _Level5)/(_PeakLevel - _Level5));
	}	       
        if (IN.worldPos.y <= _Level5) {
	    float4 level5Color = tex2D(_Level5Tex, IN.uv_WaterTex);
	    float4 level4Color = tex2D(_Level4Tex, IN.uv_WaterTex);
            o.Albedo = lerp(level4Color, level5Color, (IN.worldPos.y - _Level4)/(_Level5 - _Level4));
	}
        if (IN.worldPos.y <= _Level4) {
	    float4 level4Color = tex2D(_Level4Tex, IN.uv_WaterTex);
	    float4 level3Color = tex2D(_Level3Tex, IN.uv_WaterTex);
            o.Albedo = lerp(level3Color, level4Color, (IN.worldPos.y - _Level3)/(_Level4 - _Level3));
	}
        if (IN.worldPos.y <= _Level3) {
	    float4 level3Color = tex2D(_Level3Tex, IN.uv_WaterTex);
	    float4 level2Color = tex2D(_Level2Tex, IN.uv_WaterTex);
            o.Albedo = lerp(level2Color, level3Color, (IN.worldPos.y - _Level2)/(_Level3 - _Level2));
	}
        if (IN.worldPos.y <= _Level2) {
	    float4 level2Color = tex2D(_Level2Tex, IN.uv_WaterTex);
	    float4 level1Color = tex2D(_Level1Tex, IN.uv_WaterTex);
            o.Albedo = lerp(level1Color, level2Color, (IN.worldPos.y - _WaterLevel)/(_Level2 - _WaterLevel));
	}
        if (IN.worldPos.y <= _WaterLevel) {
	    float4 waterColor = tex2D(_WaterTex, IN.uv_WaterTex);
            o.Albedo = waterColor;
	    // transparent: 
	    //clip (frac((IN.worldPos.y+IN.worldPos.z)) - 1.0);
	}
        o.Albedo *= saturate(IN.customColor + _Slope);
	o.Specular = 0;
	half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
	o.Emission = _RimColor.rgb * pow (rim, _RimPower);
    }
    ENDCG
}
Fallback "Diffuse"
}
