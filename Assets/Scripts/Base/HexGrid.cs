using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public abstract class HexGrid<T> : BaseMonoBehaviour where T : Hex 
{
    #region abstract Methods

        public abstract GameObject GetHexPrefab();
        public abstract T GenerateHex(int q, int r);
    #endregion abstract Methods

    #region Data

        [BoxGroup("Data")]
        protected T[,] hexes;
        
        #region Derived Data
            private int framCountForBoundriesCalc;
            private Rect _boundries;
            [BoxGroup("Data/Derived"),ShowInInspector]
            public Rect Boundries {
                get {
                    if(framCountForBoundriesCalc == Time.frameCount) return _boundries;
                    Rect rect = new Rect(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);
                    ForEachHex((int q, int r, T hex) => {
                        if(hex != null) {
                            if(hex.BoundingBox.x < rect.x) rect.x = hex.BoundingBox.x;
                            if(hex.BoundingBox.y < rect.y) rect.y = hex.BoundingBox.y;
                            if(hex.BoundingBox.x + hex.BoundingBox.width > rect.width) rect.width = hex.BoundingBox.x + hex.BoundingBox.width;
                            if(hex.BoundingBox.y + hex.BoundingBox.height > rect.height) rect.height = hex.BoundingBox.y + hex.BoundingBox.height;
                            
                        }
                    });
                    //adjust rect for width and height
                    rect.width -= rect.x;
                    rect.height -= rect.y;
                    
                    _boundries = rect;
                    framCountForBoundriesCalc = Time.frameCount;
                    return rect;
                }
            }
            private int _hexCount;
            private bool reCalcHexCount = true;
            [BoxGroup("Data/Derived"),ShowInInspector]
            public int hexCount {
                get {
                    if(reCalcHexCount) {
                        int re = 0; 
                        ForEachHex((int q, int r, T hex) => {
                            if(hex != null) re++;
                        });
                        reCalcHexCount = false;
                        _hexCount = re;
                    }
                    return _hexCount;
                }
            }
            [BoxGroup("Data/Derived"),ShowInInspector]
            protected Vector3 rotationPoint {
                get {
                    return new Vector3(
                        Boundries.width/2 + Boundries.x,
                        Boundries.height/2 + Boundries.y,
                        0
                    );
                }
            }

            [BoxGroup("Data/Derived"),ShowInInspector]
            public Vector3 CenterMass {
                get {
                    return new Vector3(Boundries.center.x, Boundries.center.y, 0);
                }
            }

            public Vector2 CenterMassV2
            {
                get
                {
                    return new Vector2(CenterMass.x, CenterMass.y);
                }
            }

            [BoxGroup("Data/Derived"),ShowInInspector]
            public Vector3 ScaledCenterMass {
                get {
                    float x = 0;
                    float y = 0;
                    int h = 0;
                    ForEachHex((int q, int r, T hex) => {
                        if(hex != null) {
                            x += hex.transform.position.x * transform.localScale.x;
                            y += hex.transform.position.y * transform.localScale.y;
                            h ++;
                        }
                    });

                    if(h == 0) {
                        return new Vector3(0,0,0);
                    }
                    return new Vector3(x/h-transform.localPosition.x,y/h - transform.localPosition.y,0);
        
                }
            }
            [BoxGroup("Data/Derived"),ShowInInspector]
            public Vector2 UnitSize {
                get {
                    float w = 2 * hexSize;
                    float h = 1.73205080757f * hexSize;
                    return new Vector2(
                        w * (width),// * 0.75f + w,
                        (1+height) * h
                    );
                }
            }
            [BoxGroup("Data/Derived"),ShowInInspector]
            public Vector2 ScaledUnitSize {
                get {
                    Vector2 us = UnitSize;
                    return new Vector2(
                        us.x * transform.localScale.x,
                        us.y * transform.localScale.y
                    );
                }
            }
        #endregion Derived Data

        #region Settings
            [BoxGroup("Data")]
            [BoxGroup("Data/Settings")]
            public int width;
            [BoxGroup("Data/Settings")]
            public int height;
            [BoxGroup("Data/Settings")]
            public float hexSize {
                get
                {
                    return TerrainSettings.Instance.HexSize;
                }
            }


        #endregion Settings

    #endregion Data
    
    #region Getters
        public Vector2 findCenter(Hex hex) {
            
            float w = hex.BoundingBox.width;
            float h = hex.BoundingBox.height;
            return new Vector2(
                w * ( hex.q - 0.5f * (hex.r&1))+1.0f*w,
                h * (0.75f * hex.r) + 0.5f*h
            );
            
            /*
            float w = 1.732050807568877f * hexSize;
            float h = 2 * hexSize;
            return new Vector2(
                w * ( q - 0.5f * (r&1))+1.0f*w,
                h * (0.75f * r) + 0.5f*h
            );
            float size = hexSize;
            return  new Vector2(
                size * (3.0f / 2.0f * q),
                size * Mathf.Sqrt(3) * (r - 0.5f * (q&1)));*/
        }
        public T GrabRandomHex(List<HexLoc> forbiden= null) {
            T re = null;
            if(forbiden == null) forbiden = new List<HexLoc>();
            ForEachRandomOrderHex((int q, int r, T hex) => {
                if(hex != null && !forbiden.Contains(new HexLoc(q,r))) {
                    re = hex;
                }
            });
            return re;
        }
        public virtual T GetHex(int q, int r) {
            if(q < width && r < height && q >= 0 && r >=0) {
                return hexes[q,r];
            }
            return null;
        }
        public virtual T GetHex(HexLoc loc) {
            return GetHex(loc.q, loc.r);
        }
        public virtual void RemoveHex(int q, int r) {
            reCalcHexCount = true;
            if(GetHex(q,r) != null) {
                GameObject.DestroyImmediate(hexes[q,r].gameObject);
                hexes[q,r] = null;
            }
        }
        
        

    #endregion Getters
    #region Rotation

        [Button("Center Grid")]
        public void CenterGrid()
        {
            Vector3 diff = CenterMass - transform.position;
            if(diff.x is Single.NaN || diff.y is Single.NaN) return;
            ForEachHex(h =>
            {
                h.transform.position -= diff;
            });
        }
    
        public void centerRotationPoint() {
            
        }
    
        public void ResetOrientation() {
            while(transform.localEulerAngles.z != 0) {
                RotateCCW();
            }
        }
        [Button("Rotate CW")]
        public void RotateCW(bool animate = false){
            Rotate(60f);
        }
        [Button("Rotate CCW")]
        public void RotateCCW(bool animate = false) {
            Rotate(-60f);
        }

        protected virtual void Rotate(float angle)
        {
            transform.localEulerAngles = new Vector3(0,0,transform.localEulerAngles.z + angle);
            ForEachHex(h =>
            {
                h.transform.localEulerAngles = new Vector3(0,0,h.transform.localEulerAngles.z - angle);
            });
        }
        protected void FixImageRotation(float deltaAngle) {
            ForEachHex((int q, int r, T hex) =>{
                if(hex != null) {
                    hex.FixImageRotation(deltaAngle);
                }
            });
        }
    #endregion Rotation
    #region Generation

    public void Generate()
    {
        
        hexes = new T[width,height];
        for (int q = 0; q < width; q++)
        {
            for (int r = 0; r < height; r++)
            {
                hexes[q, r] = GenerateHex(q, r);
            }
        }
    }

    #endregion

    #region Loops
        //TODO: allow breaks from these loops
        public void ForEachHexByRow(Action<int,int,T> a ) {
            if(hexes == null) return;
            for(int r = height-1;  r >= 0; r--) {
                for(int q = 0; q < width; q++) {
                    if(hexes[q,r]!=null) a(q,r,hexes[q,r]);
                }
            }
        }
        public void ForEachHexByRow(Action<T> a ) {
            if(hexes == null) return;
            for(int r = height-1;  r >= 0; r--) {
                for(int q = 0; q < width; q++) {
                    if(hexes[q,r]!=null) a(hexes[q,r]);
                }
            }
        }
        public void ForEachHex(Action<int,int,T> a ) {
            if(hexes == null) return;
            for(int q = 0; q < width; q++) {
                for(int r = 0;  r < height; r++) {
                    if(hexes[q,r]!=null) a(q,r,hexes[q,r]);
                }
            }
        }
        public void ForEachHex(Action<T> a ) {
            if(hexes == null) return;
            for(int q = 0; q < width; q++) {
                for(int r = 0;  r < height; r++) {
                    if(hexes[q,r]!=null) a(hexes[q,r]);
                }
            }
        }

        public void ForEachRandomOrderHex(Action<T> a)
        {
            List<int> cols = Enumerable.Range(0, width).ToList<int>();
            cols = Shuffle(cols,2);
            List<int> rows = Enumerable.Range(0, height).ToList<int>();
            rows = Shuffle(rows,2);
        
            foreach(int q in cols) {
                foreach(int r in rows) {
                    if(hexes[q,r]!=null) a(hexes[q,r]);
                }
            }
        }

        public void ForEachRandomOrderHex(Action<int,int,T> a ) {
            List<int> cols = Enumerable.Range(0, width).ToList<int>();
            cols = Shuffle(cols,2);
            List<int> rows = Enumerable.Range(0, height).ToList<int>();
            rows = Shuffle(rows,2);
        
            foreach(int q in cols) {
                foreach(int r in rows) {
                    if(hexes[q,r]!=null) a(q,r,hexes[q,r]);
                }
            }
        }

    #endregion Loops

    #region Sizing

    [Button("Debug reSize")]
    public void DebugReSize()
    {
        ResizeGridToFitSpace(requestedWorld);
    }

        protected Rect requestedWorld;
        protected Rect paddedWorld;
        public void ResizeGridToFitSpace(Rect world,float padding = 0.05f) {
            if(world.width == 0 || world.height == 0) world = requestedWorld;
            requestedWorld = world;
            // Add padding around the world
            float paddingX = world.width * padding;
            float paddingY = world.height * padding;
            world = new Rect(
                world.x + paddingX,
                world.y + paddingY,
                world.width - paddingX * 2,
                world.height - paddingY * 2
            );

            Rect scaledBoundries = Boundries;
            scaledBoundries.width /= transform.localScale.x;
            scaledBoundries.height /= transform.localScale.x;

            float sx = world.width / scaledBoundries.width;
            float sy = world.height / scaledBoundries.height;
            
            float scale = Mathf.Min(sx,sy);

            // Scale to fit the world rect
            transform.localScale = new Vector2(scale, scale);

            paddedWorld = world;

            // Move the grid to the center of the world
            transform.position = new Vector3(
                world.center.x,
                world.center.y,
                transform.position.z
            );
            
            //wait a frame for the grid to recenter
            Invoke("CenterGrid",0.1f);
        }
        
        private int framCountForGridCalc;
        private Rect _grid;
        [BoxGroup("Debug Size"),ShowInInspector]
        public Rect GridSpace {
            get {
                if(framCountForGridCalc == Time.frameCount) return _grid;
                Rect rect = new Rect(0,0,0,0);
                float w = 1.732050807568877f * hexSize;
                float h = 2 * hexSize;
                rect.width = (width+0.5f) * w;
                rect.height = 0.75f * height * h + 0.25f * h;
                rect.x = transform.position.x;
                rect.y = transform.position.y;
                //rect.width *= transform.localScale.x;
                //rect.height *= transform.localScale.y;

                _grid =rect;
                framCountForGridCalc = Time.frameCount;
                return rect;
            }
        }
    
        [BoxGroup("Debug Size"),ShowInInspector]
        private Rect AdjustedSpace{
            get {
                Vector2Int min = new Vector2Int(width,height);
                Vector2Int max = new Vector2Int(0,0);
                ForEachHex((int q, int r, T hex) => {
                    if(hex != null) {
                        if(q < min.x) min.x = q;
                        if(q > max.x) max.x = q;
                        if(r < min.y) min.y = r;
                        if(r > max.y) max.y = r; 
                    }
                });
                int left = min.x;
                int top = (height-1) - max.y;
                int right = (width-1) - max.x;
                int bot = min.y;

                int actualWidth = width - right - left;
                int actualHeight = height - top - bot;
                Rect rect = new Rect(0,0,0,0);
                float w = 1.732050807568877f * hexSize;
                float h = 2 * hexSize;
                rect.width = ((actualWidth)+0.5f) * w;
                rect.height = 0.75f * actualHeight * h + 0.25f * h;

                rect.x = ((left)+ (left > 0 ? 0.5f : 0)) * w + transform.localScale.x;
                rect.y = bot * h - 0.25f * h + transform.localScale.y;

                rect.x = transform.position.x + ((width - actualWidth) * w) / 2f;
                rect.y = transform.position.y + ((height - actualHeight) * h * 0.75f + ((height - actualHeight) >= 1 ? 0.25f * h : 0)) / 2f;
                return rect;
            }
        
        }

    #endregion Sizing
    

   
}
public class HexLoc : IEquatable<HexLoc> {
    public int q;
    public int r;

    public HexLoc(Vector2Int v) {
        q = v.x;
        r = v.x;
    }
    public HexLoc(string v) {
        string[] sp = v.Split(new char[]{','});
        q = Int32.Parse(sp[0]);
        r = Int32.Parse(sp[1]);
    }
    public int qrIndex
    {
        get { return (q % 2 == 0 ? 0 : 1) + (r % 2 == 0 ? 0 : 1) * 2; }

    }
    public HexLoc(int q, int r) {
        this.q = q;
        this.r = r;
    }
    public bool Equals(HexLoc other) {
        return this.q == other.q && this.r == other.r;
    }
    public bool Equals(HexLoc vec1, HexLoc vec2) {
        return vec1.q == vec2.q && vec1.r == vec2.r;
    }

    public override string ToString() {
        return q+","+r;
    }

    public int GetHashCode (HexLoc vec){
        return Mathf.FloorToInt (vec.q) ^ Mathf.FloorToInt (vec.r) << 2;
    }
}