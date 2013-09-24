using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class IntVector2
{
    public int x;
    public int y;

    public IntVector2()
    {
        x = y = 0;
    }

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

[Serializable]
public class PortalBlock
{   
    public int x;
    public int y;

    public int toX;
    public int toY;            
}

[Serializable]
public enum CellTypes
{
    Usual,
    Block
}

[Serializable]
public class Cell
{
    public CellTypes type;

    public int x { get; private set; }
    public int y { get; private set; }

    public Cell Left;
    public Cell Right;
    public Cell Top;
    public Cell Bottom;   

    public Cell(int x, int y, CellTypes type = CellTypes.Usual)
    {
        this.x = x;
        this.y = y;
        this.type = type;             
    }                                          
}

[Serializable]
public class SerializedLevel {

    public int Width;
    public int Height;

    public int[] TilesInColumn;
    public int[] TilesInRow;

    public List<Cell> specialCells;
}
