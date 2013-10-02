using UnityEngine;

[System.Serializable]
/// <summary>
/// Defines one character in a font
/// </summary>
public class tk2dFontChar
{
    /// <summary>
    /// End points forming a quad
    /// </summary>
    public Vector3 p0, p1;
    /// <summary>
    /// Uv for quad end points
    /// </summary>
    public Vector3 uv0, uv1;
	
    public bool flipped = false;
    /// <summary>
    /// Gradient Uvs for quad end points
    /// </summary>
    public Vector2[] gradientUv;
    /// <summary>
    /// Spacing required for current character, mix with <see cref="tk2dFontKerning"/> to get true advance value
    /// </summary>
    public float advance;

    public int channel = 0; // channel data for multi channel fonts. Lookup into an array (R=0, G=1, B=2, A=3)
}