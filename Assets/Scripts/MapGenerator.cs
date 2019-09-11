using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshGenerator))]
public class MapGenerator : MonoBehaviour
{

    private CaveTile[,] cave;
    
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

    void Start()
    {
        GenerateCave();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateCave();
        }
    }

    private void GenerateCave()
    {
        cave = new CaveTile[Width, Height];
        RandomFillMap();
        for (int i = 0; i < SmoothingCount; i++)
        {
            SmoothCave();
        }

        if (RemoveSmallRegions) RemoveSmallRegionsOfTypes(CaveTile.Air, CaveTile.Wall);

        if (ConnectAllRooms) ConnectAllRegionsOfType(CaveTile.Air);

        Generate2DMesh();
    }

    private void ConnectAllRegionsOfType(CaveTile type) // TODO crash inbound
    {
        var regions = GetRegions(type);
        while (regions.Count > 1) // TODO: this is recursive (nice) but wasteful (not nice)
        {
            ConnectFirstRegion(regions);
            regions = GetRegions(type);
        }
    }

    private void RandomFillMap()
    {       
        System.Random random = new System.Random(UseRandomSeed ? (int) DateTimeOffset.Now.ToUnixTimeSeconds() : Seed.GetHashCode());

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (x < BorderThickness || x > Width - BorderThickness - 1 || y < BorderThickness || y > Height - BorderThickness - 1)
                {
                    cave[x, y] = CaveTile.Wall;
                }
                else
                {
                    cave[x, y] = random.Next(0, 100) < RandomFillPercentage ? CaveTile.Wall : CaveTile.Air;
                }
            }
        }
    }

    private void SmoothCave()
    {
        var smoothCave = cave;

        for (uint x = BorderThickness; x < cave.GetLength(0) - BorderThickness; x++)
        {
            for (uint y = BorderThickness; y < cave.GetLength(1) - BorderThickness; y++)
            {
                uint neighbourWallTiles = GetSurroundingWallCount(x, y);

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

        cave = smoothCave;
    }

    private void RemoveSmallRegionsOfTypes(params CaveTile[] roomTypes)
    {
        foreach (var type in roomTypes)
        {
            List<Region> regions = GetRegions(type);

            foreach (var region in regions)
            {
                if (region.roomSize < MinimumRegionSize)
                {
                    foreach (Coord tile in region.tiles)
                    {
                        cave[tile.X, tile.Y] = (type == CaveTile.Wall ? CaveTile.Air : CaveTile.Wall); // TODO support for more tile types
                    }
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
                    switch (cave[x,y])
                    {
                        case CaveTile.Wall:
                            Gizmos.color = Color.black;
                            break;
                        case CaveTile.Air:
                            Gizmos.color = Color.white;
                            break;
                        default:
                            Gizmos.color = Color.clear;
                            break;
                    }
                    Vector2 pos = new Vector2(-Width / 2 + x + .5f, -Height / 2 + y + .5f);
                    Gizmos.DrawCube(pos, Vector3.one);
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
                    wallCount += (uint) (cave[neighbourX, neighbourY] == CaveTile.Wall ? 1 : 0);
                }
            }
        }
        return wallCount;
    }


    List<Coord> GetRegionTiles(uint startX, uint startY)
    {
        List<Coord> tiles = new List<Coord>();
        bool[,] checkedCave = new bool[Width, Height];

        CaveTile tileType = cave[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();

        queue.Enqueue(new Coord(startX, startY));

        checkedCave[startX, startY] = true;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (uint x = tile.X - 1; x <= tile.X + 1; x++)
            {
                for (uint y = tile.Y - 1; y <= tile.Y + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.Y || x == tile.X))
                    {
                        if (checkedCave[x, y] == false && cave[x, y] == tileType)
                        {
                            checkedCave[x, y] = true;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    List<Region> GetRegions(CaveTile tileType)
    {
        List<Region> regions = new List<Region>();
        bool[,] checkedCave = new bool[Width, Height];

        for (uint x = 0; x < Width; x++)
        {
            for (uint y = 0; y < Height; y++)
            {
                if (checkedCave[x, y] == false && cave[x, y] == tileType)
                {
                    List<Coord> tiles = GetRegionTiles(x, y);
                    Region newRegion = new Region(tiles, cave);
                    regions.Add(newRegion);

                    foreach (Coord tile in tiles)
                    {
                        checkedCave[tile.X, tile.Y] = true;
                    }
                }
            }
        }

        return regions;
    }

    bool IsInMapRange(uint x, uint y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    struct Coord
    {
        public uint X;
        public uint Y;

        public Coord(uint x, uint y)
        {
            X = x;
            Y = y;
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
            Coord other = (Coord) obj;
            return (this.X == other.X && this.Y == other.Y);
        }
    }


    void ConnectFirstRegion(List<Region> regions)
    {
        if (regions.Count < 2) return;

        double bestDistance = Width + Height; // Ensured to be larger than distance between any two regions
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Region firstRegion = regions[0];

        foreach (var region in regions.GetRange(1, regions.Count))
        {
            (Coord tileA, Coord tileB) = FindClosestTilesBetweenRegions(firstRegion, region);
            double distanceBetweenRegions = GetDistanceBetweenTiles(tileA, tileB);
            if (distanceBetweenRegions < bestDistance)
            {
                bestTileA = tileA;
                bestTileB = tileB;
            }
        }

        ConnectTiles(bestTileA, bestTileB);
    }

    Vector2 CoordToWorldPoint(Coord tile)
    {
        return new Vector2(-Width / 2 + .5f + tile.X, -Height / 2 + .5f + tile.Y);
    }

    class Region : IComparable<Region>
    {
        public List<Coord> tiles;
        public int roomSize;
        public CaveTile regionType;

        public Region()
        {
        }

        public Region(List<Coord> roomTiles, CaveTile[,] cave)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            regionType = cave[tiles[0].X, tiles[0].Y];
        }

        public int CompareTo(Region other)
        {
            return other.roomSize.CompareTo(roomSize);
        }

        public override bool Equals(object obj)
        {
            Region other = (Region)obj;
            return this.tiles == other.tiles;
        }

        public override int GetHashCode()
        {
            return tiles.GetHashCode();
        }
    }


    List<Coord> GetLine(Coord from, Coord to) // TODO rewrite
    {
        List<Coord> line = new List<Coord>();

        int x = (int) from.X;
        int y = (int) from.Y;

        int dx = (int) (to.X - from.X);
        int dy = (int) (to.Y - from.Y);

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord((uint) x, (uint) y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    void ConnectTiles(Coord tileA, Coord tileB)
    {        
        Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 10);

        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord c in line)
        {
            DrawCircle(c, PassageWidth, CaveTile.Air);
        }
    }

    private Tuple<Coord, Coord> FindClosestTilesBetweenRegions(Region regionA, Region regionB)
    {
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        double bestDistance = Width + Height;

        foreach (Coord tileA in regionA.tiles)
        {
            foreach (Coord tileB in regionB.tiles)
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
        return Math.Pow(tileA.X - tileB.X, 2) + Mathf.Pow(tileA.Y - tileB.Y, 2);
    }

    void DrawCircle(Coord c, uint r, CaveTile tileType)
    {
        for (uint x = r; x <= r; x++)
        {
            for (uint y = r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    uint drawX = c.X + x;
                    uint drawY = c.Y + y;
                    if (IsInMapRange(drawX, drawY))
                    {
                        cave[drawX, drawY] = tileType;
                    }
                }
            }
        }
    }

}

public enum CaveTile
{
    Wall, Air
}
