[System.Serializable]
/// <summary>
/// Defines an animation frame and associated data.
/// </summary>
public class tk2dSpriteAnimationFrame
{
    /// <summary>
    /// The sprite collection.
    /// </summary>
    public tk2dSpriteCollectionData spriteCollection;
    /// <summary>
    /// The sprite identifier.
    /// </summary>
    public int spriteId;
	
    /// <summary>
    /// When true will trigger an animation event when this frame is displayed
    /// </summary>
    public bool triggerEvent = false;
    /// <summary>
    /// Custom event data (string)
    /// </summary>
    public string eventInfo = "";
    /// <summary>
    /// Custom event data (int)
    /// </summary>
    public int eventInt = 0;
    /// <summary>
    /// Custom event data (float)
    /// </summary>
    public float eventFloat = 0.0f;
	
    public void CopyFrom(tk2dSpriteAnimationFrame source)
    {
        CopyFrom(source, true);
    }

    public void CopyTriggerFrom(tk2dSpriteAnimationFrame source)
    {
        triggerEvent = source.triggerEvent;
        eventInfo = source.eventInfo;
        eventInt = source.eventInt;
        eventFloat = source.eventFloat;		
    }

    public void ClearTrigger()
    {
        triggerEvent = false;
        eventInt = 0;
        eventFloat = 0;
        eventInfo = "";
    }
	
    public void CopyFrom(tk2dSpriteAnimationFrame source, bool full)
    {
        spriteCollection = source.spriteCollection;
        spriteId = source.spriteId;
		
        if (full) CopyTriggerFrom(source);
    }
}