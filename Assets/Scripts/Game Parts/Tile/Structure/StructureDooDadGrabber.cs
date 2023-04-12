using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public abstract class StructureDooDadGrabber : BaseMonoBehaviour
{
    public SpriteRenderer FakeLand;
    public List<SpriteRenderer> DooDads;
    public TextMeshPro DebugText;
    public abstract void ClearDooDads();
    public abstract bool isRGO { get; }
    public abstract bool isBuilding { get; }
}
