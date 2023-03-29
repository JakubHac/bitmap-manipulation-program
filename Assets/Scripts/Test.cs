using OpenCvSharp;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    [SerializeField] private RawImage RawImage;
    [SerializeField] private Texture2D Input;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;
    

    void Start()
    {
        Mat mat = OpenCvSharp.Unity.TextureToMat (this.Input);
        Mat grayMat = new Mat ();
        Cv2.CvtColor (mat, grayMat, ColorConversionCodes.BGR2GRAY);
        Texture2D texture = OpenCvSharp.Unity.MatToTexture (grayMat);
        RawImage rawImage = gameObject.GetComponent<RawImage> ();
        rawImage.texture = texture;
        aspectRatioFitter.aspectRatio = (float)texture.width / texture.height;
    }
    
}
