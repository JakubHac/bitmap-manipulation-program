using Doozy.Engine.UI;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class ImageReplaceOrNewHandler : MonoBehaviour
{
    [SerializeField] private RawImage OldImage;
    [SerializeField] private RawImage NewImage;
    [SerializeField] private UIView ReplaceView;
    

    private string NewImageName;
    private ImageHolder CurrentImageHolder;
    
    private void Start()
    {
        Messenger.Default.Subscribe<ImageReplaceOrNewEvent>(HandleEvent);
    }

    private void HandleEvent(ImageReplaceOrNewEvent newOrReplace)
    {
       OldImage.texture = newOrReplace.OldTexture;
       NewImage.texture = newOrReplace.NewTexture;
       NewImageName = newOrReplace.NewName;
       CurrentImageHolder = newOrReplace.CurrentImageHolder;
       ReplaceView.Show();
    }

    private void OnDestroy()
    {
	    Messenger.Default.Unsubscribe<ImageReplaceOrNewEvent>(HandleEvent);
    }

    public void OnReplace()
    {
	    CurrentImageHolder.ReplaceTexture((Texture2D)NewImage.texture);
	    ReplaceView.Hide();
    }

    public void OnNew()
    {
	    ImageLoader.Instance.SpawnWithTexture((Texture2D)NewImage.texture, title: NewImageName);
	    ReplaceView.Hide();
    }
    
    public void AfterHide()
	{
	    NewImage.texture = null;
	    OldImage.texture = null;
	}
}
