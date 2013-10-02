using System.Collections.Generic;
using System.Linq;

public class TargetPlaceGameMode : GameMode
{
    public List<IntVector2> CurrentPlaceCoordinates;
    public List<IntVector2> PlaceCoordinates;
    public Gamefield gamefield;

    public TargetPlaceGameMode(Gamefield gamefield)
    {
        this.gamefield = gamefield;
        
        OnReset();
    }

    public override void OnDestroy()
    {
        gamefield.TileDestroyed -= OnTileDestroyed;
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
            InvokeWin();
        }     
    }

    public override void Action()
    {
        SpendTurn();

        if (Turns == 0)
        {
            InvokeGameOver();
        }
    }

    public override void OnReset()
    {
        gamefield.TileDestroyed += OnTileDestroyed;
        CurrentPlaceCoordinates.Clear();
        CurrentPlaceCoordinates.AddRange(PlaceCoordinates);
    }
}