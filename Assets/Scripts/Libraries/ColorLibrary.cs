using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Utilities;
using UnityEditor;

[GlobalConfig("Assets/Resources/Libraries/")]
public class ColorLibrary : SerializedGlobalConfig<ColorLibrary>
{
   [BoxGroup("Generic")] public Color clear;

   [BoxGroup("Titles")] public List<Color> TitleColors;
   
   [BoxGroup("Tile Hex")]
   public Color TileHexGlow;
   [BoxGroup("Tile Hex")]
   public Color TileHexPinnedGlow;

   
   [BoxGroup("Terrain Hex")]
   public Color TerrainHexGlow;
   [BoxGroup("Terrain Hex")]
   public Color TerrainHexPinnedGlow;

   [BoxGroup("Terrain Hex")]
   public Color TerrainHexFloatingValid;
   [BoxGroup("Terrain Hex")]
   public Color TerrainHexFloatingBlocked;
   [BoxGroup("Terrain Hex")]
   public Color TerrainHexFloatingOtherBlocked;
   
   
   [MenuItem("10K islands/Libraries/Color Library")]
   public static void SelectLibrary()
   {
      Selection.activeObject = Instance;
   }
}
