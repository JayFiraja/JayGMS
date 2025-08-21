Shader "URP/GMS/Samples/Crystal"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (0.5, 0.8, 1.0, 0.5)
        _SpecularColor("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
        _RefractionIndex("Refraction Index", Range(1.0, 2.0)) = 1.02
        _FresnelPower("Fresnel Power", Range(0.0, 5.0)) = 1.0
        _RimPower("Rim Power", Range(1.0, 5.0)) = 2.0
        _RimColor("Rim Color", Color) = (0.7, 0.9, 1.0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 viewDirWS : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 worldPos = TransformObjectToWorld(IN.positionOS);
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetCameraPositionWS() - worldPos;
                OUT.worldPos = worldPos;

                return OUT;
            }

            half4 _BaseColor;
            half4 _SpecularColor;
            half _Smoothness;
            half _RefractionIndex;
            half _FresnelPower;
            half _RimPower;
            half4 _RimColor;

            half4 frag(Varyings IN) : SV_Target
            {
                // Normalize directions
                half3 viewDir = normalize(IN.viewDirWS);
                half3 normal = normalize(IN.worldNormal);

                // Calculate refraction
                half3 refraction = refract(viewDir, normal, 1.0 / _RefractionIndex);

                // Fresnel effect
                half fresnel = pow(1.0 - dot(viewDir, normal), _FresnelPower);

                // Rim lighting effect
                half rim = pow(1.0 - saturate(dot(viewDir, normal)), _RimPower);

                // Base color and specular
                half4 baseColor = _BaseColor;
                half3 reflectDir = reflect(-viewDir, normal);
                half spec = pow(max(dot(reflectDir, viewDir), 0.0), _Smoothness * 128.0);

                half3 albedo = baseColor.rgb * (1.0 - fresnel) + refraction * fresnel * _BaseColor.rgb;
                half3 specular = _SpecularColor.rgb * spec;

                // Calculate lighting
                half3 lighting = (albedo + specular) * _BaseColor.rgb;

                // Add rim lighting
                lighting += _RimColor.rgb * rim;

                return half4(lighting, baseColor.a);
            }

            ENDHLSL
        }
    }
}
