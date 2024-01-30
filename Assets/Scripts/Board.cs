using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public static Board instance;

    public int column;
    public int row;
    public GameObject tilePrefab;
    private Tile[,] _allBackGround;
    private Tile[,] _allCandies;


    private Sequence _mySequence = DOTween.Sequence();
    [SerializeField] private Transform parentCandy, parentTile;
    [SerializeField] private GameObject[] candies;


    public Vector2 selectedObject;

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
        _allBackGround = new Tile[column, row];
        _allCandies = new Tile[column, row];

        SetUp();
    }

    async void SetUp()
    {
        for (var i = 0; i < column; i++)
        {
            for (var j = 0; j < row; j++)
            {
                var tempPosition = new Vector2(i, j);
                var backGroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity, parentTile);
                _allBackGround[i, j] = backGroundTile.GetComponent<Tile>();
            }
        }

        await FillEmptyTile();
        AddRigidbody();
    }


    private async Task FillEmptyTile()
    {
        for (int i = 0; i < row; i++)
        {
            _mySequence = DOTween.Sequence();
            for (int j = 0; j < column; j++)
            {
                if (_allBackGround[j, i].candyType.Equals(CandyType.Empty))
                {
                    var candy = Instantiate(candies[Random.Range(0, candies.Length)], new Vector2(j, row + 1),
                        Quaternion.identity, parentCandy);

                    _allCandies[j, i] = candy.GetComponent<Tile>();
                    candy.GetComponent<Tile>().ArrayPos = new Vector2(j, i);
                    MoveCandy(candy.transform, i, j);
                }
            }

            await _mySequence.Play().AsyncWaitForCompletion();
        }
    }

    private void AddRigidbody()
    {
        foreach (Transform tiles in parentCandy)
        {
            tiles.gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void MoveCandy(Transform candy, int column, int row)
    {
        Tween move = candy.DOMove(new Vector2(row, column), 0.3f).SetDelay(0.05f);
        _mySequence.Join(move);
    }

    private const float TWEEN_DURATION = 0.2f;

    private async Task Swap(Tile obj1, Tile obj2)
    {
        var temp = obj1.ArrayPos;
        obj1.ArrayPos = obj2.ArrayPos;
        _allCandies[(int)temp.x, (int)temp.y] = obj2;
        _allCandies[(int)obj2.ArrayPos.x, (int)obj2.ArrayPos.y] = obj1;
        obj2.ArrayPos = temp;


        var sequence = DOTween.Sequence();
        sequence.Join(obj1.transform.DOMove(obj2.transform.position, TWEEN_DURATION))
            .Join(obj2.transform.DOMove(obj1.transform.position, TWEEN_DURATION));

        await sequence.Play().AsyncWaitForCompletion();
        GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
    }


    public void TileSwapCheck(Vector2 pos, Direction moveDir)
    {
        var outOfBoundsCheck = (pos.x < 0) && (pos.y < 0) && (pos.x >= row) && (pos.y >= column);
        if (outOfBoundsCheck)
        {
            return;
        }

        switch (moveDir)
        {
            case Direction.Left:
                Swap(_allCandies[(int)pos.x, (int)pos.y], _allCandies[(int)pos.x - 1, (int)pos.y]).Wait();
                break;
            case Direction.Up:
                Swap(_allCandies[(int)pos.x, (int)pos.y], _allCandies[(int)pos.x, (int)pos.y + 1]).Wait();
                break;
            case Direction.Right:
                Swap(_allCandies[(int)pos.x, (int)pos.y], _allCandies[(int)pos.x + 1, (int)pos.y]).Wait();
                break;
            case Direction.Down:
                Swap(_allCandies[(int)pos.x, (int)pos.y], _allCandies[(int)pos.x, (int)pos.y - 1]).Wait();
                break;
            case Direction.None:
                throw new ArgumentOutOfRangeException(nameof(moveDir), moveDir, null);
        }
    }
}