using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorldGen
{
    void Apply(TilemapStructure tilemap);
}
