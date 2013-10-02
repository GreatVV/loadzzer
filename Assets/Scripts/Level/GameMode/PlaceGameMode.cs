using System.Collections.Generic;
using System.Linq;

public class PlaceGameMode : GameMode
{
    public List<IntVector2> CurrentPlaceCoordinates;
    public List<IntVector2> PlaceCoordinates;
    public Gamefield gamefield;

    private void Awake()
    {
        gamefield.TileDestroyed += OnTileDestroyed;
        OnReset();
    }

    private void OnDestroy()
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
        CurrentPlaceCoordinates.Clear();
        CurrentPlaceCoordinates.AddRange(PlaceCoordinates);
    }
}