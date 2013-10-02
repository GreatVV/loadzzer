using UnityEngine;

namespace tk2dRuntime.TileMap
{
    [System.Serializable]
    public class SpriteChunk
    {
        bool dirty;
        public int[] spriteIds;
        public GameObject gameObject;
        public Mesh mesh;
        public MeshCollider meshCollider;
        public Mesh colliderMesh;
        public SpriteChunk() { spriteIds = new int[0]; }
		
        public bool Dirty
        {
            get { return dirty; }
            set { dirty = value; }
        }
		
        public bool IsEmpty
        {
            get { return spriteIds.Length == 0; }
        }
		
        public bool HasGameData
        {
            get { return gameObject != null || mesh != null || meshCollider != null ||  colliderMesh != null; }
        }
		
        public void DestroyGameData(tk2dTileMap tileMap)
        {
            if (mesh != null) tileMap.DestroyMesh(mesh);
            if (gameObject != null) GameObject.DestroyImmediate(gameObject);
            gameObject = null;
            mesh = null;
			
            DestroyColliderData(tileMap);
        }
		
        public void DestroyColliderData(tk2dTileMap tileMap)
        {
            if (colliderMesh != null) 
                tileMap.DestroyMesh(colliderMesh);
            if (meshCollider != null && meshCollider.sharedMesh != null && meshCollider.sharedMesh != colliderMesh) 
                tileMap.DestroyMesh(meshCollider.sharedMesh);
            if (meshCollider != null) GameObject.DestroyImmediate(meshCollider);
            meshCollider = null;
            colliderMesh = null;
        }
    }
}