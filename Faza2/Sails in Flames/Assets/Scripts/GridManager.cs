using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.SceneManagement;
 
public class GridManager : MonoBehaviour 
{
    [SerializeField] private int _width, _height;
    
    [SerializeField] private int _n; 
 
    [SerializeField] private Tile _tilePrefab;
 
    [SerializeField] private Transform _cam;
    [SerializeField] private float _camDistance;
 
    private Dictionary<Vector2, Tile> _tiles;
    int ennemyTerrain;
 
    void Start() 
    {
        GenerateGrid(true);
        if(SceneManager.GetActiveScene().buildIndex == 3)
        {
            GenerateGrid(false);
        }
        _cam.transform.position = new Vector3((float)(_width) / 2 + 3.5f, (float)_height / 2, _camDistance);
    }
 
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
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _n; x++) 
        {
            for (int y = 0; y < _n; y++) 
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x + ennemyTerrain, y), Quaternion.identity);
                spawnedTile.name = $"Tile {_n-y-1} {x}";
 
                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(friend, isOffset, _n-y-1, x);
                _tiles[new Vector2(x + ennemyTerrain, y)] = spawnedTile;
            }
        }
    }

    public Tile GetTileAtPosition(Vector2 pos) {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }
}