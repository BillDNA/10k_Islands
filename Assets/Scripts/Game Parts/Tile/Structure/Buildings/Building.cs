using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "10kIslands/Building", order = 1)]
public class Building : BaseScriptableObject
{
    [BoxGroup("Score")] public int score;
    [BoxGroup("Rules")] public TerrainType terrainUnder;
    [BoxGroup("Rules")] public ResourceGatheringOperation[] requiredRGOs;
    [BoxGroup("Rules")] public Building[] requiredBuildings;
    [BoxGroup("Rules")] public int requiredOcean;

    [BoxGroup("Assets")]
    public Sprite sprite00;
    [BoxGroup("Assets")]
    public Sprite sprite01;
    [BoxGroup("Assets")]
    public Sprite sprite10;
    [BoxGroup("Assets")]
    public Sprite sprite11;

    private Sprite[] sprites
    {
        get
        {
            return new Sprite[] { sprite00, sprite01, sprite10, sprite11 };
        }
    }
    public Sprite GetSpriteFor(Hex h)
    {
        return sprites[h.qrIndex];
    }

    private static bool debugValid = false;
    public bool isValidForHex(TerrainHex hex)
    {
        if (hex == null) return false;
        if(debugValid) Debug.Log($"Calculating Building Valid - {this.name}");
        //check terrain type
        if (terrainUnder != hex.TerrainType) return false;
        if(debugValid) Debug.Log($"Calculating Building Valid - {this.name} - valid terrain");
        //Check city
        TileHex over = hex.tileHex;
        if (!over.isCityTile) return false;
        if(debugValid) Debug.Log($"Calculating Building Valid - {this.name} - is city");
        //check ocean
        int water = hex.GetWaterCount();
        if (water < requiredOcean) return false;
        if(debugValid) Debug.Log($"Calculating Building Valid - {this.name} - valid ocean");
        //check rgo
        List<TerrainHex> connected = over.tile.GetConnectedTerrain();

        List<ResourceGatheringOperation> missingRGOs = requiredRGOs.ToList();
        List<Building> missingBuildings = requiredBuildings.ToList();
        
        foreach (TerrainHex h in connected)
        {
            if(h == null) continue;
            if (h.tileHex != null)
            {
                if (h.tileHex.isCityTile)
                {
                    if (missingBuildings.Contains(h.tileHex.currentBuilding))
                        missingBuildings.Remove(h.tileHex.currentBuilding);
                }
                else
                {
                    if (missingRGOs.Contains(h.tileHex.currentRGO)) missingRGOs.Remove(h.tileHex.currentRGO);
                }
            }
        }
        if(debugValid) Debug.Log($"Calculating Building Valid - {this.name} - missing rgo = {string.Join(", ",missingRGOs.Select(rgo => rgo.name).ToList())}, missing building = {string.Join(", ",missingBuildings.Select(b =>b.name).ToList())}");
        return missingRGOs.Count == 0 && missingBuildings.Count == 0;
    }

}
