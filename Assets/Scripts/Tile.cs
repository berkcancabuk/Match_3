using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour
{
    public enum CandyType
    {
        Green,
        Blue,
        Purple,
        Red,
        Empty
    }

    public CandyType candyType = CandyType.Empty;
    
    private Vector2 m_tilePosition;
    
    public void SetTilePosition(Vector2 tilePosition)
    {
        m_tilePosition = tilePosition;
    }

    // Checks the below 
    private void CheckBottom()
    {

    }

    private void MoveBottom()
    {

    }
    private void OnMouseDown()
    {
        PlayerController.Instance.SelectObject(this);
        
    }
}
