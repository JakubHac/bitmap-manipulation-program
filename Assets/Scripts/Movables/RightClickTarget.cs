using System;
using UnityEngine;

public class RightClickTarget
{
    public readonly GameObject[] GameObjects;
    public readonly Action OnRightClick;

    public RightClickTarget(GameObject[] gameObjects, Action onRightClick)
    {
        GameObjects = gameObjects;
        OnRightClick = onRightClick;
    }
    
    public RightClickTarget(GameObject gameObject, Action onRightClick)
    {
        GameObjects = new[] {gameObject};
        OnRightClick = onRightClick;
    }
}
