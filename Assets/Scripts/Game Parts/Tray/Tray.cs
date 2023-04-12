using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
public class Tray : BaseSingleton<Tray>
{
    #region Components

        [FoldoutGroup("Components")] 
        public BoxCollider2D DropOffBox;
    #endregion Companents

    #region Data

    
    [BoxGroup("Data")] public Tile[] tiles;

    [BoxGroup("Data")] public int slots;
    #endregion Data

    #region Life Cycle

        private void Awake()
        {
            tiles = new Tile[slots];
        }

    #endregion Life Cycle

    public void PickUpTile(Tile t)
    {
        int s = 0;
        foreach (Tile tt in tiles)
        {
            if (tt == t) break;
            s++;
        }
        if (s >= tiles.Length) return;
        tiles[s] = null;
        t.transform.parent = null;
    }

    public void DropTileOff(Tile t)
    {
        int s = 0;
        foreach (Tile tt in tiles)
        {
            if (tt == null) break;
            s++;
        }

        if (s >= tiles.Length) ExtendSlots();

        tiles[s] = t;
        
        t.transform.parent = transform;
        t.ResizeGridToFitSpace(GetRectForSlot(s));
        t.MoveToLayer(LayerType.Tray);
    }

    private void ExtendSlots(int v = 1)
    {
        slots += v;
        List<Tile> temp = tiles.ToList();
        tiles = new Tile[slots];
        foreach (Tile t in temp)
        {
            DropTileOff(t);
        }
    }

    public void Clear()
    {
        
    }
    private Rect GetRectForSlot(int s)
    {
        Rect r = new Rect(
            transform.position.x - DropOffBox.size.x / 2f,
            transform.position.y - DropOffBox.size.y / 2f,
            DropOffBox.size.x,
            DropOffBox.size.y
        );
        float size = r.width / slots;
        return new Rect(r.x + s * size, r.y, size, r.height);
    }

    #region Debug

    public void OnDrawGizmosSelected()
    {
        for (int i = 0; i < slots; i++)
        {
            Rect r = GetRectForSlot(i);
            DrawGizmoRect(Color.green,r);
            DrawGizmoCross(new Vector3(r.x, r.y, 0), Color.green);
        }
    }

    #endregion Debug
    
}
