[System.Serializable]
/// <summary>
/// Defines kerning within a font
/// </summary>
public class tk2dFontKerning
{
    /// <summary>
    /// First character to match
    /// </summary>
    public int c0;
	
    /// <summary>
    /// Second character to match
    /// </summary>
    public int c1;
	
    /// <summary>
    /// Kern amount.
    /// </summary>
    public float amount;
}