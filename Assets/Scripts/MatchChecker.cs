using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchChecker : MonoBehaviour
{
    private Board _board = Board.Instance;

    readonly List<Tile> xArray = new();
    readonly List<Tile> yArray = new();


    private void ConditionLoop(Tile[,] candies, List<Tile> arrayToAdd, int[] coordinates, int mainPos, int[] increase, int dimension)
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

    public bool CheckExplosion(Candy candy, Tile[,] candies)
    {
        xArray.Clear();
        yArray.Clear();
        var x = (int)candy.arrayPos.x;
        var y = (int)candy.arrayPos.y;

        ConditionLoop(candies, xArray,new int[] { x, y}, x, new int[]{ 1, 0}, 0 );
        ConditionLoop(candies, xArray, new int[] { x, y }, x, new int[] { -1, 0 }, -1);
        ConditionLoop(candies, yArray, new int[] { x, y }, y, new int[] { 0, 1 }, 1);
        ConditionLoop(candies, yArray, new int[] { x, y }, y, new int[] { 0, -1 }, -1);

        if (xArray.Count < 2 && yArray.Count < 2)
            return false;
        
        DestroyCandies(xArray,candy);
        DestroyCandies(yArray,candy);

        return true;

    }
    
    private void DestroyCandies(List<Tile> candies,Candy candys)
    {
        if (candies.Count <2) return;
        if (candys.gameObject != null) candies.Add(candys);

        foreach (var candy in candies)
        {
            candy.candyType = CandyType.Empty;
            Board.Instance.MakeTileNull((int)candy.arrayPos.x, (int)candy.arrayPos.y);
            Destroy(candy.gameObject);
        }

        Board.Instance._tileMover.TileBottomMovement(Board.Instance._allCandies);
    }
    
    
}



