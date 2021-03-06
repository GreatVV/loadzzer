﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

#endregion

[Serializable]
public class CheckSpecialState : GamefieldState
{
    public List<Chuzzle> SpecialTilesAnimated = new List<Chuzzle>();

    public CheckSpecialState(Gamefield gamefield = null) : base(gamefield)
    {
    }

    #region Event Handlers

    public override void OnEnter()
    {
        var combinations = GamefieldUtility.FindCombinations(Gamefield.Level.ActiveChuzzles);
        if (!CheckForSpecial(combinations))
        {
            Gamefield.SwitchStateTo(Gamefield.RemoveState);
        }
    }

    public override void OnExit()
    {
    }

    private void OnCreateLineTweenComplete(object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;

        SpecialTilesAnimated.Remove(chuzzle);
        Object.Destroy(chuzzle.gameObject);

        if (SpecialTilesAnimated.Count == 0)
        {
            Gamefield.SwitchStateTo(Gamefield.CreateNew);
        }
    }

    #endregion

    public override void Update()
    {
    }

    public override void LateUpdate()
    {
    }

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


    public bool CreateSpecialWithType(List<Chuzzle> comb, PowerType powerType)
    {
        var ordered = comb;

        var targetTile = comb[Random.Range(0, ordered.Count)];
        var cellForNew = targetTile.Current;
        foreach (var chuzzle in ordered)
        {
            if (chuzzle != targetTile)
            {
                chuzzle.MoveTo = cellForNew;
            }
        }

        var powerUp =
            Gamefield.PowerTypePrefabs.First(x => x.Type == powerType && x.ChuzzleType == targetTile.Type).Prefab;
        var powerUpChuzzle = Gamefield.Level.CreateChuzzle(targetTile.Current.x, targetTile.Current.y, powerUp);
        powerUpChuzzle.Type = targetTile.Type;
        powerUpChuzzle.PowerType = powerType;

        Debug.Log("Child number: " + powerUpChuzzle.transform.childCount);
        var child = powerUpChuzzle.transform.GetChild(0).gameObject;
        Object.Destroy(child.GetComponent<BoxCollider>());
        Gamefield.Level.ScaleSprite(child.GetComponent<tk2dSprite>());


        Object.Destroy(targetTile.gameObject);
        Gamefield.Level.ActiveChuzzles.Remove(targetTile);
        Gamefield.Level.Chuzzles.Remove(targetTile);
        ordered.Remove(targetTile);

        foreach (var c in ordered)
        {
            Gamefield.RemoveChuzzle(c);

            var targetPosition = new Vector3(c.MoveTo.x*c.Scale.x, c.MoveTo.y*c.Scale.y, 0);

            SpecialTilesAnimated.Add(c);

            iTween.MoveTo(c.gameObject,
                iTween.Hash(
                    "x", targetPosition.x,
                    "y", targetPosition.y,
                    "z", targetPosition.z,
                    "time", 0.3f,
                    "easetype", iTween.EaseType.easeInOutQuad,
                    "oncomplete", new Action<object>(OnCreateLineTweenComplete),
                    "oncompleteparams", c
                    ));
        }

        return true;
    }

    private bool CreateLine(List<Chuzzle> comb)
    {
        return CreateSpecialWithType(comb,
            UnityEngine.Random.Range(0, 100) > 50 ? PowerType.HorizontalLine : PowerType.VerticalLine);
    }
}