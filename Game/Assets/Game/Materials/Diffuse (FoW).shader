Shader "Fog of War/Diffuse"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        sampler2D _MainTex;
        fixed4 _Color;

        sampler2D _FogTex0;
        sampler2D _FogTex1;

        uniform float4 _Params;
        uniform half4 _Unexplored;
        uniform half4 _Explored;

        struct Input
        {
            float2 uv_MainTex;
            float2 fog : TEXCOORD2;
        };

        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            float4 worldPos = mul (_Object2World, v.vertex);
            o.fog.xy = worldPos.xz * _Params.z + _Params.xy;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            half4 fog = lerp(tex2D(_FogTex0, IN.fog), tex2D(_FogTex1, IN.fog), _Params.w);
            c.rgb = lerp(lerp(c.rgb * _Unexplored, c.rgb * _Explored, fog.g), c.rgb, fog.r);

            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    Fallback "Diffuse"
}