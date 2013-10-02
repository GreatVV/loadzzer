using UnityEngine;

[System.Serializable]
/// <summary>
/// Sprite Definition.
/// </summary>
public class tk2dSpriteDefinition
{
    /// <summary>
    /// Collider type.
    /// </summary>
    public enum ColliderType
    {
        /// <summary>
        /// Do not create or destroy anything.
        /// </summary>
        Unset,
		
        /// <summary>
        /// If a collider exists, it will be destroyed. The sprite will be responsible in making sure there are no other colliders attached.
        /// </summary>
        None,
		
        /// <summary>
        /// Create a box collider.
        /// </summary>
        Box,
		
        /// <summary>
        /// Create a mesh collider.
        /// </summary>
        Mesh,
    }
	
    /// <summary>
    /// Name
    /// </summary>
    public string name;
	
    public Vector3[] boundsData;
    public Vector3[] untrimmedBoundsData;
	
    public Vector2 texelSize;
	
    /// <summary>
    /// Array of positions for sprite geometry.
    /// </summary>
    public Vector3[] positions;
	
    /// <summary>
    /// Array of normals for sprite geometry, zero length array if they dont exist
    /// </summary>
    public Vector3[] normals;
	
    /// <summary>
    /// Array of tangents for sprite geometry, zero length array if they dont exist
    /// </summary>
    public Vector4[] tangents;
	
    /// <summary>
    /// Array of UVs for sprite geometry, will match the position count.
    /// </summary>
    public Vector2[] uvs;
    /// <summary>
    /// Array of indices for sprite geometry.
    /// </summary>
    public int[] indices = new int[] { 0, 3, 1, 2, 3, 0 };
    /// <summary>
    /// The material used by this sprite. This is generally the same on all sprites in a colletion, but this is not
    /// true when multi-atlas spanning is enabled.
    /// </summary>
    public Material material;

    [System.NonSerialized]
    public Material materialInst;

    /// <summary>
    /// The material id used by this sprite. This is an index into the materials array and corresponds to the 
    /// material flag above.
    /// </summary>
    public int materialId;
	
	
    /// <summary>
    /// Source texture GUID - this is used by the inspector to find the source image without adding a unity dependency.
    /// </summary>
    public string sourceTextureGUID;
    /// <summary>
    /// Speficies if this texture is extracted from a larger texture source, for instance an atlas. This is used in the inspector.
    /// </summary>
    public bool extractRegion;
    public int regionX, regionY, regionW, regionH;
	
    public enum FlipMode {
        None,
        Tk2d,
        TPackerCW,
    }

    /// <summary>
    /// Specifies if this texture is flipped to its side (rotated) in the atlas
    /// </summary>
    public FlipMode flipped;
	
    /// <summary>
    /// Specifies if this texture has complex geometry
    /// </summary>
    public bool complexGeometry = false;
	
    /// <summary>
    /// Collider type
    /// </summary>
    public ColliderType colliderType = ColliderType.Unset;
	
    /// <summary>
    /// v0 and v1 are center and size respectively for box colliders when colliderType is Box.
    /// It is an array of vertices, and the geometry defined by indices when colliderType is Mesh.
    /// </summary>
    public Vector3[] colliderVertices; 
    public int[] colliderIndicesFwd;
    public int[] colliderIndicesBack;
    public bool colliderConvex;
    public bool colliderSmoothSphereCollisions;


    [System.Serializable]
    public class AttachPoint
    {
        public string name = "";
        public Vector3 position = Vector3.zero;
        public float angle = 0;

        public void CopyFrom( AttachPoint src ) {
            name = src.name;
            position = src.position;
            angle = src.angle;
        }

        public bool CompareTo( AttachPoint src ) {
            return (name == src.name && src.position == position && src.angle == angle);
        }
    }

    public AttachPoint[] attachPoints = new AttachPoint[0];
	
    public bool Valid { get { return name.Length != 0; } }

    /// <summary>
    /// Gets the trimmed bounds of the sprite.
    /// </summary>
    /// <returns>
    /// Local space bounds
    /// </returns>
    public Bounds GetBounds()
    {
        return new Bounds(new Vector3(boundsData[0].x, boundsData[0].y, boundsData[0].z),
            new Vector3(boundsData[1].x, boundsData[1].y, boundsData[1].z));
    }
	
    /// <summary>
    /// Gets untrimmed bounds of the sprite.
    /// </summary>
    /// <returns>
    /// Local space untrimmed bounds
    /// </returns>
    public Bounds GetUntrimmedBounds()
    {
        return new Bounds(new Vector3(untrimmedBoundsData[0].x, untrimmedBoundsData[0].y, untrimmedBoundsData[0].z),
            new Vector3(untrimmedBoundsData[1].x, untrimmedBoundsData[1].y, untrimmedBoundsData[1].z));
    }
}