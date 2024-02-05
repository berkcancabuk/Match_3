using Abstracts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxBoard : MonoBehaviour
{
    public static SandboxBoard Instance;

    [Header("Map Settings")] // Maybe to its own class
    public int column;
    public int row;
    
    public Tile[,] _allBackGround;
    public Tile[,] allCandies;
    
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject _parentTile;
    [SerializeField] private GameObject _sandParentTile;
    [SerializeField] private GameObject _sandObject;
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
    }


    private void Start()
    {
        _allBackGround = new Tile[column, row];
        SetUp();
        Debug.Log("Board Set Up");
    }

    // Init function for setting up the board
    private async void SetUp()
    {
        for (var i = 0; i < column; i++)
        {
            for (var j = 0; j < row; j++)
            {
                var tempPosition = new Vector2(i, j);
                var backGroundTile = Instantiate(_tilePrefab, tempPosition, Quaternion.identity, _parentTile.transform);
                _allBackGround[i, j] = backGroundTile.GetComponent<Tile>();
                ((SandTile)_allBackGround[i, j]).arrayPos = new Vector2(i, j);
            }
        }
    }

    public void InstantiateSand(Vector2 arrPos)
    {
        if (CheckBackground(arrPos))
        {
            SandboxSand obj = Instantiate(_sandObject, arrPos, Quaternion.identity, _sandParentTile.transform).GetComponent<SandboxSand>(); 
            obj.arrPos = arrPos;
            _allBackGround[(int)arrPos.x, (int)arrPos.y].candyType = Enums.CandyType.Blue;
        }
    }
    public bool CheckBackground(Vector2 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        if (_allBackGround[x,y].candyType == Enums.CandyType.Empty)
        {
            return true;
        }

        return false;
    }

    public bool CheckBelow(Vector2 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y-1;

        if (y < 0 || y >=  _allBackGround.GetLength(1))
        {
            Debug.Log("Sand out of bounds");
            return false;
        }

        if (_allBackGround[x, y].candyType == Enums.CandyType.Empty)
        {
            Debug.Log("Sand can swap");
            return true;
        }
        return false;
    }

    public bool CheckLeft (Vector2 pos)
    {
        int x = (int)pos.x-1;
        int y = (int)pos.y - 1;

        if (y < 0) { return false; }
        if (x < 0 || x >= _allBackGround.GetLength(0)) { return false; }


        if (_allBackGround[x, y].candyType == Enums.CandyType.Empty)
        {
            Debug.Log("Sand can swap");
            _allBackGround[x +1, y + 1].candyType = Enums.CandyType.Empty;
            _allBackGround[x, y].candyType = Enums.CandyType.Blue;
            return true;
        }
        return false;
    }
    public bool CheckRight(Vector2 pos)
    {
        int x = (int)pos.x+1;
        int y = (int)pos.y - 1;

        if (y < 0) { return false; }
        if (x < 0 || x >= _allBackGround.GetLength(0)) { return false; }

        if (_allBackGround[x, y].candyType == Enums.CandyType.Empty)
        {
            Debug.Log("Sand can swap");
            _allBackGround[x-1, y+1].candyType = Enums.CandyType.Empty;
            _allBackGround[x, y].candyType = Enums.CandyType.Blue;
            return true;
        }
        return false;
    }
    public void SwapBelow(Vector2 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        _allBackGround[x, y].candyType = Enums.CandyType.Empty;
        _allBackGround[x, y -1].candyType = Enums.CandyType.Blue;

        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
