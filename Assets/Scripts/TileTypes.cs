using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileTypes : MonoBehaviour
{
    [Serializable]
    public class GroundTiles : TileData<GroundTileType>
    { }

    [Serializable]
    public class ObjectTiles : TileData<ObjectTileType>
    { }

    public abstract class TileData<T> : TileData
        where T : Enum
    {
        public T TileType;
        public override int TileTypeId { get { return Convert.ToInt32(TileType); } }
    }

    public abstract class TileData
    {
        public Sprite Sprite;
        public Color Color;
        public TileBase Tile;
        public Tile.ColliderType ColliderType;

        public bool overrideColor;
        public bool randomTransform;
        [ShowIf("randomTransform")]
        [AllowNesting]
        public Vector3 minPosition;
        [ShowIf("randomTransform")]
        [AllowNesting]
        public Vector3 maxPosition;
        [ShowIf("randomTransform")]
        [AllowNesting]
        public Vector3 minScale;
        [ShowIf("randomTransform")]
        [AllowNesting]
        public Vector3 maxScale;
        public virtual int TileTypeId { get; }
    }
}
