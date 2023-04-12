using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Tile : HexGrid<TileHex>, IDragable<TileHex>
{
    #region Components
        [FoldoutGroup("Components")]
        public GameObject rotationPointObj;
        [FoldoutGroup("Components")]
        public SortingGroup SortingGroup;

    #endregion Components
    #region Data
        private bool _inTray;

        [BoxGroup("Data"),ShowInInspector]
        public bool inTray
        {
            get
            {
                return _inTray;
            }
            protected set
            {
                _inTray = value;
                UpdateTitleColor();
            }
        }
        
        public int solution;

        private bool _isPlaced;
        [BoxGroup("Data"),ShowInInspector]
        public bool isPlaced
        {
            get
            {
                return _isPlaced;
            }
            protected set
            {
                _isPlaced = value;
                UpdateTitleColor();
            }
        }
    #endregion Data
    
    #region Life Cycle

        public void Update()
        {
            UpdateDrag();
            RGOAndBuildingUpdate();
            if(debugRotate) DebugUpdate();
        }

        public void LateUpdate()
        {
            if(_needRotationPointUpdate) UpdateRotationPoint();
        }

    #endregion Life Cycle

    #region RGO and Buildings

        public int score
        {
            get
            {
                if (!isPlaced) return 0;
                int s = 0;
                ForEachHex((int q, int r, TileHex h) =>
                {
                    if (h.currentRGO != null) s += h.currentRGO.score;
                    if (h.currentBuilding != null) s += h.currentBuilding.score;
                });
                return s;
            }
        }

        private bool needsRGOAndBuildingRecalulate = true;
        [Button("Force Recalculate RGO and Buildings")]
        public void ForceRecalcuateRGOAndBuildings()
        {
            needsRGOAndBuildingRecalulate = true;
        }
        private void RGOAndBuildingUpdate()
        {
            if (!needsRGOAndBuildingRecalulate) return;

            if (isPlaced)
            {
                needsRGOAndBuildingRecalulate = false;
                ForEachHex((int q, int r, TileHex h) =>
                {
                    h.CheckActiveStructure();
                    GameManager.Instance.needsScoreRecalculation = true;
                });

                return;
            }
            
            ForEachHex((int q, int r, TileHex h) =>
            {
                h.CheckActiveStructure();
            });
            needsRGOAndBuildingRecalulate = false;
        }
    
    #endregion RGO and Buildings
        
    #region Generation

        public override GameObject GetHexPrefab()
        {
            return PrefabLibrary.Instance.tileHex;
        }

        public override TileHex GenerateHex(int q, int r)
        {
            GameObject go = Instantiate(PrefabLibrary.Instance.tileHex, new Vector3(0,0,0) , Quaternion.identity);
            go.transform.localRotation = transform.localRotation;
            go.transform.parent = gameObject.transform;
            go.transform.localScale = new Vector2(1,1);
            TileHex h = go.GetComponent<TileHex>();
            h.q = q;
            h.r = r;
            Vector2 loc = findCenter(h);
            go.transform.localPosition =  loc;
            go.name = q+","+r;


            h.isCityTile = false;
            h.tile = this;
            return h;
        }
        

    #endregion Generation

    #region Tile Generations

        private bool _needRotationPointUpdate = false;
        public void GenerateFullTile(int w, int h)
        {
            RemoveAllChildrenTransforms(new List<Transform>(){rotationPointObj.transform});
            width = w;
            height = h;
            Generate();
            _needRotationPointUpdate = true;
        }
        [Button("Force Update Rotation Point")]
        public void UpdateRotationPoint()
        {
            rotationPointObj.transform.position = CenterMass;
            _needRotationPointUpdate = false;
        }

        public void BuildCityLoc(HexLoc loc = null)
        {
            if (loc == null) return;
            ForceBuildCity(loc.q,loc.r);
        }
        public TileHex cityHex;
        public void ForceBuildCity(int q, int r) {
            TileHex hex = GetHex(q,r);
            if(hex != null) {
                hex.isCityTile = true;
                cityHex = hex;
            }
        }
        
        protected override void Rotate(float angle)
        {
            base.Rotate(angle);
            UpdateBorders();
        }
        
        
        
        
        
        
        
        public void UpdateBorders()
        {
            ForEachHex((int q, int r, TileHex h) =>
            {
                h.UpdateBorder();
            });
        }

    #endregion Tile Generation 
    
    #region User Interaction

        public void UserInputRotate(InputAction.CallbackContext context)
        {
            Debug.Log($"HERE {name}");
            if(!isDragging)return;
            if(!context.performed) return;
               
            RotateCW();
        }    
    
        #region IDragable

            #region IDragable Getters

                [BoxGroup("Dragging"), ShowInInspector]
                public bool isDragging
                {
                    get
                    {
                        return _draggingObj != null;
                    }
                }
                private TileHex _draggingObj;
                [BoxGroup("Dragging"), ShowInInspector]
                public TileHex draggingObj
                {
                    get
                    {
                        return _draggingObj;
                    }
                    protected set
                    {
                        _draggingObj = value;
                    }
                }
                
                private float _draggingElapsedTime = 0;
                [BoxGroup("Dragging"), ShowInInspector]
                public float draggingElapsedTime {
                    get{
                        return _draggingElapsedTime;
                    }
                    set {
                        _draggingElapsedTime = value;
                    }
                }
                
                private Vector3 _pickUpScale;
                [BoxGroup("Dragging"),ShowInInspector]
                public Vector3 pickUpScale
                {
                    get
                    {
                        return _pickUpScale;
                    }
                    protected set
                    {
                        _pickUpScale = value;
                    }
                }
                
                private Vector3 _pickUpPosition;
                [BoxGroup("Dragging"),ShowInInspector]
                public Vector3 pickUpPosition
                {
                    get
                    {
                        return _pickUpPosition;
                    }
                    protected set
                    {
                        _pickUpPosition = value;
                    }
                }

                private float _draggingDistance;
                [BoxGroup("Dragging"),ShowInInspector]
                public float draggingDistance
                {
                    get
                    {
                        return _draggingDistance;
                    }
                    protected set
                    {
                        _draggingDistance = value;
                    }
                }

            #endregion IDragable Getters
            #region IDragable Events
                [ShowInInspector]
                private bool updateDrag = false;
                public void OnStartDrag(TileHex hex)
                {
                    updateDrag = true;
                    draggingObj = hex;
                    //Grab Pickup Data
                    pickUpScale = transform.localScale;
                    draggingElapsedTime = 0;
                    
                    //Calculate the distance from the camera to the rotation point
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    draggingDistance = Vector3.Distance(rotationPointObj.transform.position, Camera.main.transform.position);
                    pickUpPosition = transform.position;
                    
                    //Pickup the tile
                    if(inTray) Tray.Instance.PickUpTile(this);
                    
                    //Clear terrain hexes from the tile
                    ForEachHex((TileHex h) =>
                    { 
                        h.PickUpHex();
                        h.CheckActiveStructure();
                    });
                    
                }

                public void OnEndDrag(TileHex hex)
                {
                    //check if this hex is our dragging hex
                    if (hex != draggingObj) return;
                    updateDrag = false;
                    ClearFloatingStatus();
                    
                    //check if we are under withing the rotation time
                    if(draggingElapsedTime <= UserControlSettings.Instance.clickRotationTime)
                    {
                        RotateCW();
                        draggingObj = null;
                        Tray.Instance.DropTileOff(this);
                        return;
                    }
                    
                    //check if we are over the Island

                    CalculateIsCompletelyOverIsland();
                    if (isCompletelyOverIsland)
                    {
                        //If we are over the island, place the tile
                        Vector3 dif = hex.transform.position - hex.hoverHex.transform.position;
                        transform.position -= dif;
                        MoveToLayer(LayerType.Tile);
                        draggingObj = null;
                        ForEachHex((TileHex h) =>
                        {
                            h.PlaceHexOnIsland(dif);
                            h.CheckActiveStructure();
                        });
                        return;
                    }
                    
                    
                    //We didn't place the tile, so put it back in the tray
                    Tray.Instance.DropTileOff(this);
                    draggingObj = null;
                    
                    
                }

                public void UpdateDrag()
                {
                    //Don't do anything if we are not dragging
                    if (!updateDrag || !isDragging) return;
                    //Check to see if we are in the grow time
                    draggingElapsedTime += Time.deltaTime;
                    
                    //Figure out where the mouse is in world space
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    draggingDistance = Vector3.Distance(CenterMass, Camera.main.transform.position);
                    Vector3 newPos = ray.GetPoint(draggingDistance) - 
                                     (CenterMass - transform.position);
                    newPos.z = 0;
                    if (draggingElapsedTime <= IDragable<TileHex>.pickUpTime)
                    {
                        //Calculate the percentage of the grow time, max out at 1
                        float p = Mathf.Min(draggingElapsedTime / IDragable<TileHex>.pickUpTime,1);
                        //Calculate the scale from pickup to Island scale
                        transform.localScale = Vector3.Lerp(pickUpScale,GameManager.Instance.island.transform.localScale, p);
                        //Move position closer to the mouse
                        transform.position = Vector3.Lerp(pickUpPosition, newPos, p);
                    }
                    else
                    {
                        
                        transform.position = newPos;
                        
                        //Check to see if we are over a hex
                        CalculateIsCompletelyOverIsland();
                    }
                }


            #endregion IDragable Events

            #region IDragable Helpers
            
                private bool isOverTray
                {
                    get
                    {
                        //Check if any hexes are over the tray
                        foreach (TileHex hex in hexes)
                        {
                            if (hex != null)
                            {
                                if (hex.isOverTray) return true;
                            }
                        }
                        return false;
                    }
                }
                private bool _isCompletelyOverBoard;
                private int _lastCompletelyOverIslandCalcFrame = -1;
                [BoxGroup("Data/Hover"),ShowInInspector]
                public bool isCompletelyOverIsland {
                    get {
                        CalculateIsCompletelyOverIsland();       
                        return _isCompletelyOverBoard;
                    }
                }

                private void CalculateIsCompletelyOverIsland(bool debug = false)
                {
                    //No need to calculate if we already have
                    if (_lastCompletelyOverIslandCalcFrame == Time.frameCount) return;
                    _lastCompletelyOverIslandCalcFrame = Time.frameCount;
                    //Assume we are over the board
                    _isCompletelyOverBoard = true;
                    //Check if we have a dragging hex
                    if (draggingObj == null)
                    {
                        _isCompletelyOverBoard = false;
                        return;
                    }
                    //Check if we are over a hex
                    if (draggingObj.hoverHex == null)
                    {
                        _isCompletelyOverBoard = false;
                        return;
                    }
                    //Calculate the difference between the dragging hex and the hex we are over
                    Vector3 hexUnderPos = draggingObj.hoverHex.transform.position;
                    Vector3 draggingHexPos = draggingObj.transform.position;
                    Vector3 diff = draggingHexPos - hexUnderPos;
                    
                    //Create a list of floating statuses
                    int i = 0;
                    List<FloatingStatus> floatingStatuses = new List<FloatingStatus>();
                    //Loop through all the hexes and check if they are over the board
                    foreach (TileHex h in hexes)
                    {
                        if (h != null)
                        {
                            bool found = h.IsHexUnderEmpty(diff);
                            if (debug)
                            {
                                Debug.Log($"{h.name} - {found} - {(found ? h.hoverHex.name : "null")} vs {(h.terrainHex != null ? h.terrainHex.name : "null")}");
                            }
                            _isCompletelyOverBoard = _isCompletelyOverBoard && found;
                            if (found)
                            {
                                floatingStatuses.Add(FloatingStatus.Valid);
                            }
                            else
                            {
                                floatingStatuses.Add(FloatingStatus.Blocked);
                            }

                            i++;
                        }
                    }
                    //Adjust the floating statuses if we are not over the board
                    if (!_isCompletelyOverBoard)
                    {
                        floatingStatuses = floatingStatuses.Select(x =>
                            (x == FloatingStatus.Valid ? FloatingStatus.OtherBlocking : x)).ToList();
                    }
                    //Set the floating statuses
                    i = 0;
                    foreach(TileHex h in hexes) {
                        if(h != null)
                        {
                            if (updateDrag)
                            {
                                h.IndicateFloatingStatus(floatingStatuses[i]);
                            }
                            else
                            {
                                h.IndicateFloatingStatus(FloatingStatus.NotFloating);
                            }
                            i++;
                        }
                    }
                }

            #endregion IDragable Helpers
        #endregion IDragable
    #endregion User Interaction

    #region Title Structures

        public void UpdateStructures()
        {
            if (isPlaced) UpdateStructurePlaced();
            if (inTray) UpdateStructureTray();
            if (updateDrag) UpdateStructureDragging();
        }

        private void UpdateStructurePlaced()
        {
            
        }

        private void UpdateStructureDragging()
        {
            
        }

        private void UpdateStructureTray()
        {
            
        }

        public void UpdateTitleColor()
        {
            ForEachHex((TileHex h) =>
            {
                if (h != null)
                {
                    //h.TitleColor.color = isPlaced ? TileSettings.Instance.GetTitleColor(solution) : ColorLibrary.Instance.clear;
                }
            });
        }
        
        public void ClearFloatingStatus()
        {
            ForEachHex((TileHex h) =>
            {
                if (h != null)
                {
                    h.IndicateFloatingStatus(FloatingStatus.NotFloating);
                }
            });
        }
    
    #endregion Title Structures
    #region Layers


        public void MoveToLayer(LayerType layer)
        {
            ForEachHex((hex) =>
            {
                hex.MoveToLayer(layer);
                
            });

            transform.localPosition = new Vector3(localX,localY, -1);
            //Set local variables based on the layer
            switch (layer)
            {
                case LayerType.Tray:
                    inTray = true;
                    isPlaced = false;
                    break;
                case LayerType.Tile:
                    inTray = false;
                    isPlaced = true;
                    transform.parent = GameManager.Instance.island.transform;
                    break;
                case LayerType.Dragging:
                    inTray = false;
                    isPlaced = false;
                    break;
            }
        }

    #endregion Layers

    #region Connected Hexes

        public List<TerrainHex> GetConnectedTerrain()
        {
            if (cityHex == null) return new List<TerrainHex>();
            if(!isPlaced) return new List<TerrainHex>();
            List<TerrainHex> re = cityHex.terrainHex.neighbors;
            ForEachHex((int q, int r, TileHex h) =>
            {
                if (!re.Contains(h.terrainHex) && !h.isCityTile) //don't add this tiles city hex
                {
                    re.Add(h.terrainHex);
                }
            });
            return re;
        }
    #endregion Connected Hexes

    
    #region Debug

        public void OnDrawGizmosSelected()
        { 
            DrawGizmoRect(Color.yellow,Boundries);
            
            DrawGizmoCross(rotationPointObj.transform.position,Color.yellow,0.25f);
            DrawGizmoCross(CenterMass, Color.yellow, 0.25f);
            
            ForEachHex((h) =>
            {
                DrawGizmoLine(debugUseCenterMass ? CenterMass : rotationPoint, h.transform.position, Color.yellow);
            });

            DrawGizmoRect(Color.white,requestedWorld);
            DrawGizmoCross(new Vector3(requestedWorld.center.x,requestedWorld.center.y,0),Color.white,0.25f);
            DrawGizmoRect(Color.green,paddedWorld);
        }

        public bool debugRotate = false;
        public float debugRotation = 0.1f;
        public bool debugUseCenterMass = false;
        public void DebugUpdate()
        {
            Rotate(debugRotation);
        }
    #endregion Debug
}

public enum FloatingStatus
{
    NotFloating,
    Valid,
    Blocked,
    OtherBlocking
}