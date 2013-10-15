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


    public tk2dSprite Sprite;

    public bool _shine;

    public float Alpha;
    private bool _frozen;

    public bool Shine
    {
        get { return _shine; }
        set
        {
            _shine = value;
            Alpha = 1;
        }
    }


    public Vector3 Scale
    {
        get { return collider.bounds.size; }
    }

    public override string ToString()
    {
        return "" + Type + " (" + Current.x + "," + Current.y + ")";
    }

    private void Awake()
    {
        Sprite = GetComponent<tk2dSprite>();
    }

    private void Update()
    {
        if (Shine && !Frozen)
        {
            Alpha = Alpha + Time.deltaTime*3;
            Sprite.color = new Color(Sprite.color.r, Sprite.color.g, Sprite.color.b, Mathf.Sin(Alpha)/2f + 0.5f);
        }
    }

    public bool Frozen
    {
        get { return _frozen; }
        set
        {
            _frozen = value;
            Sprite.color = new Color(Sprite.color.r, Sprite.color.g, Sprite.color.b, _frozen? 0.1f : 1f);
        }
    }
}