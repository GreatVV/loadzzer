namespace tk2dRuntime.TileMap
{
    [System.Serializable]
    public class SpriteChannel
    {
        public SpriteChunk[] chunks;
        public SpriteChannel() { chunks = new SpriteChunk[0]; }
    }
}