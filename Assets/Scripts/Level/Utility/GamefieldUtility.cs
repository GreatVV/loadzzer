using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;


public class GamefieldUtility
{
    #region Find Combination

    public static List<List<Chuzzle>> FindCombinations(List<Chuzzle> chuzzles, int combinationSize = 3)
    {
        var combinations = new List<List<Chuzzle>>();

        foreach (var c in chuzzles)
        {
            c.IsCheckedForSearch = false;
        }                                

        //find combination
        foreach (var c in chuzzles)
        {
            if (c.IsCheckedForSearch) continue;

            var combination = RecursiveFind(c, new List<Chuzzle>(), chuzzles);

            if (combination.Count() >= combinationSize)
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

    public static List<Chuzzle> RecursiveFind(Chuzzle chuzzle, List<Chuzzle> combination, IEnumerable<Chuzzle> chuzzles)
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

    public static Chuzzle GetLeftFor(Chuzzle c, IEnumerable<Chuzzle> chuzzles)
    {
        return chuzzles.FirstOrDefault(x => x.Real == c.Real.Left);
    }

    public static Chuzzle GetRightFor(Chuzzle c, IEnumerable<Chuzzle> chuzzles)
    {
        return chuzzles.FirstOrDefault(x => x.Real == c.Real.Right);
    }

    public static Chuzzle GetTopFor(Chuzzle c, IEnumerable<Chuzzle> chuzzles)
    {
        return chuzzles.FirstOrDefault(x => x.Real == c.Real.Top);
    }

    public static Chuzzle GetBottomFor(Chuzzle c, IEnumerable<Chuzzle> chuzzles)
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
        var bottom =
            chuzzles.FirstOrDefault(
                x => BetweenYCheck(x, chuzzles));

        if (bottom != null && bottom.Current.Top.Type != CellTypes.Block)
        {
            var top = chuzzles.First(ch => ch.Current == bottom.Current.Top.Top);

            var bottomPart = RecursiveFind(bottom, new List<Chuzzle>(), chuzzles);
            var middlePart = GetHorizontalLineChuzzles(bottom.Current.y+1, bottom.Type, chuzzles);    
            var topPart = RecursiveFind(top, new List<Chuzzle>(), chuzzles);                                      
          
            var posibleCombination = new List<Chuzzle>{};
            posibleCombination.AddRange(bottomPart);
            posibleCombination.AddRange(middlePart);
            posibleCombination.AddRange(topPart);
            Debug.Log("Combination 1");
            return posibleCombination;
        }

        var left = chuzzles.FirstOrDefault(x => BetweenXCheck(x, chuzzles));

        if (left != null && left.Current.Left.Type != CellTypes.Block)
        {
            var right = chuzzles.First(ch => ch.Current == left.Current.Right.Right);

            var leftPart = RecursiveFind(left, new List<Chuzzle>(), chuzzles);
            var middlePart = GetVerticalLineChuzzles(left.Current.x+1, left.Type, chuzzles);
            var rightPart = RecursiveFind(right, new List<Chuzzle>(), chuzzles);

            var posibleCombination = new List<Chuzzle> { };
            posibleCombination.AddRange(leftPart);
            posibleCombination.AddRange(middlePart);
            posibleCombination.AddRange(rightPart);
            Debug.Log("Combination 2");
            return posibleCombination;
        }

        var combinations = FindCombinations(chuzzles, 2);

        foreach (var combination in combinations)
        {
            
            var first = combination[0];
            var second = combination[1];

            //vertical combination
            if (first.Current.x == second.Current.x)
            {
                //try left             
                if ((first.Current.Left != null && first.Current.Left.Type != CellTypes.Block) ||
                    (second.Current.Left != null && second.Current.Left.Type != CellTypes.Block))
                {
                    var leftPart = GetVerticalLineChuzzles(first.Current.x - 1, first.Type, chuzzles).ToList();
                    if (leftPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(leftPart);
                        Debug.Log("Combination 3");
                        return possibleCombination;
                    }
                }

                //try right
                if ((first.Current.Right != null && first.Current.Right.Type != CellTypes.Block) ||
                    (second.Current.Right != null && second.Current.Right.Type != CellTypes.Block))
                {
                    var rightPart = GetVerticalLineChuzzles(first.Current.x + 1, first.Type, chuzzles).ToList();
                    if (rightPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(rightPart);
                        Debug.Log("Combination 4");
                        return possibleCombination;
                    }
                }

                //try top    
                if (second.Current.Top != null && second.Current.Top.Type != CellTypes.Block && chuzzles.Any(x=>x.Current == second.Current.Top))
                {
                    var topPart = GetHorizontalLineChuzzles(second.Current.Top.y, second.Type, chuzzles).ToList();
                    if (topPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(topPart);
                        Debug.Log("Combination 5");
                        return possibleCombination;
                    }
                }

                //try bottom    
                if (first.Current.Bottom != null && first.Current.Bottom.Type != CellTypes.Block && chuzzles.Any(x => x.Current == first.Current.Bottom))
                {
                    var bottomPart = GetHorizontalLineChuzzles(first.Current.Bottom.y, first.Type, chuzzles).ToList();
                    if (bottomPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(bottomPart);
                        Debug.Log("Combination 6");
                        return possibleCombination;
                    }
                }
            }
            else
            {
                //horizontal combinations

                //try left             
                if (first.Current.Left != null && first.Current.Left.Type != CellTypes.Block)
                {
                    var leftPart = GetVerticalLineChuzzles(first.Current.x - 1, first.Type, chuzzles).ToList();
                    if (leftPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(leftPart);
                        Debug.Log("Combination 7");
                        return possibleCombination;
                    }
                }

                //try right
                if (second.Current.Right != null && second.Current.Right.Type != CellTypes.Block)
                {
                    var rightPart = GetVerticalLineChuzzles(second.Current.x + 1, second.Type, chuzzles).ToList();
                    if (rightPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(rightPart);
                        Debug.Log("Combination 8");
                        return possibleCombination;
                    }
                }

                //try top    
                if (
                    (first.Current.Top != null && first.Current.Top.Type != CellTypes.Block && chuzzles.Any(x => x.Current == first.Current.Top)) ||
                    (second.Current.Top != null && second.Current.Top.Type != CellTypes.Block && chuzzles.Any(x => x.Current == second.Current.Top))
                    )
                {
                    var topPart = GetHorizontalLineChuzzles(second.Current.y+1, second.Type, chuzzles).ToList();
                    if (topPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(topPart);
                        Debug.Log("Combination 9");
                        return possibleCombination;
                    }
                }

                //try bottom    
                if (
                    (first.Current.Bottom != null && first.Current.Bottom.Type != CellTypes.Block && chuzzles.Any(x => x.Current == first.Current.Bottom)) ||
                    (second.Current.Bottom != null && second.Current.Bottom.Type != CellTypes.Block && chuzzles.Any(x => x.Current == second.Current.Bottom)) 
                    )
                {
                    var bottomPart = GetHorizontalLineChuzzles(first.Current.y-1, first.Type, chuzzles).ToList();
                    if (bottomPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(bottomPart);
                        Debug.Log("Combination 10");
                        return possibleCombination;
                    }
                }
            }
        }
        Debug.Log("Combination NOOOOOOOOOO 11");
        return new List<Chuzzle>();
    }

    public static bool BetweenYCheck(Chuzzle chuzzle, List<Chuzzle> allChuzzles)
    {
        var firstChuzzle = chuzzle;
        var secondChuzzle =
            allChuzzles.FirstOrDefault(
                ch =>
                    ch.Current.x == firstChuzzle.Current.x && ch.Current.y == firstChuzzle.Current.y + 2 &&
                    ch.Type == firstChuzzle.Type);

        if (secondChuzzle == null)
            return false;

        return allChuzzles.Any(x => x.Current.y == firstChuzzle.Current.y + 1 && x.Type == firstChuzzle.Type);

    }

    public static bool BetweenXCheck(Chuzzle chuzzle, List<Chuzzle> allChuzzles)
    {
        var firstChuzzle = chuzzle;
        var secondChuzzle =
            allChuzzles.FirstOrDefault(
                ch =>
                    ch.Current.y == firstChuzzle.Current.y && ch.Current.x == firstChuzzle.Current.x + 2 &&
                    ch.Type == firstChuzzle.Type);

        if (secondChuzzle == null)
            return false;

        return allChuzzles.Any(x => x.Current.x == firstChuzzle.Current.x + 1 && x.Type == firstChuzzle.Type);

    }

    // вертикальная и горизонтальная проверка для второго случая
    public static bool AnotherVerticalCheck(Chuzzle chuzzle, List<Chuzzle> allChuzzles)
    {
        var firstChuzzle = chuzzle;
        var secondChuzzle =
            allChuzzles.FirstOrDefault(
                ch => ch.Current.x == firstChuzzle.Current.x && ch.Current.y == firstChuzzle.Current.y + 1 && ch.Type == firstChuzzle.Type);

        if (secondChuzzle == null) return false;

        return
            allChuzzles.Where(
                ch =>
                    Math.Abs(ch.Current.x - firstChuzzle.Current.x) == 1 || ch.Current.y == firstChuzzle.Current.y - 1 ||
                    ch.Current.y == firstChuzzle.Current.y + 2).Any(ch => ch.Type == firstChuzzle.Type);

    }

    public static bool AnotherHorizontalCheck(Chuzzle chuzzle, List<Chuzzle> allChuzzles)
    {
        var firstChuzzle = chuzzle;
        var secondChuzzle =
            allChuzzles.FirstOrDefault(
                ch => ch.Current.y == firstChuzzle.Current.y && ch.Current.x == firstChuzzle.Current.x + 1 && ch.Type == firstChuzzle.Type);

        if (secondChuzzle == null) return false;

        return
            allChuzzles.Where(
                ch =>
                    Math.Abs(ch.Current.y - firstChuzzle.Current.y) == 1 || ch.Current.x == firstChuzzle.Current.x - 1 ||
                    ch.Current.x == firstChuzzle.Current.x + 2).Any(ch => ch.Type == firstChuzzle.Type);

        //return false;
    }

    #endregion

    #region New Tips

    public static IEnumerable<Chuzzle> GetHorizontalLineChuzzles(int y, ChuzzleType chuzzleType, IEnumerable<Chuzzle> chuzzles)
    {
        var enumerable = chuzzles as IList<Chuzzle> ?? chuzzles.ToList();
        var firstChuzzle = enumerable.FirstOrDefault(x => x.Real.y == y && x.Type == chuzzleType);
        if (firstChuzzle != null)
        {
            var secondChuzzle =
                enumerable.FirstOrDefault(
                    c =>
                        c.Type == firstChuzzle.Type &&
                        (c.Current == firstChuzzle.Current.Left || c.Current == firstChuzzle.Current.Right));
            if (secondChuzzle != null)
            {
                return new List<Chuzzle>() { firstChuzzle, secondChuzzle };
            }
            return new List<Chuzzle>() { firstChuzzle };
        }
        return new List<Chuzzle>();
    }

    public static IEnumerable<Chuzzle> GetVerticalLineChuzzles(int x, ChuzzleType chuzzleType, IEnumerable<Chuzzle> chuzzles)
    {   
        var enumerable = chuzzles as IList<Chuzzle> ?? chuzzles.ToList();
        var firstChuzzle = enumerable.FirstOrDefault(c => c.Real.x == x && c.Type == chuzzleType);
        if (firstChuzzle != null)
        {
            var secondChuzzle =
                enumerable.FirstOrDefault(
                    c =>
                        c.Type == firstChuzzle.Type &&
                        (c.Current == firstChuzzle.Current.Top || c.Current == firstChuzzle.Current.Bottom));
            if (secondChuzzle != null)
            {
                return new List<Chuzzle>() {firstChuzzle, secondChuzzle};
            }
            return new List<Chuzzle>() {firstChuzzle};

        }
        return new List<Chuzzle>();
    }

    public static void BetweenHorizontal(List<Chuzzle> chuzzles)
    {
        //var anyBetweenHorizontal = chuzzles.Any(x=> x.Type == chuzzles.FirstOrDefault())
    }

    #endregion

    public static IntVector2 ToRealCoordinates(Chuzzle chuzzle)
    {
        return new IntVector2(Mathf.RoundToInt(chuzzle.transform.localPosition.x / chuzzle.Scale.x),
            Mathf.RoundToInt(chuzzle.transform.localPosition.y / chuzzle.Scale.y));
    }

    public static Cell CellAt(List<Cell> cells, int x, int y)
    {
        return cells.FirstOrDefault(c => c.x == x && c.y == y);
    }

    public static Vector3 ConvertXYToPosition(int x, int y, Vector3 scale)
    {
        return new Vector3(x * scale.x, y * scale.y, 0);
    }

}

