Shader "Hidden/DeathShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Off
        //Blend SrcAlpha One
        Blend Zero SrcColor
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = v.vertex * float4(2,-4,1,1) + float4(0,1,0,0);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(1,0,0,1);
            }
            ENDCG
        }
    }
}
