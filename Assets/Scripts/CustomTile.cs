using UnityEngine;
using UnityEngine.Tilemaps;
using Venomaus.FlowVitae.Cells;

public class CustomTile : ICell<int>
{
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

    public int X { get; set; }
    public int Y { get; set; }
    public int CellType { get; set; }

    public CustomTile()
    { }

    public CustomTile(bool overrideColor, Color color, bool randomTransform, Vector3 minPosition, Vector3 maxPosition, Vector3 minScale, Vector3 maxScale)
    {
        this.overrideColor = overrideColor;
        this.color = color;
        this.randomTransform = randomTransform;
        this.minPosition = minPosition;
        this.maxPosition = maxPosition;
        this.minScale = minScale;
        this.maxScale = maxScale;
    }

    public bool Equals(ICell<int> other)
    {
        return other != null && other.X == X && other.Y == Y;
    }

    public bool Equals((int x, int y) other)
    {
        return other.x == X && other.y == Y;
    }
}
