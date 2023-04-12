using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.Utilities;

[GlobalConfig("Assets/Resources/Global Config/")]
public class SortingOrderSettings : SerializedGlobalConfig<SortingOrderSettings>
{
    
}

public enum TileSortingOrderNames
{
    PlacedTile,
    FloatingTile,
    TrayTile,
}
