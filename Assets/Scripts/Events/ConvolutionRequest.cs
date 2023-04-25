public class ConvolutionRequest
{
    public readonly ImageHolder Source;
    public readonly ConvolutionOperation Operation;
    public readonly ConvolutionBlurType BlurType;
    public readonly ConvolutionEdgeDetectMethod EdgeDetectMethod;
    public readonly ConvolutionEdgeDetectDirection EdgeDetectDirection;
    public readonly ConvolutionSharpenType SharpenType;
    public readonly ConvolutionLaplacianEdgeDetectionType LaplacianEdgeDetectionType;

    public ConvolutionRequest(ImageHolder source)
    {
        Source = source;
        Operation = ConvolutionOperation.Custom;
    }

    public ConvolutionRequest(ImageHolder source, ConvolutionEdgeDetectMethod edgeDetectMethod,
        ConvolutionEdgeDetectDirection convolutionEdgeDetectDirection)
    {
        Source = source;
        Operation = ConvolutionOperation.EdgeDetection;
        EdgeDetectMethod = edgeDetectMethod;
        EdgeDetectDirection = convolutionEdgeDetectDirection;
    }
    
    public ConvolutionRequest(ImageHolder source, ConvolutionEdgeDetectMethod edgeDetectMethod,
        ConvolutionLaplacianEdgeDetectionType laplacianEdgeDetectionType)
    {
        Source = source;
        Operation = ConvolutionOperation.EdgeDetection;
        EdgeDetectMethod = edgeDetectMethod;
        LaplacianEdgeDetectionType = laplacianEdgeDetectionType;
    }
    
    public ConvolutionRequest(ImageHolder source, ConvolutionBlurType blurType)
    {
        Source = source;
        Operation = ConvolutionOperation.Blur;
        BlurType = blurType;
    }
    
    public ConvolutionRequest(ImageHolder source, ConvolutionSharpenType sharpenType)
    {
        Source = source;
        Operation = ConvolutionOperation.Sharpen;
        SharpenType = sharpenType;
    }
}
