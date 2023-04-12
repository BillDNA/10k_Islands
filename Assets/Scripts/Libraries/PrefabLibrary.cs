using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Utilities;
using UnityEditor;

[GlobalConfig("Assets/Resources/Global Config/")]
public class PrefabLibrary : SerializedGlobalConfig<PrefabLibrary>
{
    public GameObject island;
    public GameObject terrainHex;

    public GameObject EmptyTile;
    public GameObject tileHex;

    public GameObject RGOPrefab;
    public GameObject BuildingPrefab;

    [MenuItem("10K islands/Libraries/Prefabs")]
    public static void SelectLibrary()
    {
        Selection.activeObject = Instance;
    }
}
