using System.Collections.Generic;
using Doozy.Engine.UI;
using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PosterizeView : MonoBehaviour
{
    [SerializeField] private UIView PosterizeUIView;
    [SerializeField] private TMP_InputField PosterizeInputField;
    [SerializeField] private RawImage TargetImage;
    [SerializeField] private AspectRatioFitter TargetAspectRatioFitter;

    private Queue<PosterizeRequest> RequestQueue = new Queue<PosterizeRequest>();
    private PosterizeRequest CurrentRequest;
    private ImageHolder source => CurrentRequest.Source;
    private int posterizationValue;
    
    private void Start()
    {
        Messenger.Default.Subscribe<PosterizeRequest>(AddToQueue);
        posterizationValue = 256;
        PosterizeInputField.text = string.Empty;
        PosterizeInputField.onValueChanged.AddListener(OnPosterizeValueChanged);
    }

    private void OnPosterizeValueChanged(string textvalue)
    {
        var posterizeValue = ReadPosterizeValue();
        if (posterizeValue is {})
        {
            if (posterizationValue != posterizeValue.Value)
            {
                posterizationValue = posterizeValue.Value;
                Posterize();
            }
        }
    }

    private int? ReadPosterizeValue()
    {
        string text = PosterizeInputField.text;
        if (string.IsNullOrEmpty(text)) return null;
        if (int.TryParse(text, out int value))
        {
            if (value is > 0 and < 257)
            {
                return value;
            }
        }
        return null;
    }

    private void Posterize()
    {
        if (PosterizeUIView.Visibility == VisibilityState.NotVisible) return;

        var posterized = ImageActions.PosterizeTexture(source, posterizationValue);

        TargetImage.texture = posterized;
    }

    private void AddToQueue(PosterizeRequest obj)
    {
        RequestQueue.Enqueue(obj);
        if (CurrentRequest == null)
        {
            ShowNextRequest();
        }
    }
    
    private void ShowNextRequest()
    {
        PosterizeUIView.Show();
        CurrentRequest = RequestQueue.Dequeue();
        TargetImage.texture = CurrentRequest.Source.Texture;
        TargetImage.texture.filterMode = FilterMode.Point;
        TargetAspectRatioFitter.aspectRatio = (float)TargetImage.texture.width / TargetImage.texture.height;
        posterizationValue = 256;
        PosterizeInputField.text = string.Empty;
        Posterize();
    }
    
    public void AcceptPosterize()
    {
        Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, (Texture2D)TargetImage.texture, source, source.GetComponent<DragableUIWindow>().WindowTitle + " - Posteryzacja"));
        CurrentRequest = null;
        
        if (RequestQueue.Count == 0)
        {
            PosterizeUIView.Hide();
        }
        else
        {
            ShowNextRequest();
        }
    }

    public void AfterClose()
    {
        CurrentRequest = null;
        TargetImage.texture = null;
    }
    
    private void OnDestroy()
    {
        Messenger.Default.Unsubscribe<PosterizeRequest>(AddToQueue);
    }
}
