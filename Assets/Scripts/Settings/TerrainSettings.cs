using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEditor;

[GlobalConfig("Assets/Resources/Global Config/")]
public class TerrainSettings : SerializedGlobalConfig<TerrainSettings>
{
    [BoxGroup("Sizes")]
    public float HexSize = 0.5f;
    [BoxGroup("Sprites"),SerializeField,ShowIf("@this.validTerrainSprites")]
    public TerrainSpriteBook terrainSpriteBook;

    
    [BoxGroup("Biomes Generation"),ShowIf("@this.validTerrainChances")]
    public TerrainBiomesChances TerrainChances;

    [BoxGroup("Biomes Generation")]
    public float NeighborInfluenceToBiome;

    private bool validTerrainChances
    {
        get
        {
            if (TerrainChances == null) TerrainChances = new TerrainBiomesChances();
            if (TerrainChances.Count != Enum.GetValues(typeof(TerrainType)).Length)
            {
                foreach (TerrainType t in Enum.GetValues(typeof(TerrainType)))
                {
                    if(!TerrainChances.ContainsKey(t)) TerrainChances.Add(t, new BiomesData());
                }
            }
            return true;
        }
    }

    private bool validTerrainSprites
    {
        get
        {
            if (terrainSpriteBook == null) terrainSpriteBook = new TerrainSpriteBook();
            if (terrainSpriteBook.Count != Enum.GetValues(typeof(TerrainType)).Length)
            {
                foreach (TerrainType t in Enum.GetValues(typeof(TerrainType)))
                {
                    if(!terrainSpriteBook.ContainsKey(t)) terrainSpriteBook.Add(t, new TerrainSpriteSet());
                }
            }
            return true;
        }
    }

    public Sprite GetSpriteForHex(TerrainHex hex)
    {
        return (Sprite)terrainSpriteBook[hex.TerrainType][hex.qrIndex];
    }

    [MenuItem("10K islands/Libraries/Terrain Settings")]
    public static void SelectLibrary()
    {
        Selection.activeObject = Instance;
    }
}
[System.Serializable]
public class BiomesData
{
    [HideInInspector,SerializeField]
    private TerrainBiomesInfluence _influence;
    [ShowInInspector]
    public TerrainBiomesInfluence influence
    {
        get
        {
            if (_influence == null)
            {
                _influence = new TerrainBiomesInfluence();
                _influence.VerifyAllKeys();
            }
            return _influence;
        }
        set
        {
            _influence = value;
        }
    }
}

[System.Serializable]
public class TerrainSpriteBook : UnitySerializedDictionary<TerrainType,TerrainSpriteSet> { }
[System.Serializable]
public class TerrainSpriteSet
{
    [SerializeField] public List<Sprite> sprites;
    
    public object this[int i]
    {
        get
        {
            if (sprites == null) sprites = new List<Sprite>() { null, null, null, null };
            if(i < 0 || i >= sprites.Count) return null;
            return sprites[i];
        }
        set
        {
            if (sprites == null) sprites = new List<Sprite>() { null, null, null, null };
            if(i < 0 || i >= sprites.Count) return;
            if (value == null)
            {
                sprites[i] = null;
                return;
            }

            if (value.GetType() != typeof(Sprite)) return;
            
            sprites[i] = ((Sprite)value);
        }
    }
}
[System.Serializable]
public class TerrainBiomesChances : UnitySerializedDictionary<TerrainType, BiomesData> { }

[System.Serializable]
public class TerrainBiomesInfluence : UnitySerializedDictionary<TerrainType, int>
{
    public TerrainBiomesInfluence() : base ()
    {
        VerifyAllKeys();
    }

    public override void OnAfterDeserialize()
    {
        base.OnAfterDeserialize();
        VerifyAllKeys();
    }

    [Button("Verify All Keys"), ShowIf("@this.Count != Enum.GetValues(typeof(TerrainType)).Length")]
    public void VerifyAllKeys()
    {
        foreach(TerrainType t in Enum.GetValues(typeof(TerrainType))) {
            if(!this.ContainsKey(t)) this.Add(t,0);
        }
    }
    
}
public enum TerrainType
{
    Unset = 0,
    Plains = 1,
    Hills = 2,
    Mountains = 3,
    Forest = 4,
    Water = 5,
}