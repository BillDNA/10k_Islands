using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[System.Serializable]
public class UIAnimationSlide<T> where T : Enum
{
    public float animationSpeed;
    
    [ShowInInspector, SerializeField]
    private AnimSet _positions;
    //[ShowInInspector, InlineEditor]
    public AnimSet positions
    {
        get
        {
            if (_positions == null)
            {
                CreateEmptyAnimationSet();
            }

            return _positions;
        }
    }

    [Button("Create Empty Animation Set"), ShowIf("@this._positions == null || this._positions.Count == 0")]
    private void CreateEmptyAnimationSet()
    {
        _positions = new AnimSet();
        foreach (T t in Enum.GetValues(typeof(T)))
        {
            _positions.Add(t,new UIAnimationAnchorSet(0,0,1,1));
        }
    }
    
    public void AddPosition(T name, UIAnimationAnchorSet anchorSet)
    {
        if (positions.ContainsKey(name))
        {
            positions[name] = anchorSet;
            return;
        }
        positions.Add(name,anchorSet);
    }

    public bool isAtPosition(T pos, RectTransform rectTransform)
    {
        if (!positions.ContainsKey(pos))
        {
            Debug.LogError($"[{this.GetType()}] can not find position {pos}.");
            return false;
        }
        UIAnimationAnchorSet dest = positions[pos];
        return dest.isAtPostion(rectTransform);
    }
    
    public bool UpdateRectToPosition(T pos, RectTransform rectTransform)
    {
        if (!positions.ContainsKey(pos))
        {
            Debug.LogError($"[{this.GetType()}] can not find position {pos}.");
            return false;
        }
        UIAnimationAnchorSet dest = positions[pos];
        Vector2 diff = new Vector2(
            dest.Min.x - rectTransform.anchorMin.x,
            dest.Min.y - rectTransform.anchorMin.y
        );
        
        if(diff.magnitude > 1) diff = diff.normalized;
        
        rectTransform.anchorMin = new Vector2(
            rectTransform.anchorMin.x + animationSpeed*diff.x,
            rectTransform.anchorMin.y + animationSpeed*diff.y
        );

        rectTransform.anchorMax = new Vector2(
            rectTransform.anchorMax.x + animationSpeed*diff.x,
            rectTransform.anchorMax.y + animationSpeed*diff.y
        );

        Vector2 diffAfter = new Vector2(
            dest.Min.x - rectTransform.anchorMin.x,
            dest.Min.y - rectTransform.anchorMin.y
        );
        bool xIsZero = Mathf.Approximately(diff.x, 0);
        bool yIsZero = Mathf.Approximately(diff.y, 0);
        bool isDone = false;
        if (xIsZero && !yIsZero)
        {
            isDone = (diff.y / diffAfter.y < 0);
        }
        else if(!xIsZero && yIsZero)
        {
            isDone = (diff.x / diffAfter.x < 0);
        }
        
        if (isDone || dest.isAtPostion(rectTransform))
        {
            rectTransform.anchorMin = dest.Min;
            rectTransform.anchorMax = dest.Max;
            return true;
        }
        return false;
    }
    [Button("Jump to position")]
    public void JumpToPosition(T pos, RectTransform rectTransform)
    {
        if (!positions.ContainsKey(pos))
        {
            Debug.LogError($"[{this.GetType()}] can not find position {pos}.");
            return;
        }
        UIAnimationAnchorSet dest = positions[pos];
        rectTransform.anchorMin = dest.Min;
        rectTransform.anchorMax = dest.Max;
    }

    [Button("Set Current Position")]
    public void SetPosition(T pos, RectTransform rectTransform)
    {
        if (!positions.ContainsKey(pos))
        {
            positions.Add(pos, new UIAnimationAnchorSet(rectTransform));
            return;
        }

        positions[pos] = new UIAnimationAnchorSet(rectTransform);
    }

    [System.Serializable]
    public class AnimSet : UnitySerializedDictionary<T, UIAnimationAnchorSet> { }
}
[Serializable]
public class UIAnimationAnchorSet
{
    public Vector2 Min;
    public Vector2 Max;

    public UIAnimationAnchorSet(float minX, float minY, float maxX, float maxY)
    {
        Min = new Vector2(minX, minY);
        Max = new Vector2(maxX, maxY);
    }
    public UIAnimationAnchorSet(RectTransform rectTransform)
    {
        Min = rectTransform.anchorMin;
        Max = rectTransform.anchorMax;
    }

    public bool isAtPostion(RectTransform rectTransform)
    {
        return Mathf.Approximately(Min.x, rectTransform.anchorMin.x) &&
               Mathf.Approximately(Min.y, rectTransform.anchorMin.y) &&
               Mathf.Approximately(Max.x, rectTransform.anchorMax.x) &&
               Mathf.Approximately(Max.y, rectTransform.anchorMax.y);
    }
}