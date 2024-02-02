using Enums;
using System;
using UnityEngine;


public class CandySettings
{
    [SerializeField] private Sprite[] m_candySprites;
    [SerializeField] private CandyType[] m_candyTypes;

    public CandySettings(Sprite[] sprites, CandyType[] types)
    {
        this.m_candySprites = sprites;
        this.m_candyTypes = types;
    }


    public Tuple<Sprite, CandyType> GetRandomStyle()
    {
        int random = UnityEngine.Random.Range(0, m_candyTypes.Length);

        return new Tuple<Sprite, CandyType>(m_candySprites[random], m_candyTypes[random]);


    }


}