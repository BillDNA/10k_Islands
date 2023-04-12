using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "island", menuName = "10kIslands/island/GenerationSettings", order = 1)]
public class IslandGenerationSettings : BaseScriptableObject
{
    [BoxGroup("Size")] public int width;
    [BoxGroup("Size")] public int height;
    
    [BoxGroup("Tiles")] 
    public List<int> tileSizes;

    [BoxGroup("Tiles")] public bool rotationOn;
    [BoxGroup("Tiles")] public int minSizeForCity;
    [BoxGroup("Tiles")] public int minTileSizeToSplit;
    [BoxGroup("Tiles")] public int minTileSize;
    [BoxGroup("Tiles"),Range(0,1)] public float blockyness; //1 full block, 0 void 
    
}
