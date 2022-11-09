using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapHelper
{
    public static List<Vector2Int> GetTilesByType(TilemapStructure tilemap, IEnumerable<int> enumerable)
    {
        // Best to ToList() the IEnumerable because they will otherwise cause multiple enumerations.
        var tileTypes = enumerable.ToList();
        var validTilePositions = new List<Vector2Int>();
        for (int x = 0; x < tilemap.width; x++)
        {
            for (int y = 0; y < tilemap.height; y++)
            {
                var tileType = tilemap.GetTile(x, y);
                // Here we use Any to check if any of the tile types match the current tile
                if (tileTypes.Any(a => a == tileType))
                {
                    validTilePositions.Add(new Vector2Int(x, y));
                }
            }
        }
        return validTilePositions;
    }

    public static Vector2Int? FindClosestTileByType(TilemapStructure tilemap, Vector2Int startPos, int tileType)
    {
        float smallestDistance = float.MaxValue;
        Vector2Int? smallestDistancePosition = null;
        for (int x = 0; x < tilemap.width; x++)
        {
            for (int y = 0; y < tilemap.height; y++)
            {
                if (tilemap.GetTile(x, y) == tileType)
                {
                    // Here we check the distance between the start position and the current tile
                    float distance = ((startPos.x - x) * (startPos.x - x) + (startPos.y - y) * (startPos.y - y));
                    // If this distance is smaller, than the smallest one we have so far encountered
                    // Then let's update it
                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        smallestDistancePosition = new Vector2Int(x, y);
                    }
                }
            }
        }
        return smallestDistancePosition;
    }

    public static Vector3 RandomVector3(Vector3 min, Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }

    public static Vector3 RandomScaleVector3(Vector3 min, Vector3 max)
    {
        var val = (Random.Range(min.x, max.x));
        return new Vector3(val,val, 1);
    }

    /// <summary>
    /// Get the transform components for a tile. Convenience Function.
    /// </summary>
    /// <param name="map">Tilemap</param>
    /// <param name="position">position on map</param>
    /// <param name="tPosition">transform's position placed here</param>
    /// <param name="tRotation">transform's rotation placed here</param>
    /// <param name="tScale">transform's scale placed here</param>
    /// <remarks>Handy for tweening the transform (pos,scale,rot) of a tile</remarks>
    /// <remarks>No checking for whether or not a tile exists at that position</remarks>
    public static void GetTransformComponents(Tilemap map,
        Vector3Int position,
        out Vector3 tPosition,
        out Vector3 tRotation,
        out Vector3 tScale)
    {
        var transform = map.GetTransformMatrix(position);
        tPosition = transform.GetPosition();
        tRotation = transform.rotation.eulerAngles;
        tScale = transform.lossyScale;
    }


    /// <summary>
    /// Get the rotation of a tile's sprite.
    /// </summary>
    /// <param name="map">The tilemap</param>
    /// <param name="position">the position of the tile</param>
    /// <returns>the rotation of the sprite</returns>
    public static Vector3 GetTransformRotation(Tilemap map, Vector3Int position)
    {
        return map.GetTransformMatrix(position).rotation.eulerAngles;
    }

    /// <summary>
    /// Get the position of a tile's sprite
    /// </summary>
    /// <param name="map">the tilemap</param>
    /// <param name="position">position of tile</param>
    /// <returns>the position of the sprite</returns>
    public static Vector3 GetTransformPosition(Tilemap map, Vector3Int position)
    {
        return map.GetTransformMatrix(position).GetPosition();
    }

    /// <summary>
    /// Get the scale of a tile's sprite
    /// </summary>
    /// <param name="map">the tilemap</param>
    /// <param name="position">position of tile</param>
    /// <returns>the scale of the sprite</returns>
    public static Vector3 GetTransformScale(Tilemap map, Vector3Int position)
    {
        return map.GetTransformMatrix(position).lossyScale;
    }


    /// <summary>
    /// Set the transform for a tile. Convenience function.
    /// </summary>
    /// <param name="map">tilemap</param>
    /// <param name="position">position on map</param>
    /// <param name="tPosition">position for the tile transform</param>
    /// <param name="tRotation">rotation for the tile transform</param>
    /// <param name="tScale">scale for the tile transform</param>
    /// <remarks>Handy for tweening the transform (pos,scale,rot) of a tile's sprite</remarks>
    /// <remarks>No checking for whether or not a tile exists at that position</remarks>
    public static void SetTransform(Tilemap map,
        Vector3Int position,
        Vector3 tPosition,
        Vector3 tRotation,
        Vector3 tScale)
    {
        map.SetTransformMatrix(position, Matrix4x4.TRS(tPosition, Quaternion.Euler(tRotation), tScale));
    }

    public static void SetColor(Tilemap map,
    Vector3Int position,
    Color color)
    {
        map.SetColor(position, color);
    }

}
