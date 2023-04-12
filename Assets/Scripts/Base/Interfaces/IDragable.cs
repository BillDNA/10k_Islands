using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for all dragable objects
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IDragable<T> where T : ILayers
{
    //Settings
    public const float pickUpTime = 0.5f;
    
    public bool isDragging { get; }
    public T draggingObj { get; }
    //Pick up Data
    public Vector3 pickUpScale { get; }
    public Vector3 pickUpPosition { get; }
    //Dragging Data
    public float draggingElapsedTime { get; }
    public float draggingDistance { get; }
    
    //Events
    public void OnStartDrag(T obj);
    public void OnEndDrag(T obj);
    public void UpdateDrag();

    //Layer Management

}

//extend Vector3 to create a Vector3 from a float
public static class Vector3Extensions
{
    public static Vector3 ToVector3(this float f)
    {
        return new Vector3(f, f, f);
    }
}