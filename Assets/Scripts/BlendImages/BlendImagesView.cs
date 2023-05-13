using Doozy.Engine.UI;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class BlendImagesView : MonoBehaviour
{
    [SerializeField] private RawImage OriginalImage;
    [SerializeField] private RawImage BlendImage;
    [SerializeField] private RawImage OtherImage;
    [SerializeField] private AspectRatioFitter[] AspectRatioFitters;
    [SerializeField] private UIView View;
    [SerializeField] private Slider Slider;
    
    private ImageHolder OriginalImageHolder;
    private ImageHolder OtherImageHolder;

    private BlendImagesRequest Request;
    
    private void Start()
    {
        Messenger.Default.Subscribe<BlendImagesRequest>(HandleRequest);
        Slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float arg)
    {
	    RefreshBlendImage();
    }

    private void HandleRequest(BlendImagesRequest obj)
    {
	    Request = obj;
	    OriginalImageHolder = obj.OriginalImage;
	    OtherImageHolder = obj.OtherImage;
	    OriginalImage.texture = obj.OriginalImage.Texture;
	    OtherImage.texture = obj.OtherImage.Texture;
	    Slider.SetValueWithoutNotify(0.5f);
	    float aspectRatio = (float)obj.OriginalImage.Texture.width / obj.OriginalImage.Texture.height;
	    foreach (var aspectRatioFitter in AspectRatioFitters)
	    {
		    aspectRatioFitter.aspectRatio = aspectRatio;
	    }
	    
	    RefreshBlendImage();
	    
	    View.Show();
    }

    private void RefreshBlendImage()
    {
	    if (BlendImage.texture != null)
	    {
		    DestroyImmediate(BlendImage.texture, true);
	    }

	    BlendImage.texture = ImageActions.BlendImages(OriginalImageHolder, OtherImageHolder, Slider.value);
    }

    public void Accept()
    {
	    ImageFiles.Instance.SpawnWithTexture((Texture2D)BlendImage.texture, title: OriginalImageHolder.GetComponent<DragableUIWindow>().WindowTitle + " blend " + OtherImageHolder.GetComponent<DragableUIWindow>().WindowTitle);
	    BlendImage.texture = null;
	    View.Hide(true);
    }

}
