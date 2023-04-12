using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RGODooDadGrabber : StructureDooDadGrabber
{
    

    public override bool isRGO
    {
        get
        {
            return true;
        }
    }

    public override bool isBuilding
    {
        get
        {
            return false;
        }
    }
    
    public override void ClearDooDads()
    {
        foreach (SpriteRenderer sr in DooDads)
        {
            sr.sprite = null;
        }
    }
}
