using Enums;
using System;
using UnityEngine;

public class ControlledRandomData
{
    int[] candyCounts;
    float[] randomCandyRatios;
    int _candyTypeCount;

    CandyType[] _typeArray;


    public ControlledRandomData(int candyTypeCount)
    {
        candyCounts = new int[candyTypeCount];
        randomCandyRatios = new float[candyTypeCount];
        this._candyTypeCount = candyTypeCount;
        _typeArray = new CandyType[]{ CandyType.Blue, CandyType.Green, CandyType.Purple, CandyType.Red };
    }

    public CandyType GetLowestRatio()
    {

        int index = 0;
        float lowest = Mathf.Infinity;
        for (int i = 0; i < randomCandyRatios.Length; i++)
        {
            if (randomCandyRatios[i] < lowest)
            {
                lowest = randomCandyRatios[i];
                index = i;
            }
        }

        //return lowest;

        return _typeArray[index];
    }

    public void SetRatio(int[] candyCounts)
    {
        this.candyCounts = candyCounts;
        int totalCandies = TotalCount(candyCounts);

        for (int i = 0; i < randomCandyRatios.Length; i++ )
        {
            randomCandyRatios[i] = candyCounts[i] * (1 / totalCandies);
        }
    }

    private int TotalCount(int[] arr)
    {
        int total = 0;

        foreach (int item in arr)
        {
            total += item;
        }
        return total;
    }

}