using System;
using Enums;
using System.Collections.Generic;
using System.Linq;
using Abstracts;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class ControlledRandomData
{
    
    float[] randomCandyRatios;
    int _candyTypeCount;

    private int _candyCounts;
    private CandyType[] _typeArray;
    private CandyType _dominantCandy;

    

    private Dictionary<Tuple<int,int>, CandyType> candyTable = new Dictionary<Tuple<int,int>, CandyType>(); 

    public ControlledRandomData(int candyCount,CandyType dominantCandy, params CandyType[] candyTypes)
    {
        this._candyCounts = candyCount;
        this._typeArray = candyTypes;
        this._dominantCandy = dominantCandy;
        
        ArrayAssignCandyTypes();
        
    }



    public void PrintCandies()
    {
        foreach (var candy in candyTable)
        {
            Debug.Log("Candy Type = " + candy.Value 
            + " Place = " + candy.Key);
        }
    }

    public CandyType GetCandyTypeOfIndex(Tuple<int, int> index)
    {
        return candyTable[index];
    }
    public void ArrayAssignCandyTypes()
    {

        int rand = Random.Range(0, _typeArray.Length);
        CandyType domType = _typeArray[rand];
        List<CandyType> types = _typeArray.ToList();
        types.Remove(domType);
        int resetCount = 0;
        int dominantCount = DominantCalc();
        int normalCount = NonDominantCalc();

        for (int i = 0; i < _candyCounts; i++)
        {
            int x;
            int y;
            while (true)
            {
                x = Random.Range(0, 5);
                y = Random.Range(0, 7);

                if (!candyTable.ContainsKey(new Tuple<int, int>(x, y)))
                {
                    break;
                }
            }

            if (i < dominantCount)
            {
                candyTable.Add(new Tuple<int, int>(x, y), domType);
                continue;
            }

            candyTable.Add(new Tuple<int, int>(x, y), types[0]);
            resetCount++;
            if (resetCount >= normalCount)
            {
                resetCount = 0;
                types.RemoveAt(0);
            }

        }
    }
    
    private int DominantCalc()
    {
        var ratio = _candyCounts / _typeArray.Length;
        return ratio + _typeArray.Length - 1;
    }
    
    private int NonDominantCalc()
    {
        return (_candyCounts - DominantCalc()) / (_typeArray.Length - 1);
    }
    
    
   


}