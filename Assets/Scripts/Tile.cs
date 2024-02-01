
using System;
using UnityEngine;
[Serializable]
public class Tile : MonoBehaviour
{
    public CandyType candyType = CandyType.Empty;
    public Vector2 arrayPos = Vector2.zero;
    [SerializeField] private ParticleSystem _particles;

    public void ExplodingTile()
    {
        if (_particles != null)
        {
            _particles.Play();
        }
    }

}

