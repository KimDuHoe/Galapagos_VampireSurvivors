Shader "Sprites/RadialFill"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _FillAmount ("Fill Amount", Range(0,1)) = 0.5
        [KeywordEnum(Clockwise, CounterClockwise)] _FillDirection ("Fill Direction", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _FILLDIRECTION_CLOCKWISE _FILLDIRECTION_COUNTERCLOCKWISE

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _FillAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 relativePos = i.uv - 0.5;
                float angle = atan2(relativePos.y, relativePos.x);

                // Angle mapping: Unity's atan2 is from -PI to PI. We map it to 0-1.
                // Default: Starts from the left (-PI) and goes counter-clockwise.
                // We shift it to start from the top.
                angle /= (2.0 * 3.14159265); // normalize to -0.5 to 0.5
                angle += 0.25; // shift to start from top. now 0 (top) to 1 (top again)
                if(angle < 0) angle += 1.0;


                #if _FILLDIRECTION_COUNTERCLOCKWISE
                    if (angle > _FillAmount)
                    {
                        discard;
                    }
                #else // Clockwise is default
                    if (1.0 - angle > _FillAmount)
                    {
                        discard;
                    }
                #endif


                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                col.a *= _Color.a; // Apply tint alpha
                return col;
            }
            ENDCG
        }
    }
}