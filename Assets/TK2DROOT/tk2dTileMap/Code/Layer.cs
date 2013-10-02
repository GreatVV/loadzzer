using UnityEngine;

namespace tk2dRuntime.TileMap
{
    [System.Serializable]
    /// <summary>
    /// Layer.
    /// </summary>
    public class Layer
    {
        public int hash;
        public SpriteChannel spriteChannel;
        public Layer(int hash, int width, int height, int divX, int divY)
        {
            spriteChannel = new SpriteChannel();
            Init(hash, width, height, divX, divY);
        }
		
        public void Init(int hash, int width, int height, int divX, int divY)
        {
            this.divX = divX;
            this.divY = divY;
            this.hash = hash;
            this.numColumns = (width + divX - 1) / divX;
            this.numRows = (height + divY - 1) / divY;
            this.width = width;
            this.height = height;
            spriteChannel.chunks = new SpriteChunk[numColumns * numRows];
            for (int i = 0; i < numColumns * numRows; ++i)
                spriteChannel.chunks[i] = new SpriteChunk();
        }
		
        public bool IsEmpty { get { return spriteChannel.chunks.Length == 0; } }
		
        public void Create()
        {
            spriteChannel.chunks = new SpriteChunk[numColumns * numRows];
        }
		
        public int[] GetChunkData(int x, int y)
        {
            return GetChunk(x, y).spriteIds;
        }
		
        public SpriteChunk GetChunk(int x, int y)
        {
            return spriteChannel.chunks[y * numColumns + x];
        }
		
        SpriteChunk FindChunkAndCoordinate(int x, int y, out int offset)
        {
            int cellX = x / divX;
            int cellY = y / divY;
            var chunk = spriteChannel.chunks[cellY * numColumns + cellX];
            int localX = x - cellX * divX;
            int localY = y - cellY * divY;
            offset = localY * divX + localX;
            return chunk;
        }

        const int tileMask = 0x00ffffff;
        const int flagMask = unchecked((int)0xff000000);

        private bool GetRawTileValue(int x, int y, ref int value) {
            int offset;
            SpriteChunk chunk = FindChunkAndCoordinate(x, y, out offset);
            if (chunk.spriteIds == null || chunk.spriteIds.Length == 0)
                return false;
            value = chunk.spriteIds[offset];
            return true;
        }

        private void SetRawTileValue(int x, int y, int value) {
            int offset;
            SpriteChunk chunk = FindChunkAndCoordinate(x, y, out offset);
            if (chunk != null) {
                CreateChunk(chunk);
                chunk.spriteIds[offset] = value;
                chunk.Dirty = true;
            }
        }

        // Get functions

        /// <summary>Gets the tile at x, y</summary> 
        /// <returns>The tile - either a sprite Id or -1 if the tile is empty.</returns>
        public int GetTile(int x, int y) {
            int rawTileValue = 0;
            if (GetRawTileValue(x, y, ref rawTileValue)) {
                if (rawTileValue != -1) {
                    return rawTileValue & tileMask;
                }
            }
            return -1;
        }

        /// <summary>Gets the tile flags at x, y</summary> 
        /// <returns>The tile flags - a combination of tk2dTileFlags</returns>
        public tk2dTileFlags GetTileFlags(int x, int y) {
            int rawTileValue = 0;
            if (GetRawTileValue(x, y, ref rawTileValue)) {
                if (rawTileValue != -1) {
                    return (tk2dTileFlags)(rawTileValue & flagMask);
                }
            }
            return tk2dTileFlags.None;
        }

        /// <summary>Gets the raw tile value at x, y</summary> 
        /// <returns>Either a combination of Tile and flags or -1 if the tile is empty</returns>
        public int GetRawTile(int x, int y) {
            int rawTileValue = 0;
            if (GetRawTileValue(x, y, ref rawTileValue)) {
                return rawTileValue;
            }
            return -1;
        }

        // Set functions

        /// <summary>Sets the tile at x, y - either a sprite Id or -1 if the tile is empty.</summary> 
        public void SetTile(int x, int y, int tile) {
            tk2dTileFlags currentFlags = GetTileFlags(x, y);
            int rawTileValue = (tile == -1) ? -1 : (tile | (int)currentFlags);
            SetRawTileValue(x, y, rawTileValue);
        }

        /// <summary>Sets the tile flags at x, y - a combination of tk2dTileFlags</summary> 
        public void SetTileFlags(int x, int y, tk2dTileFlags flags) {
            int currentTile = GetTile(x, y);
            if (currentTile != -1) {
                int rawTileValue = currentTile | (int)flags;
                SetRawTileValue(x, y, rawTileValue);
            }
        }

        /// <summary>Clears the tile at x, y</summary> 
        public void ClearTile(int x, int y) {
            SetTile(x, y, -1);
        }

        /// <summary>Sets the raw tile value at x, y</summary> 
        /// <returns>Either a combination of Tile and flags or -1 if the tile is empty</returns>
        public void SetRawTile(int x, int y, int rawTile) {
            SetRawTileValue(x, y, rawTile);
        }


		
        void CreateChunk(SpriteChunk chunk)
        {
            if (chunk.spriteIds == null || chunk.spriteIds.Length == 0)
            {
                chunk.spriteIds = new int[divX * divY];
                for (int i = 0; i < divX * divY; ++i)
                    chunk.spriteIds[i] = -1;
            }
        }
		
        void Optimize(SpriteChunk chunk)
        {
            bool empty = true;
            foreach (var v in chunk.spriteIds)
            {
                if (v != -1)
                {
                    empty = false;
                    break;
                }
            }
            if (empty)
                chunk.spriteIds = new int[0];
        }
		
        public void Optimize()
        {
            foreach (var chunk in spriteChannel.chunks)
                Optimize(chunk);
        }
		
        public void OptimizeIncremental()
        {
            foreach (var chunk in spriteChannel.chunks)
            {
                if (chunk.Dirty)				
                    Optimize(chunk);
            }
        }
		
        public void ClearDirtyFlag()
        {
            foreach (var chunk in spriteChannel.chunks)
                chunk.Dirty = false;
        }
		
        public int NumActiveChunks
        {
            get
            {
                int numActiveChunks = 0;
                foreach (var chunk in spriteChannel.chunks)
                    if (!chunk.IsEmpty)
                        numActiveChunks++;
                return numActiveChunks;
            }
        }
		
        public int width, height;
        public int numColumns, numRows;
        public int divX, divY;
        public GameObject gameObject;
    }
}