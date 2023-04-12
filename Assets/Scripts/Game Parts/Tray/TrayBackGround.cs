using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayBackGround : BaseMonoBehaviour, IHoverable
{
    #region IHoverable
    
        public int GetHoverPriority()
        {
            return 10000;
        }

        public void OnMouseEnter()
        {
            UIHoverDetails.Instance.ActivelyHoveringOver(this);
        }

        public void OnMouseExit()
        {
            UIHoverDetails.Instance.HoveringStopped(this);
        }

        public string GetDebugHoverText()
        {
            return $"Tray";
        }

        public void IndicateIsActiveHover()
        {
            
        }

        public void IndicateIsPinnedHover()
        {
            
        }

        public void IndicateHoverStopped()
        {
            
        }
    
    #endregion IHoverable
}
