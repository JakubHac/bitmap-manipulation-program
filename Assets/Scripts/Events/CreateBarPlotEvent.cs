public class CreateBarPlotEvent
{
    public string Title;
    public string SubText;
    public double[] Values;
    public string DataLabel;

    public CreateBarPlotEvent(string title, string subText, double[] values, string dataLabel)
    {
        Title = title;
        SubText = subText;
        Values = values;
        DataLabel = dataLabel;
    }
}
