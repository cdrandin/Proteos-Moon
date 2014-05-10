Shader "Transparent/Cutout/Soft Edge Unlit +" {

Properties {
    _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
    _Cutoff ("Base Alpha cutoff  (Use Script)", Float) = 1
    _CutoffInverseQuarter ("Cutoff inverse / 4  (Use Script)", Float) = 1
}

Category {
    Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}

    SubShader {
        Pass {
            GLSLPROGRAM
            varying mediump vec2 uv;

            #ifdef VERTEX
            void main() {
                gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
                uv = gl_MultiTexCoord0.xy;
            }
            #endif

            #ifdef FRAGMENT
            uniform lowp sampler2D _MainTex;
            uniform lowp float _Cutoff;
            void main() {
                vec4 texture = texture2D(_MainTex, uv);
                if (texture.a < _Cutoff) discard;
                gl_FragColor = texture;
            }
            #endif      
            ENDGLSL
        }
        Pass {
            ZTest Less  ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            GLSLPROGRAM
            varying mediump vec2 uv;

            #ifdef VERTEX
            void main() {
                gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
                uv = gl_MultiTexCoord0.xy;
            }
            #endif

            #ifdef FRAGMENT
            uniform lowp sampler2D _MainTex;
            uniform lowp float _Cutoff;
            void main() {
                vec4 texture = texture2D(_MainTex, uv);
                gl_FragColor = vec4(texture.rgb, texture.a / _Cutoff);
            }
            #endif      
            ENDGLSL
        }   
    }

    SubShader {
        Pass {
            AlphaTest GEqual[_Cutoff]
            SetTexture[_MainTex]
        }
        Pass {
            ZTest Less  ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            SetTexture[_MainTex] {
                ConstantColor(0,0,0, [_CutoffInverseQuarter])
                Combine texture, texture * constant Quad
            }
        }
    }
}

}