Shader "2D/Texture Only Alpha"
{   
    Properties
    {
        _MainTex ("Texture", 2D) = ""
    }   

    Category
    {
        ZWrite Off
        Tags {"Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        SubShader
        {
            Pass
            {
                SetTexture[_MainTex]
            }
        }
    }
}