using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    private TileBackGround[,] _allTiles;
    public static Board instance;

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
    _allTiles = new TileBackGround[width, height];
    
    SetUp();
    }

    void SetUp()
    {
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                var tempPosition = new Vector2(i, j);
                var backGroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);

                backGroundTile.GetComponent<Tile>().SetTilePosition(tempPosition);

            }
        }
    }
}
