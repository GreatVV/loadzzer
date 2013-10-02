using UnityEngine;

[System.Serializable]
public class tk2dSpriteCollectionFont
{
    public bool active = false;
    public Object bmFont;
    public Texture2D texture;
    public bool dupeCaps = false; // duplicate lowercase into uc, or vice-versa, depending on which exists
    public bool flipTextureY = false;
    public int charPadX = 0;
    public tk2dFontData data;
    public tk2dFont editorData;
    public int materialId;

    public bool useGradient = false;
    public Texture2D gradientTexture = null;
    public int gradientCount = 1;
	
    public void CopyFrom(tk2dSpriteCollectionFont src)
    {
        active = src.active;
        bmFont = src.bmFont;
        texture = src.texture;
        dupeCaps = src.dupeCaps;
        flipTextureY = src.flipTextureY;
        charPadX = src.charPadX;
        data = src.data;
        editorData = src.editorData;
        materialId = src.materialId;
        gradientCount = src.gradientCount;
        gradientTexture = src.gradientTexture;
        useGradient = src.useGradient;
    }
	
    public string Name
    {
        get
        {
            if (bmFont == null || texture == null)
                return "Empty";
            else
            {
                if (data == null)
                    return bmFont.name + " (Inactive)";
                else
                    return bmFont.name;
            }
        }
    }
	
    public bool InUse
    {
        get { return active && bmFont != null && texture != null && data != null && editorData != null; }
    }
}