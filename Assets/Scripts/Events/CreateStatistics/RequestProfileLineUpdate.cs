using System;
using UnityEngine;

public class RequestProfileLineUpdate
{
    public ImageHolder ImageHolder;
    public Vector2 StartPoint;
    public Vector2 EndPoint;
    public Action<Vector2, Vector2> Callback;
    
    public RequestProfileLineUpdate(ImageHolder imageHolder, Vector2 startPoint, Vector2 endPoint, Action<Vector2, Vector2> callback)
    {
        ImageHolder = imageHolder;
        StartPoint = startPoint;
        EndPoint = endPoint;
        Callback = callback;
    }
}
