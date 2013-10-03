using System.Collections.Generic;
using System.Linq;

public class TargetPlaceGameMode : GameMode
{
    public List<IntVector2> CurrentPlaceCoordinates;
    public List<IntVector2> PlaceCoordinates;

    public TargetPlaceGameMode(GameModeDescription description) : base(description)
    {
    }

    public override void OnDestroy()
    {
        Gamefield.TileDestroyed -= OnTileDestroyed;
    }

    private void OnTileDestroyed(Chuzzle destroyedChuzzle)
    {
        var place =
            CurrentPlaceCoordinates.FirstOrDefault(
                x => x.x == destroyedChuzzle.Current.x && x.y == destroyedChuzzle.Current.y);
        if (place != null)
        {
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
        Gamefield.TileDestroyed += OnTileDestroyed;

        PlaceCoordinates.Clear();
        var placeCell = Gamefield.Level.cells.Where(x => x.HasPlace);
        foreach (var cell in placeCell)
        {
            PlaceCoordinates.Add(new IntVector2(cell.x, cell.y));
        }
    }

    public override void OnReset()
    {
        CurrentPlaceCoordinates.Clear();
        CurrentPlaceCoordinates.AddRange(PlaceCoordinates);
    }
}