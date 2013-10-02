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

    public int x;
    public int y;

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

    public Cell GetBottomWithType(CellTypes type = CellTypes.Usual)
    {
        var bottom = Bottom;
        while (bottom != null)
        {
            if (bottom.type == type)
            {
                return bottom;
            }
            bottom = bottom.Bottom;
        }
        return null;
    }

    public Cell GetLeftWithType(CellTypes type = CellTypes.Usual)
    {
        var left = Left;
        while (left != null)
        {
            if (left.type == type)
            {
                return left;
            }
            left = left.Left;
        }
        return null;
    }

    public Cell GetRightWithType(CellTypes type = CellTypes.Usual)
    {
        var right = Right;
        while (right != null)
        {
            Debug.Log("Right" + right.ToString());
            if (right.type == type)
            {
                return right;
            }
            right = right.Right;
        }
        return null;
    }

    public Cell GetTopWithType(CellTypes type = CellTypes.Usual)
    {
        var top = Top;
        while (top != null)
        {
            if (top.type == type)
            {
                return top;
            }
            top = top.Top;
        }
        return null;
    }

    public override string ToString()
    {
        return string.Format("({0},{1}):{2}", x,y, type);
    }                        
}

[Serializable]
public class SerializedLevel {

    public GameMode gameMode;

    public int Width;
    public int Height;
    public string Name;

    public int NumberOfColors = -1;

    public List<Cell> specialCells = new List<Cell>();

    public static SerializedLevel FromJson(JSONObject jsonObject)
    {           
        Debug.Log("Print: \n" + jsonObject.ToString());
        var serializedLevel = new SerializedLevel();
        serializedLevel.Name = jsonObject.GetField("name").str;
        serializedLevel.Width = (int)jsonObject.GetField("width").n;
        serializedLevel.Height = (int)jsonObject.GetField("height").n;
        serializedLevel.NumberOfColors = jsonObject.HasField("NumberOfColor")? (int)jsonObject.GetField("NumberOfColors").n : 8;

        var array = jsonObject.GetField("map").list;
        foreach(var tile in array)
        {
            if (tile.n != 0)
            {
                var x = array.IndexOf(tile)%serializedLevel.Width;
                var y = serializedLevel.Height - (array.IndexOf(tile) / serializedLevel.Width) - 1 ;
                serializedLevel.specialCells.Add(new Cell(x, y, CellTypes.Block));
            }
        }   
        return serializedLevel;
    }
}
