using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct CustomTile
{
    private TileBase tile;
    public TileBase Tile { get { return tile; } }

    private bool overrideColor;
    public bool OverrideColor { get { return overrideColor; } }

    private Color color;
    public Color Color { get { return color; } }

    private bool randomTransform;
    public bool RandomTransform { get { return randomTransform; } }

    private Vector3 minPosition;
    public Vector3 MinPosition { get { return minPosition; } }

    private Vector3 maxPosition;
    public Vector3 MaxPosition { get { return maxPosition; } }

    private Vector3 minScale;
    public Vector3 MinScale { get { return minScale; } }

    private Vector3 maxScale;
    public Vector3 MaxScale { get { return maxScale; } }


    public CustomTile(TileBase tile, bool overrideColor, Color color, bool randomTransform, Vector3 minPosition, Vector3 maxPosition, Vector3 minScale, Vector3 maxScale)
    {
        this.tile = tile;
        this.overrideColor = overrideColor;
        this.color = color;
        this.randomTransform = randomTransform;
        this.minPosition = minPosition;
        this.maxPosition = maxPosition;
        this.minScale = minScale;
        this.maxScale = maxScale;
    }
}
