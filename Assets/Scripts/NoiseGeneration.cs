using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Assets/Scripts/Algorithms/NoiseGeneration", menuName = "Algorithms/NoiseGeneration")]
public class NoiseGeneration : AlgorithmBase
{
    [Header("Noise settings")]
    // The more octaves, the longer generation will take
    public int Octaves;
    [Range(0, 1)]
    public float Persistance;
    public float Lacunarity;
    public float NoiseScale;
    public Vector2 Offset;
    public bool ApplyIslandGradient;

    [Serializable]
    class NoiseValues
    {
        [Range(0f, 1f)]
        public float Height;
        public GroundTileType GroundTile;
    }

    [SerializeField]
    private NoiseValues[] TileTypes;

    public override void Apply(TilemapStructure tilemap)
    {
        // Make sure that TileTypes are ordered from small to high height
        TileTypes = TileTypes.OrderBy(a => a.Height).ToArray();

        // Pass along our parameters to generate our noise
        var noiseMap = Noise.GenerateNoiseMap(tilemap.width, tilemap.height, tilemap.Grid.Seed, NoiseScale, Octaves, Persistance, Lacunarity, Offset);

        if (ApplyIslandGradient)
        {
            var islandGradient = Noise.GenerateIslandGradientMap(tilemap.width, tilemap.height);
            for (int x = 0, y; x < tilemap.width; x++)
            {
                for (y = 0; y < tilemap.height; y++)
                {
                    // Subtract the islandGradient value from the noiseMap value
                    float subtractedValue = noiseMap[y * tilemap.width + x] - islandGradient[y * tilemap.width + x];

                    // Apply it into the map, but make sure we clamp it between 0f and 1f
                    noiseMap[y * tilemap.width + x] = Mathf.Clamp01(subtractedValue);
                }
            }
        }

        for (int x = 0; x < tilemap.width; x++)
        {
            for (int y = 0; y < tilemap.height; y++)
            {
                // Get height at this position
                var height = noiseMap[y * tilemap.width + x];

                // Loop over our configured tile types
                for (int i = 0; i < TileTypes.Length; i++)
                {
                    // If the height is smaller or equal then use this tiletype
                    if (height <= TileTypes[i].Height)
                    {
                        tilemap.SetTile(x, y, (int)TileTypes[i].GroundTile, setDirty: false);
                        break;
                    }
                }
            }
        }
    }
}
