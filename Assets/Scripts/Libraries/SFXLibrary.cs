using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.Utilities;

[GlobalConfig("Assets/Resources/Global Config/")]
public class SFXLibrary :  SerializedGlobalConfig<SFXLibrary>
{
    public SFXBook AllBooks;
}

[System.Serializable]
public class SFXBook : UnitySerializedDictionary<SFX, AudioClip> { }


public enum SFX
{
    RGO_build = 0,
    RGO_LevelDown_1 = 1,
    RGO_LevelUp_2 = 2,
    RGO_LevelDown_2 = 3,
    RGO_LevelUp_3 = 4,
    
}