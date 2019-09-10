using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    private bool[,] cave;
    
    public uint Width = 128;
    public uint Height = 72;
    [Range(0,100)]
    public uint RandomFillPercentage = 50;
    public uint BorderThickness = 1;
    [Range(0,10)]
    public uint SmoothingCount = 1;
    public string Seed;
    public bool UseRandomSeed = true;
    
    public bool DrawGizmos = true;

    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }

    private void GenerateMap()
    {
        cave = new bool[Width, Height];
        RandomFillMap();
        for (int i = 0; i < SmoothingCount; i++)
        {
            SmoothMap();
        }
        Generate2DMesh();
    }

    private void RandomFillMap()
    {       
        System.Random random = new System.Random(UseRandomSeed ? (int) DateTimeOffset.Now.ToUnixTimeSeconds() : Seed.GetHashCode());

        for (int x = 0; x < cave.GetLength(0); x++)
        {
            for (int y = 0; y < cave.GetLength(1); y++)
            {
                if (x < BorderThickness || x > cave.GetLength(0) - BorderThickness - 1 || y < BorderThickness || y > cave.GetLength(1) - BorderThickness - 1)
                {
                    cave[x, y] = true;
                }
                else
                {
                    cave[x, y] = random.Next(0, 100) < RandomFillPercentage ? true : false;
                }
            }
        }
    }

    private void Generate2DMesh()
    {
        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(cave, 1);
    }

    private void OnDrawGizmos()
    {
        if (!DrawGizmos) return;

        if (cave != null)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Gizmos.color = cave[x, y] ? Color.black : Color.white;
                    Vector2 pos = new Vector2(-Width / 2 + x + .5f, -Height / 2 + y + .5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }

    private void SmoothMap()
    {
        for (uint x = BorderThickness; x < cave.GetLength(0) - BorderThickness; x++)
        {
            for (uint y = BorderThickness; y < cave.GetLength(1) - BorderThickness; y++)
            {
                uint neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                {
                    cave[x, y] = true;
                }
                else if (neighbourWallTiles < 4)
                {
                    cave[x, y] = false;
                }
            }
        }
    }

    private uint GetSurroundingWallCount(uint gridX, uint gridY)
    {        
        uint wallCount = 0;
        for (uint neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (uint neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX != gridX || neighbourY != gridY)
                {
                    wallCount += (uint) (cave[neighbourX, neighbourY] ? 1 : 0);
                }
            }
        }
        return wallCount;
    }
}
