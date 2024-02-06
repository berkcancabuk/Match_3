using System;
using System.Collections.Generic;
using Abstracts;
using DG.Tweening;
using Enums;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public static Board Instance;
    [SerializeField] private MatchChecker _matchChecker;
    public TileMover _tileMover = new();
    private CandySettings _candySettings;


    [Header("Map Settings")] // Maybe to its own class
    public int column;

    public int row;
    public GameObject tilePrefab;
    public Tile[,] _allBackGround;
    public Tile[,] allCandies;


    private Sequence _mySequence;

    [Header("Candy Settings")] [SerializeField]
    private Transform parentCandy, parentTile;

    [SerializeField] private GameObject candyPrefab;
    [SerializeField] private Sprite[] _candySprites;
    public bool isSwapStarted;

    // Maybe move it to another class
    [SerializeField] private CandyType[] _candyTypes;
    [SerializeField] private string _candyDefaultName;

    public Vector2 selectedObject;

    private bool _isInit;
    private List<Tile> _explotionCheckCandies = new();
    private bool isReady = false;

    private List<Tile> _candiesToExplode = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        _mySequence = DOTween.Sequence();
        _matchChecker = GetComponent<MatchChecker>();
        _candySettings = new CandySettings(_candySprites, _candyTypes);
    }

    private void Start()
    {
        _allBackGround = new Tile[column, row];
        allCandies = new Tile[column, row];

        SetUp();
    }

    public void SetReady(bool isReady)
    {
        this.isReady = isReady;
    }

    // Init function for setting up the board
    private async void SetUp()
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

        isReady = true;
        await StartFill(_explotionCheckCandies);
    }


    public async UniTask FillEmptyTile()
    {
        if (!isReady)
        {
            return;
        }

        await ExplosionFill();
        var sequencer = DOTween.Sequence();
        // Used to reset and for not exploding untouched candies
        isReady = false;
        // Always checks the explosion of candies
        // A way to stop the explosion of untouched and unrelated candies can be implemented

        _candiesToExplode.Clear();

        _candiesToExplode = await _matchChecker.CheckMovedCandies();

        if (_candiesToExplode.Count < 2)
        {
            
            return;
        }

        foreach (var item in _candiesToExplode)
        {
            sequencer.Join(ExplodingTile(item.GetComponent<Candy>()));
        }

        await sequencer.AsyncWaitForCompletion();
        await UniTask.Delay(400);
        await _tileMover.TileBottomMovement(allCandies);
    }

    private Tween ExplodingTile(Candy candy)
    {
        if (candy._particles != null)
            candy.GetComponentInChildren<ParticleSystem>().Play();
        return candy.transform.DOScale(new Vector3(.5f, .5f, .5f), 0.4f)
            .SetEase(Ease.InBounce).OnComplete(() => Destroy(candy.gameObject));
    }


    private async UniTask StartFill(List<Tile> tiles)
    {
        _explotionCheckCandies.Clear();
        for (int i = 0; i < row; i++)
        {
            _mySequence = DOTween.Sequence();
            for (int j = 0; j < column; j++)
            {
                if (allCandies[j, i] == null)
                {
                    var candy = Instantiate(candyPrefab, new Vector2(j, row + 1),
                        Quaternion.identity, parentCandy);
                    int random = Random.Range(0, _candyTypes.Length);

                    allCandies[j, i] = candy.GetComponent<Tile>();
                    allCandies[j, i].arrayPos = new Vector2(j, i);
                    int count = 0;
                    while (count < 10)
                    {
                        Tuple<Sprite, CandyType> candySettings = _candySettings.GetRandomStyle();
                        candy.GetComponent<SpriteRenderer>().sprite = candySettings.Item1;
                        candy.GetComponent<Candy>().candyType = candySettings.Item2;
                        candy.gameObject.name = _candyDefaultName + candySettings.Item2 + " " + j + "," + i;
                        count++;

                        if (!await _matchChecker.CheckExplosion(candy.GetComponent<Candy>()))
                        {
                            break;
                        }
                    }

                    MoveCandy(candy.transform, i, j);
                }
            }

            await _mySequence.Play().AsyncWaitForCompletion();
        }
    }

    private async UniTask ExplosionFill()
    {
        _explotionCheckCandies.Clear();
        _mySequence = DOTween.Sequence();

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (allCandies[j, i] == null)
                {
                    //var candy = Instantiate(candies[Random.Range(0, candies.Length)], new Vector2(j, row + 1),
                    //    Quaternion.identity, parentCandy);

                    var candy = Instantiate(candyPrefab, new Vector2(j, row + 1),
                        Quaternion.identity, parentCandy);
                    int random = Random.Range(0, _candyTypes.Length);

                    Tuple<Sprite, CandyType> candySettings = _candySettings.GetRandomStyle();

                    candy.GetComponent<SpriteRenderer>().sprite = candySettings.Item1;
                    candy.GetComponent<Candy>().candyType = candySettings.Item2;
                    candy.gameObject.name = _candyDefaultName + candySettings.Item2 + " " + j + "," + i;

                    allCandies[j, i] = candy.GetComponent<Tile>();
                    allCandies[j, i].arrayPos = new Vector2(j, i);
                    _explotionCheckCandies.Add(allCandies[j, i]);
                    MoveCandy(candy.transform, i, j);
                }
            }
        }

        await _mySequence.Play().AsyncWaitForCompletion();
    }


    private void MoveCandy(Transform candy, int column, int row)
    {
        Tween move = candy.DOMove(new Vector2(row, column), TWEEN_DURATION);
        _mySequence.Join(move);
    }

    private const float TWEEN_DURATION = 0.2f;

    private async UniTask Swap(Tile obj1, Tile obj2)
    {
        if (obj1.gameObject == null || obj2.gameObject == null) return;
        if (obj1.candyType == CandyType.Empty || obj2.candyType.Equals(CandyType.Empty)) return;
        _mySequence = DOTween.Sequence();
        (obj2.arrayPos, obj1.arrayPos) = (obj1.arrayPos, obj2.arrayPos);
        var tempObj = obj1;
        allCandies[(int)obj2.arrayPos.x, (int)obj2.arrayPos.y] = obj2;
        allCandies[(int)obj1.arrayPos.x, (int)obj1.arrayPos.y] = tempObj;
        
        _mySequence.Join(obj1.transform.DOMove(obj2.transform.position, TWEEN_DURATION))
            .Join(obj2.transform.DOMove(obj1.transform.position, TWEEN_DURATION));
        await _mySequence.Play().AsyncWaitForCompletion();
    }

    public bool CheckBottomIfEmpty(Vector2 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;

        if (x.Equals(0))
        {
            return false;
        }

        return allCandies[x, y - 1].candyType == CandyType.Empty;
    }

    // Check with the direction
    public async UniTask TileSwapCheck(Vector2 pos, Direction moveDir)
    {
        var x = (int)pos.x;
        var y = (int)pos.y;
        Tile candy = allCandies[x, y];
        Tile secondCandy = null;
        switch (moveDir)
        {
            case Direction.Left:
                if (x - 1 < 0) return;
                if (allCandies[x, y].gameObject == null || allCandies[x - 1, y].gameObject == null) return;
                await Swap(allCandies[x, y], allCandies[x - 1, y]);
                secondCandy = allCandies[x, y];
                break;
            case Direction.Up:
                if (y + 1 > column + 1) return;
                if (allCandies[x, y].gameObject == null || allCandies[x, y + 1].gameObject == null) return;
                await Swap(allCandies[x, y], allCandies[x, y + 1]);
                secondCandy = allCandies[x, y];
                break;
            case Direction.Right:
                if (x + 1 > row) return;
                if (allCandies[x, y].gameObject == null || allCandies[x + 1, y].gameObject == null) return;
                await Swap(allCandies[x, y], allCandies[x + 1, y]);
                secondCandy = allCandies[x, y];
                break;
            case Direction.Down:
                if (y - 1 < 0) return;
                if (allCandies[x, y].gameObject == null || allCandies[x, y - 1].gameObject == null) return;
                await Swap(allCandies[x, y], allCandies[x, y - 1]);
                secondCandy = allCandies[x, y];
                break;
            case Direction.None:
                
                isSwapStarted = false;
                return;
        }


        // Exploding candies block Should change it to support more special candies
        if (candy.candyType.Equals(CandyType.Exploding) && await _matchChecker.CheckExplosion((Candy)candy, allCandies))
        {
            await UniTask.Delay(400);
            await _tileMover.TileBottomMovement(allCandies);
            return;
        }

        bool condition1 = !await _matchChecker.CheckExplosion((Candy)candy, allCandies);
        bool condition2 = !await _matchChecker.CheckExplosion((Candy)secondCandy, allCandies);
        if (condition1 && condition2 && isSwapStarted)
        {
            // Swap back
            await Swap(candy, secondCandy);
            isSwapStarted = false;
            return;
        }

        await UniTask.Delay(400);
        await _tileMover.TileBottomMovement(allCandies);
    }

    public void MakeTileNull(int x, int y)
    {
        allCandies[x, y] = null;
    }

    public void MoveSingleTileToBottom(int x, int y)
    {
        if (allCandies[x, y].candyType == CandyType.Empty) return;
        allCandies[x, y - 1] = allCandies[x, y];
        allCandies[x, y - 1].gameObject.transform.DOMove(new Vector2(x, y - 1), TWEEN_DURATION);
        allCandies[x, y - 1].candyType = allCandies[x, y].candyType;
        allCandies[x, y - 1].arrayPos = new Vector2(x, y - 1);
        allCandies[x, y] = null;
        //await _matchChecker.CheckExplosion(_allCandies[x, y - 1].GetComponent<Candy>(), _allCandies);
    }
}