using UnityEngine;
using System.Collections;

public enum ChuzzleType
{
    Red,
    Orange,
    Yellow,
    Black,
    Green,
    White,
    Blue,
    Pink
}

public enum PowerType
{
    Usual,
    VerticalLine,
    HorizontalLine,
    Bomb
}

public class Chuzzle : MonoBehaviour {

    public Cell Current;

    public Cell MoveTo;

    public Cell Real;    

    public ChuzzleType Type;

    public Vector3 Scale { get { return collider.bounds.size; } }

    public bool isCheckedForSearch;

    public PowerType PowerType;
    

    public override string ToString()
    {
        return ""+Type+" ("+Current.x+","+Current.y+")";
    }
}
