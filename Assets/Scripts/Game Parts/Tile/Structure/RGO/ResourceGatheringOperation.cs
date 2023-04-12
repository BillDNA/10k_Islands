using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "RGO", menuName = "10kIslands/RGO", order = 2)]
public  class ResourceGatheringOperation : BaseScriptableObject
{
    [BoxGroup("Score")] public int score;
    
    [BoxGroup("Rules")] public TerrainType terrainUnder;
    
    [BoxGroup("Assets")]
    public Sprite sprite00;
    [BoxGroup("Assets")]
    public Sprite sprite01;
    [BoxGroup("Assets")]
    public Sprite sprite10;
    [BoxGroup("Assets")]
    public Sprite sprite11;

    private Sprite[] sprites
    {
        get
        {
            return new Sprite[] { sprite00, sprite01, sprite10, sprite11 };
        }
    }
    public Sprite GetSpriteFor(Hex h)
    {
        return sprites[h.qrIndex];
    }
    /*
    [FormerlySerializedAs("DoDads")] [SerializeField,HideInInspector]
    private DoDadData[] dooDads;

    [BoxGroup("DoDads"),Button("Initialize Dodads"),ShowIf("@this.DoDadsAreInitialized == false")]
    private void CreateBlankSetOfDoDads()
    {
        dooDads = new DoDadData[12];
    }
    
    
    private bool DoDadsAreInitialized
    {
        get
        {
            return dooDads != null && dooDads.Length == 12;
        }
    }
    [BoxGroup("DoDads")]
    [BoxGroup("DoDads/Level 1"), ShowInInspector,ShowIf("@this.DoDadsAreInitialized")]
    public DoDadData[] Level1DoDads
    {
        get
        {
            if (!DoDadsAreInitialized) return null;
            return new DoDadData[] { dooDads[0], dooDads[1], dooDads[2], dooDads[3] };
        }
        set
        {
            if (!DoDadsAreInitialized) return;
            if(value == null) return;
            if (value.Length > 0) dooDads[0] = value[0];
            if (value.Length > 1) dooDads[1] = value[1];
            if (value.Length > 2) dooDads[2] = value[2];
            if (value.Length > 3) dooDads[3] = value[3];
        }
    } 
    
    [BoxGroup("DoDads")]
    [BoxGroup("DoDads/Level 2"), ShowInInspector,ShowIf("@this.DoDadsAreInitialized")]
    public DoDadData[] Level2DoDads
    {
        get
        {
            if (!DoDadsAreInitialized) return null;
            return new DoDadData[] { dooDads[4], dooDads[5], dooDads[6], dooDads[7] };
        }
        set
        {
            if (!DoDadsAreInitialized) return;
            if(value == null) return;
            if (value.Length > 0) dooDads[4] = value[0];
            if (value.Length > 1) dooDads[5] = value[1];
            if (value.Length > 2) dooDads[6] = value[2];
            if (value.Length > 3) dooDads[7] = value[3];
        }
    } 

    
    [BoxGroup("DoDads")]
    [BoxGroup("DoDads/Level 3"), ShowInInspector,ShowIf("@this.DoDadsAreInitialized")]
    public DoDadData[] Level3DoDads
    {
        get
        {
            if (!DoDadsAreInitialized) return null;
            return new DoDadData[] { dooDads[8], dooDads[9], dooDads[10], dooDads[11] };
        }
        set
        {
            if (!DoDadsAreInitialized) return;
            if(value == null) return;
            if (value.Length > 0) dooDads[8] = value[0];
            if (value.Length > 1) dooDads[9] = value[1];
            if (value.Length > 2) dooDads[10] = value[2];
            if (value.Length > 3) dooDads[11] = value[3];
        }
    } 
*/

    public bool isValidForHex(TerrainHex h)
    {
        if (h == null) return false;
        return h.TerrainType == terrainUnder;
    }
/*
    public void PopulateDoDads(RGODooDadGrabber grabber) 
    {
        Debug.Log($"populating RGO {grabber != null} && {DoDadsAreInitialized}");
        if (grabber == null) return;
        if (!DoDadsAreInitialized) return;
        
        int i = 0;
        foreach (SpriteRenderer sr in grabber.DooDads)
        {
            if (i < dooDads.Length)
            {
                sr.sprite = dooDads[i].sprite;
                sr.transform.localPosition = dooDads[i].position;
            }

            i++;
        }
    }
    */
}
