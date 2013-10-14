using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Chuzzle : MonoBehaviour
{
    public Cell Current;

    public Cell MoveTo;

    public Cell Real;

    public ChuzzleType Type;

    public bool IsCheckedForSearch;

    public PowerType PowerType;

    public int Counter;

    public Vector3 Scale
    {
        get { return collider.bounds.size; }
    }

    public override string ToString()
    {
        return "" + Type + " (" + Current.x + "," + Current.y + ")";
    }


    public tk2dSprite Sprite;

    void Awake()
    {
        Sprite = GetComponent<tk2dSprite>();
    }

    public bool Shine;
    public float Alpha;

    void Update()
    {
        if (Shine)
        {
            Alpha = Alpha + Time.deltaTime;
            Sprite.color = new Color(Sprite.color.r, Sprite.color.g, Sprite.color.b, Mathf.Sin(Alpha)/2f + 0.5f);
        }
    }
}