using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class SpecialCreationUtility
{
    #region Set in Editor

    public Gamefield Gamefield;
    public List<Pair> PowerTypePrefabs = new List<Pair>();

    #endregion

    public List<Chuzzle> SpecialTilesAnimated;

    public bool CheckForSpecial(List<List<Chuzzle>> combinations)
    {
        var isNewSpecial = false;

        foreach (var comb in combinations)
        {
            //if any tile is powerup - then don't check for new bonuses
            //or any tile has counter
            if (comb.Any(x => x.PowerType != PowerType.Usual) || comb.Any(x => x.Counter != 0))
            {
                continue;
            }

            if (comb.Count == 4)
            {
                isNewSpecial = CreateLine(comb);
            }
            else
            {
                if (comb.Count >= 5)
                {
                    isNewSpecial = CreateBomb(comb);
                }
            }
        }

        return isNewSpecial;
    }

    private bool CreateBomb(List<Chuzzle> comb)
    {
        return CreateSpecialWithType(comb, PowerType.Bomb);
    }

    private void MoveToSpecialTween(Chuzzle c)
    {
        Gamefield.RemoveChuzzle(c);

        var targetPosition = new Vector3(c.MoveTo.x*c.Scale.x, c.MoveTo.y*c.Scale.y, 0);

        SpecialTilesAnimated.Add(c);
        // iTween.ScaleTo(c.gameObject, Vector3.zero, 1f);
        iTween.MoveTo(c.gameObject,
            iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", targetPosition.z, "time", 0.3f, "easetype",
                iTween.EaseType.easeInOutQuad, "oncomplete", "OnCreateLineTweenComplete", "oncompletetarget",
                Gamefield.gameObject,
                "oncompleteparams", c));
    }

    public bool CreateSpecialWithType(List<Chuzzle> comb, PowerType powerType)
    {
        var ordered = comb.OrderBy(x => x.Current.y).ToList();
        var targetTile = ordered.First();
        var cellForNew = ordered.First().Current;
        for (var i = 1; i < ordered.Count; i++)
        {
            var chuzzle = ordered[i];
            chuzzle.MoveTo = cellForNew;
        }

        var powerUp = PowerTypePrefabs.First(x => x.type == powerType).prefab;
        var powerUpChuzzle = Gamefield.Level.CreateChuzzle(targetTile.Current.x, targetTile.Current.y, powerUp);
        powerUpChuzzle.Type = targetTile.Type;
        powerUpChuzzle.PowerType = powerType;

        var color =
            Object.Instantiate(
                Gamefield.Level.ChuzzlePrefabs.First(x => x.GetComponent<Chuzzle>().Type == targetTile.Type)) as
                GameObject;
        color.transform.parent = powerUpChuzzle.transform;
        color.transform.localPosition = Vector3.zero;
        Gamefield.Level.ScaleSprite(color.GetComponent<tk2dSprite>());
        
        Object.Destroy(color.gameObject.GetComponent<Chuzzle>());
        Object.Destroy(color.gameObject.GetComponent<BoxCollider>());
        color.GetComponent<tk2dSprite>().SortingOrder = -1;

        Object.Destroy(targetTile.gameObject);
        Gamefield.Level.ActiveChuzzles.Remove(targetTile);
        Gamefield.Level.Chuzzles.Remove(targetTile);
        ordered.Remove(targetTile);

        foreach (var c in ordered)
        {
            if (c.PowerType == PowerType.Usual)
            {
                MoveToSpecialTween(c);
            }
        }

        return true;
    }

    private bool CreateLine(List<Chuzzle> comb)
    {
        return CreateSpecialWithType(comb,
            UnityEngine.Random.Range(0, 100) > 50 ? PowerType.HorizontalLine : PowerType.VerticalLine);
    }

    public bool KillSpecials(IEnumerable<List<Chuzzle>> combinations)
    {
        var tilesToKill = new List<Chuzzle>();
        foreach (var combination in combinations)
        {
            foreach (var chuzzle in combination)
            {
                if (chuzzle.PowerType != PowerType.Usual)
                {
                    tilesToKill.AddUniqRange(combination);
                    Gamefield.ApplyPowerUp(tilesToKill, chuzzle);
                }
            }
        }
        Gamefield.RemoveTiles(tilesToKill, true);

        return tilesToKill.Any();
    }
}