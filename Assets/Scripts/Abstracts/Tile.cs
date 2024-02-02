using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Enums;
using UnityEngine;

namespace Abstracts
{
    [Serializable]
    public abstract class Tile : MonoBehaviour
    {
        public CandyType candyType = CandyType.Empty;
        public Vector2 arrayPos = Vector2.zero;
        [SerializeField] protected ParticleSystem _particles;

        public abstract Tween ExplodingTile();
    }
}

