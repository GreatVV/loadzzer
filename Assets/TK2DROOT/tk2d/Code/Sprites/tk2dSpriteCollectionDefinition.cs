using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class tk2dSpriteCollectionDefinition
{
    public enum Anchor
    {
        UpperLeft,
        UpperCenter,
        UpperRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        LowerLeft,
        LowerCenter,
        LowerRight,
        Custom
    }
	
    public enum Pad
    {
        Default,
        BlackZeroAlpha,
        Extend,
        TileXY,
    }
	
    public enum ColliderType
    {
        UserDefined,		// don't try to create or destroy anything
        ForceNone,			// nothing will be created, if something exists, it will be destroyed
        BoxTrimmed, 		// box, trimmed to cover visible region
        BoxCustom, 			// box, with custom values provided by user
        Polygon, 			// polygon, can be concave
    }
	
    public enum PolygonColliderCap
    {
        None,
        FrontAndBack,
        Front,
        Back,
    }
	
    public enum ColliderColor
    {
        Default, // default unity color scheme
        Red,
        White,
        Black
    }
	
    public enum Source
    {
        Sprite,
        SpriteSheet,
        Font
    }

    public enum DiceFilter
    {
        Complete,
        SolidOnly,
        TransparentOnly,
    }
	
    public string name = "";
	
    public bool disableTrimming = false;
    public bool additive = false;
    public Vector3 scale = new Vector3(1,1,1);
    
    public Texture2D texture = null;
	
    [System.NonSerialized]
    public Texture2D thumbnailTexture;
	
    public int materialId = 0;
	
    public Anchor anchor = Anchor.MiddleCenter;
    public float anchorX, anchorY;
    public Object overrideMesh;

    public bool doubleSidedSprite = false;
    public bool customSpriteGeometry = false;
    public tk2dSpriteColliderIsland[] geometryIslands = new tk2dSpriteColliderIsland[0];
	
    public bool dice = false;
    public int diceUnitX = 64;
    public int diceUnitY = 64;
    public DiceFilter diceFilter = DiceFilter.Complete;

    public Pad pad = Pad.Default;
    public int extraPadding = 0; // default
	
    public Source source = Source.Sprite;
    public bool fromSpriteSheet = false;
    public bool hasSpriteSheetId = false;
    public int spriteSheetId = 0;
    public int spriteSheetX = 0, spriteSheetY = 0;
    public bool extractRegion = false;
    public int regionX, regionY, regionW, regionH;
    public int regionId;
	
    public ColliderType colliderType = ColliderType.UserDefined;
    public Vector2 boxColliderMin, boxColliderMax;
    public tk2dSpriteColliderIsland[] polyColliderIslands;
    public PolygonColliderCap polyColliderCap = PolygonColliderCap.FrontAndBack;
    public bool colliderConvex = false;
    public bool colliderSmoothSphereCollisions = false;
    public ColliderColor colliderColor = ColliderColor.Default;

    public List<tk2dSpriteDefinition.AttachPoint> attachPoints = new List<tk2dSpriteDefinition.AttachPoint>();

    public void CopyFrom(tk2dSpriteCollectionDefinition src)
    {
        name = src.name;
		
        disableTrimming = src.disableTrimming;
        additive = src.additive;
        scale = src.scale;
        texture = src.texture;
        materialId = src.materialId;
        anchor = src.anchor;
        anchorX = src.anchorX;
        anchorY = src.anchorY;
        overrideMesh = src.overrideMesh;
		
        doubleSidedSprite = src.doubleSidedSprite;
        customSpriteGeometry = src.customSpriteGeometry;
        geometryIslands = src.geometryIslands;
		
        dice = src.dice;
        diceUnitX = src.diceUnitX;
        diceUnitY = src.diceUnitY;
        diceFilter = src.diceFilter;
        pad = src.pad;
		
        source = src.source;
        fromSpriteSheet = src.fromSpriteSheet;
        hasSpriteSheetId = src.hasSpriteSheetId;
        spriteSheetX = src.spriteSheetX;
        spriteSheetY = src.spriteSheetY;
        spriteSheetId = src.spriteSheetId;
        extractRegion = src.extractRegion;
        regionX = src.regionX;
        regionY = src.regionY;
        regionW = src.regionW;
        regionH = src.regionH;
        regionId = src.regionId;
		
        colliderType = src.colliderType;
        boxColliderMin = src.boxColliderMin;
        boxColliderMax = src.boxColliderMax;
        polyColliderCap = src.polyColliderCap;
		
        colliderColor = src.colliderColor;
        colliderConvex = src.colliderConvex;
        colliderSmoothSphereCollisions = src.colliderSmoothSphereCollisions;
		
        extraPadding = src.extraPadding;
		
        if (src.polyColliderIslands != null)
        {
            polyColliderIslands = new tk2dSpriteColliderIsland[src.polyColliderIslands.Length];
            for (int i = 0; i < polyColliderIslands.Length; ++i)
            {
                polyColliderIslands[i] = new tk2dSpriteColliderIsland();
                polyColliderIslands[i].CopyFrom(src.polyColliderIslands[i]);
            }
        }
        else
        {
            polyColliderIslands = new tk2dSpriteColliderIsland[0];
        }
		
        if (src.geometryIslands != null)
        {
            geometryIslands = new tk2dSpriteColliderIsland[src.geometryIslands.Length];
            for (int i = 0; i < geometryIslands.Length; ++i)
            {
                geometryIslands[i] = new tk2dSpriteColliderIsland();
                geometryIslands[i].CopyFrom(src.geometryIslands[i]);
            }
        }
        else
        {
            geometryIslands = new tk2dSpriteColliderIsland[0];
        }

        attachPoints = new List<tk2dSpriteDefinition.AttachPoint>(src.attachPoints.Count);
        foreach (tk2dSpriteDefinition.AttachPoint srcAp in src.attachPoints) {
            tk2dSpriteDefinition.AttachPoint ap = new tk2dSpriteDefinition.AttachPoint();
            ap.CopyFrom(srcAp);
            attachPoints.Add(ap);
        }
    }
	
    public void Clear()
    {
        // Reinitialize
        var tmpVar = new tk2dSpriteCollectionDefinition();
        CopyFrom(tmpVar);
    }
	
    public bool CompareTo(tk2dSpriteCollectionDefinition src)
    {
        if (name != src.name) return false;
		
        if (additive != src.additive) return false;
        if (scale != src.scale) return false;
        if (texture != src.texture) return false;
        if (materialId != src.materialId) return false;
        if (anchor != src.anchor) return false;
        if (anchorX != src.anchorX) return false;
        if (anchorY != src.anchorY) return false;
        if (overrideMesh != src.overrideMesh) return false;
        if (dice != src.dice) return false;
        if (diceUnitX != src.diceUnitX) return false;
        if (diceUnitY != src.diceUnitY) return false;
        if (diceFilter != src.diceFilter) return false;
        if (pad != src.pad) return false;
        if (extraPadding != src.extraPadding) return false;

        if (doubleSidedSprite != src.doubleSidedSprite) return false;

        if (customSpriteGeometry != src.customSpriteGeometry) return false;
        if (geometryIslands != src.geometryIslands) return false;
        if (geometryIslands != null && src.geometryIslands != null)
        {
            if (geometryIslands.Length != src.geometryIslands.Length) return false;
            for (int i = 0; i < geometryIslands.Length; ++i)
                if (!geometryIslands[i].CompareTo(src.geometryIslands[i])) return false;
        }

        if (source != src.source) return false;
        if (fromSpriteSheet != src.fromSpriteSheet) return false;
        if (hasSpriteSheetId != src.hasSpriteSheetId) return false;
        if (spriteSheetId != src.spriteSheetId) return false;
        if (spriteSheetX != src.spriteSheetX) return false;
        if (spriteSheetY != src.spriteSheetY) return false;
        if (extractRegion != src.extractRegion) return false;
        if (regionX != src.regionX) return false;
        if (regionY != src.regionY) return false;
        if (regionW != src.regionW) return false;
        if (regionH != src.regionH) return false;
        if (regionId != src.regionId) return false;
		
        if (colliderType != src.colliderType) return false;
        if (boxColliderMin != src.boxColliderMin) return false;
        if (boxColliderMax != src.boxColliderMax) return false;
		
        if (polyColliderIslands != src.polyColliderIslands) return false;
        if (polyColliderIslands != null && src.polyColliderIslands != null)
        {
            if (polyColliderIslands.Length != src.polyColliderIslands.Length) return false;
            for (int i = 0; i < polyColliderIslands.Length; ++i)
                if (!polyColliderIslands[i].CompareTo(src.polyColliderIslands[i])) return false;
        }
		
        if (polyColliderCap != src.polyColliderCap) return false;
		
        if (colliderColor != src.colliderColor) return false;
        if (colliderSmoothSphereCollisions != src.colliderSmoothSphereCollisions) return false;
        if (colliderConvex != src.colliderConvex) return false;

        if (attachPoints.Count != src.attachPoints.Count) return false;
        for (int i = 0; i < attachPoints.Count; ++i) {
            if (!attachPoints[i].CompareTo(src.attachPoints[i])) return false;
        }
		
        return true;
    }	
}