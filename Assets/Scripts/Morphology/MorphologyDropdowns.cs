using System.Collections.Generic;
using OpenCvSharp;

public static class MorphologyDropdowns
{
    private static readonly IReadOnlyDictionary<BorderTypes, string> BorderTypes =
        new Dictionary<BorderTypes, string>()
        {
            {OpenCvSharp.BorderTypes.Isolated, "Izolacja"},
            {OpenCvSharp.BorderTypes.Replicate, "Powielenie (abc|ccc)"},
            {OpenCvSharp.BorderTypes.Reflect, "Odbicie (abc|cba)"},
            {OpenCvSharp.BorderTypes.Reflect101, "Odbicie (abc|ba)"},
            {OpenCvSharp.BorderTypes.Wrap, "Zawijanie (abc|abc)"}
        };
    
    private static readonly IReadOnlyDictionary<MorphTypes, string> MorphologyOperations =
        new Dictionary<MorphTypes, string>()
        {
            {MorphTypes.ERODE, "Erozja"},
            {MorphTypes.DILATE, "Dylatacja"},
            {MorphTypes.Open, "Otwarcie"},
            {MorphTypes.Close, "Zamknięcie"},
            {MorphTypes.Skeletonize, "Szkieletyzacja"}
        };
    
    private static readonly IReadOnlyDictionary<bool, string> StructuringElements =
        new Dictionary<bool, string>()
        {
            {true, "Kwadrat"},
            {false, "Romb"}
        };
    
    public static string GetDropdownValue(BorderTypes borderType) => BorderTypes[borderType];
    public static string GetDropdownValue(MorphTypes morphOperation) => MorphologyOperations[morphOperation];
    public static string GetDropdownValue(bool allNeighbours) => StructuringElements[allNeighbours];
}
