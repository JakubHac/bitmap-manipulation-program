using System;
using UnityEngine;
using XCharts.Runtime;

public class HistogramPlotHolder : MonoBehaviour
{
	[SerializeField] private BarChart BarChart;
	private ImageHolder ImageHolder = null;
	
	public void AssignImageHolder(ImageHolder imageHolder)
	{
		if (ImageHolder != null)
		{
			Debug.LogError("ImageHolder already assigned", this.gameObject);
			return;
		}
		ImageHolder = imageHolder;
		PlotHistogram();
		ImageHolder.OnTextureUpdated += PlotHistogram;
		ImageHolder.OnCloseImage += DestroySelf;
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
			ImageHolder.OnTextureUpdated -= PlotHistogram;
			ImageHolder.OnCloseImage -= PlotHistogram;
		}
	}

	private void PlotHistogram()
	{
		BarChart plot = BarChart;
		var window = GetComponent<DragableUIWindow>();
		var imageHolderWindow = ImageHolder.GetComponent<DragableUIWindow>();
		plot.RemoveChartComponent<Title>();
		window.WindowTitle = $"Histogram {imageHolderWindow.WindowTitle}";
        
		var yAxis = plot.EnsureChartComponent<YAxis>();
		yAxis.minMaxType = Axis.AxisMinMaxType.Custom;
		yAxis.min = 0;
		yAxis.max = 255;
        
		plot.RemoveData();
		var serie = plot.AddSerie<Bar>("Wartość");
		serie.barGap = 0;
		serie.barWidth = 1;
		
		var histogram = ImageActions.GetHistogram(ImageHolder);

		for (int i = 0; i < histogram.Length; i++)
		{
			plot.AddXAxisData(i.ToString());
			plot.AddData(0, histogram[i]);
		}
	}
	
	
}
