using Doozy.Engine.UI;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class BinaryView : MonoBehaviour
{
    [SerializeField] private RawImage BinaryImage;
    [SerializeField] private AspectRatioFitter AspectRatioFitter;
    [SerializeField] private UIView View;
    [SerializeField] private Slider Slider;
    
    private ImageHolder OriginalImageHolder;
    
    private BinaryRequest Request;
    
    private void Start()
    {
        Messenger.Default.Subscribe<BinaryRequest>(HandleRequest);
        Slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void HandleRequest(BinaryRequest obj)
    {
        Request = obj;
        OriginalImageHolder = obj.Source;
        Slider.SetValueWithoutNotify(0.5f);
        float aspectRatio = (float)obj.Source.Texture.width / obj.Source.Texture.height;
        AspectRatioFitter.aspectRatio = aspectRatio;
        RefreshBinaryImage();
	    
        View.Show();
    }
    
    private void OnSliderValueChanged(float arg)
    {
        RefreshBinaryImage();
    }

    private void RefreshBinaryImage()
    {
        if (BinaryImage.texture != null)
        {
            DestroyImmediate(BinaryImage.texture, true);
        }

        BinaryImage.texture = ImageActions.MakeBinaryTexture(OriginalImageHolder, Slider.value);
    }
    
    public void Accept()
    {
        Messenger.Default.Publish(new ImageReplaceOrNewEvent(OriginalImageHolder.Texture, (Texture2D)BinaryImage.texture, OriginalImageHolder, OriginalImageHolder.GetComponent<DragableUIWindow>().WindowTitle + " - Binaryzacja"));
        BinaryImage.texture = null;
        View.Hide(true);
    }
}
