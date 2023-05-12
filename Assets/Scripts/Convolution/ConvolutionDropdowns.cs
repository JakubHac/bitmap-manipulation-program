using System.Collections.Generic;
using OpenCvSharp;

public static class ConvolutionDropdowns
{
    private static readonly IReadOnlyDictionary<ConvolutionOperation, string> Operations =
        new Dictionary<ConvolutionOperation, string>()
        {
            { ConvolutionOperation.Custom, "Własna maska"},
            { ConvolutionOperation.EdgeDetection, "Detkcja krawędzi"},
            { ConvolutionOperation.Blur, "Wygładzanie"},
            { ConvolutionOperation.Sharpen, "Wyostrzanie"}
        };
    
    private static readonly IReadOnlyDictionary<ConvolutionEdgeDetectMethod, string> EdgeDetectionMethods =
        new Dictionary<ConvolutionEdgeDetectMethod, string>()
        {
            { ConvolutionEdgeDetectMethod.Canny, "Canny"},
            { ConvolutionEdgeDetectMethod.Laplacian, "Laplacian"},
            { ConvolutionEdgeDetectMethod.Sobel, "Sobel"},
            { ConvolutionEdgeDetectMethod.Prewitt, "Prewitt"}
        };
    
    private static readonly IReadOnlyDictionary<ConvolutionEdgeDetectDirection, string> EdgeDetectionDirections =
        new Dictionary<ConvolutionEdgeDetectDirection, string>()
        {
            { ConvolutionEdgeDetectDirection.North, "Północ"},
            { ConvolutionEdgeDetectDirection.East, "Wschód"},
            { ConvolutionEdgeDetectDirection.West, "Zachód"},
            { ConvolutionEdgeDetectDirection.South, "Południe"},
            { ConvolutionEdgeDetectDirection.NorthEast, "Północny-wschód"},
            { ConvolutionEdgeDetectDirection.NorthWest, "Północny-zachód"},
            { ConvolutionEdgeDetectDirection.SouthEast, "Południowy-wschód"},
            { ConvolutionEdgeDetectDirection.SouthWest, "Południowy-zachód"}
        };
    
    private static readonly IReadOnlyDictionary<ConvolutionLaplacianEdgeDetectionType, string> EdgeDetectionLaplacian =
        new Dictionary<ConvolutionLaplacianEdgeDetectionType, string>()
        {
            { ConvolutionLaplacianEdgeDetectionType.FourConnected, "Tylko boki"},
            { ConvolutionLaplacianEdgeDetectionType.EightConnected, "Wszyscy sąsiedzi"}
        };
    
    private static readonly IReadOnlyDictionary<ConvolutionSharpenType, string> SharpenMethods =
        new Dictionary<ConvolutionSharpenType, string>()
        {
            { ConvolutionSharpenType.Laplacian1, "Laplacian 1"},
            { ConvolutionSharpenType.Laplacian2, "Laplacian 2"},
            { ConvolutionSharpenType.Laplacian3, "Laplacian 3"}
        };
    
    private static readonly IReadOnlyDictionary<ConvolutionBlurType, string> BlurMethods =
        new Dictionary<ConvolutionBlurType, string>()
        {
            { ConvolutionBlurType.Neighbours, "Tylko boki"},
            { ConvolutionBlurType.Neighbours_Weighted, "Boki ważone"},
            { ConvolutionBlurType.All, "Wszyscy sąsiedzi"},
            { ConvolutionBlurType.All_Weighted, "Wszyscy ważone"},
            { ConvolutionBlurType.Gauss, "Gauss"}
        };

    private static readonly IReadOnlyDictionary<BorderTypes, string> BorderTypes =
        new Dictionary<BorderTypes, string>()
        {
            {OpenCvSharp.BorderTypes.Isolated, "Izolacja"},
            {OpenCvSharp.BorderTypes.Replicate, "Powielenie (abc|ccc)"},
            {OpenCvSharp.BorderTypes.Reflect, "Odbicie (abc|cba)"},
            {OpenCvSharp.BorderTypes.Reflect101, "Odbicie (abc|ba)"},
            {OpenCvSharp.BorderTypes.Wrap, "Zawijanie (abc|abc)"}
        };

    public static string GetDropdownValue(ConvolutionOperation operation) => Operations[operation];
    public static string GetDropdownValue(ConvolutionEdgeDetectMethod edgeDetectMethod) => EdgeDetectionMethods[edgeDetectMethod];
    public static string GetDropdownValue(ConvolutionEdgeDetectDirection edgeDetectDirection) => EdgeDetectionDirections[edgeDetectDirection];
    public static string GetDropdownValue(ConvolutionLaplacianEdgeDetectionType edgeDetectLaplacian) => EdgeDetectionLaplacian[edgeDetectLaplacian];
    public static string GetDropdownValue(ConvolutionSharpenType sharpenType) => SharpenMethods[sharpenType];
    public static string GetDropdownValue(ConvolutionBlurType blurType) => BlurMethods[blurType];
    public static string GetDropdownValue(BorderTypes borderType) => BorderTypes[borderType];
}
