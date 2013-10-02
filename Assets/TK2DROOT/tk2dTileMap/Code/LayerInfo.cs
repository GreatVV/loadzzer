using UnityEngine;

namespace tk2dRuntime.TileMap
{
    [System.Serializable]
    public class LayerInfo
    {
        public string name;
        public int hash;
        public bool useColor;
        public bool generateCollider;
        public float z = 0.1f;
        public int unityLayer = 0;
        public bool skipMeshGeneration = false;
        public PhysicMaterial physicMaterial = null;
		
        public LayerInfo()
        {
            unityLayer = 0;
            useColor = true;
            generateCollider = true;
            skipMeshGeneration = false;
        }
    }
}