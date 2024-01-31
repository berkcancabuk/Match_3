using System.Collections.Generic;
using UnityEngine;



public class TileMover
{

    private Queue<Tile> m_candyQueue;
    private Tile[,] m_allCandies;


    public void AddRange(Queue<Tile> queue, Tile[,] enu)
    {
        foreach (Tile obj in enu)
            queue.Enqueue(obj);
    }

    public void TileBottomMovement(Tile[,] candies)
    {
        m_allCandies = candies;
        AddRange(m_candyQueue, m_allCandies);

        while (m_candyQueue.Count > 0 )
        {
            Tile t = m_candyQueue.Dequeue();

            int x = (int)t.arrayPos.x;
            int y = (int)t.arrayPos.y;

            if (TileBottomCheck(x,y))
            {

                // Move single candy from board
                t.arrayPos = new Vector2(x, y - 1);

                m_candyQueue.Enqueue(t);
            }
            
        }

    }


    private bool TileBottomCheck(int x, int y)
    {
        if (m_allCandies[x, y-1].gameObject == null)
        {
            return true;
        }
        return false;

    }






}