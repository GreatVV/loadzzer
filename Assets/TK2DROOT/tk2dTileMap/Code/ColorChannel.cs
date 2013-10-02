using UnityEngine;

namespace tk2dRuntime.TileMap
{
    [System.Serializable]
    /// <summary>
    /// Color channel.
    /// </summary>
    public class ColorChannel
    {
        public ColorChannel(int width, int height, int divX, int divY)
        {
            Init(width, height, divX, divY);	
        }
		
        public Color clearColor = Color.white;
        public ColorChunk[] chunks;
        public ColorChannel() { chunks = new ColorChunk[0]; }

        public void Init(int width, int height, int divX, int divY)
        {
            numColumns = (width + divX - 1) / divX;
            numRows = (height + divY - 1) / divY;
            chunks = new ColorChunk[0];
            this.divX = divX;
            this.divY = divY;
        }
		
        public ColorChunk FindChunkAndCoordinate(int x, int y, out int offset)
        {
            int cellX = x / divX;
            int cellY = y / divY;
            cellX = Mathf.Clamp(cellX, 0, numColumns - 1);
            cellY = Mathf.Clamp(cellY, 0, numRows - 1);
            int idx = cellY * numColumns + cellX;
            var chunk = chunks[idx];
            int localX = x - cellX * divX;
            int localY = y - cellY * divY;
            offset = localY * (divX + 1) + localX;
            return chunk;
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public Color GetColor(int x, int y)
        {
            if (IsEmpty)
                return clearColor;
			
            int offset;
            var chunk = FindChunkAndCoordinate(x, y, out offset);
            if (chunk.colors.Length == 0)
                return clearColor;
            else
                return chunk.colors[offset];
        }
		
        // create chunk if it doesn't already exist
        void InitChunk(ColorChunk chunk)
        {
            if (chunk.colors.Length == 0)
            {
                chunk.colors = new Color32[(divX + 1) * (divY + 1)];
                for (int i = 0; i < chunk.colors.Length; ++i)				
                    chunk.colors[i] = clearColor;
            }
        }

        /// <summary>
        /// Sets the color.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="color">Color.</param>
        public void SetColor(int x, int y, Color color)
        {
            if (IsEmpty)
                Create();
			
            int cellLocalWidth = divX + 1;
			
            // at edges, set to all overlapping coordinates
            int cellX = Mathf.Max(x - 1, 0) / divX;
            int cellY = Mathf.Max(y - 1, 0) / divY;
            var chunk = GetChunk(cellX, cellY, true);
            int localX = x - cellX * divX;
            int localY = y - cellY * divY;
            chunk.colors[localY * cellLocalWidth + localX] = color;
            chunk.Dirty = true;
			
            // May need to set it to adjancent cells too
            bool needX = false, needY = false;
            if (x != 0 && (x % divX) == 0 && (cellX + 1) < numColumns)
                needX = true;
            if (y != 0 && (y % divY) == 0 && (cellY + 1) < numRows)
                needY = true;
			
            if (needX)
            {
                int cx = cellX + 1;
                chunk = GetChunk(cx, cellY, true);
                localX = x - cx * divX;
                localY = y - cellY * divY;
                chunk.colors[localY * cellLocalWidth + localX] = color;
                chunk.Dirty = true;
            }
            if (needY)
            {
                int cy = cellY + 1;
                chunk = GetChunk(cellX, cy, true);
                localX = x - cellX * divX;
                localY = y - cy * divY;
                chunk.colors[localY * cellLocalWidth + localX] = color;
                chunk.Dirty = true;
            }
            if (needX && needY)
            {
                int cx = cellX + 1;
                int cy = cellY + 1;
                chunk = GetChunk(cx, cy, true);
                localX = x - cx * divX;
                localY = y - cy * divY;
                chunk.colors[localY * cellLocalWidth + localX] = color;
                chunk.Dirty = true;
            }
        }
		
        public ColorChunk GetChunk(int x, int y)
        {
            if (chunks == null || chunks.Length == 0)
                return null;
			
            return chunks[y * numColumns + x];
        }		
		
        public ColorChunk GetChunk(int x, int y, bool init)
        {
            if (chunks == null || chunks.Length == 0)
                return null;
			
            var chunk = chunks[y * numColumns + x];
            InitChunk(chunk);
            return chunk;
        }		
		
        public void ClearChunk(ColorChunk chunk)
        {
            for (int i = 0; i < chunk.colors.Length; ++i)
                chunk.colors[i] = clearColor;
        }
		
        public void ClearDirtyFlag()
        {
            foreach (var chunk in chunks)
                chunk.Dirty = false;
        }

        /// <summary>
        /// Clear the specified color.
        /// </summary>
        /// <param name="color">Color.</param>
        public void Clear(Color color)
        {
            clearColor = color;
            foreach (var chunk in chunks)
                ClearChunk(chunk);
            Optimize();
        }
		
        public void Delete()
        {
            chunks = new ColorChunk[0];
        }
		
        public void Create()
        {
            chunks = new ColorChunk[numColumns * numRows];
            for (int i = 0; i < chunks.Length; ++i)
                chunks[i] = new ColorChunk();
        }
		
        void Optimize(ColorChunk chunk)
        {
            bool empty = true;
            Color32 clearColor32 = this.clearColor;
            foreach (var c in chunk.colors)
            {
                if (c.r != clearColor32.r ||
                    c.g != clearColor32.g ||
                    c.b != clearColor32.b ||
                    c.a != clearColor32.a)
                {
                    empty = false;
                    break;
                }
            }
			
            if (empty)
                chunk.colors = new Color32[0];
        }
		
        public void Optimize()
        {
            foreach (var chunk in chunks)
                Optimize(chunk);
        }
		
        public bool IsEmpty { get { return chunks.Length == 0; } }
		
        public int NumActiveChunks
        {
            get
            {
                int numActiveChunks = 0;
                foreach (var chunk in chunks)
                    if (chunk != null && chunk.colors != null && chunk.colors.Length > 0)
                        numActiveChunks++;
                return numActiveChunks;
            }
        }
		
        public int numColumns, numRows;
        public int divX, divY;
    }
}