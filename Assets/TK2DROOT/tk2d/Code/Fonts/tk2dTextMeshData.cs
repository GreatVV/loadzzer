using UnityEngine;

[System.Serializable]
public class tk2dTextMeshData
{
    public int version = 0;

    public tk2dFontData font;
    public string text = ""; 
    public Color color = Color.white; 
    public Color color2 = Color.white; 
    public bool useGradient = false; 
    public int textureGradient = 0;
    public TextAnchor anchor = TextAnchor.LowerLeft; 
    public int renderLayer = 0;
    public Vector3 scale = Vector3.one; 
    public bool kerning = false; 
    public int maxChars = 16; 
    public bool inlineStyling = false;

    public bool formatting = false; 
    public int wordWrapWidth = 0; 

    public float spacing = 0.0f;
    public float lineSpacing = 0.0f;
}