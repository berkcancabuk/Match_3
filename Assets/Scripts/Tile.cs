
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public abstract class Tile : MonoBehaviour
{
    public CandyType candyType = CandyType.Empty;
    public Vector2 arrayPos = Vector2.zero;
    [SerializeField] protected ParticleSystem _particles;

    public abstract void ExplodingTile();
}

