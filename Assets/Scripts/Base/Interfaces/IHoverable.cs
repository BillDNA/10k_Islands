using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoverable
{
    public int GetHoverPriority();
    public void OnMouseEnter();
    public void OnMouseExit();
    
    public string GetDebugHoverText();

    public void IndicateIsActiveHover();
    public void IndicateIsPinnedHover();
    public void IndicateHoverStopped();
}
