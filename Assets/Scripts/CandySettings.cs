using Enums;
using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class CandySettings
{
    private Sprite[] m_candySprites;
    private CandyType[] m_candyTypes;

    public CandySettings(Sprite[] sprites, CandyType[] types)
    {
        this.m_candySprites = sprites;
        this.m_candyTypes = types;
    }

    //OLABİLME OLASILIĞI
    private float[] colorProbablities = new float[4] { 5f, 3f, 12f, 80f };
    public Tuple<Sprite, CandyType> GetRandomStyle()
    {
        
        int randomNumber = Random.Range(0, 100);

        float cumulativeProbablitiy = 0f;

        for (int i = 0; i < colorProbablities.Length; i++)
        {
            cumulativeProbablitiy += colorProbablities[i];
            if (randomNumber <= cumulativeProbablitiy) // HANGİ OLASILIK OLDUĞUNU BELİRLİYOR
            {
                return new Tuple<Sprite, CandyType>(m_candySprites[i], m_candyTypes[i]); // TYPEDA HANGİSİNİ DÖNDÜRECEĞİNİ SÖYLÜYOR
            }
        }

        return new Tuple<Sprite, CandyType>(m_candySprites[randomNumber], m_candyTypes[randomNumber]); // EĞER YOKSA DİREKT MAVİ BASIYOR    

    }


    public Sprite GetCandySprite(CandyType type)
    {
        return m_candySprites[Array.IndexOf(m_candyTypes, type)];
    }
    
}