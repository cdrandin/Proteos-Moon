Shader "Hidden/FogVolume"
{
    CGINCLUDE
       
            #include "UnityCG.cginc"
            
            
    sampler2D _MainTex;
    sampler2D _CameraDepthTexture;
	sampler3D _3DTex;
    float4 _Color, _InscatteringColor, _BoxMin, _BoxMax, Speed, Stretch;
    float3 L = float3(0, 0, 1);
    float _InscatteringIntensity=1, _InscateringExponent=2, _Visibility, InscatteringStartDistance = 100, InscatteringTransitionWideness = 500, _3DNoiseScale, _3DNoiseStepSize, gain=1, threshold=0;

	float FogMinHeight = 0, FogMaxHeight=-20;

    //http://www.cs.cornell.edu/courses/CS4620/2013fa/lectures/03raytracing1.pdf
	//http://www.clockworkcoders.com/oglsl/rt/gpurt1.htm
    //http://webcache.googleusercontent.com/search?q=cache:9r5sCd1f2hsJ:www.clockworkcoders.com/oglsl/rt/gpurt1.htm+&cd=1&hl=es&ct=clnk&gl=es
 //float hitbox(Ray r, vec3 m1, vec3 m2, out float tmin, out float tmax) 
 float hitbox (float3 startpoint, float3 direction, float3 m1, float3 m2, inout float tmin, inout float tmax)
 {
        float tymin, tymax, tzmin, tzmax;
        float flag = 1.0;
        if (direction.x > 0) 
    {
            tmin = (m1.x - startpoint.x) / direction.x;
            tmax = (m2.x - startpoint.x) / direction.x;
        }


    else 
    {
            tmin = (m2.x - startpoint.x) / direction.x;
            tmax = (m1.x - startpoint.x) / direction.x;
        }


    if (direction.y > 0) 
    {
            tymin = (m1.y - startpoint.y) / direction.y;
            tymax = (m2.y - startpoint.y) / direction.y;
        }


    else 
    {
            tymin = (m2.y - startpoint.y) / direction.y;
            tymax = (m1.y - startpoint.y) / direction.y;
        }


     
    if ((tmin > tymax) || (tymin > tmax)) flag = -1.0;
        if (tymin > tmin) tmin = tymin;
        if (tymax < tmax) tmax = tymax;
        if (direction.z > 0) 
    {
            tzmin = (m1.z - startpoint.z) / direction.z;
            tzmax = (m2.z - startpoint.z) / direction.z;
        }


    else 
    {
            tzmin = (m2.z - startpoint.z) / direction.z;
            tzmax = (m1.z - startpoint.z) / direction.z;
        }


    if ((tmin > tzmax) || (tzmin > tmax)) flag = -1.0;
        if (tzmin > tmin) tmin = tzmin;
        if (tzmax < tmax) tmax = tzmax;
        return (flag > 0);
    }
		
    struct v2f
    {
        float4 SampleCoordinates         : SV_POSITION;
        float3 Wpos        : TEXCOORD0;
        float4 ScreenUVs   : TEXCOORD1;
        float3 LocalPos    : TEXCOORD2;
        float3 ViewPos     : TEXCOORD3;
        float3 LocalEyePos : TEXCOORD4;

    };
	half Threshold(float noise)
	{
		
		float Guy = noise * gain;
		float thresh =  Guy - threshold;
		return max(0.0f,(lerp(0.0f ,Guy , thresh )));	
	  
	}
    v2f vert (appdata_full i)
            {
        v2f o;
		half index = 0;
        o.SampleCoordinates = mul(UNITY_MATRIX_MVP, i.vertex);
        o.Wpos.xyz = mul((float4x4)_Object2World, float4(i.vertex.xyz, 1)).xyz;
        o.ScreenUVs = ComputeScreenPos(o.SampleCoordinates);

        o.ViewPos = mul((float4x4)UNITY_MATRIX_MV, float4(i.vertex.xyz, 1)).xyz;
        o.LocalPos = i.vertex.xyz;
        o.LocalEyePos = mul((float4x4)_World2Object, (float4(_WorldSpaceCameraPos, 1))).xyz;

        return o;
    }

            float4 frag (v2f i) : COLOR
            {
				float3 direction = normalize(i.LocalPos - i.LocalEyePos);
				float tmin, tmax;
				float Volume = hitbox(i.LocalEyePos, direction, _BoxMin, _BoxMax, tmin, tmax);
				// tmin must be 0 when inside the volume
				int Inside[3] = {0, 0, 0}, bOutside;
				Inside[0] = step(0, abs(i.LocalEyePos.x) - _BoxMax.x);
				Inside[1] = step(0, abs(i.LocalEyePos.y) - _BoxMax.y);
				Inside[2] = step(0, abs(i.LocalEyePos.z) - _BoxMax.z);
				bOutside  = min(1,(float)(Inside[0] + Inside[1] + Inside[2]));
				tmin*=bOutside;
		
				float2 ScreenUVs = i.ScreenUVs.xy/i.ScreenUVs.w;
				float Depth =  length(DECODE_EYEDEPTH(tex2D(_CameraDepthTexture, ScreenUVs).r )/normalize(i.ViewPos).z);
				float MinMax[2] = {max(tmin, tmax), min(tmin, tmax)};
		
				float thickness = min(MinMax[0], Depth) - min(MinMax[1], Depth);	
		
				float Fog = thickness / _Visibility  ;
				Fog = 1-exp2(-Fog);
				Fog *= Volume;
				
				float4 Final;
				float3 Normalized_CameraWSdir = normalize(i.Wpos - _WorldSpaceCameraPos);
				
				half DistanceClamp = saturate( Depth/InscatteringTransitionWideness- InscatteringStartDistance);



				#ifdef _FOG_VOLUME_NOISE
				//High end machines only
				#if SHADER_API_D3D11

				int sampleCount = 20;
				float invSampleCount = 2.0f/(float)sampleCount;                
				float sampleStep = thickness * invSampleCount;
				float Noise=0;
				
				float3 rayStart = _WorldSpaceCameraPos + Normalized_CameraWSdir*tmin;
				float3 rayStop = _WorldSpaceCameraPos + Normalized_CameraWSdir*tmax;
				rayStart = 0.5 * (rayStart + 1.0);
				rayStop = 0.5 * (rayStop + 1.0);
				float3 SampleDirection = rayStop - rayStart;
								
				float3 step = normalize(SampleDirection) * _3DNoiseStepSize;
				float RayCollision = distance(rayStop, rayStart);
				float3 SampleCoordinates = rayStart;
				Speed *=_Time.x;							

				
				for(int s=0; s<sampleCount && RayCollision > 0.0; s++, SampleCoordinates += step, RayCollision -= _3DNoiseStepSize)
                {
                 	
					Noise += tex3D(_3DTex,  SampleCoordinates * _3DNoiseScale * Stretch.rgb+ Speed.rgb)*invSampleCount;

					}
										
					_Color.rgb+=Threshold(Noise)*_Color.rgb; 
					_InscatteringColor*=Noise; 
					#endif
					#endif
				#ifdef _FOG_VOLUME_INSCATTERING
				
				//Inscattering						
				float Inscattering = pow(max(0, dot(L, Normalized_CameraWSdir)), _InscateringExponent);
				//clamp by distance:
						Inscattering *= DistanceClamp;
				Final = float4(_Color.rgb + _InscatteringColor * _InscatteringIntensity * Inscattering, 1);
				#else
						Final = float4(_Color.rgb , 1);
				#endif
				
				

					Final.a *=(Fog * _Color.a); 
				return Final;
			}


            
            ENDCG
            SubShader
    {
        Tags {
            "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha   

        Cull Front  
		Lighting Off 
		ZWrite Off  
		ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma multi_compile _ _FOG_VOLUME_INSCATTERING  
			#pragma multi_compile _ _FOG_VOLUME_NOISE 
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
              ENDCG
        }
      
	} 
	Fallback off
}
