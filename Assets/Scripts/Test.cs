using System.Drawing;
using OpenCvSharp;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    [SerializeField] private RawImage RawImage;
    [SerializeField] private Texture2D Input;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;
    [FormerlySerializedAs("ImageLoader")] [SerializeField] private ImageFiles ImageFiles;
    
    
    void Start()
    {
        TestPlots();
    }

    private void TestPlots()
    {
        
        
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
