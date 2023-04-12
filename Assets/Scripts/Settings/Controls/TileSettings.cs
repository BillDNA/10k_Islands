using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Utilities;
using UnityEditor;

[GlobalConfig("Assets/Resources/Global Config/")]
public class TileSettings :  SerializedGlobalConfig<TileSettings>
{
    [BoxGroup("Drag Settings")]
    public Vector2 pickUpScale;

    [BoxGroup("Drag Settings")] 
    public float pickUpTime;

    [BoxGroup("Assets")] public Sprite RGODefault;
    [BoxGroup("Assets")] public Sprite BuildingDefault;
    
    [BoxGroup("Assets")] public Sprite EmptyRGO;
    [BoxGroup("Assets")] public Sprite EmptyBuilding;

    
    
    [MenuItem("10K islands/Libraries/Tile Settings")]
    public static void SelectLibrary()
    {
        Selection.activeObject = Instance;
    }

    public Color GetTitleColor(int solution)
    {
        return ColorLibrary.Instance.TitleColors[solution % ColorLibrary.Instance.TitleColors.Count];
    }
}
