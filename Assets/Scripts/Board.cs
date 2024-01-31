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
    private Tile[,] _allBackGround;
    private Tile[,] _allCandies;


    private Sequence _mySequence = DOTween.Sequence();
    [SerializeField] private Transform parentCandy, parentTile;
    [SerializeField] private GameObject[] candies;
    [SerializeField] private GameObject _emptyCandy;

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
                    candy.GetComponent<Tile>().arrayPos = new Vector2(j, i);
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
        var temp = obj1.arrayPos;
        obj1.arrayPos = obj2.arrayPos;
        _allCandies[(int)temp.x, (int)temp.y] = obj2;
        _allCandies[(int)obj2.arrayPos.x, (int)obj2.arrayPos.y] = obj1;
        obj2.arrayPos = temp;


        var sequence = DOTween.Sequence();
        sequence.Join(obj1.transform.DOMove(obj2.transform.position, TWEEN_DURATION))
            .Join(obj2.transform.DOMove(obj1.transform.position, TWEEN_DURATION));

        await sequence.Play().AsyncWaitForCompletion();
    }

    public bool CheckBottomIfEmpty(Vector2 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;

        if (x.Equals(0)) { return false; }

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

        //_allCandies[x, y] = _allBackGround[x, y];

        _allCandies[x, y] = (Tile)Instantiate(_emptyCandy, new Vector2(x,y),
                        Quaternion.identity, parentCandy).GetComponent<Candy>();
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
                await Swap(_allCandies[x, y], _allCandies[x - 1, y]);
                secondCandy = _allCandies[x, y];
                break;
            case Direction.Up:
                if (y + 1 > column+1) return;
                await Swap(_allCandies[x, y], _allCandies[x, y + 1]);
                secondCandy = _allCandies[x, y];
                break;
            case Direction.Right:
                if (x + 1 > row) return;
                await Swap(_allCandies[x, y], _allCandies[x + 1, y]);
                secondCandy = _allCandies[x, y];
                break;
            case Direction.Down:
                if (y - 1 < 0) return;
                await Swap(_allCandies[x, y], _allCandies[x, y - 1]);
                secondCandy = _allCandies[x, y];
                break;
            case Direction.None:
                throw new ArgumentOutOfRangeException(nameof(moveDir), moveDir, null);
        }
        
        print((Candy)candy + " name" );
        _matchChecker.CheckExplosion((Candy)candy,_allCandies);
        _matchChecker.CheckExplosion((Candy)secondCandy,_allCandies);
    }

}