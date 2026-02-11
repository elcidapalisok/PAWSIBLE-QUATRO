Shader "Custom/WoundBrush"
{
    Properties
    {
        _BrushPos ("BrushPos", Vector) = (0,0,0,0)
        _BrushSize ("BrushSize", Float) = 0.05
        _BrushStrength ("BrushStrength", Float) = 1
        _Hardness ("Hardness", Float) = 0.5
    }
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            Blend One One

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _BrushPos;
            float _BrushSize;
            float _BrushStrength;
            float _Hardness;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float d = distance(i.uv, _BrushPos.xy);
                float t = saturate(1.0 - (d / _BrushSize));
                t = pow(t, lerp(1.0, 6.0, saturate(_Hardness)));
                float add = t * _BrushStrength;
                return fixed4(add, add, add, 1);
            }
            ENDHLSL
        }
    }
}
