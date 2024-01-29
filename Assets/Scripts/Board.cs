using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public int column;
    public int row;
    public GameObject tilePrefab;
    private Tile[,] _allTiles;
    public static Board instance;

    private Sequence _mySequence = DOTween.Sequence();
    [SerializeField] private Transform parentCandy, parentTile;
    [SerializeField] private GameObject[] candies;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }
    private void Start()
    {
        _allTiles = new Tile[column, row];
    
        SetUp();
    }

    void SetUp()
    {
        for (var i = 0; i < column; i++)
        {
            for (var j = 0; j < row; j++)
            {
                var tempPosition = new Vector2(i, j);
                var backGroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity,parentTile);
                _allTiles[i, j] = backGroundTile.GetComponent<Tile>();
                //backGroundTile.GetComponent<Tile>().SetTilePosition(tempPosition);
            }
        }
        Thread.Sleep(3000);
        FillEmptyTile();
    }


    private void FillEmptyTile()
    {
        for (int i = 0; i < column; i++)
        {
            _mySequence = DOTween.Sequence();
            for (int j = 0; j < row; j++)
            {
                if (_allTiles[i,j].candyType.Equals(Tile.CandyType.Empty))
                {
                    var candy = Instantiate(candies[Random.Range(0, candies.Length)], new Vector2(i, row + 1),
                        Quaternion.identity, parentCandy);
                    _allTiles[i, j].candyType = candy.GetComponent<Tile>().candyType;
                    MoveCandy(candy.transform, i, j);
                }
            }

            _mySequence.Play();

        }
    }

    private void MoveCandy(Transform candy,int column, int row)
    {
        Tween move =candy.DOMove(new Vector2(column, row), 0.4f).SetDelay(Random.Range(0,.4f));
        _mySequence.Join(move);
    }
}



