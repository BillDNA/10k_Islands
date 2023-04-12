using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Utilities;

[GlobalConfig("Assets/Resources/Global Config/")]
public class AnimationsSettings :  SerializedGlobalConfig<AnimationsSettings>
{
    public float rotationTime = 0.1f;
}
