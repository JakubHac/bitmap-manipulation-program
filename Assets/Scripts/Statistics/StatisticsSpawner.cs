using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.Serialization;

public class StatisticsSpawner : MonoBehaviour
{
    [FormerlySerializedAs("BarPlotPrefab")] [SerializeField] private GameObject HistogramPlotPrefab;
    [SerializeField] private GameObject HistogramTablePrefab;
    [SerializeField] private Transform BarParent;

    private void Start()
    {
        Messenger.Default.Subscribe<CreateHistogramPlotEvent>(CreateHistogramPlot);
        Messenger.Default.Subscribe<CreateHistogramTableEvent>(CreateHistogramTable);
    }
    
    private void OnDestroy()
    {
        Messenger.Default.Unsubscribe<CreateHistogramPlotEvent>(CreateHistogramPlot);
        Messenger.Default.Unsubscribe<CreateHistogramTableEvent>(CreateHistogramTable);
    }
    
    private void CreateHistogramPlot(CreateHistogramPlotEvent histogramPlotEvent)
    {
        GameObject barPlotGO = Instantiate(HistogramPlotPrefab, BarParent);
        barPlotGO.GetComponent<HistogramPlotHolder>().AssignImageHolder(histogramPlotEvent.ImageHolder);
    }
    
    private void CreateHistogramTable(CreateHistogramTableEvent histogramTableEvent)
    {
        GameObject barPlotGO = Instantiate(HistogramTablePrefab, BarParent);
        barPlotGO.GetComponent<HistogramTableHolder>().AssignImageHolder(histogramTableEvent.ImageHolder);
    }
}
