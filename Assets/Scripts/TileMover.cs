using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class TileMover
{
    
    private Tile[,] m_allCandies;


    public void AddRange(List<Tile> queue, Tile[,] enu)
    {
        foreach (Tile obj in enu)
        {
            queue.Add(obj);
        }
        
    }

    public void TileBottomMovement(Tile[,] candies)
    {
        Debug.Log("bitti mi");
        List<Tile> m_candyQueue = new();
        m_allCandies = candies;
        AddRange(m_candyQueue, m_allCandies);
        for (int i = 0; i < m_candyQueue.Count; i++)
        {
            Tile t = m_candyQueue[i];
            
            m_candyQueue.RemoveAt(i);
            
            int x = (int)t.arrayPos.x;
            int y = (int)t.arrayPos.y;
            if (y <= 0)
                continue;
            
            if (TileBottomCheck(x,y))
            {
                Board.Instance.MoveSingleTileToBottom(x,y);
                m_candyQueue.Add(t);
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