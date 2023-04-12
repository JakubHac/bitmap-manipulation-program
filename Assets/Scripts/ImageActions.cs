using System;
using System.Collections.Generic;
using OpenCvSharp;
using UnityEngine;

public static class ImageActions
{
    public static readonly IReadOnlyDictionary<string, Action<ImageHolder>> Actions = new Dictionary<string, Action<ImageHolder>>()
    {
        {"Odcienie Szarości", ToBlackAndWhite}
    };

    private static void ToBlackAndWhite(ImageHolder source)
    {
        using Mat mat = OpenCvSharp.Unity.TextureToMat(source.Texture);
        using Mat grayMat = new Mat();
        Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);
        Texture2D texture = OpenCvSharp.Unity.MatToTexture(grayMat);
        ImageLoader.Instance.SpawnWithTexture(texture);
    }
}
