using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Sirenix.OdinInspector;
public class TileHex : Hex, IHoverable
{
   
    #region Components
        
        [FoldoutGroup("Components")] public SpriteRenderer StructureRenderer;

        [FoldoutGroup("Components")] public TileHexBorderHelper BorderHelper;
    #endregion Componets

    #region Data
        
        [BoxGroup("Data")] public Tile tile;
        private bool _isCityTile;

        [BoxGroup("Data")]
        public bool isCityTile
        {
            get
            {
                return _isCityTile;
            }
            set
            {
                _isCityTile = value;
                CheckActiveStructure();
            }
        }

        [BoxGroup("Data"), ShowInInspector]
        public bool isPlaced
        {
            get
            {
                if (tile == null) return false;
                return tile.isPlaced;
            }
        }

        
        
        
        #region Draging Data
            public bool dragging {
                get {
                    if(tile == null) return false;
                    return tile.isDragging;
                }
                set
                {
                    if (value) tile.OnStartDrag(this);
                    else tile.OnEndDrag(this);
                    
                    CheckActiveStructure();
                }
            }
            private Vector2 mouseOffset;
            private float distance;
        #endregion Draging Data
    
    #endregion Data
    
    #region Life Cycle

        public void Update()
        {
            if (dragging)
            {
                UpdateHoverHex();
            }
        }

    #endregion Life Cycle

    #region Structure
        [Button("Check Active Structure")]
        public void CheckActiveStructure()
        {
            if (isCityTile) CalculateBuilding();
            else CalculateRGO();
        }

    #endregion Structure
    #region RGO

        private ResourceGatheringOperation _currentRGO;

        [BoxGroup("Score"),ShowInInspector]
        public ResourceGatheringOperation currentRGO
        {
            get
            {
                return _currentRGO;
            }
            set
            {
                if (_currentRGO == value) return;
                _currentRGO = value;
            }
        }
        public void CalculateRGO()
        {
            if (isCityTile) return;
            currentRGO = GamePartsLibrary.Instance.GetRGO(terrainHex);
            
            if (currentRGO == null)
            {
                StructureRenderer.sprite = dragging || !isPlaced ? TileSettings.Instance.RGODefault : TileSettings.Instance.EmptyRGO;
                return;
            }

            StructureRenderer.sprite = currentRGO.GetSpriteFor(this);

        }

    #endregion RGO
    #region Building

        [BoxGroup("Score")] public Building currentBuilding;

        public void CalculateBuilding()
        {
            if (!isCityTile) return;
            
            currentBuilding = GamePartsLibrary.Instance.GetBuilding(terrainHex);
            if (currentRGO == null)
            {
                StructureRenderer.sprite = dragging || tile.inTray ? TileSettings.Instance.BuildingDefault : TileSettings.Instance.EmptyBuilding;
                return;
            }

            StructureRenderer.sprite = currentBuilding.GetSpriteFor(this);

        }

        
    #endregion City
    #region Floating

        public void IndicateFloatingStatus(FloatingStatus status)
        {
            UpdateFloatingStatusColor(status);
            if (terrainHex != null) terrainHex.IndicateFloatingStatus(status);
            if (hoverHex != null) hoverHex.IndicateFloatingStatus(status);
            
        }
        private void UpdateFloatingStatusColor(FloatingStatus status)
        {
            switch (status)
            {
                case FloatingStatus.NotFloating:
                    UnderGlow.color = ColorLibrary.Instance.clear;
                    break;
                case FloatingStatus.Valid:
                    UnderGlow.color = ColorLibrary.Instance.TerrainHexFloatingValid;
                    break;
                case FloatingStatus.Blocked:
                    UnderGlow.color = ColorLibrary.Instance.TerrainHexFloatingBlocked;
                    break;
                case FloatingStatus.OtherBlocking:
                    UnderGlow.color = ColorLibrary.Instance.TerrainHexFloatingOtherBlocked;
                    break;
            }

        }

    #endregion Floating

    #region User Interaction

   
    
        #region IHoverable

            public int GetHoverPriority()
            {
                return 1;
            }

            public string GetDebugHoverText()
            {
                string t = (!isPlaced
                        ? (isCityTile ? "City" : "RGO")
                        : (isCityTile ? currentBuilding?.name : currentRGO?.name)
                    );
                return $"Tile Hex\n({loc.ToString()})\n{t}";
            }

            public void IndicateIsActiveHover()
            {
                this.UnderGlow.color = ColorLibrary.Instance.TerrainHexGlow;
                if(isPlaced) MoveToLayer(LayerType.PlaceTiledHover);
                if(tile.inTray) MoveToLayer(LayerType.TrayHover);
            }

