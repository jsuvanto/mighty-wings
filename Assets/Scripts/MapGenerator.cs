using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshGenerator))]
public class MapGenerator : MonoBehaviour
{

    private Cave cave;
    
    public uint Width = 128;
    public uint Height = 72;
    [Range(0,100)]
    public uint RandomFillPercentage = 50;
    public uint BorderThickness = 1;
    [Range(0,10)]
    public uint SmoothingCount = 1;
    public string Seed;
    public bool UseRandomSeed = true;

    public uint MinimumRegionSize;

    public bool RemoveSmallRegions;
    public uint PassageWidth;
    public bool ConnectAllRooms;
    
    public bool DrawGizmos = true;

    private void Start()
    {
        GenerateCave();
    }

    public void GenerateCave()
    {
        cave = new Cave(Width, Height, RandomFillPercentage, BorderThickness);

        cave.Generate(UseRandomSeed ? (int)DateTimeOffset.Now.ToUnixTimeSeconds() : Seed.GetHashCode() );

        for (int i = 0; i < SmoothingCount; i++)
        {
            cave.SmoothCave();
        }

        if (RemoveSmallRegions)
        {
            cave.ReplaceRegions(CaveTile.Air, CaveTile.Wall, MinimumRegionSize);
            cave.ReplaceRegions(CaveTile.Wall, CaveTile.Air, MinimumRegionSize);
        }

        if (ConnectAllRooms)
        {
            cave.ConnectAllRegionsOfType(CaveTile.Air, (int) PassageWidth);
        }

        MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
        meshGenerator.GenerateMesh(cave.Tiles, 1);
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
                    switch (cave.Tiles[x, y])
                    {
                        case CaveTile.Wall:
                            Gizmos.color = Color.black;
                            break;
                        case CaveTile.Air:
                            Gizmos.color = Color.white;
                            break;
                        default:
                            Gizmos.color = Color.red;
                            break;
                    }
                    Vector2 pos = new Vector2(-Width / 2 + x + .5f, -Height / 2 + y + .5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }

    public Vector3 RandomSpawnLocation()
    {
        var possibleSpawnLocations = cave.GetRegions(CaveTile.Air)[0].Tiles;

        var random = new System.Random();

        var randomTile = possibleSpawnLocations[random.Next(0, possibleSpawnLocations.Count)];

        return new Vector3(-Width / 2 + .5f + randomTile.X, -Height / 2 + .5f + randomTile.Y, 1);
    }
}

public enum CaveTile
{
    Wall, Air
}

public class Region : IComparable<Region>
{
    public List<Coord> Tiles { get; private set; }
    public int RoomSize { get => Tiles.Count; }
    public CaveTile Type { get; private set; }

    public Region()
    {
    }

    public Region(List<Coord> roomTiles, CaveTile type)
    {
        Tiles = roomTiles;
        Type = type;
    }

    public int CompareTo(Region other)
    {
        return other.RoomSize.CompareTo(RoomSize);
    }

    public override bool Equals(object obj)
    {
        Region other = (Region)obj;
        return this.Tiles == other.Tiles;
    }

    public override int GetHashCode()
    {
        return Tiles.GetHashCode();
    }
}

public class Coord
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Coord()
    {
    }

    public Coord(int x, int y)
    {
        X = x;
        Y = y;
    }

    public List<Coord> GetNeighbours()
    {
        return new List<Coord>
        {
            new Coord(X-1, Y), // left
            new Coord(X+1, Y), // right
            new Coord(X, Y-1), // below
            new Coord(X, Y+1), // above
        };

    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 73;
            hash = (hash * 79) + X.GetHashCode();
            hash = (hash * 83) + Y.GetHashCode();
            return hash;
        }
    }

    public override bool Equals(object obj)
    {
        Coord other = (Coord)obj;
        return (this.X == other.X && this.Y == other.Y);
    }

    public override string ToString()
    {
        return $"Coord X: {X} Y: {Y}";
    }
}

public class Cave
{    
    public CaveTile[,] Tiles { get; private set; }
    public int Width { get => Tiles.GetLength(0); }
    public int Height { get => Tiles.GetLength(1); }

    private readonly int _randomFillPercentage;
    private readonly int _borderThickness;

    public Cave(uint width, uint height, uint randomFillPercentage, uint borderThickness)
    {
        Tiles = new CaveTile[width, height];
        _randomFillPercentage = (int) randomFillPercentage;
        _borderThickness = (int) borderThickness;
    }

