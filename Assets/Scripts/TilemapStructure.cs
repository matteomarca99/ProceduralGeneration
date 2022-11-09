using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Venomaus.FlowVitae.Grids;

public class TilemapStructure : MonoBehaviour
{
    [HideInInspector]
    public TileGrid Grid;

    public Grid<int, CustomTile> _flowGrid;

    [HideInInspector]
    public int width, height;

    private Tilemap _graphicMap;

    [SerializeField]
    private TilemapType _type;
    public TilemapType Type { get { return _type; } }

    [SerializeField]
    private AlgorithmBase[] _algorithms;

    private HashSet<Vector2Int> _dirtyCoords;
    /// <summary>
    /// True if changes are done in the structure, but have not yet been applied to the tilemap
    /// </summary>
    public bool IsDirty => _dirtyCoords.Count > 0;

    /// <summary>
    /// Method to initialize our tilemap.
    /// </summary>
    public void Initialize()
    {
        // Retrieve the Tilemap component from the same object this script is attached to
        _dirtyCoords = new HashSet<Vector2Int>();
        _graphicMap = GetComponent<Tilemap>();

        // Retrieve the TileGrid component from our parent gameObject
        Grid = transform.parent.GetComponent<TileGrid>();
        _flowGrid = new Grid<int, CustomTile>(Grid.width, Grid.height);

        // Get width and height from parent
        width = Grid.width;
        height = Grid.height;

        // Apply all the algorithms to the tilemap
        foreach (var algorithm in _algorithms)
        {
            Generate(algorithm);
        }

        // Update our data so it becomes visible on the map
        UpdateTiles();
    }

    /// <summary>
    /// Updates only a specific position of the structure.
    /// Use this if you only need to update one tile, it is more efficient
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void UpdateTile(int x, int y)
    {
        // Remove coordinate from dirty list
        var coord = new Vector2Int(x, y);
        if (_dirtyCoords.Contains(coord))
        {
            _dirtyCoords.Remove(coord);
        }

        var typeOfTile = GetTileType(x, y);
        // Get the ScriptableObject that matches this type and insert it
        Grid.GetTileCache().TryGetValue(typeOfTile, out TileBase tile); // Default return null if not found
        _graphicMap.SetTile(new Vector3Int(x, y, 0), tile);
        _graphicMap.RefreshTile(new Vector3Int(x, y, 0));
    }

    /// <summary>
    /// Updates only a select few positions of the structure.
    /// Use this if you only need to update a few tiles, it is more efficient
    /// </summary>
    public void UpdateTiles(Vector2Int[] positions)
    {
        var positionsArray = new Vector3Int[positions.Length];
        var tilesArray = new TileBase[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            var typeOfTile = GetTileType(positions[i].x, positions[i].y);
            // Get the ScriptableObject that matches this type and insert it
            Grid.GetTileCache().TryGetValue(typeOfTile, out TileBase tile); // Default return null if not found
            positionsArray[i] = new Vector3Int(positions[i].x, positions[i].y, 0);
            tilesArray[i] = tile;

            // Remove coordinate from dirty list
            if (_dirtyCoords.Contains(positions[i]))
            {
                _dirtyCoords.Remove(positions[i]);
            }
        }
        _graphicMap.SetTiles(positionsArray, tilesArray);
        foreach (var position in positionsArray)
            _graphicMap.RefreshTile(position);
    }

