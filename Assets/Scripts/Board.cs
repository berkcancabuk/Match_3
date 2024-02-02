using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abstracts;
using DG.Tweening;
using Enums;
using UnityEngine;
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
    public Tile[,] _allCandies;


    private Sequence _mySequence;

    [Header("Candy Settings")]
    [SerializeField] private Transform parentCandy, parentTile;
    [SerializeField] private GameObject candyPrefab;
    [SerializeField] private Sprite[] _candySprites;

    // Maybe move it to another class
    [SerializeField] private CandyType[] _candyTypes;
    [SerializeField] private string _candyDefaultName;

    public Vector2 selectedObject;

    private bool _isInit;
    private List<Tile> _explotionCheckCandies =  new();
    private bool isReady = false;

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
        _allCandies = new Tile[column, row];

        SetUp();
    }
    
    public void SetReady(bool isReady)
    {
        this.isReady = isReady;
    }

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
        await FillEmptyTile();
    }


    public async Task FillEmptyTile()
    {
        if (!isReady)
        {
            return;
        }
        if (!_isInit)
        {
            Fill(_explotionCheckCandies);
            _isInit = true;
            return;
        }
        await Fill();
        

        // Used to reset and for not exploding untouched candies
        isReady = false;

        // Always checks the explosion of candies
        // A way to stop the explosion of untouched and unrelated candies can be implemented
        foreach (var item in _allCandies)
        {
            if (await _matchChecker.CheckExplosion(item.GetComponent<Candy>(), _allCandies))
            {
                await Task.Delay(400);
                await _tileMover.TileBottomMovement(_allCandies);
            }
        }
        
    }


    private async Task Fill(List<Tile> tiles)
    {
        _explotionCheckCandies.Clear();
        for (int i = 0; i < row; i++)
        {
            _mySequence = DOTween.Sequence();
            for (int j = 0; j < column; j++)
            {
                if (_allCandies[j, i] == null)
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

                    _allCandies[j, i] = candy.GetComponent<Tile>();
                    _allCandies[j, i].arrayPos = new Vector2(j, i);
                    _explotionCheckCandies.Add(_allCandies[j, i]);
                    MoveCandy(candy.transform, i, j);
                }
            }

            await _mySequence.Play().AsyncWaitForCompletion();

        }
    }
    private async Task Fill()
    {
        _explotionCheckCandies.Clear();
        for (int i = 0; i < row; i++)
        {
            _mySequence = DOTween.Sequence();
            for (int j = 0; j < column; j++)
            {
                if (_allCandies[j, i] == null)
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

                    _allCandies[j, i] = candy.GetComponent<Tile>();
                    _allCandies[j, i].arrayPos = new Vector2(j, i);
                    _explotionCheckCandies.Add(_allCandies[j, i]);
                    MoveCandy(candy.transform, i, j);
                }
            }

            await _mySequence.Play().AsyncWaitForCompletion();

        }
    }


    private void MoveCandy(Transform candy, int column, int row)
    {
        Tween move = candy.DOMove(new Vector2(row, column), TWEEN_DURATION);
        _mySequence.Join(move);
    }

    private const float TWEEN_DURATION = 0.2f;

    private async Task Swap(Tile obj1, Tile obj2)
    {
        if (obj1.gameObject == null || obj2.gameObject == null) return;
        if (obj1.candyType == CandyType.Empty || obj2.candyType.Equals(CandyType.Empty)) return;
        

        (obj2.arrayPos, obj1.arrayPos) = (obj1.arrayPos, obj2.arrayPos);



        var tempObj = obj1;
        _allCandies[(int)obj2.arrayPos.x, (int)obj2.arrayPos.y] = obj2;
        _allCandies[(int)obj1.arrayPos.x, (int)obj1.arrayPos.y] = tempObj;


        var sequence = DOTween.Sequence();
        sequence.Join(obj1.transform.DOMove(obj2.transform.position, TWEEN_DURATION))
            .Join(obj2.transform.DOMove(obj1.transform.position, TWEEN_DURATION));

        await sequence.Play().AsyncWaitForCompletion();
    }

    public bool CheckBottomIfEmpty(Vector2 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;

        if (x.Equals(0))
        {
            return false;
        }

        return _allCandies[x, y - 1].candyType == CandyType.Empty;
    }

    public async Task TileSwapCheck(Vector2 pos, Direction moveDir)
    {
        var x = (int)pos.x;
        var y = (int)pos.y;
        Tile candy = _allCandies[x, y];
        Tile secondCandy = null;

        switch (moveDir)
        {
            case Direction.Left:
                if (x - 1 < 0) return;
                if (_allCandies[x, y].gameObject == null || _allCandies[x - 1, y].gameObject == null) return;
                await Swap(_allCandies[x, y], _allCandies[x - 1, y]);
                secondCandy = _allCandies[x, y];
                break;
            case Direction.Up:
                if (y + 1 > column + 1) return;
                if (_allCandies[x, y].gameObject == null || _allCandies[x, y + 1].gameObject == null) return;
                await Swap(_allCandies[x, y], _allCandies[x, y + 1]);
                secondCandy = _allCandies[x, y];
                break;
            case Direction.Right:
                if (x + 1 > row) return;
                if (_allCandies[x, y].gameObject == null || _allCandies[x + 1, y].gameObject == null) return;
                await Swap(_allCandies[x, y], _allCandies[x + 1, y]);
                secondCandy = _allCandies[x, y];
                break;
            case Direction.Down:
                if (y - 1 < 0) return;
                if (_allCandies[x, y].gameObject == null || _allCandies[x, y - 1].gameObject == null) return;
                await Swap(_allCandies[x, y], _allCandies[x, y - 1]);
                secondCandy = _allCandies[x, y];
                break;
            case Direction.None:
                throw new ArgumentOutOfRangeException(nameof(moveDir), moveDir, null);
        }

        // Should change it 
        // When given in the if condition it DOES NOT check both of the candies
        if (candy.candyType.Equals(CandyType.Exploding) && await _matchChecker.CheckExplosion((Candy)candy, _allCandies)) 
         {
            await Task.Delay(400);
            await _tileMover.TileBottomMovement(_allCandies);
            return;
        }
        bool condition1 = !await _matchChecker.CheckExplosion((Candy)candy, _allCandies);
        bool condition2 = !await _matchChecker.CheckExplosion((Candy)secondCandy, _allCandies);
        if (condition1 && condition2)
        {
            // Swap back
           await Swap(candy, secondCandy);
        }
        await Task.Delay(400);
        await _tileMover.TileBottomMovement(_allCandies);
    }

    public void MakeTileNull(int x, int y)
    {
        _allCandies[x, y] = null;
    }
    public void MoveSingleTileToBottom(int x, int y)
    {
        if(_allCandies[x, y].candyType == CandyType.Empty) return;
        _allCandies[x, y - 1] = _allCandies[x, y];
        _allCandies[x, y - 1].gameObject.transform.DOMove(new Vector2(x, y - 1), TWEEN_DURATION);
        _allCandies[x, y - 1].candyType = _allCandies[x, y].candyType;
        _allCandies[x, y - 1].arrayPos = new Vector2(x, y - 1);
        _allCandies[x, y] = null;
        //await _matchChecker.CheckExplosion(_allCandies[x, y - 1].GetComponent<Candy>(), _allCandies);
    }
}