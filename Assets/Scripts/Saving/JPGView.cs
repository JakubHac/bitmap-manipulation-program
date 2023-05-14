using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Doozy.Engine.UI;
using OpenCvSharp;
using SimpleFileBrowser;
using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class JPGView : MonoBehaviour
{
    [SerializeField] private RawImage JpgImage;
    [SerializeField] private AspectRatioFitter AspectRatioFitter;
    [SerializeField] private UIView View;
    [SerializeField] private Slider Slider;
    [SerializeField] private TMP_Text SizeText;
    
    
    private ImageHolder OriginalImageHolder;
    private SaveImageRequest Request;
    int jpgQuality = 100;
    private bool SliderChanged = false;
    private double? LastImageUpdateTime;
    private double ImageUpdateInterval = 0.25;

    private void Start()
    {
        Messenger.Default.Subscribe<SaveImageRequest>(HandleRequest, RequestIsJPG);
        Slider.onValueChanged.AddListener(OnSliderValueChanged);
        SliderChanged = false;
    }

    private void OnSliderValueChanged(float arg0)
    {
        SliderChanged = true;
    }

    private bool RequestIsJPG(SaveImageRequest request) => request.isJPG;

    private void HandleRequest(SaveImageRequest obj)
    {
        Request = obj;
        OriginalImageHolder = obj.Source;
        Slider.SetValueWithoutNotify(1.0f);
        jpgQuality = 100;
        float aspectRatio = (float)obj.Source.Texture.width / obj.Source.Texture.height;
        SliderChanged = false;
        AspectRatioFitter.aspectRatio = aspectRatio;
        RemoveOldTexture();
        CreateTexture();

        View.Show();
    }

    private void Update()
    {
        if (View.Visibility != VisibilityState.Visible) return;
        if (!SliderChanged) return;
        if (Time.realtimeSinceStartupAsDouble - LastImageUpdateTime < ImageUpdateInterval) return;
        RefreshImage();
    }

    private void RefreshImage()
    {
        int newQuality = Mathf.RoundToInt(Slider.value * 100);
        if (newQuality == jpgQuality) return;
        LastImageUpdateTime = Time.realtimeSinceStartupAsDouble;
        SliderChanged = false;
        jpgQuality = newQuality;
        RemoveOldTexture();
        CreateTexture();
    }

    private void CreateTexture()
    {
        var data = CreateJPG();
        Texture2D texture = new Texture2D(2, 2, DefaultFormat.LDR, 0, TextureCreationFlags.None);
        texture.LoadImage(data);
        JpgImage.texture = texture;
        SizeText.text = new FileSize((ulong)data.LongLength).ToString(2);
    }
    
    public void Accept()
    {
        var data = CreateJPG();
        FileBrowserHelpers.WriteBytesToFile(Request.FilePath, data);
        Request = null;
        RemoveOldTexture();
        
        View.Hide(true);
    }

    private void RemoveOldTexture()
    {
        if (JpgImage.texture != null)
        {
            DestroyImmediate(JpgImage.texture, true);
            JpgImage.texture = null;
        }
    }

    private byte[] CreateJPG()
    {
        return OriginalImageHolder.Texture.EncodeToJPG(Mathf.Clamp(jpgQuality, 0, 100));
    }
}
