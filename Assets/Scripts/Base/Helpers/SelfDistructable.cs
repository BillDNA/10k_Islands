using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDistructable : BaseMonoBehaviour
{
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
