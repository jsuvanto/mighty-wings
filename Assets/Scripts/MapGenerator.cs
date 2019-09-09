using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    private bool[,] cave;

    [Min(0)]
    public int Width = 128;
    [Min(0)]
    public int Height = 72;
    [Range(0,100)]
    public int RandomFillPercentage = 50;
    [Min(0)]
    public int BorderThickness = 1;
    [Min(0)]
    public int SmoothingCount = 1;
    public string Seed;
    public bool UseRandomSeed = true;

    // debugging
    public bool DrawGizmos = true;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    // Update is called once per frame
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
        SmoothMap(SmoothingCount);
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
                    cave[x, y] = random.Next(0, 100) >= RandomFillPercentage ? true : false;
                }
            }
        }
    }

    private void SmoothMap(int count)
    {

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


}
