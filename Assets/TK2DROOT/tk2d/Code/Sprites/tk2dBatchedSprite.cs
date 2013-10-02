using UnityEngine;

[System.Serializable]
public class tk2dBatchedSprite
{
    public enum Type {
        EmptyGameObject,
        Sprite,
        TiledSprite,
        SlicedSprite,
        ClippedSprite,
        TextMesh
    }

    [System.Flags] 
    public enum Flags {
        None = 0,
        Sprite_CreateBoxCollider = 1,
        SlicedSprite_BorderOnly = 2,
    }
	
    public Type type = Type.Sprite;
    public string name = ""; // for editing
    public int parentId = -1;
    public int spriteId = 0;
    public int xRefId = -1; // index into cross referenced array. what this is depends on type.
    public tk2dSpriteCollectionData spriteCollection = null;
    public Quaternion rotation = Quaternion.identity;
    public Vector3 position = Vector3.zero;
    public Vector3 localScale = Vector3.one;
    public Color color = Color.white;
    public Vector3 baseScale = Vector3.one; // sprite/textMesh scale
    public int renderLayer = 0;
	
    [SerializeField]
    Vector2 internalData0; // Used for clipped region or sliced border
    [SerializeField]
    Vector2 internalData1; // Used for clipped region or sliced border
    [SerializeField]
    Vector2 internalData2; // Used for dimensions
    [SerializeField]
    Vector2 colliderData = new Vector2(0, 1); // collider offset z, collider extent z in x and y respectively
    [SerializeField]
    string formattedText = ""; // Formatted text cached for text mesh
	
    [SerializeField]
    Flags flags = Flags.None;
    public tk2dBaseSprite.Anchor anchor = tk2dBaseSprite.Anchor.LowerLeft;

    // Used to create batched mesh
    public Matrix4x4 relativeMatrix = Matrix4x4.identity;

    public float BoxColliderOffsetZ {
        get { return colliderData.x; }
        set { colliderData.x = value; }
    }
    public float BoxColliderExtentZ {
        get { return colliderData.y; }
        set { colliderData.y = value; }
    }
    public string FormattedText {
        get {return formattedText;}
        set {formattedText = value;}
    }
    public Vector2 ClippedSpriteRegionBottomLeft {
        get { return internalData0; }
        set { internalData0 = value; }
    }
    public Vector2 ClippedSpriteRegionTopRight {
        get { return internalData1; }
        set { internalData1 = value; }
    }
    public Vector2 SlicedSpriteBorderBottomLeft {
        get { return internalData0; }
        set { internalData0 = value; }
    }
    public Vector2 SlicedSpriteBorderTopRight {
        get { return internalData1; }
        set { internalData1 = value; }
    }
    public Vector2 Dimensions {
        get { return internalData2; }
        set { internalData2 = value; }
    }

    public bool IsDrawn { get { return type != Type.EmptyGameObject; } }
    public bool CheckFlag(Flags mask) { return (flags & mask) != Flags.None; }
    public void SetFlag(Flags mask, bool value) { if (value) flags |= mask;	else flags &= ~mask; }

    // Bounds - not serialized, but retrieved in BuildRenderMesh (used in BuildPhysicsMesh)
    Vector3 cachedBoundsCenter = Vector3.zero;
    Vector3 cachedBoundsExtents = Vector3.zero;
    public Vector3 CachedBoundsCenter {
        get {return cachedBoundsCenter;}
        set {cachedBoundsCenter = value;}
    }
    public Vector3 CachedBoundsExtents {
        get {return cachedBoundsExtents;}
        set {cachedBoundsExtents = value;}
    }
	
    public tk2dSpriteDefinition GetSpriteDefinition() {
        if (spriteCollection != null && spriteId != -1)
        {
            return spriteCollection.inst.spriteDefinitions[spriteId];
        }
        else {
            return null;
        }
    }

    public tk2dBatchedSprite()
    {
        parentId = -1;
    }
}