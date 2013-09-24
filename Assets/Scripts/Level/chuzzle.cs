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

    public Cell Current;

    public Cell MoveTo;

    public Cell Real;    

    public ChuzzleType Type;

    public Vector3 spriteScale;

    public bool isCheckedForSearch;

    

    public override string ToString()
    {
        return ""+Type+" ("+Current.x+","+Current.y+")";
    }
}
