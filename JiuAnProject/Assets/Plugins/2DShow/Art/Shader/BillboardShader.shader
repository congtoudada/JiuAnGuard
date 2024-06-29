Shader "Unlit/BillboardShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
//		_Alpha ("Alpha", Range(0,1)) = 1

	    //[Header(Debug)]
		//[Toggle(_CAMERASYN_ON)] _DiffuseCheck("Camera SYN", Float) = 1
    }
    SubShader
    {
		// Tags定义了subshader块何时以及在什么条件下执行
        Tags
        {
            //渲染类型
            "RenderType"="Transparent"
            //渲染队列
            "Queue"="Transparent+90"
            //告诉引擎，该Shader只用于 URP 渲染管线
            "RenderPipeline" = "UniversalPipeline"
        	"IgnoreProjector"="True"
        	"DisableBatching"="True"
        }
        Pass
        {
			ZWrite Off
			ZTest Always
//			Blend SrcAlpha OneMinusSrcAlpha
			Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			//#pragma shader_feature _CAMERASYN_ON

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _Alpha;
			float _Scale;
			float4x4 _RotateMatrix;

            v2f vert (appdata v)
            {
                v2f o;                
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float3 center = float3(0,0,0);

				float3 up = float3(0, 1, 0);
				up = mul(_RotateMatrix, float4(up, 0)).xyz;
				up = mul(unity_WorldToObject, float4(up, 0)).xyz;

				//float3 centerToView = mul(unity_WorldToObject , float4(_WorldSpaceCameraPos.xyz, 1)).xyz - center;
				float3 viewPos = mul(UNITY_MATRIX_MV, v.vertex).xyz; 
				float3 projZ = normalize(dot(viewPos, float3(0, 0, 1)) * float3(0,0,1));
				projZ = -projZ;
				projZ = mul(UNITY_MATRIX_T_MV, projZ);
				float3 normalDir = -normalize(projZ);
				//half3 up = abs(normalDir.y) >0.999 ? half3(0,0,1) : half3(0,1,0);
				float3 rightDir = normalize(cross(up, normalDir));
				float3 upDir = normalize(cross(normalDir, rightDir));

				//float3 viewCenter = UnityObjectToViewPos(center);
				//float scaleLerp = saturate((abs(viewCenter.z) - _ProjectionParams.y) / (_ProjectionParams.z - _ProjectionParams.y)); //��������ֵ ��0Զ1
				//float scale = lerp(1, _ProjectionParams.z - _ProjectionParams.y, scaleLerp);
				//float3 centerOffs = v.vertex.xyz * scale * _Scale * 0.05 - center;
				float3 centerOffs = v.vertex.xyz  - center;
				float3 localPos = center + centerOffs.x * rightDir + centerOffs.y * upDir + centerOffs.z * normalDir;
				o.vertex = UnityObjectToClipPos(localPos);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // sample the texture
                half4 col = tex2D(_MainTex, i.uv);
            	clip(col.a - 0.5);
				// col.a *= _Alpha;
                return col;
            }
            ENDCG
        }
    }
}
