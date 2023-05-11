public class BlendImagesRequest
{
    public readonly ImageHolder OriginalImage;
    public readonly ImageHolder OtherImage;

    public BlendImagesRequest(ImageHolder originalImage, ImageHolder otherImage)
    {
        OriginalImage = originalImage;
        OtherImage = otherImage;
    }
}
