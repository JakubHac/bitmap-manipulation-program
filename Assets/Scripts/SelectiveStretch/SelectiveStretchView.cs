using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        Messenger.Default.Subscribe<SelectiveStretchRequest>(AddToQueue);
        ResetValues();
        P1_Input.onValueChanged.AddListener(OnP1ValueChanged);
        P2_Input.onValueChanged.AddListener(OnP2ValueChanged);
        Q3_Input.onValueChanged.AddListener(OnQ3ValueChanged);
        Q4_Input.onValueChanged.AddListener(OnQ4ValueChanged);
    }

    private void ResetValues()
    {
        p1 = 0;
        p2 = 0;
        q3 = 0;
        q4 = 0;
        P1_Input.text = string.Empty;
        P2_Input.text = string.Empty;
        Q3_Input.text = string.Empty;
        Q4_Input.text = string.Empty;
    }

    private void OnQ4ValueChanged(string textvalue)
    {
        TryReadValue(textvalue, ref q4);
    }

    private void OnQ3ValueChanged(string textvalue)
    {
        TryReadValue(textvalue, ref q3);
    }

    private void OnP2ValueChanged(string textvalue)
    {
        TryReadValue(textvalue, ref p2);
    }

    private void OnP1ValueChanged(string textvalue)
    {
        TryReadValue(textvalue, ref p1);
    }

    private void TryReadValue(string textvalue, ref int target)
    {
        var value = ReadValue(textvalue);
        if (value is null || target == value.Value) return;
        target = value.Value;
        UpdateLUT();
    }

    private int? ReadValue(string text)
    {
        if (string.IsNullOrEmpty(text)) return null;
        if (int.TryParse(text, out int value))
        {
            if (value is >= 0 and <= 255)
            {
                return value;
            }
        }
        return null;
    }

    private void ShowNextRequest()
    {
        SelectiveStretchUIView.Show();
        CurrentRequest = RequestQueue.Dequeue();
        UpdateHistogram();
        ResetValues();
        UpdateLUT();
    }

    private void UpdateHistogram()
    {
        var histogram = DataPlot.GetSerie("Histogram");
        histogram.ClearData();
        var histogramData = ImageActions.GetHistogram(source);
        for (int i = 0; i < histogramData.Length; i++)
        {
            histogram.AddData(histogramData[i]);
        }
    }
    
    private void UpdateLUT()
    {
        var lut = DataPlot.GetSerie("LUT");
        List<(int, double)> data = new();
        var tmpp1 = p1;
        var tmpp2 = Mathf.Max(p2, p1+1);
        if (tmpp1 > 0)
        {
            data.Add((0, 0));
            if (tmpp1 > 1)
            {
                data.Add((tmpp1 - 1, tmpp1 - 1));
            }
        }
        
        data.Add((tmpp1, q3));
        data.Add((tmpp2, q4));

        if (tmpp2 < 255)
        {
            if (tmpp2 < 254)
            {
                data.Add((tmpp2 + 1 , tmpp2 + 1));
            }
            data.Add((255, 255));
        }
        
        lut.ClearData();
        for (int i = 0; i < data.Count; i++)
        {
            lut.AddXYData(data[i].Item1, data[i].Item2);
        }
    }

    private void AddToQueue(SelectiveStretchRequest obj)
    {
        RequestQueue.Enqueue(obj);
        if (CurrentRequest == null)
        {
            ShowNextRequest();
        }
    }
    
    public void AcceptSelectiveStretch()
    {
        Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, ImageActions.SelectiveStretchTexture(source, p1, p2, q3, q4), source, source.GetComponent<DragableUIWindow>().WindowTitle + " - Selektywne rozciągnięcie"));
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
        Messenger.Default.Unsubscribe<SelectiveStretchRequest>(AddToQueue);
    }
}
