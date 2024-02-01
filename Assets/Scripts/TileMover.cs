using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abstracts;


[Serializable]
public class TileMover
{
    
    private Tile[,] m_allCandies;


    public void AddRange(Queue<Tile> queue, Tile[,] enu)
    {
        int rows = enu.GetLength(0);
        int columns = enu.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                queue.Enqueue(enu[i, j]);
            }
        }
        
    }

    public async Task TileBottomMovement(Tile[,] candies)
    {
        Queue<Tile> m_candyQueue = new();
        m_allCandies = candies;
        AddRange(m_candyQueue, m_allCandies);
        while (m_candyQueue.Count > 0 )
        { 
            Tile t = m_candyQueue.Dequeue();

            if (t == null || (int)t.arrayPos.y <= 0)
                continue;

            int x = (int)t.arrayPos.x;
            int y = (int)t.arrayPos.y;
            
            if (TileBottomCheck(x,y))
            {
                Board.Instance.MoveSingleTileToBottom(x,y);
                m_candyQueue.Enqueue(t);
            }
        }

        await Task.Delay(0);

        await Board.Instance.FillEmptyTile();
    }


    private bool TileBottomCheck(int x, int y)
    {
        if ( m_allCandies[x,y-1] == null)
        {
            return true;
        }
        return false;

    }






}