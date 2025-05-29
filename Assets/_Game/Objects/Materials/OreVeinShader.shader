Shader "Custom/OreVeinShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "black" {}
        _MaskedColor ("Vein Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Normals ("Normals", 2D) = "normal" {}

        _ShineSpeed ("Shine Speed", float) = 1.5
        _ShinePeriod ("Shine Period", float) = 10
        _PeriodOffset ("Period Offset", float) = 0
        _ShineSize ("Shine Size", Range(0,1)) = 0.12
        _ShineColor ("Shine Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MaskTex;
        sampler2D _Normals;

        struct Input {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _MaskedColor;
        fixed _ShineSpeed;
        fixed _ShinePeriod;
        fixed _PeriodOffset;
        fixed _ShineSize;
        fixed4 _ShineColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        fixed square(fixed p, fixed t, fixed time) {
            return time % p < t ? 1 : 0;
        }

        fixed calculate_shine(float2 uv) {
            const fixed time = _Time.y % _ShinePeriod;
            return square(_ShinePeriod * _ShineSpeed, _ShineSize * 0.5, uv.x + uv.y + (time + _PeriodOffset) * _ShineSpeed);
            // return step(1.0 - _ShineSize / 10 * _ShinePeriod * 0.5, 0.5 + 0.5 * sin((uv.x + uv.y) * _ShinePeriod + _Time.x * _ShineSpeed));
            // return step(1.0 - _ShineSize / 1000 * 0.5, 0.5 + 0.5 * sin(uv.x - uv.y + _Time.x * _ShineSpeed));
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {
            const fixed mask = tex2D(_MaskTex, IN.uv_MainTex);
            const fixed4 unmasked_color = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            const fixed shine = calculate_shine(IN.uv_MainTex);
            const fixed shine_blend = shine * _ShineColor.a;
            fixed4 masked_color = _MaskedColor * (1 - shine_blend) + _ShineColor * shine_blend;
            masked_color.a = _MaskedColor.a;

            fixed4 color = unmasked_color * (1 - mask) + masked_color * mask;
            o.Albedo = color.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = color.a;
            o.Normal = tex2D(_Normals, IN.uv_MainTex);
        }
        ENDCG
    }
    FallBack "Diffuse"
}