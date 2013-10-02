using UnityEngine;

namespace tk2dRuntime.TileMap
{
    [System.Serializable]
    public class ColorChunk
    {
        public bool Dirty { get; set; }
        public bool Empty { get { return colors.Length == 0; } }
        public Color32[] colors;
        public ColorChunk() { colors = new Color32[0]; }
    }
}