using UnityEngine;

public interface IGrabbable
{
    public bool IsGrabbed { get; set; }
    
    public void Grab(Joint joint);
    public void EndGrab();
}