    public void Generate(int seed)
    {
        System.Random random = new System.Random(seed);

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (x < _borderThickness || x > Width - _borderThickness - 1 || y < _borderThickness || y > Height - _borderThickness - 1)
                {
                    Tiles[x, y] = CaveTile.Wall;
                }
                else
                {
                    Tiles[x, y] = random.Next(0, 100) < _randomFillPercentage ? CaveTile.Wall : CaveTile.Air;
                }
            }
        }

    }

    public List<Region> GetRegions(CaveTile type)
    {
        var regions = new List<Region>();
        var visited = new bool[Width, Height];

        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                if (!visited[x,y] && Tiles[x,y] == type)
                {
                    var coord = new Coord(x, y);
                    var tiles = GetRegionTiles(coord, ref visited);
                    regions.Add(new Region(tiles, Tiles[x, y]));
                }                               
            }
        }

        return regions;
    }

    private List<Coord> GetRegionTiles(Coord start, ref bool[,] visited)
    {
        var tiles = new List<Coord>();
        CaveTile tileType = Tiles[start.X, start.Y];
        var queue = new Queue<Coord>();
        queue.Enqueue(start);
        visited[start.X, start.Y] = true;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);
            
            var neighbours = tile.GetNeighbours().Where(t => IsInMapRange(t));
                
            foreach (var neighbour in neighbours)
            {
                if (Tiles[neighbour.X, neighbour.Y] == tileType && !visited[neighbour.X, neighbour.Y])
                {
                    queue.Enqueue(neighbour);
                    visited[neighbour.X, neighbour.Y] = true;
                }
            }
        }

        return tiles;
    }

    private bool IsInMapRange(Coord tile)
    {
        return IsInMapRange(tile.X, tile.Y);
    }

    private bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public void SmoothCave()
    {
        var smoothCave = Tiles;

        for (int x = _borderThickness; x < Width - _borderThickness; x++)
        {
            for (int y = _borderThickness; y < Height - _borderThickness; y++)
            {
                var neighbourWallTiles = GetSurroundingTileCount(x, y, CaveTile.Wall);

                if (neighbourWallTiles > 4)
                {
                    smoothCave[x, y] = CaveTile.Wall;
                }
                else if (neighbourWallTiles < 4)
                {
                    smoothCave[x, y] = CaveTile.Air;
                }
            }
        }

        Tiles = smoothCave;
    }

    private int GetSurroundingTileCount(int gridX, int gridY, CaveTile type)
    {
        var wallCount = 0;
        for (var neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (var neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX != gridX || neighbourY != gridY)
                {
                    wallCount += Tiles[neighbourX, neighbourY] == type ? 1 : 0;
                }
            }
        }
        return wallCount;
    }

    public void ReplaceRegions(CaveTile from, CaveTile to, uint minimumSize)
    {
        var smallRegions = GetRegions(from).Where(r => r.RoomSize < minimumSize);

        foreach (var region in smallRegions)
        {
            foreach (Coord tile in region.Tiles)
            {
                Tiles[tile.X, tile.Y] = to;
            }
        }
    }

    public void ConnectAllRegionsOfType(CaveTile type, int passageWidth)
    {
        var regions = GetRegions(type);
        
        while (regions.Count > 1)
        {
            var region = regions[0];
            regions.Remove(region);
            double bestDistance = Width + Height; // Ensured to be larger than distance between any two regions
            Coord bestTileA = new Coord();
            Coord bestTileB = new Coord();

            foreach (var otherRegion in regions)
            {
                (Coord tileA, Coord tileB) = FindClosestTilesBetweenRegions(region, otherRegion);
                double distanceBetweenRegions = GetDistanceBetweenTiles(tileA, tileB);
                if (distanceBetweenRegions < bestDistance)
                {
                    bestDistance = distanceBetweenRegions;
                    bestTileA = tileA;
                    bestTileB = tileB;
                }
            }          

            CreatePassage(bestTileA, bestTileB, type, passageWidth);

            regions = GetRegions(type);
        }                
    }

    void CreatePassage(Coord tileA, Coord tileB, CaveTile type, int width)
    {
        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord c in line)
        {
            DrawCircle(c, width, type);
        }
    }

    private Tuple<Coord, Coord> FindClosestTilesBetweenRegions(Region regionA, Region regionB)
    {
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        double bestDistance = Width + Height;

        foreach (Coord tileA in regionA.Tiles)
        {
            foreach (Coord tileB in regionB.Tiles)
            {
                double distanceBetweenRegions = GetDistanceBetweenTiles(tileA, tileB);

                if (distanceBetweenRegions < bestDistance)
                {
                    bestDistance = distanceBetweenRegions;
                    bestTileA = tileA;
                    bestTileB = tileB;
                }
            }
        }

        return new Tuple<Coord, Coord>(bestTileA, bestTileB);
    }

    private static double GetDistanceBetweenTiles(Coord tileA, Coord tileB)
    {
        int dx = (int) (tileA.X - tileB.X);
        int dy = (int) (tileA.Y - tileB.Y);

        return Math.Sqrt(dx*dx + dy*dy);
    }

    void DrawCircle(Coord c, int r, CaveTile tileType)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = c.X + x;
                    int drawY = c.Y + y;
                    if (IsInMapRange(drawX, drawY))
                    {
                        Tiles[drawX, drawY] = tileType;
                    }
                }
            }
        }
    }

    List<Coord> GetLine(Coord from, Coord to)
    {
        // Bresenham

        List<Coord> line = new List<Coord>();

        int x = from.X;
        int y = from.Y;

        int dx = Math.Abs(to.X - x);
        int sx = x < to.X ? 1 : -1;
        int dy = Math.Abs(to.Y - y);
        int sy = y < to.Y ? 1 : -1;

        int err = (dx > dy ? dx : -dy) / 2, e2;
        for (; ; )
        {
            line.Add(new Coord(x, y));
            if (x == to.X && y == to.Y) break;
            e2 = err;
            if (e2 > -dx) { err -= dy; x += sx; }
            if (e2 < dy) { err += dx; y += sy; }
        }
        return line;

    }
}