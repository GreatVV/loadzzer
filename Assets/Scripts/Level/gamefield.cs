using UnityEngine;
using System.Collections;

public class gamefield : MonoBehaviour {

    public GameObject ChuzzlePrefab;
    public Vector3 ChuzzleScale = new Vector3(1,1,1);

    public int Width = 7;
    public int Height = 10;

    void Awake()
    {
        for (int i = 0; i < Height; i++)
        {
            var row = new GameObject("Row"+(i+1));
            row.transform.parent = this.gameObject.transform;
            row.transform.localPosition = new Vector3(0, i * ChuzzleScale.y, 0);
            row.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            for (int j = 0; j < Width; j++)
            {
                var gameObject = NGUITools.AddChild(row, ChuzzlePrefab);
                gameObject.transform.localScale = ChuzzleScale;
                gameObject.transform.localPosition = new Vector3(j * ChuzzleScale.x, 0, 0);
            }
        }
    }

}
