    Shader "VTP/interal Editor/faceHighlight" {
     
        SubShader {
            Pass {
                BindChannels { Bind "Color",color }    
           
                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite Off
                //ZTest LEqual
                Cull Off
                Fog { Mode Off }
           }
        }
    }
