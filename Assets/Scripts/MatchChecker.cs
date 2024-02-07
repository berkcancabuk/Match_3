using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Abstracts;
using DG.Tweening;
using Enums;
using UnityEngine;

public class MatchChecker : MonoBehaviour
{
    readonly List<Tile> xArray = new();
    readonly List<Tile> yArray = new();

    private Sequence sequence;
    private bool isSpecialCondition = false;
    private async UniTask ConditionLoop(Tile[,] candies, List<Tile> arrayToAdd, int[] coordinates, int mainPos,
        int[] increase, int dimension)
    {
        var type = candies[coordinates[0], coordinates[1]].candyType;

        
        int dimensionLength = dimension == -1 ? -1 : candies.GetLength(dimension);

        if (type.Equals(CandyType.Exploding))
        {
            while (BoundControl(mainPos, dimensionLength)
                   && candies[coordinates[0] + increase[0], coordinates[1] + increase[1]] != null
                   && !candies[coordinates[0] + increase[0], coordinates[1] + increase[1]].candyType
                       .Equals(CandyType.Empty))
            {
                mainPos += increase[0].Equals(0) ? increase[1] : increase[0];
                arrayToAdd.Add(candies[coordinates[0] + increase[0], coordinates[1] + increase[1]]);
                coordinates[0] += increase[0];
                coordinates[1] += increase[1];
            }

            isSpecialCondition = true;
        }
        else
        {
            while (BoundControl(mainPos, dimensionLength)
                   && candies[coordinates[0] + increase[0], coordinates[1] + increase[1]] != null
                   && candies[coordinates[0] + increase[0], coordinates[1] + increase[1]].candyType == type)
            {
                mainPos += increase[0].Equals(0) ? increase[1] : increase[0];
                arrayToAdd.Add(candies[coordinates[0] + increase[0], coordinates[1] + increase[1]]);
                coordinates[0] += increase[0];
                coordinates[1] += increase[1];
            }
        }
    }


    private bool BoundControl(int pos, int dimension)
    {
        //First one pos > 0 ===> xCheck > 0 && candies[xCheck - 1, y
        //Second one  =====> xCheck > 0 && candies[xCheck - 1, y
        return dimension == -1 ? pos > 0 : pos < dimension - 1;
    }

    public async UniTask<bool> CheckExplosion(Candy candy, Tile[,] candies)
    {
        xArray.Clear();
        yArray.Clear();
        var x = (int)candy.arrayPos.x;
        var y = (int)candy.arrayPos.y;

        await ConditionLoop(candies, xArray, new int[] { x, y }, x, new int[] { 1, 0 }, 0);
        await ConditionLoop(candies, xArray, new int[] { x, y }, x, new int[] { -1, 0 }, -1);
        await ConditionLoop(candies, yArray, new int[] { x, y }, y, new int[] { 0, 1 }, 1);
        await ConditionLoop(candies, yArray, new int[] { x, y }, y, new int[] { 0, -1 }, -1);

        if (xArray.Count < 2 && yArray.Count < 2)
            return false;

        if (!isSpecialCondition)
        {
            if (xArray.Count >= 4 || yArray.Count >= 4)
            {
                if (yArray.Count >= 4)
                {
                    yArray.Add(candy);
                    await CheckSpecialCandy(yArray);
                    if (xArray.Count < 4 && xArray.Count >= 2)
                    {
                        await DestroyCandies(xArray, null);
                        return true;
                    }
                }

                if (xArray.Count >= 4)
                {
                    xArray.Add(candy);
                    await CheckSpecialCandy(xArray);
                    if (yArray.Count < 4 && yArray.Count >= 2)
                    {
                        await DestroyCandies(yArray, null);
                        return true;
                    }
                }
            }
        }

        if (xArray.Count >= 2 && yArray.Count >= 2)
        {
            xArray.Add(candy);
            xArray.AddRange(yArray);
            await DestroyCandies(xArray, null);

            return true;
        }

        await DestroyCandies(xArray, candy);
        await DestroyCandies(yArray, candy);
        return true;
    }

    public async UniTask<bool> CheckStartExplosion(Candy candy)
    {
        Tile[,] candies = Board.Instance.allCandies;

        xArray.Clear();
        yArray.Clear();
        var x = (int)candy.arrayPos.x;
        var y = (int)candy.arrayPos.y;
        if (candies[x, y] == null)
        {
            return false;
        }

        await ConditionLoop(candies, xArray, new int[] { x, y }, x, new int[] { 1, 0 }, 0);
        await ConditionLoop(candies, xArray, new int[] { x, y }, x, new int[] { -1, 0 }, -1);
        await ConditionLoop(candies, yArray, new int[] { x, y }, y, new int[] { 0, 1 }, 1);
        await ConditionLoop(candies, yArray, new int[] { x, y }, y, new int[] { 0, -1 }, -1);

        if (xArray.Count < 2 && yArray.Count < 2)
            return false;
        return true;
    }

    public async UniTask<List<Tile>> CheckMovedCandies()
    {
        List<Tile> emptyTiles = new();
        Tile[,] candies = Board.Instance.allCandies;

        
        foreach (var item in candies)
        {
            if (!await CheckStartExplosion(item.GetComponent<Candy>())) continue;
            if (emptyTiles.Contains(item) || item.candyType == CandyType.Exploding) continue;
            emptyTiles.Add(item);
        }

        return emptyTiles;
    }


    public async UniTask DestroyCandies(List<Tile> candies, Candy candys)
    {
        if (candies.Count < 2)
        {
            return;
        }

        if (candys != null) candies.Add(candys);
        
      
        sequence = DOTween.Sequence();
        foreach (var candy in candies)
        {
            if (candy == null)
                continue;
            candy.candyType = CandyType.Empty;
            Board.Instance.MakeTileNull((int)candy.arrayPos.x, (int)candy.arrayPos.y);
            sequence.Join(candy.ExplodingTile());
        }

        await sequence.Play().AsyncWaitForCompletion();
        if (isSpecialCondition)
        {
            isSpecialCondition = false;
        }
        EventManager.OnAddScore?.Invoke(candies.Count);
        EventManager.OnPlaySound?.Invoke();
    }

    private async UniTask CheckSpecialCandy(List<Tile> candies)
    {
        var midPos = Vector2.zero;
        for (int i = 0; i < candies.Count; i++)
        {
            midPos += candies[i].arrayPos;
        }
        midPos /= candies.Count;

        midPos = new Vector2(Mathf.FloorToInt(midPos.x), Mathf.FloorToInt(midPos.y));
        
        sequence = DOTween.Sequence();
        foreach (var candy in candies)
        {
            if (candy == null)
                continue;
            candy.candyType = CandyType.Empty;
            Board.Instance.MakeTileNull((int)candy.arrayPos.x, (int)candy.arrayPos.y);
            sequence.Join(candy.ExplodingTile());
        }
        
        await sequence.Play().AsyncWaitForCompletion();
        await Board.Instance.SetMidSpecialCandy(midPos);
         
             
        EventManager.OnAddScore?.Invoke(candies.Count);
        EventManager.OnPlaySound?.Invoke();
        
        
    } 
}