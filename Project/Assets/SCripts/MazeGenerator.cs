using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tile floor;
    [SerializeField] Tile wall;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject winTile;

    [SerializeField] int walkerIterations;
    [SerializeField] int minPathLength;
    [SerializeField] int maxPathLength;
    [SerializeField] int height;
    [SerializeField] int width;
    int[,] map;

    void GenerateMaze()
    {
        map = new int[width + 1, height + 1];
        tilemap.ClearAllTiles();
        AddInitialWalls();
        GeneratePath();
        AddGoals();
        FillInMap();
        AddPlayer();
    }

    void AddInitialWalls()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = 1;
            }
        }
    }

    void GeneratePath()
    {
        int xdir = 0;
        int ydir = 0;
        int streak = 0;
        int streakMax = Random.Range(minPathLength, maxPathLength);
        var walker = new Vector2Int(width / 2, height / 2);

        for (int i = 0; i < walkerIterations; i++)
        {
            map[walker.x, walker.y] = 0;

            if (streak == 0)
            {
                var r = Random.Range(0, 4);

                if (r == 0 && (xdir != -1 && xdir != 1))
                {
                    xdir = -1;
                    ydir = 0;
                }
                else if (r == 1 && xdir != -1 && xdir != 1)
                {
                    xdir = 1;
                    ydir = 0;
                }
                else if (r == 2 && ydir != 1 && ydir != -1)
                {
                    ydir = -1;
                    xdir = 0;
                }
                else if (r == 3 && ydir != -1 && ydir != 1)
                {
                    ydir = 1;
                    xdir = 0;
                }

                streakMax = Random.Range(minPathLength, maxPathLength);
            }

            if (streak < streakMax)
            {
                if (xdir != 0)
                {
                    if (walker.x + (xdir * 2) > width || walker.x + (xdir * 2) < 0) { walker = ChooseNewWalker(); streak = 0; continue; }

                    if (map[walker.x + xdir, walker.y] == 0 || map[walker.x + (xdir * 2), walker.y] == 0)
                    {
                        if (streak == 0)
                        {
                            walker = ChooseNewWalker();
                        }

                        streak = streakMax;

                    }
                    else
                    {
                        walker += new Vector2Int(xdir, 0);
                    }
                }
                else if (ydir != 0)
                {
                    if (walker.y + (ydir * 2) > height || walker.y + (ydir * 2) < 0) { walker = ChooseNewWalker(); streak = 0; continue; }

                    if (map[walker.x, walker.y + ydir] == 0 || map[walker.x, walker.y + (ydir * 2)] == 0)
                    {
                        if (streak == 0)
                        {
                            walker = ChooseNewWalker();
                        }

                        streak = streakMax;
                    }
                    else
                    {
                        walker += new Vector2Int(0, ydir);
                    }
                }

                streak++;
            }
            else
            {
                streak = 0;
            }
        }

    }

    void AddGoals()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 0)
                {
                    if (x == 1 || x == width - 2 || y == 1 || y == height - 2)
                    {
                        map[x, y] = 0;
                        Instantiate(winTile, new Vector3(x + .5f, y + .5f, 0), Quaternion.identity, this.transform);
                    }
                }
            }
        }
    }

    void AddPlayer()
    {
        if (map[width / 2, height / 2] == 1)
        {
            print("OH NOES!!");
        }

        Instantiate(playerPrefab, new Vector3(width / 2 + .5f, height / 2 + .5f, 0), Quaternion.identity);
    }

    Vector2Int ChooseNewWalker()
    {
        while (true)
        {
            var randX = Random.Range(0, width);
            var randY = Random.Range(0, height);

            if (map[randX, randY] == 0)
            {
                return new Vector2Int(randX, randY);
            }
        }
    }

    void FillInMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), TileType(map[x, y]));
            }
        }
    }

    Tile TileType(int typeNum)
    {
        if (typeNum == 1)
        {
            return wall;
        }

        return floor;
    }

    void Awake()
    {
        GenerateMaze();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(0);
        }
    }
}
