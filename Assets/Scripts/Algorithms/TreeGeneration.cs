using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Assets/Scripts/Algorithms/TreeGeneration", menuName = "Algorithms/TreeGeneration")]
public class TreeGeneration : AlgorithmBase
{
    [SerializeField]
    private TreeConfiguration[] TreeSelection;

    [Serializable]
    class TreeConfiguration
    {
        public ObjectTileType Tree;
        public GroundTileType[] SpawnOnGrounds;
        [Range(0, 100)]
        public int SpawnChancePerCell;
    }
    public override void Apply(TilemapStructure tilemap)
    {
        var groundTilemap = tilemap.Grid.GetTilemap(TilemapType.Ground);
        var random = new System.Random(tilemap.Grid.Seed);
        for (int x = 0; x < tilemap.width; x++)
        {
            for (int y = 0; y < tilemap.height; y++)
            {
                foreach (var tree in TreeSelection)
                {
                    var groundTile = groundTilemap.GetTileType(x, y);
                    if (tree.SpawnOnGrounds.Any(tile => (int)tile == groundTile))
                    {
                        // Do a random chance check
                        if (random.Next(0, 100) <= tree.SpawnChancePerCell)
                        {
                            tilemap.SetTile(x, y, (int)tree.Tree, setDirty: false);
                        }

                        // We don't break here, because other tree selections can still overrule this one.
                    }
                }
            }
        }
    }
}
