using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Points : MonoBehaviour {

    public int CurrentPoints;  
    public UILabel pointsLabel;

    public event Action<int> PointChanged;

    public void Reset()
    {
        CurrentPoints = 0;
        InvokePointChanged();
    }

    public void AddPoints(int points)
    {
        CurrentPoints += points;
        InvokePointChanged();
    }

    public void InvokePointChanged()
    {
        pointsLabel.text = string.Format("Points: {0}", CurrentPoints);
        if (PointChanged != null)
        {
            PointChanged(CurrentPoints);
        }
    }

	public void CountForCombinations(List<Chuzzle> combination)
    {
        var newPoints = combination.Count * 10;
        AddPoints(newPoints);          
    }
}
