using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abstracts;
using Enums;
using UnityEngine;

public class MatchChecker : MonoBehaviour
{
    readonly List<Tile> xArray = new();
    readonly List<Tile> yArray = new();
    
    private async Task ConditionLoop(Tile[,] candies, List<Tile> arrayToAdd, int[] coordinates, int mainPos, int[] increase, int dimension)
    {
        var type = candies[coordinates[0], coordinates[1]].candyType;

        int dimensionLength = dimension == -1 ? -1 : candies.GetLength(dimension);

        if (type.Equals(CandyType.Exploding))
        {
            while (BoundControl(mainPos, dimensionLength)
                && candies[coordinates[0] + increase[0], coordinates[1] + increase[1]] != null
                && !candies[coordinates[0] + increase[0], coordinates[1] + increase[1]].candyType.Equals(CandyType.Empty))
            {
                mainPos += increase[0].Equals(0) ? increase[1] : increase[0];
                arrayToAdd.Add(candies[coordinates[0] + increase[0], coordinates[1] + increase[1]]);
                coordinates[0] += increase[0];
                coordinates[1] += increase[1];
                await Task.Delay(0);
            }
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
                await Task.Delay(0);
            }
        }

    }


    private bool BoundControl(int pos, int dimension)
    {
        //First one pos > 0 ===> xCheck > 0 && candies[xCheck - 1, y
        //Second one  =====> xCheck > 0 && candies[xCheck - 1, y
        return dimension == -1 ? pos > 0 : pos < dimension - 1;
    }

    public async Task<bool> CheckExplosion(Candy candy, Tile[,] candies)
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

        if (xArray.Count >= 2 && yArray.Count >= 2)
        {
            xArray.Add(candy);
            print("üste girdi");
            xArray.AddRange(yArray);
            await DestroyCandies(xArray, null);
            
            return true;
        }
        
        await DestroyCandies(xArray, candy);
        await DestroyCandies(yArray, candy);
        print("alta girdi");
        return true;
    }
    public async Task<bool> CheckExplosion(Candy candy)
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

        //if (xArray.Count >= 2 && yArray.Count >= 2)
        //{
        //    return true;
        //}
        return true;
    }
    private async Task DestroyCandies(List<Tile> candies,Candy candys)
    {
        if (candies.Count < 2)
        {
            return;
        }
        if (candys != null) candies.Add(candys);

        foreach (var candy in candies)
        {
            candy.candyType = CandyType.Empty;
            Board.Instance.MakeTileNull((int)candy.arrayPos.x, (int)candy.arrayPos.y);
            candy.ExplodingTile();
        } 
        
        EventManager.OnAddScore?.Invoke(candies.Count);
        EventManager.OnPlaySound?.Invoke();
       
    }
    
    
}



