using System;
using UnityEngine;

public class RegisterMovable
{
    public readonly GameObject DragTargets;
    public readonly GameObject[] RightClickTargets;
    
    public readonly Action<bool> OnChangeDragState;
    public readonly Action OnRightClick;

    public RegisterMovable(GameObject dragTargets, GameObject[] rightClickTargets, Action<bool> onChangeDragState, Action onRightClick)
    {
        DragTargets = dragTargets;
        RightClickTargets = rightClickTargets;
        OnChangeDragState = onChangeDragState;
        OnRightClick = onRightClick;
    }
}
