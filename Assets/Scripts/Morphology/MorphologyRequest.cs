using OpenCvSharp;

public class MorphologyRequest
{
    public readonly ImageHolder Source;
    public readonly MorphTypes MorphologyOperation;

    public MorphologyRequest(ImageHolder source, MorphTypes morphologyOperation)
    {
        Source = source;
        MorphologyOperation = morphologyOperation;
    }
}
