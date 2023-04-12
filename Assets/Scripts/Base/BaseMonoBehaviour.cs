using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMonoBehaviour : MonoBehaviour
{
    #region Life Cycle

    public virtual void  Awake()
    {
    }
    public virtual void  Start()
    {
    }
    public virtual void  Update()
    {
    }

    #endregion
    #region Transform Helpers
        protected void RemoveAllChildrenTransforms(List<Transform> exceptions = null)
        {
            if (exceptions == null) exceptions = new List<Transform>();
            foreach (Transform child in transform) {
                if(!exceptions.Contains(child)) GameObject.Destroy(child.gameObject);
            }
        }

        public RectTransform rectTransform
        {
            get
            {
                return transform as RectTransform;
            }
        }
    #endregion Transform Helpers

    #region transform Getters

        public float localX
        {
            get
            {
                return transform.localPosition.x;
            }
        }
        public float localY
        {
            get
            {
                return transform.localPosition.y;
            }
        }
        public float globalX
        {
            get
            {
                return transform.position.x;
            }
        }
        public float globalY
        {
            get
            {
                return transform.position.y;
            }
        }
        
    #endregion transform Getters

    #region Random

        public List<int> Shuffle(List<int> list,int shullfes = 1) {
            for(int s = 0; s < shullfes; s++) {
                for(int i = list.Count-1; i >= 0; i--) {
                    int j = SeededRandom.GetInt(0,list.Count);
                    int temp = list[i];
                    list[i] = list[j];
                    list[j] = temp;
                }
            }
            return list;
        }
        
        public List<TerrainHex> Shuffle(List<TerrainHex> list,int shullfes = 1) {
            for(int s = 0; s < shullfes; s++) {
                for(int i = list.Count-1; i >= 0; i--) {
                    int j = SeededRandom.GetInt(0,list.Count);
                    TerrainHex temp = list[i];
                    list[i] = list[j];
                    list[j] = temp;
                }
            }
            return list;
        }

    #endregion Random

    #region Draw Helpers

        public void Fade(float a)
        {
            //_Fade(gameObject,a);
        }

        private void _Fade(GameObject go, float a)
        {
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, a);
            foreach (Transform c in go.transform)
            {
                _Fade(c.gameObject,a);   
            }
        }

    #endregion Draw Helpers
    
    #region Math Helpers

        public void CopyTransform(Transform newT, Transform oldT, bool deleteOld = true)
        {
            newT.parent = oldT.parent;
            newT.localPosition = oldT.localPosition;
            newT.localEulerAngles = oldT.localEulerAngles;
            newT.localScale = oldT.localScale;

            if (deleteOld)
            {
                Destroy(oldT.gameObject);
            }
        }
        public bool numberIsWithin(float v,float b, float r) {
            return b-r <= v && v <= b+r; 
        }

    #endregion Math Helpers

    #region Gizmo Helpers

        public void DrawGizmoCross(Color c, float size = 0.25f)
        {
            DrawGizmoCross(transform.position,c,size);
        }
        public void DrawGizmoCross(Vector3 p, Color c, float size = 0.25f)
        {
            Gizmos.color = c;
            Gizmos.DrawLine(
                new Vector3(p.x - size / 2f,p.y - size / 2f,p.z), 
                new Vector3(p.x + size / 2f,p.y + size / 2f,p.z));
            Gizmos.DrawLine(
                new Vector3(p.x - size / 2f,p.y + size / 2f,p.z), 
                new Vector3(p.x + size / 2f,p.y - size / 2f,p.z));
        }

        public void DrawGizmoLine(Vector3 a, Vector3 b, Color c)
        {
            Gizmos.color = c;
            Gizmos.DrawLine(a,b);
        }
        public void DrawGizmosCircle(Color c, float size = 0.25f)
        {
            DrawGizmosCircle(c,transform,size);
        }
        public void  DrawGizmosCircle(Color c, Transform t,float size = 0.25f)
        {
            Gizmos.color = c;
            GizmoHelpers.DrawGizmoDisk(t,c,size/2f);
        }
        
        public void DrawGizmosSquare(Color c, float size = 0.25f)
        {
            Rect r = new Rect(transform.position.x - size / 2f, transform.position.y - size / 2f, size, size);
            DrawGizmoRect(c,r);
        }
        public static void DrawGizmoRect(Color color, Rect r) {
            Debug.DrawLine(new Vector3(r.x,r.y,0),new Vector3(r.x+r.width,r.y,0),color,Time.deltaTime*1.1f);
            Debug.DrawLine(new Vector3(r.x+r.width,r.y,0),new Vector3(r.x+r.width,r.y+r.height,0),color,Time.deltaTime*1.1f);
            Debug.DrawLine(new Vector3(r.x+r.width,r.y+r.height,0),new Vector3(r.x,r.y+r.height,0),color,Time.deltaTime*1.1f);
            Debug.DrawLine(new Vector3(r.x,r.y+r.height,0),new Vector3(r.x,r.y,0),color,Time.deltaTime*1.1f);
        }
    #endregion Gizmo Helpers
}
