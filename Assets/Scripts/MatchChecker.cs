using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchChecker : MonoBehaviour
{
    private Board _board = Board.Instance;

    List<Tile> xArray = new();
    List<Tile> yArray = new();

    private Candy currentCandy;

    // private void ConditionLoop(Tile[,] candies, int position,int checkPosX, int dimension,int checkPosY, int dimensionDecrease,int positionAddition,ref List<Tile> tiles)
    // {
    //     while (BoundsCheck(candies, dimension,position,dimensionDecrease) && candies[checkPosX, checkPosY ].candyType == currentCandy.candyType)
    //     {
    //         print(candies[checkPosX , checkPosY].name);
    //         tiles.Add(candies[checkPosX, checkPosY]);
    //         position += positionAddition;
    //     }
    // }
    //
    // private bool BoundsCheck(Tile[,] array,int dimension, int position,int dimensionDecrease)
    // {
    //     return dimension == -1 ? position > 0 : position + 1 < array.GetLength(dimension)-dimensionDecrease;
    // }

    public void CheckExplosion(Candy candy, Tile[,] candies)
    {
        xArray.Clear();
        yArray.Clear();
        currentCandy = candy;
        var x = (int)candy.arrayPos.x;
        var y = (int)candy.arrayPos.y;
    
        
        // Right
        int xCheck = x;
        while (xCheck < candies.GetLength(0) - 1 && candies[xCheck + 1, y].candyType == candy.candyType)
        {
            xArray.Add(candies[xCheck + 1, y]);
            xCheck++;
        }
        
        //Left
        xCheck = x;
        while (xCheck > 0 && candies[xCheck - 1, y].candyType == candy.candyType)
        {
            xArray.Add(candies[xCheck - 1, y]);
            xCheck--;
        }
        
        int yCheck = y;
        while (yCheck < candies.GetLength(1)-1 && candies[x, yCheck+1].candyType == candy.candyType)
        {
            yArray.Add(candies[x , yCheck+1]);
            yCheck++;
        }
        
        yCheck = y;
        while (yCheck > 0 && candies[x, yCheck-1].candyType == candy.candyType)
        {
            yArray.Add(candies[x, yCheck-1]);
            yCheck--;
        }
        
        DestroyCandies(xArray,candy);
        DestroyCandies(yArray,candy);
        
        
    }
    
    private void DestroyCandies(List<Tile> candies,Candy candys)
    {
        if (candies.Count <2) return;
        foreach (var candy in candies)
        {
            Destroy(candy.gameObject);
        }

        if (candys.gameObject == null) return;
        Destroy(candys.gameObject);
    }
    
    
}



