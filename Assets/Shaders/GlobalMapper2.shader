Shader "Global-Mapper2" {
Properties {
    _RimColor ("Rim Color", Color) = (0.26, 0.19, 0.16, 0.0)
    _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
    _PeakTex ("PeakTexture", 2D) = "white" {}
    _PeakLevel ("PeakLevel", Float) = 300
    _Level5Tex ("Level5Texture", 2D) = "white" {}
    _Level5 ("Level5", Float) = 400
    _Level4Color ("Level4Color", Color) = (0.80,0.53,0,1)
    _Level4 ("Level4", Float) = 300
    _Level3Color ("Level3Color", Color) = (0.75,0.53,0,1)
    _Level3 ("Level3", Float) = 200
    _Level2Color ("Level2Color", Color) = (0.69,0.63,0.31,1)
    _Level2 ("Level2", Float) = 100
    _Level1Color ("Level1Color", Color) = (0.65,0.86,0.63,1)
    _WaterLevel ("WaterLevel", Float) = 0
    _WaterColor ("WaterColor", Color) = (0.37,0.78,0.92,1)
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
	float2 uv_PeakTex;
	float2 uv_Level5Tex;
	float3 viewDir;
    };
    void vert (inout appdata_full v, out Input o) {
        o.customColor = abs(v.normal.y);
    }
    float4 _RimColor;
    float _RimPower;
    sampler2D _PeakTex;
    float _PeakLevel;
    float4 _PeakColor;
    float _Level5;
    sampler2D _Level5Tex;
    float _Level4;
    float4 _Level4Color;
    float _Level3;
    float4 _Level3Color;
    float _Level2;
    float4 _Level2Color;
    float _Level1;
    float4 _Level1Color;
    float _Slope;
    float _WaterLevel;
    float4 _WaterColor;
    void surf (Input IN, inout SurfaceOutput o) {
        if (IN.worldPos.y >= _PeakLevel) {
	    float4 peakColor = tex2D(_PeakTex, IN.uv_PeakTex);
	    o.Albedo = peakColor;
	}
        if (IN.worldPos.y <= _PeakLevel) {
	    float4 peakColor = tex2D(_PeakTex, IN.uv_PeakTex);
	    float4 level5Color = tex2D(_Level5Tex, IN.uv_Level5Tex);
            o.Albedo = lerp(level5Color, peakColor, (IN.worldPos.y - _Level5)/(_PeakLevel - _Level5));
	}	       
        if (IN.worldPos.y <= _Level5) {
	    float4 level5Color = tex2D(_Level5Tex, IN.uv_Level5Tex);
            o.Albedo = lerp(_Level4Color, level5Color, (IN.worldPos.y - _Level4)/(_Level5 - _Level4));
	}
        if (IN.worldPos.y <= _Level4)
            o.Albedo = lerp(_Level3Color, _Level4Color, (IN.worldPos.y - _Level3)/(_Level4 - _Level3));
        if (IN.worldPos.y <= _Level3)
            o.Albedo = lerp(_Level2Color, _Level3Color, (IN.worldPos.y - _Level2)/(_Level3 - _Level2));
        if (IN.worldPos.y <= _Level2)
            o.Albedo = lerp(_Level1Color, _Level2Color, (IN.worldPos.y - _WaterLevel)/(_Level2 - _WaterLevel));
        if (IN.worldPos.y <= _WaterLevel) {
            //o.Albedo = _WaterColor;
	    clip (frac((IN.worldPos.y+IN.worldPos.z)) - 1.0);
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
