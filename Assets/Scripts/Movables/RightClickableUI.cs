using SuperMaxim.Messaging;
using UnityEngine;

public class RightClickableUI : MonoBehaviour
{
    private RightClickTarget self_RightClickTarget;
    
    void Start()
    {
        self_RightClickTarget = new RightClickTarget(gameObject, OnRightClick);
        Messenger.Default.Publish(new RegisterRightClickTarget(self_RightClickTarget));
    }

    private void OnRightClick()
    {
        Debug.Log($"Right Clicked {this.gameObject.name}");
    }
    
    private void OnDestroy()
    {
        Messenger.Default.Publish(new UnRegisterRightClickTarget(self_RightClickTarget));
    }
}