            public void IndicateIsPinnedHover()
            {
                this.UnderGlow.color = ColorLibrary.Instance.TerrainHexPinnedGlow;
                
                if(isPlaced) MoveToLayer(LayerType.PlaceTiledHover);
                if(tile.inTray) MoveToLayer(LayerType.TrayHover);
            }

            public void IndicateHoverStopped()
            {
                this.UnderGlow.color = ColorLibrary.Instance.clear;
                
                if(isPlaced) MoveToLayer(LayerType.Tile);
                if(tile.inTray) MoveToLayer(LayerType.Tray);
            }

        #endregion IHoverable

        
        
        public void OnMouseEnter()
        {
            if (!UserInteractionEnabled) return;
            if(dragging) return;
            UIHoverDetails.Instance.ActivelyHoveringOver(this);
            
        }

        public void OnMouseExit()
        {
            if (!UserInteractionEnabled) return;
            if(dragging) return;
            UIHoverDetails.Instance.HoveringStopped(this);
        }

        public void OnMouseDown()
        {
            if (!UserInteractionEnabled) return;
            mouseOffset = new Vector2(transform.localPosition.x, transform.localPosition.y);
            distance = Vector3.Distance(transform.parent.transform.position, Camera.main.transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Vector3 rayPoint = ray.GetPoint(distance);
            dragging = true;
        }
        public virtual void OnMouseUp() {
            if (!UserInteractionEnabled) return;
            dragging = false;
        }
    
    #endregion User Interaction 
    
    #region Hex Under
    
        
        private TerrainHex _terrainHex;
        [BoxGroup("Hex Overlap Data"),ShowInInspector]
        public TerrainHex terrainHex
        {
            get
            {
                return _terrainHex;
            }
            set
            {
                if(_terrainHex == value) return;
                if (_terrainHex != null)
                {
                    _terrainHex.ClearTileHex();
                }
                _terrainHex = value;
                if (_terrainHex != null)
                {
                    _terrainHex.tileHex = this;
                }
            }
        }
            
        private TerrainHex _hoverHex;
        [BoxGroup("Hex Overlap Data"),ShowInInspector]
        public TerrainHex hoverHex
        {
            get
            {
                return _hoverHex;
            }
            set
            {
                if(_hoverHex == value) return;
                if (_hoverHex != null)
                {
                    _hoverHex.IndicateFloatingStatus(FloatingStatus.NotFloating);
                }
                _hoverHex = value;
            }
        }
        
        private bool _isOverTray;
        [BoxGroup("Hex Overlap Data"),ShowInInspector]
        public bool isOverTray {
            get {
                return _isOverTray;
            }
            protected set {
                _isOverTray = value;
            }
        }
       
        private void UpdateHoverHex() {
            float min = float.MaxValue;
            TerrainHex hex = null;
            foreach(Collider2D col in overlappingColliders) {
                float dist = Mathf.Abs(Vector2.Distance(transform.position,col.transform.position));
                if(dist < min) {
                    hex = col.gameObject.GetComponent<TerrainHex>();
                    min = dist;
                }
            }
            hoverHex = hex;
        } 
        public bool IsHexUnderEmpty(Vector3 diff)
        {
            TerrainHex t = FindHexUnder(diff);
            if (t == null) return false;
            if(t.TerrainType == TerrainType.Water) return false;
            return (t.tileHex == null);
        }
        public TerrainHex FindHexUnder(Vector3 diff) {
            float persicion = 0.25f;
            Vector3 aimPoint = transform.position - diff;
            foreach(Collider2D col in overlappingColliders) {
                Vector3 check = col.gameObject.transform.position;  
                if(
                    numberIsWithin(aimPoint.x, check.x, persicion) &&
                    numberIsWithin(aimPoint.y, check.y, persicion) &&
                    numberIsWithin(aimPoint.z, check.z, persicion) 
                ) {
                    TerrainHex bh = col.gameObject.GetComponent<TerrainHex>();
                    return bh;
                } 
            }
            return null;
        }

        #region pick up and place
        
            public void PickUpHex()
            {
                terrainHex = null;
                hoverHex = null;
                //clear colliders
                overlappingColliders.Clear();
            }
            public void PlaceHexOnIsland(Vector3 diff)
            {
                terrainHex = hoverHex;
                transform.position = terrainHex.transform.position;
                
            }
            public void PlaceHexInTray()
            {
                terrainHex = null;
            }
            
        #endregion pick up and place

        #region Colliders Managment

            public List<Collider2D> overlappingColliders = new List<Collider2D>();
            private void OnTriggerEnter2D(Collider2D collider)
            {
                if (collider.gameObject.GetComponent<Tray>() != null) isOverTray = true;
                if(collider.gameObject.GetComponent<TerrainHex>() == null) return;
                if(!overlappingColliders.Contains(collider)) {
                    overlappingColliders.Add(collider);
                }
                //HexUnder = GetBoardHexUnderMe();
            }
            private void OnTriggerExit2D(Collider2D collider) {
                if(overlappingColliders.Contains(collider)) {
                    overlappingColliders.Remove(collider);
                }
                if (collider.gameObject.GetComponent<Tray>() != null) isOverTray = false;
                //HexUnder = GetBoardHexUnderMe();
            }

        #endregion Colliders Managment
    #endregion Hex Under
    
    
    #region Neighbors

    public void UpdateBorder()
    {
        
        BorderHelper.East = (GetNeighbor(HexNeighbor.East) == null);
        BorderHelper.NorthEast = (GetNeighbor(HexNeighbor.NorthEast) == null);
        BorderHelper.NorthWest = (GetNeighbor(HexNeighbor.NorthWest) == null);
        BorderHelper.West = (GetNeighbor(HexNeighbor.West) == null);
        BorderHelper.SouthWest = (GetNeighbor(HexNeighbor.SouthWest) == null);
        BorderHelper.SouthEast = (GetNeighbor(HexNeighbor.SouthEast) == null);
        
        BorderHelper.color = ColorLibrary.Instance.TitleColors[tile.solution % ColorLibrary.Instance.TitleColors.Count];
        
    }
    [ShowInInspector]
    public List<TileHex> neighbors
    {
        get
        {
            var hn = EnumUtil.GetValues<HexNeighbor>();
            List<TileHex> r = new List<TileHex>();
            foreach (HexNeighbor h in hn)
            {
                TileHex t = GetNeighbor(h);
                if(t != null) r.Add(t);
            }
            return r;
        }
    }
    [ShowInInspector]
    public Dictionary<HexNeighbor,TileHex> DebugNeighbors
    {
        get
        {
            Dictionary<HexNeighbor,TileHex> r = new Dictionary<HexNeighbor, TileHex>();
            foreach (HexNeighbor h in EnumUtil.GetValues<HexNeighbor>())
            {
                r.Add(h,GetNeighbor(h));
            }
            return r;
        }
    }
    [ShowInInspector]
    public Dictionary<HexNeighbor,string> DebugNeighborsOffSetts
    {
        get
        {
            Dictionary<HexNeighbor,string> r = new Dictionary<HexNeighbor, string>();
            foreach (HexNeighbor h in EnumUtil.GetValues<HexNeighbor>())
            {
                r.Add(h,GetOffSet(h).ToString());
            }
            return r;
        }
    }
    public TileHex GetNeighbor(HexNeighbor n)
    {
        if (tile == null) return null;
        //translate neighbor based on borderHelpers euler angle
        HexNeighbor old = n;
        float z = transform.localEulerAngles.z;
        n = (HexNeighbor) ((((int) n - Mathf.RoundToInt(z / 60f)) + 6) % 6);
        HexLoc hn = GetOffSet(n);
        return tile.GetHex(hn.q, hn.r);
    }

    
    #endregion Neigbors

    #region Layer Managment

        protected override void SetSortingLayerName(string layerName)
        {
            base.SetSortingLayerName(layerName);
            StructureRenderer.sortingLayerName = layerName;
            StructureRenderer.sortingOrder = 0 + QRLayer;
        }
        

    #endregion Layer Managment
    
    #region Debug
    

        public void OnDrawGizmosSelected()
        {
            DrawGizmoRect(Color.red, BoundingBox);
            for(int i = 0; i < 6; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(GlobalHexVertexes[i], GlobalHexVertexes[(i + 1) % 6]);
            }
            /*
            if (hoverHex != null)
            {
                foreach (TerrainHex h in hoverHex.neighbors)
                {
                    h.DrawGizmosSquare(Color.blue);
                }
            }

            if (dragging)
            {
                Gizmos.color = Color.magenta;
                foreach (Collider2D t in overlappingColliders)
                {
                    Gizmos.DrawLine(transform.position,t.transform.position);
                }
                if(hoverHex != null) hoverHex.DrawGizmosSquare(Color.magenta);
            }
            */
        }

        public void ForceBuildBuilding(Building b)
        {
            currentBuilding = b;
            CheckActiveStructure();
        }
        public void ForceBuildRGO(ResourceGatheringOperation rgo)
        {
            currentRGO = rgo;
            CheckActiveStructure();
        }
    #endregion Debug 
}
