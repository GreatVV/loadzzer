using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("2D Toolkit/Backend/tk2dSpriteCollection")]
public class tk2dSpriteCollection : MonoBehaviour 
{
	public const int CURRENT_VERSION = 4;

	public enum NormalGenerationMode
	{
		None,
		NormalsOnly,
		NormalsAndTangents,
	};
	
    // Deprecated fields
    [SerializeField] private tk2dSpriteCollectionDefinition[] textures; 
    [SerializeField] private Texture2D[] textureRefs;

    public Texture2D[] DoNotUse__TextureRefs { get { return textureRefs; } set { textureRefs = value; } } // Don't use this for anything. Except maybe in tk2dSpriteCollectionBuilderDeprecated...

    // new method
	public tk2dSpriteSheetSource[] spriteSheets;

	public tk2dSpriteCollectionFont[] fonts;
	public tk2dSpriteCollectionDefault defaults;

	// platforms
	public List<tk2dSpriteCollectionPlatform> platforms = new List<tk2dSpriteCollectionPlatform>();
	public bool managedSpriteCollection = false; // true when generated and managed by system, eg. platform specific data
	public bool HasPlatformData { get { return platforms.Count > 1; } }
	public bool loadable = false;
	
	public int maxTextureSize = 2048;
	
	public bool forceTextureSize = false;
	public int forcedTextureWidth = 2048;
	public int forcedTextureHeight = 2048;
	
	public enum TextureCompression
	{
		Uncompressed,
		Reduced16Bit,
		Compressed,
		Dithered16Bit_Alpha,
		Dithered16Bit_NoAlpha,
	}

	public TextureCompression textureCompression = TextureCompression.Uncompressed;
	
	public int atlasWidth, atlasHeight;
	public bool forceSquareAtlas = false;
	public float atlasWastage;
	public bool allowMultipleAtlases = false;
	public bool removeDuplicates = true;
	
    public tk2dSpriteCollectionDefinition[] textureParams;
    
	public tk2dSpriteCollectionData spriteCollection;
    public bool premultipliedAlpha = false;
	
	public Material[] altMaterials;
	public Material[] atlasMaterials;
	public Texture2D[] atlasTextures;
	
	[SerializeField] private bool useTk2dCamera = false;
	[SerializeField] private int targetHeight = 640;
	[SerializeField] private float targetOrthoSize = 10.0f;
	
	// New method of storing sprite size
	public tk2dSpriteCollectionSize sizeDef = tk2dSpriteCollectionSize.Default();

	public float globalScale = 1.0f;
	public float globalTextureRescale = 1.0f;

	// Remember test data for attach points
	[System.Serializable]
	public class AttachPointTestSprite {
		public string attachPointName = "";
		public tk2dSpriteCollectionData spriteCollection = null;
		public int spriteId = -1;
		public bool CompareTo(AttachPointTestSprite src) {
			return src.attachPointName == attachPointName && src.spriteCollection == spriteCollection && src.spriteId == spriteId;
		}
		public void CopyFrom(AttachPointTestSprite src) {
			attachPointName = src.attachPointName;
			spriteCollection = src.spriteCollection;
			spriteId = src.spriteId;
		}
	}
	public List<AttachPointTestSprite> attachPointTestSprites = new List<AttachPointTestSprite>();
	
	// Texture settings
	[SerializeField]
	private bool pixelPerfectPointSampled = false; // obsolete
	public FilterMode filterMode = FilterMode.Bilinear;
	public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
	public bool userDefinedTextureSettings = false;
	public bool mipmapEnabled = false;
	public int anisoLevel = 1;

	public float physicsDepth = 0.1f;
	
	public bool disableTrimming = false;
	
	public NormalGenerationMode normalGenerationMode = NormalGenerationMode.None;
	
	public int padAmount = -1; // default
	
	public bool autoUpdate = true;
	
	public float editorDisplayScale = 1.0f;

	public int version = 0;

	public string assetName = "";

	// Fix up upgraded data structures
	public void Upgrade()
	{
		if (version == CURRENT_VERSION)
			return;

		Debug.Log("SpriteCollection '" + this.name + "' - Upgraded from version " + version.ToString());

		if (version == 0)
		{
			if (pixelPerfectPointSampled)
				filterMode = FilterMode.Point;
			else
				filterMode = FilterMode.Bilinear;

			// don't bother messing about with user settings
			// on old atlases
			userDefinedTextureSettings = true; 
		}

		if (version < 3)
		{
			if (textureRefs != null && textureParams != null && textureRefs.Length == textureParams.Length)
			{
				for (int i = 0; i < textureRefs.Length; ++i)
					textureParams[i].texture = textureRefs[i];

				textureRefs = null;
			}
		}

		if (version < 4) {
			sizeDef.CopyFromLegacy( useTk2dCamera, targetOrthoSize, targetHeight );
		}

		version = CURRENT_VERSION;

#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(this);
#endif
	}
}
