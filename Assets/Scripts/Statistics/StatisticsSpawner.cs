using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.Serialization;

public class StatisticsSpawner : MonoBehaviour
{
    [FormerlySerializedAs("BarPlotPrefab")] [SerializeField] private GameObject HistogramPlotPrefab;
    [SerializeField] private GameObject HistogramTablePrefab;
    [SerializeField] private GameObject ProfileLinePlotPrefab;
    [SerializeField] private GameObject ProfileLineTablePrefab;
    [SerializeField] private Transform BarParent;

    private void Start()
    {
        Messenger.Default.Subscribe<CreateHistogramPlotEvent>(CreateHistogramPlot);
        Messenger.Default.Subscribe<CreateHistogramTableEvent>(CreateHistogramTable);
        Messenger.Default.Subscribe<CreateProfileLinePlotEvent>(CreateProfileLinePlot);
        Messenger.Default.Subscribe<CreateProfileLineTableEvent>(CreateProfileLineTable);
    }

    private void CreateProfileLineTable(CreateProfileLineTableEvent obj)
    {
        GameObject barPlotGO = Instantiate(ProfileLineTablePrefab, BarParent);
        barPlotGO.GetComponent<ProfileLineTableHolder>().AssignImageHolder(obj.ImageHolder);
    }

    private void CreateProfileLinePlot(CreateProfileLinePlotEvent obj)
    {
        GameObject barPlotGO = Instantiate(ProfileLinePlotPrefab, BarParent);
        barPlotGO.GetComponent<ProfileLinePlotHolder>().AssignImageHolder(obj.ImageHolder);
    }

    private void OnDestroy()
    {
        Messenger.Default.Unsubscribe<CreateHistogramPlotEvent>(CreateHistogramPlot);
        Messenger.Default.Unsubscribe<CreateHistogramTableEvent>(CreateHistogramTable);
        Messenger.Default.Unsubscribe<CreateProfileLinePlotEvent>(CreateProfileLinePlot);
        Messenger.Default.Unsubscribe<CreateProfileLineTableEvent>(CreateProfileLineTable);
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
