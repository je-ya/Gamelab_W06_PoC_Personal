Shader "Custom/SpriteClip"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _ClipRect ("Clip Rect", Vector) = (0, 0, 1, 1) // (xMin, yMin, xMax, yMax)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _ClipRect;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 클리핑 범위 확인
                if (i.worldPos.x < _ClipRect.x || i.worldPos.x > _ClipRect.z ||
                    i.worldPos.y < _ClipRect.y || i.worldPos.y > _ClipRect.w)
                    discard;

                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}