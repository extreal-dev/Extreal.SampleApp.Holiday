Shader "Holiday/Skybox/Panoramic"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Enum(Down, 0, Up, 2)] _Flip("Y-Flip", Float) = 0
        [Enum(360 Degrees, 0, 180 Degrees, 1)] _ImageType("Image Type", Float) = 0
        [Toggle] _MirrorOnBack("Mirror on Back", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Front

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            int _Flip;
            int _ImageType;
            bool _MirrorOnBack;

            inline float2 ToRadialCoords(float3 coords)
            {
                float3 normalizedCoords = normalize(coords);
                float latitude = acos(normalizedCoords.y);
                float longitude = atan2(normalizedCoords.z, normalizedCoords.x);
                float2 sphereCoords = float2(longitude, latitude) * float2(0.5/UNITY_PI, 1.0/UNITY_PI);
                return float2(0.5,1.0) - sphereCoords;
            }

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 texcoord : TEXCOORD0;
                float2 image180ScaleAndCutoff : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                float flipSign = _Flip - 1;

                o.texcoord.x = v.vertex.x;
                o.texcoord.y = flipSign * v.vertex.y;
                o.texcoord.z = v.vertex.z;

                if (_ImageType == 0)  // 360 degree
                    o.image180ScaleAndCutoff = float2(1.0, 1.0);
                else  // 180 degree
                    o.image180ScaleAndCutoff = float2(2.0, _MirrorOnBack ? 1.0 : 0.5);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 texcoord = ToRadialCoords(i.texcoord);

                if (texcoord.x > i.image180ScaleAndCutoff[1])
                    return half4(0,0,0,1);
                texcoord.x = fmod(texcoord.x*i.image180ScaleAndCutoff[0], 1);
    
                // sample the texture
                fixed4 col = tex2D(_MainTex, texcoord);
    
                return col;
            }
            ENDCG
        }
    }
}
