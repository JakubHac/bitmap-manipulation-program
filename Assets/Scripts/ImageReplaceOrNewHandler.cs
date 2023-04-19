using Doozy.Engine.UI;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class ImageReplaceOrNewHandler : MonoBehaviour
{
    [SerializeField] private RawImage OldImage;
    [SerializeField] private AspectRatioFitter OldImageAspectRatioFitter;
    [SerializeField] private RawImage NewImage;
    [SerializeField] private AspectRatioFitter NewImageAspectRatioFitter;
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
       OldImage.texture.filterMode = FilterMode.Point;
       NewImage.texture.filterMode = FilterMode.Point;
       NewImageName = newOrReplace.NewName;
       CurrentImageHolder = newOrReplace.CurrentImageHolder;
       OldImageAspectRatioFitter.aspectRatio = (float)OldImage.texture.width / OldImage.texture.height;
       NewImageAspectRatioFitter.aspectRatio = (float)NewImage.texture.width / NewImage.texture.height;
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
