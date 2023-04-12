using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHexBorderHelper : BaseMonoBehaviour
{
    public Material material;
    
    public SpriteRenderer spriteRenderer;


    public bool East;
    public bool West;
    public bool SouthEast;
    public bool SouthWest;
    public bool NorthEast;
    public bool NorthWest;
    [Range(0,1)]
    public float borderSize = 0.1f;

    public Color color;
    public override void Start()
    {
        base.Start();
        
        spriteRenderer.material = Instantiate(material);
        
        
    }

    public override void Update()
    {
        base.Update();
        //Set sprite renderer material bools to match the bools in this script
        spriteRenderer.material.SetInt("_East", East ? 1 : 0);
        spriteRenderer.material.SetInt("_West", West ? 1 : 0);
        spriteRenderer.material.SetInt("_SouthEast", SouthEast ? 1 : 0);
        spriteRenderer.material.SetInt("_SouthWest", SouthWest ? 1 : 0);
        spriteRenderer.material.SetInt("_NorthEast", NorthEast ? 1 : 0);
        spriteRenderer.material.SetInt("_NorthWest", NorthWest ? 1 : 0);
        
        //Set the border size
        spriteRenderer.material.SetFloat("_Width", borderSize);
        
        //Set the color
        spriteRenderer.material.SetColor("_Color", color);
        
    }
}
