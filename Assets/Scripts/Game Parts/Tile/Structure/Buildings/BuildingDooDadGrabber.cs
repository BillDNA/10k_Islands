using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BuildingDooDadGrabber : StructureDooDadGrabber
{
    public override bool isRGO
    {
        get
        {
            return false;
        }
    }

    public override bool isBuilding
    {
        get
        {
            return true;
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
