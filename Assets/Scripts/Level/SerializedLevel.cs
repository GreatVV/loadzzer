using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortalBlock
{   
    public int x;
    public int y;

    public int toX;
    public int toY;            
}

public class Cell
{
    public int x;
    public int y;

    public int fromTopX;
    public int fromTopY;

    public int fromBottomX;
    public int fromBottomY;

    public int fromLeftX;
    public int fromLeftY;

    public int fromRightX;
    public int fromRightY;
}

public class SerializedLevel {

    public int Width;
    public int Height;

    public int[] TilesInColumn;
    public int[] TilesInRow;

    public List<Cell> cells;
}
