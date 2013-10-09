using UnityEngine;
using System.Collections;

public class Chuzzle : MonoBehaviour {

    public Cell Current;

    public Cell MoveTo;

    public Cell Real;    

    public ChuzzleType Type;

    public Vector3 Scale { get { return collider.bounds.size; } }

    public bool IsCheckedForSearch;

    public PowerType PowerType;
    
    public int Counter;

    public override string ToString()
    {
        return ""+Type+" ("+Current.x+","+Current.y+")";
    }
}
