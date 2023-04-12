using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : BaseMonoBehaviour   
{
    public void PlaySFX(SFX sfx)
    {
        SFXManager.Instance.PlaySFX(sfx);
    }
}
