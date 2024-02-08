using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abstracts;
using DG.Tweening;
using Enums;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Board : MonoBehaviour
{
    public static Board Instance;
    [SerializeField] private MatchChecker _matchChecker;
    public TileMover _tileMover = new();
    private CandySettings _candySettings;
    private ControlledRandomData _randData;

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
    [SerializeField] private GameObject candyExplosion;
    [SerializeField] private Sprite[] _candySprites;
    public bool isSwapStarted = true;

    // Maybe move it to another class
    [SerializeField] private CandyType[] _candyTypes;
    [SerializeField] private string _candyDefaultName;
    [SerializeField] private int _levelCandyTypes;

    public Vector2 selectedObject;

    private bool _isInit;
    private List<Tile> _explotionCheckCandies = new();
    private bool isReady = false;

    private List<Tile> _candiesToExplode = new();

    private const float SCALE_DURATION = 10;

    public float lastDeltaTime = 0; 
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
        isSwapStarted = false;
        Application.targetFrameRate = 60;
        lastDeltaTime =  20*Time.fixedDeltaTime;
        
    }
    public async UniTask FillEmptyTile()
    {
        if (!isReady)
        {
            return;
        }

        
        await ExplosionFill();
       
        isReady = false;

        _candiesToExplode.Clear();

        _candiesToExplode = await _matchChecker.CheckMovedCandies();

        if (_candiesToExplode.Count < 2)
        {
            return;
        }
        _mySequence = DOTween.Sequence();

        foreach (var item in _candiesToExplode)
        {
            _mySequence.Join(ExplodingTile(item.GetComponent<Candy>()));
        }
        EventManager.OnAddScore?.Invoke(_candiesToExplode.Count);
        EventManager.OnPlaySound?.Invoke();

        // Append a function at the end of the anim cycle 
        //
        _mySequence.AppendCallback(async () => {
            await UniTask.Delay(10);
            await _tileMover.TileBottomMovement(allCandies);
        });
        _mySequence.Play();

        // 
        //while (!_mySequence.IsComplete)
        //{
        //    await UniTask.Delay(0);
        //}
        //await UniTask.Delay((int)(lastDeltaTime*1000));
        //await _tileMover.TileBottomMovement(allCandies);
    }

    private Tween ExplodingTile(Candy candy)
    {
        if (candy._particles != null)
            candy.GetComponentInChildren<ParticleSystem>().Play();
        return candy.transform.DOScale(new Vector3(.5f, .5f, .5f), lastDeltaTime)
            .SetEase(Ease.InBounce).OnComplete(() => Destroy(candy.gameObject));
    }
    
    private async void WaitSequence()
    {
        await _mySequence.Play().AsyncWaitForCompletion();
        return;
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

                        if (!await _matchChecker.CheckStartExplosion(candy.GetComponent<Candy>()))
                        {
                            break;
                        }
                    }
                    _explotionCheckCandies.Add(candy.GetComponent<Candy>());
                    MoveCandy(candy.transform, i, j);
                }
                
            }
            print(DOTween.TotalActiveTweeners() + " tweener -- " + DOTween.TotalActiveSequences() + " sequencceesss" + DOTween.TotalActiveTweens() + " active tweens");
            await _mySequence.Play().AsyncWaitForCompletion();
        }
        _randData = new ControlledRandomData(_levelCandyTypes, _explotionCheckCandies);
    }

    private async UniTask ExplosionFill()
    {
        _explotionCheckCandies.Clear();
        for (int i = 0; i < row; i++)
        {
            _mySequence = DOTween.Sequence();
            for (int j = 0; j < column; j++)
            {
                if (allCandies[j, i] == null)
                {
                    //var candy = Instantiate(candies[Random.Range(0, candies.Length)], new Vector2(j, row + 1),
                    //    Quaternion.identity, parentCandy);

                    var candy = Instantiate(candyPrefab, new Vector2(j, row + 1),
                        Quaternion.identity, parentCandy);
                    

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
            await _mySequence.Play().AsyncWaitForCompletion();
        }

    }

    public async UniTask SetMidSpecialCandy(Vector2 midPos)
    {
        var x = (int)midPos.x;
        var y = (int)midPos.y;
        var candy = Instantiate(candyExplosion, midPos,
            Quaternion.identity, parentCandy);
        candy.gameObject.name = "SpecialCandy" + "-> " + x+" - "+y;
        allCandies[x, y] = candy.GetComponent<Tile>();
        allCandies[x, y].arrayPos = midPos;
        allCandies[x, y].candyType = CandyType.Exploding;
    }


    private void MoveCandy(Transform candy, int column, int row)
    {
        Tween move = candy.DOMove(new Vector2(row, column), .4f);
        _mySequence.Join(move);
        
    }

    private const float TWEEN_DURATION = 10f;

    private async UniTask Swap(Tile obj1, Tile obj2)
    {
        if (obj1.gameObject == null || obj2.gameObject == null) return;
        if (obj1.candyType == CandyType.Empty || obj2.candyType.Equals(CandyType.Empty)) return;
        _mySequence = DOTween.Sequence();
        
        (obj2.arrayPos, obj1.arrayPos) = (obj1.arrayPos, obj2.arrayPos);
        var tempObj = obj1;
        allCandies[(int)obj2.arrayPos.x, (int)obj2.arrayPos.y] = obj2;
        allCandies[(int)obj1.arrayPos.x, (int)obj1.arrayPos.y] = tempObj;
        
        _mySequence.Join(obj1.transform.DOMove(obj2.transform.position, lastDeltaTime))
            .Join(obj2.transform.DOMove(obj1.transform.position, lastDeltaTime));
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
        isSwapStarted = true;
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
                if (y + 1 >= row) return;
                if (allCandies[x, y].gameObject == null || allCandies[x, y + 1].gameObject == null) return;
                await Swap(allCandies[x, y], allCandies[x, y + 1]);
                secondCandy = allCandies[x, y];
                break;
            case Direction.Right:
                if (x + 1 >= column) return;
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
            await MovementBottomCheckDelay();
            return;
        }

        if (await CheckSwappable(candy, secondCandy)) return;

        await MovementBottomCheckDelay();
    }

    private async Task<bool> CheckSwappable(Tile candy, Tile secondCandy)
    {
        var condition1 = !await _matchChecker.CheckExplosion((Candy)candy, allCandies);
        var condition2 = !await _matchChecker.CheckExplosion((Candy)secondCandy, allCandies);
        if (!condition1 || !condition2) return false;
        // Swap back
        await Swap(candy, secondCandy);
        isSwapStarted = false;
        return true;

    }

    private async Task MovementBottomCheckDelay()
    {
        await UniTask.Delay((int)(lastDeltaTime*1000));
        await _tileMover.TileBottomMovement(allCandies);
        isSwapStarted = false;
    }

    public void MakeTileNull(int x, int y)
    {
        allCandies[x, y] = null;
    }

    public void MoveSingleTileToBottom(int x, int y)
    {
        if (allCandies[x, y].candyType == CandyType.Empty) return;
        allCandies[x, y - 1] = allCandies[x, y];
        allCandies[x, y - 1].gameObject.transform.DOMove(new Vector2(x, y - 1), lastDeltaTime);
        allCandies[x, y - 1].candyType = allCandies[x, y].candyType;
        allCandies[x, y - 1].arrayPos = new Vector2(x, y - 1);
        allCandies[x, y] = null;
        //await _matchChecker.CheckExplosion(_allCandies[x, y - 1].GetComponent<Candy>(), _allCandies);
    }
}