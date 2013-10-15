﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;

[Serializable]
public class TargetPlaceGameMode : GameMode
{
    public List<IntVector2> CurrentPlaceCoordinates = new List<IntVector2>();
    public List<IntVector2> PlaceCoordinates = new List<IntVector2>();

    public TargetPlaceGameMode(GameModeDescription description) : base(description)
    {
    }

    public override void OnDestroy()
    {
        Gamefield.TileDestroyed -= OnTileDestroyed;
    }

    private void OnTileDestroyed(Chuzzle destroyedChuzzle)
    {
        if (PlaceCoordinates.Count == 0)
        {
            return;
        }

        var place =
            CurrentPlaceCoordinates.FirstOrDefault(
                x => x.x == destroyedChuzzle.Current.x && x.y == destroyedChuzzle.Current.y);
        if (place != null)
        {
            NGUITools.ClearChildren(destroyedChuzzle.Current.GameObject);
            CurrentPlaceCoordinates.Remove(place);
        }

        if (CurrentPlaceCoordinates.Count == 0)
        {
            IsWin = true;
        }
    }

    public override void HumanTurn()
    {
        SpendTurn();
    }

    protected override void OnInit()
    {
        Gamefield.TileDestroyed -= OnTileDestroyed;
        Gamefield.TileDestroyed += OnTileDestroyed;

        PlaceCoordinates.Clear();
        var placeCell = Gamefield.Level.ActiveCells.Where(x => x.HasPlace);
        Debug.Log("Now of cells: "+placeCell.Count());
        foreach (var cell in placeCell)
        {
            PlaceCoordinates.Add(new IntVector2(cell.x, cell.y));
        }
        OnReset();
    }

    public override void OnReset()
    {
        CurrentPlaceCoordinates.Clear();
        CurrentPlaceCoordinates.AddRange(PlaceCoordinates);
    }

    public override string ToString()
    {
        return "Target place";
    }
}