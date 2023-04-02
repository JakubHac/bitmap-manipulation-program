using System.Drawing;
using OpenCvSharp;
using SkiaSharp;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    [SerializeField] private RawImage RawImage;
    [SerializeField] private Texture2D Input;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;
    [SerializeField] private ImageLoader ImageLoader;
    
    
    void Start()
    {
        TestPlots();
    }

    private void TestPlots()
    {
        ScottPlot.Plot myPlot = new();

        double[] dataX = { 1, 2, 3, 4, 5 };
        double[] dataY = { 1, 4, 9, 16, 25 };
        myPlot.Add.Scatter(dataX, dataY);
        
        SKSurface surface = SKSurface.Create(new SKImageInfo(512, 512));
        
        myPlot.Render(surface);
        
        var bitmap = new Bitmap(surface.Snapshot().Encode().AsStream());
        ImageLoader.LoadFromBitmap(bitmap);
    }

    private void TestImageFunctions()
    {
        Mat mat = OpenCvSharp.Unity.TextureToMat(this.Input);
        Mat grayMat = new Mat();
        Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);
        Texture2D texture = OpenCvSharp.Unity.MatToTexture(grayMat);
        RawImage rawImage = gameObject.GetComponent<RawImage>();
        rawImage.texture = texture;
        aspectRatioFitter.aspectRatio = (float)texture.width / texture.height;
    }
}
