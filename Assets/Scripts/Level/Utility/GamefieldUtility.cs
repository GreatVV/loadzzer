﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

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
    ///     Находит любую возможную комбинацию
    /// </summary>
    /// <param name="chuzzles">Список элементов в котором надо найти комбинацию</param>
    /// <returns>Список элементов которые составляют эту комбинацию</returns>
    public static List<Chuzzle> Tip(List<Chuzzle> chuzzles, out IntVector2 isHorizontalMove, out Chuzzle chuzzleToMove)
    {
        var bottom =
            chuzzles.FirstOrDefault(
                x => BetweenYCheck(x, chuzzles));

        if (bottom != null && bottom.Current.Top != null && bottom.Current.Top.Type != CellTypes.Block)
        {
            var top = chuzzles.First(ch => ch.Current == bottom.Current.Top.Top);

            var bottomPart = RecursiveFind(bottom, new List<Chuzzle>(), chuzzles);
            var middlePart = GetHorizontalLineChuzzles(bottom.Current.y + 1, bottom.Type, chuzzles);
            var topPart = RecursiveFind(top, new List<Chuzzle>(), chuzzles);

            var posibleCombination = new List<Chuzzle>();
            posibleCombination.AddRange(bottomPart);
            posibleCombination.AddRange(middlePart);
            posibleCombination.AddRange(topPart);

            Debug.Log("Combination 1");
            isHorizontalMove = new IntVector2(bottom.Current.x, bottom.Current.y + 1);
            chuzzleToMove = middlePart.First();
            return posibleCombination;
        }

        var left = chuzzles.FirstOrDefault(x => BetweenXCheck(x, chuzzles));

        if (left != null && left.Current.Left != null && left.Current.Left.Type != CellTypes.Block)
        {
            var right = chuzzles.First(ch => ch.Current == left.Current.Right.Right);

            var leftPart = RecursiveFind(left, new List<Chuzzle>(), chuzzles);
            var middlePart = GetVerticalLineChuzzles(left.Current.x + 1, left.Type, chuzzles);
            var rightPart = RecursiveFind(right, new List<Chuzzle>(), chuzzles);

            var posibleCombination = new List<Chuzzle>();
            posibleCombination.AddRange(leftPart);
            posibleCombination.AddRange(middlePart);
            posibleCombination.AddRange(rightPart);

            Debug.Log("Combination 2");
            isHorizontalMove = new IntVector2(left.Current.x + 1, left.Current.y);
            chuzzleToMove = middlePart.First();
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
                        isHorizontalMove = new IntVector2(first.Current.x - 1, first.Current.y);
                        chuzzleToMove = leftPart.First();
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
                        isHorizontalMove = new IntVector2(first.Current.x + 1, first.Current.y);
                        chuzzleToMove = rightPart.First();
                        return possibleCombination;
                    }
                }

                //try top    
                if (second.Current.Top != null && second.Current.Top.Type != CellTypes.Block &&
                    chuzzles.Any(x => x.Current == second.Current.Top))
                {
                    var topPart = GetHorizontalLineChuzzles(second.Current.Top.y, second.Type, chuzzles).ToList();
                    if (topPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(topPart);

                        Debug.Log("Combination 5");
                        isHorizontalMove = new IntVector2(second.Current.x, second.Current.Top.y);
                        chuzzleToMove = topPart.First();
                        return possibleCombination;
                    }
                }

                //try bottom    
                if (first.Current.Bottom != null && first.Current.Bottom.Type != CellTypes.Block &&
                    chuzzles.Any(x => x.Current == first.Current.Bottom))
                {
                    var bottomPart = GetHorizontalLineChuzzles(first.Current.Bottom.y, first.Type, chuzzles).ToList();
                    if (bottomPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(bottomPart);

                        Debug.Log("Combination 6");
                        isHorizontalMove = new IntVector2(second.Current.x, second.Current.Bottom.y);
                        chuzzleToMove = bottomPart.First();
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
                        isHorizontalMove = new IntVector2(first.Current.x - 1, first.Current.y);
                        chuzzleToMove = leftPart.First();
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
                        isHorizontalMove = new IntVector2(second.Current.x + 1, second.Current.y);
                        chuzzleToMove = rightPart.First();
                        return possibleCombination;
                    }
                }

                //try top    
                if (
                    (first.Current.Top != null && first.Current.Top.Type != CellTypes.Block &&
                     chuzzles.Any(x => x.Current == first.Current.Top)) ||
                    (second.Current.Top != null && second.Current.Top.Type != CellTypes.Block &&
                     chuzzles.Any(x => x.Current == second.Current.Top))
                    )
                {
                    var topPart = GetHorizontalLineChuzzles(second.Current.y + 1, second.Type, chuzzles).ToList();
                    if (topPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(topPart);

                        Debug.Log("Combination 9");
                        isHorizontalMove = new IntVector2(second.Current.x, second.Current.y + 1);
                        chuzzleToMove = topPart.First();
                        return possibleCombination;
                    }
                }

                //try bottom    
                if (
                    (first.Current.Bottom != null && first.Current.Bottom.Type != CellTypes.Block &&
                     chuzzles.Any(x => x.Current == first.Current.Bottom)) ||
                    (second.Current.Bottom != null && second.Current.Bottom.Type != CellTypes.Block &&
                     chuzzles.Any(x => x.Current == second.Current.Bottom))
                    )
                {
                    var bottomPart = GetHorizontalLineChuzzles(first.Current.y - 1, first.Type, chuzzles).ToList();
                    if (bottomPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(bottomPart);

                        Debug.Log("Combination 10");
                        isHorizontalMove = new IntVector2(first.Current.x, first.Current.y - 1);
                        chuzzleToMove = bottomPart.First();
                        return possibleCombination;
                    }
                }
            }
        }
        Debug.Log("Combination NOOOOOOOOOO 11");
        isHorizontalMove = new IntVector2();
        chuzzleToMove = null;
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
                ch =>
                    ch.Current.x == firstChuzzle.Current.x && ch.Current.y == firstChuzzle.Current.y + 1 &&
                    ch.Type == firstChuzzle.Type);

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
                ch =>
                    ch.Current.y == firstChuzzle.Current.y && ch.Current.x == firstChuzzle.Current.x + 1 &&
                    ch.Type == firstChuzzle.Type);

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

    public static IEnumerable<Chuzzle> GetHorizontalLineChuzzles(int y, ChuzzleType chuzzleType,
        IEnumerable<Chuzzle> chuzzles)
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
                return new List<Chuzzle> {firstChuzzle, secondChuzzle};
            }
            return new List<Chuzzle> {firstChuzzle};
        }
        return new List<Chuzzle>();
    }

    public static IEnumerable<Chuzzle> GetVerticalLineChuzzles(int x, ChuzzleType chuzzleType,
        IEnumerable<Chuzzle> chuzzles)
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
                return new List<Chuzzle> {firstChuzzle, secondChuzzle};
            }
            return new List<Chuzzle> {firstChuzzle};
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
        return new IntVector2(Mathf.RoundToInt(chuzzle.transform.localPosition.x/chuzzle.Scale.x),
            Mathf.RoundToInt(chuzzle.transform.localPosition.y/chuzzle.Scale.y));
    }

    public static Cell CellAt(List<Cell> cells, int x, int y)
    {
        return cells.FirstOrDefault(c => c.x == x && c.y == y);
    }

    public static Vector3 ConvertXYToPosition(int x, int y, Vector3 scale)
    {
        return new Vector3(x*scale.x, y*scale.y, 0);
    }


    public static void ShowArrow(Chuzzle from, IntVector2 to, GameObject downArrowPrefab)
    {
        var down = Object.Instantiate(downArrowPrefab) as GameObject;
        ScaleSprite(down.GetComponent<tk2dSprite>(), from.Scale);

        if (from.Current.x == to.x)
        {
            //vertical
            if (from.Current.y >= to.y)
            {
                //to down
                //do nothing
            }
            else
            {
                //to up
                //mirror vertical
                down.GetComponent<tk2dSprite>().FlipY = true;
            }
        }
        else
        {
            //horizontal
            if (from.Current.x < to.x)
            {
                //to right
                down.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else
            {
                //to left
                //to right
                down.transform.rotation = Quaternion.Euler(0, 0, -90);
            }
        }

        down.transform.parent = from.transform;
        down.transform.localPosition = from.Scale/2f;

        from.Arrow = down;
    }

    public static void ScaleSprite(tk2dBaseSprite sprite, Vector3 size)
    {
        if (sprite.CurrentSprite.regionW != 0)
        {
            sprite.scale = new Vector3(size.x/sprite.CurrentSprite.regionW,
                size.x/sprite.CurrentSprite.regionW, 1);
        }
        else
        {
            //BUG can contain bug when sprite is single texture and there is no box collider attached to it
            sprite.scale = new Vector3(size.x/sprite.CurrentSprite.GetUntrimmedBounds().max.x,
                size.x/sprite.CurrentSprite.GetUntrimmedBounds().max.x, 1);
        }
    }
}