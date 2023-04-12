using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TerrainHex : Hex,IHoverable
{
    #region Components

        [FoldoutGroup("Components"), ShowInInspector]
        public SpriteRenderer terrainSpriteRenderer;

        
    #endregion Componets

    #region Data

        private TerrainType _terrainType;
        [BoxGroup("Data"),ShowInInspector]
        public TerrainType TerrainType
        {
            get { return _terrainType; }
            set
            {
                _terrainType = value;
                if (terrainSpriteRenderer != null)
                    terrainSpriteRenderer.sprite = TerrainSettings.Instance.GetSpriteForHex(this);
                
            }
        }
        
        private Island _island;
        [BoxGroup("Data"),ShowInInspector]
        public Island island
        {
            get
            {
                if (_island == null && Application.isPlaying)
                {
                    _island = transform.parent.GetComponent<Island>();
                }

                return _island;
            }
            set
            {
                _island = value;
            }
        }

        private TileHex _tileHex;
        
        [BoxGroup("Hex Overlap Data"),ShowInInspector]
        public TileHex tileHex
        {
            get
            {
                return _tileHex;
            }
            set
            {
                if (_tileHex != null)
                {
                    _tileHex.terrainHex = null; //remove old reference
                }
                _tileHex = value;
                if(_tileHex != null)
                {
                    _tileHex.terrainHex = this; //add new reference
                }
            }
        }
        public void ClearTileHex()
        {
            _tileHex = null;
        }
        
        private TileHex _hoverHex;
        [BoxGroup("Hex Overlap Data"),ShowInInspector]
        public TileHex hoverHex
        {
            get
            {
                return _hoverHex;
            }
            set
            {
                if (_hoverHex != null)
                {
                    _hoverHex.terrainHex = null; //remove old reference
                } 
                _hoverHex = value;
                if (_hoverHex != null)
                {
                    _hoverHex.terrainHex = this; //add new reference
                } 
            }
        }
        
    #endregion Data

    #region Life Cycle

        public override void Update()
        {
            base.Update();
            
        }

    #endregion Life Cycle
    #region Animations
        [BoxGroup("Animation")]
        public AnimationTriggerDelay buildTrigger;
    #endregion Animations
    #region User Interaction
        #region Floating
            public void IndicateFloatingStatus(FloatingStatus status)
            {
                switch (status)
                {
                    case FloatingStatus.NotFloating:
                        OverGlow.color = ColorLibrary.Instance.clear;
                        break;
                    case FloatingStatus.Valid:
                        OverGlow.color = ColorLibrary.Instance.TerrainHexFloatingValid;
                        break;
                    case FloatingStatus.Blocked:
                        OverGlow.color = ColorLibrary.Instance.TerrainHexFloatingBlocked;
                        break;
                    case FloatingStatus.OtherBlocking:
                        OverGlow.color = ColorLibrary.Instance.TerrainHexFloatingOtherBlocked;
                        break;
                }
            }

        #endregion Floating
        #region IHoverable
            public int GetHoverPriority()
            {
                return 0;
            }

            public string GetDebugHoverText()
            {
                return $"Terrain Hex\n({loc.ToString()})\n{TerrainType}";
            }

            public void IndicateIsActiveHover()
            {
                this.UnderGlow.color = ColorLibrary.Instance.TerrainHexGlow;
                MoveToLayer(LayerType.TerrainHover);
            }

            public void IndicateIsPinnedHover()
            {
                this.UnderGlow.color = ColorLibrary.Instance.TerrainHexPinnedGlow;
            }

            public void IndicateHoverStopped()
            {
                this.UnderGlow.color = ColorLibrary.Instance.clear;
                MoveToLayer(LayerType.Terrain);
            }

        
        #endregion IHoverable
        public void OnMouseEnter()
        {
            if(!UserInteractionEnabled) return;
            UIHoverDetails.Instance.ActivelyHoveringOver(this);
            
        }

        public void OnMouseExit()
        {
            if(!UserInteractionEnabled) return;
            UIHoverDetails.Instance.HoveringStopped(this);
        }

    #endregion User Interaction
    
    #region Layer Managment

        protected override void SetSortingLayerName(string layerName)
        {
            base.SetSortingLayerName(layerName);
            terrainSpriteRenderer.sortingLayerName = layerName;
            terrainSpriteRenderer.sortingOrder = 0 + QRLayer;
        }
            

    #endregion Layer Managment
    
    #region Neighbors
        [BoxGroup("Neighbors"),ShowInInspector]
        public List<TerrainHex> neighbors
        {
            get
            {
                var hn = EnumUtil.GetValues<HexNeighbor>();
                List<TerrainHex> r = new List<TerrainHex>();
                foreach (HexNeighbor h in hn)
                {
                    TerrainHex t = GetNeighbor(h);
                    if(t != null) r.Add(t);
                }
                return r;
            }
        }
            
        public TerrainHex GetNeighbor(HexNeighbor n)
        {
            if (island == null) return null;
            return island.GetHex(GetOffSet(n));
        }

        public int GetWaterCount()
        {
            var hn = EnumUtil.GetValues<HexNeighbor>();
            int r = 0;
            foreach (HexNeighbor h in hn)
            {
                TerrainHex t = GetNeighbor(h);
                if(t == null) r++;
            }
            return r;
        }
    
    #endregion Neigbors
    #region Debug

        public void OnDrawGizmosSelected()
        {
            DrawGizmoCross(Color.green);
            foreach (TerrainHex h in neighbors)
            {
                h.DrawGizmoCross(Color.blue);
            }
            DrawGizmoRect(Color.magenta, BoundingBox);
        }
        [BoxGroup("Debug")]
        [Button("Force Update Terrain")]
        public void ForceUpdateTerrainSprite()
        {
            terrainSpriteRenderer.sprite = TerrainSettings.Instance.GetSpriteForHex(this);
            
        }

    #endregion Debug 
}
