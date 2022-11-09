using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Venomaus.FlowVitae.Cells;

public class CustomTile : ICell<int>
{
    // These values can be different per cell
    public Vector3 CustomPosition { get; set; }
    public Vector3 CustomScale { get; set; }
    public Color CustomColor { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    public int CellType { get; set; }

    public CustomTile()
    { }

    public bool Equals(ICell<int> other)
    {
        return other != null && other.X == X && other.Y == Y;
    }

    public bool Equals((int x, int y) other)
    {
        return other.x == X && other.y == Y;
    }
}
