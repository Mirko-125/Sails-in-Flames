using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
 
public class GridManager : MonoBehaviour 
{
    [SerializeField] private int _width, _height;
    
    [SerializeField] private int _n; 
 
    [SerializeField] private GameObject _tilePrefab;

    [SerializeField] private GameObject friendlyHolder;
    [SerializeField] private GameObject enemyHolder;

    [SerializeField] private Transform _cam;
    [SerializeField] private float _camDistance;
 
    private Dictionary<int, TileUI> _tiles;
    int ennemyTerrain;
 
    void Start() 
    {
        _tiles = new Dictionary<int, TileUI>();

        GenerateGrid(true);
        if(SceneManager.GetActiveScene().buildIndex == 4)
        {
            GenerateGrid(false);
        }
        _cam.transform.position = new Vector3((float)(_width) / 2 + 3.5f, (float)_height / 2, _camDistance);
    }

    public void TimeToGo()
    {
        UnityHub hub = FindFirstObjectByType<UnityHub>();

        DragManager shop = FindFirstObjectByType<DragManager>();

        string map = shop.ReturnFilledMap();

        print(map);

        int count = 0;

        for (int i = 0; i < map.Length; i++)
        {
            if (map[i] == '2')
            {
                count++;
            }
        }

        print(count);

        if (hub != null && shop != null && count == 18)
        {
            hub.AcceptPlacement(PlayerPrefs.GetString("userID"), map);

            GameObject.Find("WaitingOverlay").GetComponent<NetLoader>().Reveal();
        }
    }

    //tile/grid factory metoda
 
    void GenerateGrid(bool friend) 
    {
        if (!friend)
        {
            ennemyTerrain = _n + 2;
        }
        else
        {
            ennemyTerrain = 0;
        }
        
        for (int x = 0; x < _n; x++) 
        {
            for (int y = 0; y < _n; y++) 
            {
                GameObject spawnedTile;
                if (friend)
                {
                    spawnedTile = Instantiate(_tilePrefab, friendlyHolder.transform);
                }
                else
                {
                    spawnedTile = Instantiate(_tilePrefab, enemyHolder.transform);
                }
                spawnedTile.transform.localPosition = (new Vector3(x * 32 - 160, 160 - y * 32, 0));
                spawnedTile.name = $"Tile {_n-y-1} {x}";
 
                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.GetComponent<TileUI>().Init(friend, isOffset, x, y);
                _tiles.Add((x + ennemyTerrain) * 100 + y, spawnedTile.GetComponent<TileUI>());
            }
        }
    }

    public TileUI GetTileAtPosition(int pos) {
        //TileUI tile;
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }
}