using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchChecker : MonoBehaviour
{
    private Board _board = Board.Instance;

    private int _counterY = 0;
    private int _counterX = 0;
    List<Tile> xArray = new();
    List<Tile> yArray = new();

    private Candy currentCandy;

    private void ConditionLoop(Tile[,] candies, int checkPos, int dimension, int secondDimension, int condition, ref List<Tile> tiles)
    {
        while (BoundsCheck(candies, dimension,checkPos) && candies[checkPos + condition, secondDimension ].candyType == currentCandy.candyType)
        {
            print(candies[checkPos + condition, secondDimension].name);
            tiles.Add(candies[checkPos + condition, secondDimension]);
            checkPos++;
            _counterX++;
        }
    }

    private bool BoundsCheck(Tile[,] array,int dimension, int position)
    {
        return position + 1 < array.GetLength(dimension);
    }

    public void CheckExplosion(Candy candy, Tile[,] candies)
    {
        xArray.Clear();
        yArray.Clear();
        currentCandy = candy;
        var x = (int)candy.arrayPos.x;
        var y = (int)candy.arrayPos.y;
        // [,] candies
        //while if (x - 1 < 0) return; if (x + 1 > row) return;
        //x[
    
        //ConditionLoop(candies,x,0,1, 1,ref xArray);
        // Right
        int xCheck = x;
        while (xCheck < candies.GetLength(0) - 1 && candies[xCheck + 1, y].candyType == candy.candyType)
        {
            print(candies[xCheck + 1, y].name);
            xArray.Add(candies[xCheck + 1, y]);
            xCheck++;
            _counterX++;
        }

        //Left
        xCheck = x;
        while (xCheck > 0 && candies[xCheck - 1, y].candyType == candy.candyType)
        {
            print(candies[xCheck - 1, y].name);
            xArray.Add(candies[xCheck - 1, y]);
            xCheck--;
            _counterX++;
        }
        
        int yCheck = y;
        while (yCheck < candies.GetLength(1)-1 && candies[x, yCheck+1].candyType == candy.candyType)
        {
            print(candies[x, yCheck+1].name);
            yArray.Add(candies[x , yCheck+1]);
            yCheck++;
            _counterY++;
        }
    
        yCheck = y;
        while (yCheck > 0 && candies[x, yCheck-1].candyType == candy.candyType)
        {
            print(candies[x, yCheck+1].name);
            yArray.Add(candies[x, yCheck-1]);
            yCheck--;
            _counterY++;
        }
        
        if (xArray.Count >= 2)
        {
            DestroyCandies(xArray);
            Destroy(candy.gameObject);
        }
    
        if (yArray.Count >= 2)
        {
            DestroyCandies(yArray);
    
            if (candy != null)
            {
                Destroy(candy.gameObject);
            }
        }
        
    }
    
    private void DestroyCandies(List<Tile> candies)
    {
        foreach (var candy in candies)
        {
            Board.Instance.MakeCandyEmpty(candy.arrayPos);
            // Need to check with empty 
            Destroy(candy.gameObject);
        }
    }
    
    
}



