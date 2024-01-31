using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public static Board Instance;

    [SerializeField] private MatchChecker _matchChecker;
    public int column;
    public int row;
    public GameObject tilePrefab;
    public Tile[,] _allBackGround;
    public Tile[,] _allCandies;


    private Sequence _mySequence = DOTween.Sequence();
    [SerializeField] private Transform parentCandy, parentTile;
    [SerializeField] private GameObject[] candies;
    [SerializeField] private GameObject _emptyCandy;

    public TileMover _tileMover = new();
    public Vector2 selectedObject;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        _matchChecker = GetComponent<MatchChecker>();
    }

    private void Start()
    {
        _allBackGround = new Tile[column, row];
        _allCandies = new Tile[column, row];

        SetUp();
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

        await FillEmptyTile();
    }


    private async Task FillEmptyTile()
    {
        for (int i = 0; i < row; i++)
        {
            _mySequence = DOTween.Sequence();
            for (int j = 0; j < column; j++)
            {
                if (_allBackGround[j, i]?.candyType == CandyType.Empty)
                {
                    var candy = Instantiate(candies[Random.Range(0, candies.Length)], new Vector2(j, row + 1),
                        Quaternion.identity, parentCandy);

                    _allCandies[j, i] = candy.GetComponent<Tile>();
                    _allCandies[j, i].arrayPos = new Vector2(j, i);
                    _allCandies[j, i].GetComponent<Candy>().text.text = j + "-" + i;
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


        var temp = obj1.arrayPos;
        obj1.arrayPos = obj2.arrayPos;
        obj2.arrayPos = temp;
        obj1.GetComponent<Candy>().text.text = String.Format("{0}-{1}", obj1.arrayPos.x, obj1.arrayPos.y);
        obj2.GetComponent<Candy>().text.text = String.Format("{0}-{1}", obj2.arrayPos.x, obj2.arrayPos.y);


        var tempObj = obj1;
        _allCandies[(int)temp.x, (int)temp.y] = obj2;
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

    [ContextMenu("TEST MOVE BOTTOM")]
    private void MoveCandiesBottom()
    {
        foreach (var item in _allCandies)
        {
            if (item.candyType.Equals(CandyType.Empty))
            {
                continue;
            }


            if (CheckBottomIfEmpty(item.arrayPos))
            {
                TileSwapCheck(item.arrayPos, Direction.Down);
            }
        }
    }

    public void MakeCandyEmpty(Vector2 pos)
    {
        // Maybe make pos taker to int
        int x = (int)pos.x;
        int y = (int)pos.y;


        _allCandies[x, y] = Instantiate(_emptyCandy, new Vector2(x, y),
            Quaternion.identity, parentCandy).GetComponent<Tile>();
        _allCandies[x, y].candyType = CandyType.Empty;
        _allCandies[x, y].arrayPos = new Vector2(x, y);
        _allCandies[x, y].GetComponent<Candy>().text.text = x + "  " + y;
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

        _matchChecker.CheckExplosion((Candy)candy, _allCandies);
        _matchChecker.CheckExplosion((Candy)secondCandy, _allCandies);
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
    }
}