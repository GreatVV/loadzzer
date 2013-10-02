using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class CellSprite
{
    public CellTypes type;
    public GameObject cellPrefab;
}

[Serializable]
public class Level
{
    public int Width = 6;
    public int Height = 6;

    public int NumberOfColors = -1;
                                                               
    public List<PortalBlock> portals = new List<PortalBlock>();
    public List<Cell> cells = new List<Cell>();           

    public Vector3 ChuzzleSize = new Vector3(80, 80);
    public GameObject[] ChuzzlePrefabs;

    public List<Chuzzle> chuzzles = new List<Chuzzle>();

    public GameObject Gamefield;

    public CellSprite[] cellPrefabs;

    public List<GameObject> cellSprites;

    public void InitRandom()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (GetCellAt(x, y).type == CellTypes.Usual)
                {
                    CreateRandomChuzzle(x, y);
                }
            }

            var leftPortalBlock = new PortalBlock();
            leftPortalBlock.x = -1;
            leftPortalBlock.y = y;
            leftPortalBlock.toX = Width - 1;
            leftPortalBlock.toY = y;

            var rightPortalBlock = new PortalBlock();
            rightPortalBlock.x = Width;
            rightPortalBlock.y = y;
            rightPortalBlock.toX = 0;
            rightPortalBlock.toY = y;

            this.portals.Add(leftPortalBlock);
            this.portals.Add(rightPortalBlock);
        }


        for (int j = 0; j < Width; j++)
        {
            var upPortalBlock = new PortalBlock();
            upPortalBlock.x = j;
            upPortalBlock.y = Height;
            upPortalBlock.toX = j;
            upPortalBlock.toY = 0;

            var bottomPortalBlock = new PortalBlock();
            bottomPortalBlock.toX = j;
            bottomPortalBlock.toY = Height - 1;
            bottomPortalBlock.x = j;
            bottomPortalBlock.y = -1;

            this.portals.Add(upPortalBlock);
            this.portals.Add(bottomPortalBlock);
        }       
    }

    private void CreateTileSprite(Cell cell)
    {
        var prefab = cellPrefabs.First(x => x.type == cell.type).cellPrefab;
        var cellSprite = NGUITools.AddChild(Gamefield, prefab);
        cellSprites.Add(cellSprite);
        cellSprite.transform.localPosition = ConvertXYToPosition(cell.x, cell.y, ChuzzleSize);
        var sprite = cellSprite.GetComponent<tk2dSprite>();
        
        sprite.scale = new Vector3(ChuzzleSize.x / sprite.CurrentSprite.regionW, ChuzzleSize.y / sprite.CurrentSprite.regionH, 1);     
    }

    public void InitFromFile(SerializedLevel level)
    {
        cells.Clear();
        portals.Clear();

        Width = level.Width;
        Height = level.Height;
        ChuzzleSize = new Vector3(480, 480, 0) / Width;
        foreach(var newCell in level.specialCells)
        {
            AddCell(newCell.x, newCell.y, newCell);
        }
        NumberOfColors = level.NumberOfColors;
        
        InitRandom();
    }

    public Chuzzle CreateRandomChuzzle(int x, int y)
    {
        var colorsNumber = NumberOfColors == -1 ? ChuzzlePrefabs.Length : NumberOfColors;
        var prefab = ChuzzlePrefabs[Random.Range(0, colorsNumber)];
        return CreateChuzzle(x, y, prefab);
    }

    public Chuzzle CreateChuzzle(int x, int y, GameObject prefab)
    {
        var gameObject = NGUITools.AddChild(Gamefield.gameObject, prefab);
        gameObject.layer = prefab.layer;

        var sprite = gameObject.GetComponent<tk2dSprite>();
        ScaleSprite(sprite);                                                  

        (gameObject.collider as BoxCollider).size = ChuzzleSize;
        (gameObject.collider as BoxCollider).center = ChuzzleSize / 2;
        var chuzzle = gameObject.GetComponent<Chuzzle>();
        chuzzle.Real = chuzzle.MoveTo = chuzzle.Current = GetCellAt(x, y);

        gameObject.transform.parent = Gamefield.transform;
        gameObject.transform.localPosition = new Vector3(x * gameObject.GetComponent<Chuzzle>().Scale.x, y * gameObject.GetComponent<Chuzzle>().Scale.y, 0);

        
        chuzzles.Add(chuzzle);
        return chuzzle;
    }

    public void ScaleSprite(tk2dBaseSprite sprite)
    {
        if (sprite.CurrentSprite.regionW != 0)
        {
            sprite.scale = new Vector3(ChuzzleSize.x/sprite.CurrentSprite.regionW,
                ChuzzleSize.x/sprite.CurrentSprite.regionW, 1);
        }
        else
        {
            sprite.scale = new Vector3(ChuzzleSize.x/sprite.CurrentSprite.GetUntrimmedBounds().max.x,
                ChuzzleSize.x/sprite.CurrentSprite.GetUntrimmedBounds().max.x, 1);
        }
    }

    public Cell GetCellAt(int x, int y)
    {
        var cell = cells.FirstOrDefault(c => c.x == x && c.y == y);
        if (cell == null)
        {
            var newCell = new Cell(x, y);            
            AddCell(x, y, newCell);

            return newCell;
        }
        return cell;
    }

    private void AddCell(int x, int y, Cell newCell)
    {
        cells.Add(newCell);
        //set left
        var left = cells.FirstOrDefault(c => c.x == x - 1 && c.y == y);
        if (left != null)
        {
            newCell.Left = left;
            left.Right = newCell;
        }

        //set right
        var right = cells.FirstOrDefault(c => c.x == x + 1 && c.y == y);
        if (right != null)
        {
            newCell.Right = right;
            right.Left = newCell;
        }

        //set top
        var top = cells.FirstOrDefault(c => c.x == x && c.y == y + 1);
        if (top != null)
        {
            newCell.Top = top;
            top.Bottom = newCell;
        }

        //set bottom
        var bottom = cells.FirstOrDefault(c => c.x == x && c.y == y - 1);
        if (bottom != null)
        {
            newCell.Bottom = bottom;
            bottom.Top = newCell;
        }

        if (newCell.y < Height)
        {
            CreateTileSprite(newCell);
        }
    }

    #region Block and Portals

    public bool IsPortal(int x, int y)
    {
        return portals.Any(p => p.x == x && p.y == y);
    }

    public PortalBlock GetPortalAt(int x, int y)
    {
        return portals.First(p => p.x == x && p.y == y);
    }

    public Vector3 ConvertXYToPosition(int x, int y, Vector3 scale)
    {
        return new Vector3(x * scale.x, y * scale.y, 0);
    }

    #endregion

    public void Reset()
    {
        portals.Clear();
        foreach (var chuzzle in chuzzles)
        {
            GameObject.Destroy(chuzzle.gameObject);
        }
        chuzzles.Clear();

        foreach (var cellSprite in cellSprites)
        {
            GameObject.Destroy(cellSprite.gameObject);
        }
        cellSprites.Clear();
    }

}