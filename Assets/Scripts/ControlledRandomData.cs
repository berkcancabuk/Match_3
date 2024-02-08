using Enums;
using UnityEngine;
using Unity;
using System.Collections.Generic;
using Abstracts;
using Unity.VisualScripting;
using System;

public class ControlledRandomData
{
    int[] candyCounts;
    float[] randomCandyRatios;
    int _candyTypeCount;

    CandyType[] _typeArray;


    public ControlledRandomData(int candyTypeCount, List<Tile> candies)
    {
        candyCounts = new int[candyTypeCount];
        randomCandyRatios = new float[candyTypeCount];
        this._candyTypeCount = candyTypeCount;
        _typeArray = new CandyType[]{ CandyType.Blue, CandyType.Green, CandyType.Purple, CandyType.Red };
        SetRatiosOfCandies(candies);
    }

    public ControlledRandomData(int[] candyCounts, float[] randomCandyRatios, int candyTypeCount, CandyType[] typeArray)
    {
        this.candyCounts = candyCounts;
        this.randomCandyRatios = randomCandyRatios;
        _candyTypeCount = candyTypeCount;
        _typeArray = typeArray;
    }

    public ControlledRandomData()
    {
        //Empty Constructor
        _typeArray = new CandyType[] { CandyType.Blue, CandyType.Green, CandyType.Purple, CandyType.Red };
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

    // Berkcancan burada mı buldurak yoksa boarddan mı alaq
    // Overload yaptım agla
    // Candy counts already known 
    public void SetRatiosOfCandies(int[] candyCounts)
    {
        this.candyCounts = candyCounts;
        int totalCandies = TotalCount(this.candyCounts);

        for (int i = 0; i < randomCandyRatios.Length; i++ )
        {
            randomCandyRatios[i] = candyCounts[i] * (1 / totalCandies);
        }
    }

    // Candy counts are not known set them beforehand spawning
    // Blue 0, Green 1, Purple 2, Red 3
    public void SetRatiosOfCandies(List<Tile> candies)
    {
        candyCounts = new int[_candyTypeCount];
        for (int i = 0; i < candies.Count; i++)
        {
            Tile item = candies[i];
            switch (item.candyType)
            {
                case CandyType.Blue:
                    candyCounts[0]++;
                    break;
                case CandyType.Green:
                    candyCounts[1]++;
                    break;
                case CandyType.Purple:
                    candyCounts[2]++;
                    break;
                case CandyType.Red:
                    candyCounts[3]++;
                    break;
                case CandyType.Empty:
                    break;
            }
        }

        int totalCandies = TotalCount(this.candyCounts);

        for (int i = 0; i < randomCandyRatios.Length; i++)
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

    bool GetItem(int percent)
    {
        int rand = UnityEngine.Random.Range(0, 100);
        return rand < percent;
    }

}