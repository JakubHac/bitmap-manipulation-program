using SuperMaxim.Messaging;
using UnityEngine;
using XCharts.Runtime;

public class StatisticsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject BarPlotPrefab;
    [SerializeField] private Transform BarParent;

    private void Start()
    {
        Messenger.Default.Subscribe<CreateBarPlotEvent>(CreateBarPlot);
    }

    private void OnDestroy()
    {
        Messenger.Default.Unsubscribe<CreateBarPlotEvent>(CreateBarPlot);
    }
    
    private void CreateBarPlot(CreateBarPlotEvent barPlotEvent)
    {
        GameObject barPlotGO = Instantiate(BarPlotPrefab, BarParent);
        BarChart plot = barPlotGO.GetComponent<BarChart>();
        plot.EnsureChartComponent<Title>().text = barPlotEvent.Title;
        plot.EnsureChartComponent<Title>().subText = barPlotEvent.SubText;
        
        var yAxis = plot.EnsureChartComponent<YAxis>();
        yAxis.minMaxType = Axis.AxisMinMaxType.Default;
        
        plot.RemoveData();
        var serie = plot.AddSerie<Bar>(barPlotEvent.DataLabel);
        serie.barGap = 0;
        serie.barWidth = 1;

        for (int i = 0; i < barPlotEvent.Values.Length; i++)
        {
            plot.AddXAxisData(i.ToString());
            plot.AddData(0, barPlotEvent.Values[i]);
        }
    }
}
