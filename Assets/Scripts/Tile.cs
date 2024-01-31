using System;
using UnityEngine;
using UnityEngine.Serialization;
public abstract class Tile : MonoBehaviour
{
    public CandyType candyType = CandyType.Empty;
    public Vector2 arrayPos = Vector2.zero;
}

