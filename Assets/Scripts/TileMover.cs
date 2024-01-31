using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class TileMover
{
    
    private Tile[,] m_allCandies;


    public void AddRange(List<Tile> queue, Tile[,] enu)
    {
        int rows = enu.GetLength(0);
        int columns = enu.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                queue.Add(enu[i, j]);
            }
        }
        
    }

    public void TileBottomMovement(Tile[,] candies)
    {
        List<Tile> m_candyQueue = new();
        m_allCandies = candies;
        AddRange(m_candyQueue, m_allCandies);
        for (int i = 0; i < m_candyQueue.Count; i++)
        {
            Tile t = m_candyQueue[i];
            
            
            
            int x = (int)t.arrayPos.x;
            int y = (int)t.arrayPos.y;
            if (y <= 0)
                continue;
            
            if (TileBottomCheck(x,y))
            {
                Board.Instance.MoveSingleTileToBottom(x,y);
                //m_candyQueue.RemoveAt(i);
                //m_candyQueue.Add(t);
            }
        }
    }


    private bool TileBottomCheck(int x, int y)
    {
        if ( m_allCandies[x,y-1].candyType == CandyType.Empty)
        {
            return true;
        }
        return false;

    }






}