using SuperMaxim.Messaging;
using UnityEngine;
using XCharts.Runtime;

public class ProfileLinePlotHolder : MonoBehaviour
{
    [SerializeField] private BarChart BarChart;
    private ImageHolder ImageHolder = null;
    private Vector2 StartPoint = new Vector2(0.25f, 0.75f);
    private Vector2 EndPoint = new Vector2(0.75f, 0.25f);
    
    public void AssignImageHolder(ImageHolder imageHolder)
    {
        if (ImageHolder != null)
        {
            Debug.LogError("ImageHolder already assigned", this.gameObject);
            return;
        }
        ImageHolder = imageHolder;
        PlotProfileLine();
        ImageHolder.OnTextureUpdated += PlotProfileLine;
        ImageHolder.OnCloseImage += DestroySelf;
        RequestProfileLineUpdate();
    }

    private void PlotProfileLine()
    {
        BarChart plot = BarChart;
        var window = GetComponent<DragableUIWindow>();
        var imageHolderWindow = ImageHolder.GetComponent<DragableUIWindow>();
        plot.RemoveChartComponent<Title>();
        window.WindowTitle = $"Linia Profilu {imageHolderWindow.WindowTitle}";
        
        var yAxis = plot.EnsureChartComponent<YAxis>();
        yAxis.minMaxType = Axis.AxisMinMaxType.Default;
        
        plot.RemoveData();
        var serie = plot.AddSerie<Bar>("Wartość");
        serie.barGap = 0;
        serie.barWidth = 1;
		
        var line = ImageActions.GetProfileLine(ImageHolder, StartPoint, EndPoint);
        
        Debug.Log("Line Length: " + line.Count);
        
        for (int i = 0; i < line.Count; i++)
        {
            plot.AddXAxisData(i.ToString());
            plot.AddData(0, line[i]);
        }
    }

    private void DestroySelf()
    {
        if (this != null)
        {
            if (GetComponent<DragableUIWindow>() is { } dragableUIWindow) 
            {
                dragableUIWindow.ExecuteClose();
            }
        }
    }
    
    private void OnDestroy()
    {
        if (ImageHolder != null)
        {
            ImageHolder.OnTextureUpdated -= PlotProfileLine;
            ImageHolder.OnCloseImage -= PlotProfileLine;
        }
    }
    
    
    public void RequestProfileLineUpdate()
    {
        Messenger.Default.Publish(new RequestProfileLineUpdate(ImageHolder, StartPoint, EndPoint, RequestProfileLineUpdateCallback));
    }
    
    private void RequestProfileLineUpdateCallback(Vector2 startPoint, Vector2 endPoint)
    {
        UpdateProfileLine(startPoint, endPoint);
    }
    
    public void UpdateProfileLine(Vector2 startPoint, Vector2 endPoint)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        PlotProfileLine();
    }
}
