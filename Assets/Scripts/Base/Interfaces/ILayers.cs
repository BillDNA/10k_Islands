using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public interface ILayers
{
    public void MoveToLayer(LayerType layer);
    
    
}

public enum LayerType
{
    Terrain,
    Tile,
    TerrainHover,
    PlaceTiledHover,
    Tray,
    TrayHover,
    Dragging
}