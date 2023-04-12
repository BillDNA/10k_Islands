using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Rendering;

public class Hex : BaseMonoBehaviour, ILayers
{
    #region Hex Data
        [BoxGroup("Data")]
        [BoxGroup("Data/QR")]
        public int q;
        [BoxGroup("Data/QR")]
        public int r;
        public HexLoc loc {
            get {
                return new HexLoc(q,r);
            }
        }
        [BoxGroup("Data/QR"),ShowInInspector]
        public int qrIndex
        {
            get { return loc.qrIndex; }

        }
        
        
        public HexLoc GetOffSet(HexNeighbor n)
        {
            int parity = (r & 1);
            switch (n)
            {
                case HexNeighbor.East:
                    return new HexLoc(q +1 , r);
                case HexNeighbor.SouthEast:
                    return parity == 0 ? new HexLoc(q + 1 , r - 1) :  new HexLoc(q , r - 1);
                case HexNeighbor.SouthWest:
                    return parity == 0 ?  new HexLoc(q , r - 1) :  new HexLoc(q - 1, r - 1);
                case HexNeighbor.West:
                    return new HexLoc(q - 1, r);
                case HexNeighbor.NorthWest:
                    return parity == 0 ?  new HexLoc(q , r + 1) :  new HexLoc(q - 1, r + 1);
                case HexNeighbor.NorthEast:
                    return parity == 0 ?  new HexLoc(q + 1 , r + 1) :  new HexLoc(q , r + 1);
            }

            return new HexLoc(-100, -100);

        }
    #endregion Hex Data

    
    [FoldoutGroup("Components"), PropertyOrder(-40)]
    public TextMeshPro DebugText;
    [BoxGroup("Debug")]
    public bool UserInteractionEnabled = true;

    public override void Update()
    {
        base.Update();
        //DebugText.text = $"{QRLayer}\n--\n{ActiveLayers.x}\n{ActiveLayers.y}\n{ActiveLayers.z}";
    }

    public override void Start()
    {
        OverGlow.color = ColorLibrary.Instance.clear;
        UnderGlow.color = ColorLibrary.Instance.clear;
    }

    #region Draw Helpers
        
        [FoldoutGroup("Components"), PropertyOrder(-30)]
        public SpriteRenderer OverGlow;

        
        [FoldoutGroup("Components"), PropertyOrder(-10)]
        public SpriteRenderer UnderGlow;
    
        #region Layer Managment
            protected virtual void SetSortingLayerName(string layerName)
            {
                UnderGlow.sortingLayerName = layerName;
                OverGlow.sortingLayerName = layerName;

                UnderGlow.sortingOrder = -1000 + QRLayer;
                OverGlow.sortingOrder = 1000 + QRLayer;
            }
            //implement ILayers
            public virtual void MoveToLayer(LayerType layer)
            {
                //Set the layer to the string og the layer type
                SetSortingLayerName($"{layer}");
            }
            protected int QRLayer
            {
                get
                {
                    return Mathf.RoundToInt(-globalY*10);
                }
            }
        #endregion Layer Managment
        
        [FoldoutGroup("Components")] public GameObject offSetContainer;
        public virtual void FixImageRotation(float deltaAngle) {
            offSetContainer.transform.RotateAround(transform.position, Vector3.forward, deltaAngle);
        }
        
    #endregion Draw Helpers
    
    #region Collieder
    
        private PolygonCollider2D _collider;
        protected PolygonCollider2D collider
        {
            get
            {
                if (_collider == null) _collider = GetComponent<PolygonCollider2D>();
                return _collider;
            }
        }
        
        [BoxGroup("Hex Sizing"),ShowInInspector]
        protected Vector2[] HexVertexes
        {
            get
            {
                if (collider == null) return new Vector2[] { };

                return collider.points;
            }
        }

        public Vector2[] GlobalHexVertexes
        {
            get
            {
                return HexVertexes.Select(
                    (x) =>
                    {
                        Vector3 v = transform.TransformPoint(new Vector3(x.x, x.y, 0));
                        return new Vector2(v.x, v.y);
                    }
                ).ToArray();
            }
        }

        public Rect BoundingBox
        {
            get
            {
                return new Rect(
                    GlobalHexVertexes.Min(x => x.x), 
                    GlobalHexVertexes.Min(x => x.y), 
                    (GlobalHexVertexes.Max(x => x.x) - GlobalHexVertexes.Min(x => x.x)), 
                    (GlobalHexVertexes.Max(x => x.y) - GlobalHexVertexes.Min(x => x.y))
                    );
            }
        }
        
        #endregion Collieder

}

public enum HexNeighbor
{
    NorthEast,
    East,
    SouthEast,
    SouthWest,
    West,
    NorthWest
}
