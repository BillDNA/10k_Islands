using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

using Sirenix.Utilities;
using UnityEditor;
using UnityEngine.SocialPlatforms.Impl;

[GlobalConfig("Assets/Resources/Libraries/")]
public class GamePartsLibrary : SerializedGlobalConfig<GamePartsLibrary>
{
    [BoxGroup("RGO")]
    public ResourceGatheringOperation[] availableRGOs;

    [BoxGroup("Building")] public Building[] availableBuildings;
    
    
    public ResourceGatheringOperation GetRGO(TerrainHex h)
    {
        List<ResourceGatheringOperation> valid = availableRGOs.Where(rgo => rgo.isValidForHex(h))
            .OrderByDescending(rgo => rgo.score).ToList();
        
        valid = valid.Where(rgo => rgo.score == valid[0].score).ToList();
        return SeededRandom.GeElement(valid);
    }

    public Building GetBuilding(TerrainHex h)
    {
        List<Building> valid = availableBuildings.Where(rgo => rgo.isValidForHex(h))
            .OrderByDescending(rgo => rgo.score).ToList();
        
        valid = valid.Where(rgo => rgo.score == valid[0].score).ToList();
        return SeededRandom.GeElement(valid);
        
        
    }
    
    [MenuItem("10K islands/Libraries/Game Parts")]
    public static void SelectLibrary()
    {
        Selection.activeObject = Instance;
    }
}
