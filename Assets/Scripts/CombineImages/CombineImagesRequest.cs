using System;

public class CombineImagesRequest
{
    public readonly ImageHolder OriginalImage;
    private readonly Action<ImageHolder, ImageHolder> CombineAction;

    public CombineImagesRequest(ImageHolder originalImage, Action<ImageHolder, ImageHolder> combineAction)
    {
        OriginalImage = originalImage;
        CombineAction = combineAction;
    }

    public void Combine(ImageHolder other)
    {
        CombineAction(OriginalImage, other);
    }
}
