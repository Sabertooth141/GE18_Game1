using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ProceduralMazeGen : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public int jumpDistance = 2;
    public int leveltoGen = 2;

    public int wallSpawnHeight = 100;
    public int floorSpawnHeight = 0;
    public int playerSpawnHeight = 150;

    public float wallInitialFallSpeed = -2;
    public float wallFallAcc = -5;

    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject playerPrefab;

    private int[,] _map;
    private List<Vector3> _wallPositions = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // mapWidth = (int)floor.transform.localScale.x;
        // mapHeight = (int)floor.transform.localScale.z;

        MazeGen();
        // Debug.Log("21");
        DrawMap();
    }

    private void MazeGen()
    {
        _map = new int[mapHeight, mapWidth];
        System.Random random = new System.Random();

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                _map[y, x] = 1;
            }
        }

        List<Vector2Int> directions = new()
        {
            new Vector2Int(0, jumpDistance),
            new Vector2Int(0, -jumpDistance),
            new Vector2Int(-jumpDistance, 0),
            new Vector2Int(jumpDistance, 0)
        };

        Stack<Vector2Int> path = new();
        path.Push(new Vector2Int(1, 1));
        _map[1, 1] = 0;

        while (path.Count > 0)
        {
            Vector2Int current = path.Peek();
            List<Vector2Int> possibleDirs = new();

            foreach (Vector2Int dir in directions)
            {
                int newY = current.y + dir.y;
                int newX = current.x + dir.x;

                if (IsInBounds(newY, newX) && _map[newY, newX] == 1)
                {
                    possibleDirs.Add(dir);
                }
            }

            if (possibleDirs.Count > 0)
            {
                Vector2Int chosen = possibleDirs[random.Next(0, possibleDirs.Count)];
                int newY = current.y + chosen.y;
                int newX = current.x + chosen.x;

                // for (int i = 1; i <= jumpDistance; i++)
                // {
                //     int carveY = current.y + (chosen.y / jumpDistance) * i;
                //     int carveX = current.x + (chosen.x / jumpDistance) * i;
                //     _map[carveY, carveX] = 0;
                // }
                _map[(current.y + newY) / 2, (current.x + newX) / 2] = 0;
                _map[newY, newX] = 0;
                // CarvePaths(current.x, current.y, newX, newY);

                path.Push(new Vector2Int(newX, newY));
            }
            else
            {
                path.Pop();
            }
        }

        _map[mapHeight - 2, mapWidth - 3] = 0;
        _map[mapHeight - 1, mapWidth - 3] = 0;
    }

    private void DrawMap()
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                Vector3 wallPos = new Vector3(x, wallSpawnHeight, y);
                Vector3 floorPos = new Vector3(x, floorSpawnHeight, y);

                Instantiate(floorPrefab, floorPos, Quaternion.identity, transform);

                if (_map[y, x] == 1)
                {
                    _wallPositions.Add(wallPos);
                }
            }
        }

        _wallPositions.Sort((a, b) =>
            Vector3.Distance(Vector3.zero, a).CompareTo(Vector3.Distance(Vector3.zero, b))
        );

        StartCoroutine(SpawnWalls());

        Instantiate(playerPrefab, new Vector3(1, playerSpawnHeight, 1), Quaternion.identity, transform);
    }

    private IEnumerator SpawnWalls()
    {
        foreach (Vector3 wallPos in _wallPositions)
        {
            GameObject tempWall = Instantiate(wallPrefab, wallPos, Quaternion.identity, transform);
            if (tempWall.TryGetComponent(out WallSpawn ws))
            {
                ws.Init(wallInitialFallSpeed, wallFallAcc, floorSpawnHeight);
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    private bool IsInBounds(int y, int x)
    {
        return y >= 0 && y < mapHeight - 1 && x >= 0 && x < mapWidth - 1;
    }
}