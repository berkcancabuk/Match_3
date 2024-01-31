using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchChecker : MonoBehaviour
{
    private Board _board = Board.Instance;

    readonly List<Tile> xArray = new();
    readonly List<Tile> yArray = new();


    public void CheckExplosion(Candy candy, Tile[,] candies)
    {
        xArray.Clear();
        yArray.Clear();
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
            candy.candyType = CandyType.Empty;
            Destroy(candy.gameObject);
        }
        
        if (candys.gameObject == null) return;
        candys.candyType = CandyType.Empty;
        Destroy(candys.gameObject);
        Board.Instance._tileMover.TileBottomMovement(Board.Instance._allCandies);
        
        
        
        
    }
    
    
}



