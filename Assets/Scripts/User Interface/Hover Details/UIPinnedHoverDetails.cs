using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIPinnedHoverDetails : UIHoverDetails
{
    
    public override void Awake()
    {
        UserInterfaceManager.Instance.PinnedHoverDetails = this;
    }

    public static UIPinnedHoverDetails Instance
    {
        get
        {
            return UserInterfaceManager.Instance.PinnedHoverDetails;
        }
    }
    public override void Update()
    {
        base.Update();
    }

    public void UserInputPinActiveHover(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        _activeHoveringObjs = new List<IHoverable>();
        foreach (IHoverable h in UIHoverDetails.Instance.activeHoveringObjs)
        {
            _activeHoveringObjs.Add(h);
        }
        RecalculateActiveHover();
    }

    protected override void IndicateActiveHover(IHoverable obj)
    {
        obj.IndicateIsPinnedHover();
        
    }
}