    /// <summary>
    /// Updates the entire structure, preferably only use this on map initialization
    /// </summary>
    public void UpdateTiles()
    {
        // Create a positions array and tile array required by _graphicMap.SetTiles
        var positionsArray = new Vector3Int[width * height];
        var tilesArray = new TileBase[width * height];
        // Loop over all our tiles in our data structure
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                positionsArray[x * width + y] = new Vector3Int(x, y, 0);
                // Get what tile is at this position
                var typeOfTile = GetTileType(x, y);
                // Get the ScriptableObject that matches this type and insert it
                Grid.GetTileCache().TryGetValue(typeOfTile, out TileBase tile); // Default return null if not found
                tilesArray[x * width + y] = tile;

                // Custom logic
                AdjustTile(GetTile(x, y), positionsArray);
            }
        }

        // Clear all dirty coordinates
        _dirtyCoords.Clear();
        _graphicMap.SetTiles(positionsArray, tilesArray);
        _graphicMap.RefreshAllTiles();
    }

    private void AdjustTile(CustomTile tile, Vector3Int[] positionsArray)
    {
        if (tile.RandomTransform)
        {
            _graphicMap.SetTileFlags(positionsArray[tile.Y * width + tile.X], TileFlags.None);
            var randomPosition = TilemapHelper.RandomVector3(tile.MinPosition, tile.MaxPosition);
            var randomScale = TilemapHelper.RandomScaleVector3(tile.MinScale, tile.MaxScale);
            TilemapHelper.SetTransform(_graphicMap, positionsArray[tile.Y * width + tile.X], randomPosition, Vector3.zero, randomScale);
            _graphicMap.SetTileFlags(positionsArray[tile.Y * width + tile.X], TileFlags.LockTransform);

            // Update cell in underlying grid
            tile.CustomPosition = randomPosition;
            tile.CustomScale = randomScale;
            SetTile(tile, storeState: true);
        }
        if (tile.OverrideColor)
        {
            _graphicMap.SetTileFlags(positionsArray[tile.Y * width + tile.X], TileFlags.None);
            TilemapHelper.SetColor(_graphicMap, positionsArray[tile.Y * width + tile.X], tile.Color);
            _graphicMap.SetTileFlags(positionsArray[tile.Y * width + tile.X], TileFlags.LockColor);

            // Update cell in underlying grid
            tile.CustomColor = tile.Color;
            SetTile(tile, storeState: true);
        }
    }

    /// <summary>
    /// Returns all 8 neighbors (vertical, horizontal, diagonal)
    /// </summary>
    /// <param name="tileX"></param>
    /// <param name="tileY"></param>
    /// <returns></returns>
    public IEnumerable<CustomTile> GetNeighbors(int tileX, int tileY)
    {
        return _flowGrid.GetNeighbors(tileX, tileY, AdjacencyRule.EightWay);
    }

    /// <summary>
    /// Returns only the direct 4 neighbors (horizontal and vertical)
    /// </summary>
    /// <param name="tileX"></param>
    /// <param name="tileY"></param>
    /// <returns></returns>
    public IEnumerable<CustomTile> Get4Neighbors(int tileX, int tileY)
    {
        return _flowGrid.GetNeighbors(tileX, tileY, AdjacencyRule.FourWay);
    }

    /// <summary>
    /// Return type of tile, otherwise 0 if invalid position.
    /// </summary>
    public int GetTileType(int x, int y)
    {
        return _flowGrid.GetCellType(x, y);
    }

    /// <summary>
    /// Return underlying tile object
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public CustomTile GetTile(int x, int y)
    {
        return _flowGrid.GetCell(x, y);
    }

    /// <summary>
    /// Set type of tile at the given position.
    /// </summary>
    public void SetTile(int x, int y, int? value, bool updateTilemap = false, bool setDirty = true, bool storeState = false)
    {
        _flowGrid.SetCell(x, y, value ?? 0, storeState);
        if (setDirty)
            _dirtyCoords.Add(new Vector2Int(x, y));
        if (updateTilemap)
            UpdateTile(x, y);
    }

    public void SetTile(CustomTile tile, bool updateTilemap = false, bool setDirty = true, bool storeState = false)
    {
        _flowGrid.SetCell(tile, storeState);
        if (setDirty)
            _dirtyCoords.Add(new Vector2Int(tile.X, tile.Y));
        if (updateTilemap)
            UpdateTile(tile.X, tile.Y);
    }

    /// <summary>
    /// Applies the algorithm code to the tilemap structure
    /// </summary>
    /// <param name="algorithm"></param>
    public void Generate(AlgorithmBase algorithm)
    {
        algorithm.Apply(this);
    }
}


    /*public void RenderAllTiles()
    {
        // Create a positions array and tile array required by _graphicMap.SetTiles
        var positionsArray = new Vector3Int[width * height];
        var tilesArray = new TileBase[width * height];
        // Loop over all our tiles in our data structure
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                positionsArray[x * width + y] = new Vector3Int(x, y, 0);
                // Get what tile is at this position
                var typeOfTile = GetTile(x, y);
                // Get the ScriptableObject that matches this type and insert it
                if (!Grid.Tiles.TryGetValue(typeOfTile, out CustomTile tile))
                {
                    if (typeOfTile != 0)
                    {
                        Debug.LogError("Tile not defined for id: " + typeOfTile);
                    }

                    tilesArray[x * width + y] = null;
                    continue;
                }
                tilesArray[x * width + y] = tile.Tile;
                _graphicMap.SetTile(positionsArray[x * width + y], tilesArray[x * width + y]);

                if (tile.RandomTransform)
                {
                    _graphicMap.SetTileFlags(positionsArray[x * width + y], TileFlags.None);
                    TilemapHelper.SetTransform(_graphicMap, positionsArray[x * width + y], TilemapHelper.RandomVector3(tile.MinPosition, tile.MaxPosition), Vector3.zero, TilemapHelper.RandomScaleVector3(tile.MinScale, tile.MaxScale));
                    _graphicMap.SetTileFlags(positionsArray[x * width + y], TileFlags.LockTransform);
                }
                if (tile.OverrideColor)
                {
                    _graphicMap.SetTileFlags(positionsArray[x * width + y], TileFlags.None);
                    TilemapHelper.SetColor(_graphicMap, positionsArray[x * width + y], tile.Color);
                    _graphicMap.SetTileFlags(positionsArray[x * width + y], TileFlags.LockColor);
                }
            }
        }

        //_graphicMap.SetTiles(positionsArray, tilesArray);

        //_graphicMap.RefreshAllTiles();
    }
}*/
