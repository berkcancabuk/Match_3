using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MatchChecker : MonoBehaviour
{

    readonly List<Tile> xArray = new();
    readonly List<Tile> yArray = new();


    private async Task ConditionLoop(Tile[,] candies, List<Tile> arrayToAdd, int[] coordinates, int mainPos, int[] increase, int dimension)
    {
        var type = candies[coordinates[0], coordinates[1]].candyType;

        int dimensionLength = dimension == -1 ? -1 : candies.GetLength(dimension);


        while (BoundControl(mainPos, dimensionLength) 
            && candies[coordinates[0] + increase[0], coordinates[1] +increase[1]] != null 
            && candies[coordinates[0] + increase[0], coordinates[1] + increase[1]].candyType == type)
        {
            mainPos += increase[0].Equals(0) ? increase[1] : increase[0];
            arrayToAdd.Add(candies[coordinates[0] + increase[0], coordinates[1] + increase[1]]);
            coordinates[0] += increase[0];
            coordinates[1] += increase[1];
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

        await DestroyCandies(xArray, candy);
        await DestroyCandies(yArray, candy);

        return true;
    }
    
    private async Task DestroyCandies(List<Tile> candies,Candy candys)
    {
        if (candies.Count <2) return;
        if (candys.gameObject != null) candies.Add(candys);

        foreach (var candy in candies)
        {
            candy.candyType = CandyType.Empty;
            Board.Instance.MakeTileNull((int)candy.arrayPos.x, (int)candy.arrayPos.y);
            candy.ExplodingTile();
            Destroy(candy.gameObject);
            await Task.Delay(0);
        }

        await Board.Instance._tileMover.TileBottomMovement(Board.Instance._allCandies);
    }
    
    
}



