using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;
using XCharts.Runtime;

public class SelectiveStretchView : MonoBehaviour
{
    [SerializeField] private UIView SelectiveStretchUIView;
    [SerializeField] private BarChart DataPlot;
    [SerializeField] private TMP_InputField P1_Input;
    [SerializeField] private TMP_InputField P2_Input;
    [SerializeField] private TMP_InputField Q3_Input;
    [SerializeField] private TMP_InputField Q4_Input;
    
    private Queue<SelectiveStretchRequest> RequestQueue = new();
    private SelectiveStretchRequest CurrentRequest;
    private ImageHolder source => CurrentRequest.Source;
    
    private int p1;
    private int p2;
    private int q3;
    private int q4;
    
    private void Start()
    {
        Messenger.Default.Subscribe<PosterizeRequest>(AddToQueue);
        p1 = 0;
        p2 = 0;
        q3 = 0;
        q4 = 0;
        PosterizeInputField.text = string.Empty;
        PosterizeInputField.onValueChanged.AddListener(OnPosterizeValueChanged);
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
    
    private void AddToQueue(PosterizeRequest obj)
    {
        RequestQueue.Enqueue(obj);
        if (CurrentRequest == null)
        {
            ShowNextRequest();
        }
    }
    
    public void AcceptSelectiveStretch()
    {
        Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, ImageActions.SelectiveStretchTexture(source.Texture, p1, p2, q3, q4), source, source.GetComponent<DragableUIWindow>().WindowTitle + " - Posteryzacja"));
        CurrentRequest = null;
        
        if (RequestQueue.Count == 0)
        {
            SelectiveStretchUIView.Hide();
        }
        else
        {
            ShowNextRequest();
        }
    }

    public void AfterClose()
    {
        CurrentRequest = null;
    }
    
    private void OnDestroy()
    {
        Messenger.Default.Unsubscribe<PosterizeRequest>(AddToQueue);
    }
}
