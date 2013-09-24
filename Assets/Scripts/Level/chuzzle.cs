using UnityEngine;
using System.Collections;

public enum ChuzzleType
{
    Red,
    Orange,
    Yellow,
    Black,
    Green,
    White
}

public class Chuzzle : MonoBehaviour {

    public Chuzzle left;
    public Chuzzle right;
    public Chuzzle top;
    public Chuzzle bottom;

    public Cell Current { get; set; }

    public Cell MoveTo {get;set;}

    public Cell Real { get; set; }

    //public int x;
    //public int y;        

    ////for animation of move
    //public int moveToX;
    //public int moveToY;

    ////
    //public int realX;
    //public int realY;

    public ChuzzleType Type;

    public Vector3 spriteScale;

    public bool isCheckedForSearch;

    

    public override string ToString()
    {
        return ""+Type+" ("+Current.x+","+Current.y+")";
    }
}
