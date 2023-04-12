using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.ExceptionServices;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using System.Linq;

public class UIHoverDetails : BaseMonoBehaviour
{
    #region Fields
        public TextMeshProUGUI DebugText;
    #endregion Fields

    #region Properties
        [ShowInInspector]
        public UIAnimationSlide<UIElementState> animations;
        public bool DebugEnabled = true;

        protected IHoverable currentDisplay;
    #endregion Properties

    #region Life Cycle
        [ShowInInspector]
        private bool wantsToBeOnScreen
        {
            get
            {
                return activeHoveringObjs.Count > 0;
            }
        }

        public override void Awake()
        {
            UserInterfaceManager.Instance.HoverDetails = this;
        }

        public static UIHoverDetails Instance
        {
            get
            {
                return UserInterfaceManager.Instance.HoverDetails;
            }
        }

        public override void Update()
        {
            if (wantsToBeOnScreen && !animations.isAtPosition(UIElementState.onScreen, rectTransform))
            {
                animations.UpdateRectToPosition(UIElementState.onScreen, rectTransform);
            } else if (!wantsToBeOnScreen && !animations.isAtPosition(UIElementState.offScreen, rectTransform))
            {
                animations.UpdateRectToPosition(UIElementState.offScreen, rectTransform);
            }
        }

    #endregion Life Cycle

    #region Hoverable Items


        protected List<IHoverable> _activeHoveringObjs;
        [ShowInInspector]
        public List<IHoverable> activeHoveringObjs
        {
            get
            {
                if (_activeHoveringObjs == null)
                {
                    _activeHoveringObjs = new List<IHoverable>();
                }

                return _activeHoveringObjs;
            }
        }

        public void ActivelyHoveringOver(IHoverable obj)
        {
            if (activeHoveringObjs.Contains(obj)) return;

            activeHoveringObjs.Add(obj);
            RecalculateActiveHover();
        }
        
        public void HoveringStopped(IHoverable obj)
        {
            obj.IndicateHoverStopped();
            if (!activeHoveringObjs.Remove(obj)) return;
            activeHoveringObjs.Remove(obj);
            RecalculateActiveHover();
        }

        protected void RecalculateActiveHover()
        {
            if (currentDisplay != null)
            {
                currentDisplay.IndicateHoverStopped();
            }
            if (activeHoveringObjs.Count == 0)
            {
                SetDebugText("");
                return;
            }
         
            IHoverable obj = activeHoveringObjs.OrderByDescending(i => i.GetHoverPriority()).First();
            currentDisplay = obj;
            Type t = obj.GetType();
            if (t == typeof(TerrainHex))
            {
                ActivelyHoveringOver((TerrainHex) obj);
            } else if (t == typeof(TileHex))
            {
                ActivelyHoveringOver((TileHex) obj);
            }
            
            IndicateActiveHover(obj);
        }

        protected virtual void IndicateActiveHover(IHoverable obj)
        {
            obj.IndicateIsActiveHover();
        }
        
        private void ActivelyHoveringOver(TerrainHex terrainHex)
        {
            SetDebugText(terrainHex.GetDebugHoverText());
        }
        private void ActivelyHoveringOver(TileHex tileHex)
        {
            SetDebugText(tileHex.GetDebugHoverText());
        }

    #endregion Hoverable Items

    #region Helpers

        private void SetDebugText(string val)
        {
            if (!DebugEnabled) return;
            DebugText.text = val;
        }

    #endregion  Helpers
}
