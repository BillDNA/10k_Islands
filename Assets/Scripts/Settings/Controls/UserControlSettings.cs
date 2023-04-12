using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Utilities;
using UnityEngine.InputSystem;

[GlobalConfig("Assets/Resources/Settings/")]
public class UserControlSettings : SerializedGlobalConfig<UserControlSettings>
{
    [BoxGroup("Camera")]
    public float panSpeed;
    [BoxGroup("Camera")]
    public float zoomSpeed;

    [BoxGroup("Drag Controls")] public float clickRotationTime;
    [BoxGroup("Drag Controls")] public float verticalOffSet = 0f;
    [BoxGroup("Drag Controls")] public float verticalOffSetTime = 0.2f;


    public List<PlayerInput> InputActions;
}

public enum inputDevice
{
    mouse,
    keyboard
}
