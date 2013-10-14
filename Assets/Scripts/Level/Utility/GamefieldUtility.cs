using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class GamefieldUtility
{
    #region Find Combination
    
    public static List<List<Chuzzle>> FindCombinations(List<Chuzzle> chuzzles)
    {
        var combinations = new List<List<Chuzzle>>();

        //find combination
        foreach (var c in chuzzles)
        {
            if (c.IsCheckedForSearch) continue;

            var combination = RecursiveFind(c, new List<Chuzzle>(), chuzzles);

            if (combination.Count() >= 3)
            {
                combinations.Add(combination);
            }
        }

        foreach (var c in chuzzles)
        {
            c.IsCheckedForSearch = false;
        }

        return combinations;
    }

    public static List<Chuzzle> RecursiveFind(Chuzzle chuzzle, List<Chuzzle> combination, List<Chuzzle> chuzzles)
    {
        if (chuzzle == null || combination.Contains(chuzzle) || chuzzle.IsCheckedForSearch)
        {
            return new List<Chuzzle>();
        }
        combination.Add(chuzzle);
        chuzzle.IsCheckedForSearch = true;

        var left = GetLeftFor(chuzzle, chuzzles);
        if (left != null && left.Type == chuzzle.Type)
        {
            var answer = RecursiveFind(left, combination, chuzzles);
            foreach (var a in answer)
            {
                if (combination.Contains(a) == false)
                {
                    combination.Add(a);
                }
            }
        }

        var right = GetRightFor(chuzzle, chuzzles);
        if (right != null && chuzzle.Type == right.Type)
        {
            var answer = RecursiveFind(right, combination, chuzzles);
            foreach (var a in answer)
            {
                if (combination.Contains(a) == false)
                {
                    combination.Add(a);
                }
            }
        }

        var top = GetTopFor(chuzzle, chuzzles);
        if (top != null && chuzzle.Type == top.Type)
        {
            var answer = RecursiveFind(top, combination, chuzzles);
            foreach (var a in answer)
            {
                if (combination.Contains(a) == false)
                {
                    combination.Add(a);
                }
            }
        }

        var bottom = GetBottomFor(chuzzle, chuzzles);
        if (bottom != null && chuzzle.Type == bottom.Type)
        {
            var answer = RecursiveFind(bottom, combination, chuzzles);
            foreach (var a in answer)
            {
                if (combination.Contains(a) == false)
                {
                    combination.Add(a);
                }
            }
        }

        return combination;
    }

    public static Chuzzle GetLeftFor(Chuzzle c, List<Chuzzle> chuzzles)
    {
        return chuzzles.FirstOrDefault(x => x.Real == c.Real.Left);
    }

    public static Chuzzle GetRightFor(Chuzzle c, List<Chuzzle> chuzzles)
    {
        return chuzzles.FirstOrDefault(x => x.Real == c.Real.Right);
    }

    public static Chuzzle GetTopFor(Chuzzle c, List<Chuzzle> chuzzles)
    {
        return chuzzles.FirstOrDefault(x => x.Real == c.Real.Top);
    }

    public static Chuzzle GetBottomFor(Chuzzle c, List<Chuzzle> chuzzles)
    {
        return chuzzles.FirstOrDefault(x => x.Real == c.Real.Bottom);
    }

    #endregion

    #region Tips

    /// <summary>
    /// Находит любую возможную комбинацию 
    /// </summary>
    /// <param name="chuzzles">Список элементов в котором надо найти комбинацию</param>
    /// <returns>Список элементов которые составляют эту комбинацию</returns>
    public static List<Chuzzle> Tip(List<Chuzzle> chuzzles)
    {
        var vercticalTip =
            chuzzles.FirstOrDefault(
                x => VerticalCheck(x, chuzzles));

        if (vercticalTip != null)
        {
            var list = new List<Chuzzle>
            {
                vercticalTip,
                chuzzles.FirstOrDefault(y => y.Current.x == vercticalTip.Current.x && y.Current.y == vercticalTip.Current.y + 2),
                chuzzles.FirstOrDefault(x => x.Current.y == vercticalTip.Current.y + 1 && x.Type == vercticalTip.Type)
            };
            Debug.Log("Vertical Tip found");
            return list;
        }

        var horizontalTip =
            chuzzles.FirstOrDefault(
                x => VerticalCheck(x, chuzzles));

        if (horizontalTip != null)
        {
            var list = new List<Chuzzle>
            {
                horizontalTip,
                chuzzles.FirstOrDefault(y => y.Current.y == horizontalTip.Current.y && y.Current.x == horizontalTip.Current.x + 2),
                chuzzles.FirstOrDefault(x => x.Current.x == horizontalTip.Current.x + 1 && x.Type == horizontalTip.Type)
            };
            Debug.Log("Horizontal Tip found");
            return list;
        }
        return new List<Chuzzle>();
    }

    public static bool VerticalCheck(Chuzzle chuzzle, List<Chuzzle> allChuzzles)
    {
        var firstChuzzle = chuzzle;
        var secondChuzzle =
            allChuzzles.FirstOrDefault(
                y => y.Current.x == firstChuzzle.Current.x && y.Current.y == firstChuzzle.Current.y + 2);

        if (secondChuzzle == null)
            return false;

        if (firstChuzzle.Type == secondChuzzle.Type)
        {
            return allChuzzles.Any(x => x.Current.y == firstChuzzle.Current.y + 1 && x.Type == firstChuzzle.Type);
        }
        return false;
    }

    public static bool HorizontalCheck(Chuzzle chuzzle, List<Chuzzle> allChuzzles)
    {
        var firstChuzzle = chuzzle;
        var secondChuzzle =
            allChuzzles.FirstOrDefault(
                y => y.Current.y == firstChuzzle.Current.y && y.Current.x == firstChuzzle.Current.x + 2);

        if (secondChuzzle == null)
            return false;

        if (firstChuzzle.Type == secondChuzzle.Type)
        {
            return allChuzzles.Any(x => x.Current.x == firstChuzzle.Current.x + 1 && x.Type == firstChuzzle.Type);
        }
        return false;
    }

    #endregion
}

