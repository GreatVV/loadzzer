using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlaceGameMode : GameMode 
{
    public Gamefield gamefield;

    public List<IntVector2> PlaceCoordinates;

    public List<IntVector2> CurrentPlaceCoordinates;

    void Awake()
    {
        gamefield.TilesDestroyed += OnTileDestroyed;
        OnReset();
    }

    void OnDestroy()
    {
        gamefield.TilesDestroyed -= OnTileDestroyed;
    }

    void OnTileDestroyed(List<Chuzzle> destroyedChuzzles)
    {
         foreach(var destroyed in destroyedChuzzles)
         {
             var place = CurrentPlaceCoordinates.FirstOrDefault(x => x.x == destroyed.Current.x && x.y == destroyed.Current.y);
             if (place != null)
             {
                 CurrentPlaceCoordinates.Remove(place);
             }                                 
         }

         Turns--;
         
        if (CurrentPlaceCoordinates.Count == 0)
         {
             InvokeWin();
         }
         else
         {
             if (Turns == 0)
             {
                 InvokeGameOver();
             }
         }
    }

    public override void Action()
    {           
    }

    public override void OnReset()
    {
        CurrentPlaceCoordinates.Clear();
        CurrentPlaceCoordinates.AddRange(PlaceCoordinates);
    }


}
