using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using OpenCvSharp;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public static class ImageActions
{
    public static readonly IReadOnlyDictionary<string, Action<ImageHolder>> Actions = new Dictionary<string, Action<ImageHolder>>()
    {
        {"Odcienie szarości", ToBlackAndWhite},
        {"Podziel na RGB", SplitRGB},
        {"Podziel na HSV", SplitHSV},
        {"Podziel na LAB", SplitLAB},
        {"Histogram (wykres)", HistogramPlot},
        {"Histogram (tablica)", HistogramTable},
        {"Linia profilu (wykres)", ProfileLinePlot},
        {"Linia profilu (tablica)", ProfileLineTable},
        {"Rozciąganie histogramu", StretchHistogram},
        {"Equalizacja histogramu", EqualizeHistogram},
        {"Duplikacja", Duplicate},
        {"Negacja", Negate},
        {"Posteryzacja", (x) => Messenger.Default.Publish(new PosterizeRequest(x))},
        {"Selektywne rozciąganie", (x) => Messenger.Default.Publish(new SelectiveStretchRequest(x))},
        {"Konwolucja", (x) => Messenger.Default.Publish(new ConvolutionRequest(x))},
        {"Wygładzanie", (x) => Messenger.Default.Publish(new ConvolutionRequest(x, ConvolutionBlurType.Gauss))},
        {"Detekcja krawędzi", (x) => Messenger.Default.Publish(new ConvolutionRequest(x, ConvolutionEdgeDetectMethod.Canny, ConvolutionEdgeDetectDirection.North))},
        {"Wyostrzenie", (x) => Messenger.Default.Publish(new ConvolutionRequest(x, ConvolutionSharpenType.Laplacian1))},
        {"Filtr medianowy", (x) => Messenger.Default.Publish(new MedianFilterRequest(x))},
        {"Dodawanie obrazów", (x) => Messenger.Default.Publish(new CombineImagesRequest(x, AddImages))},
        {"Odejmowanie obrazów", (x) => Messenger.Default.Publish(new CombineImagesRequest(x, SubtractImages))},
        {"Mnożenie obrazów", (x) => Messenger.Default.Publish(new CombineImagesRequest(x, MultiplyImages))},
        {"AND obrazów", (x) => Messenger.Default.Publish(new CombineImagesRequest(x, ANDImages))},
        {"OR obrazów", (x) => Messenger.Default.Publish(new CombineImagesRequest(x, ORImages))},
        {"XOR obrazów", (x) => Messenger.Default.Publish(new CombineImagesRequest(x, XORImages))},
        {"Mieszanie obrazów", (x) => Messenger.Default.Publish(new CombineImagesRequest(x, HandleBlendImages))},
        {"Otwarcie", (x) => Messenger.Default.Publish(new MorphologyRequest(x, MorphTypes.Open))},
        {"Zamknięcie", (x) => Messenger.Default.Publish(new MorphologyRequest(x, MorphTypes.Close))},
        {"Erozja", (x) => Messenger.Default.Publish(new MorphologyRequest(x, MorphTypes.ERODE))},
        {"Dylatacja", (x) => Messenger.Default.Publish(new MorphologyRequest(x, MorphTypes.DILATE))},
        {"Szkieletyzacja", (x) => Messenger.Default.Publish(new MorphologyRequest(x, MorphTypes.Skeletonize))},
        {"Binaryzacja", (x) => Messenger.Default.Publish(new BinaryRequest(x))},
        {"Zapisz", (x) => ImageFiles.Instance.SaveImage(x, false)}
    };

    private static void HandleBlendImages(ImageHolder A, ImageHolder B)
    {
        Messenger.Default.Publish(new BlendImagesRequest(A, B));
    }

    public static Texture2D BlendImages(ImageHolder A, ImageHolder B, float lerp)
    {
        using var matA = OpenCvSharp.Unity.TextureToMat(A.Texture);
        using var matB = OpenCvSharp.Unity.TextureToMat(B.Texture);
        using var output = new Mat();
        Cv2.AddWeighted(matA, 1f - lerp, matB, lerp, 0, output);
        return MatToTexture(output);
    }

    private static void XORImages(ImageHolder A, ImageHolder B)
    {
        using var matA = GetBlackAndWhiteMat(A);
        using var matB = GetBlackAndWhiteMat(B);
        using var output = new Mat();
        Cv2.BitwiseXor(matA, matB, output);
        ImageFiles.Instance.SpawnWithTexture(MatToTexture(output), title: A.GetComponent<DragableUIWindow>().WindowTitle + " XOR " + B.GetComponent<DragableUIWindow>().WindowTitle);
    }
    
    private static void ORImages(ImageHolder A, ImageHolder B)
    {
        using var matA = GetBlackAndWhiteMat(A);
        using var matB = GetBlackAndWhiteMat(B);
        using var output = new Mat();
        Cv2.BitwiseOr(matA, matB, output);
        ImageFiles.Instance.SpawnWithTexture(MatToTexture(output), title: A.GetComponent<DragableUIWindow>().WindowTitle + " OR " + B.GetComponent<DragableUIWindow>().WindowTitle);
    }
    
    private static void ANDImages(ImageHolder A, ImageHolder B)
    {
        using var matA = GetBlackAndWhiteMat(A);
        using var matB = GetBlackAndWhiteMat(B);
        using var output = new Mat();
        Cv2.BitwiseAnd(matA, matB, output);
        ImageFiles.Instance.SpawnWithTexture(MatToTexture(output), title: A.GetComponent<DragableUIWindow>().WindowTitle + " AND " + B.GetComponent<DragableUIWindow>().WindowTitle);
    }
    
    private static void MultiplyImages(ImageHolder A, ImageHolder B)
    {
        using var matA = OpenCvSharp.Unity.TextureToMat(A.Texture);
        using var matB = OpenCvSharp.Unity.TextureToMat(B.Texture);
        using var output = new Mat();
        Cv2.Multiply(matA, matB, output);
        ImageFiles.Instance.SpawnWithTexture(MatToTexture(output), title: A.GetComponent<DragableUIWindow>().WindowTitle + " * " + B.GetComponent<DragableUIWindow>().WindowTitle);
    }
    
    private static void SubtractImages(ImageHolder A, ImageHolder B)
    {
        using var matA = OpenCvSharp.Unity.TextureToMat(A.Texture);
        using var matB = OpenCvSharp.Unity.TextureToMat(B.Texture);
        using var output = new Mat();
        Cv2.Subtract(matA, matB, output);
        ImageFiles.Instance.SpawnWithTexture(MatToTexture(output), title: A.GetComponent<DragableUIWindow>().WindowTitle + " - " + B.GetComponent<DragableUIWindow>().WindowTitle);
    }

    private static void AddImages(ImageHolder A, ImageHolder B)
    {
        using var matA = OpenCvSharp.Unity.TextureToMat(A.Texture);
        using var matB = OpenCvSharp.Unity.TextureToMat(B.Texture);
        using var output = new Mat();
        Cv2.Add(matA, matB, output);
        ImageFiles.Instance.SpawnWithTexture(MatToTexture(output), title: A.GetComponent<DragableUIWindow>().WindowTitle + " + " + B.GetComponent<DragableUIWindow>().WindowTitle);
    }

    public static Texture2D PosterizeTexture(ImageHolder input, int posterizeLevel)
    {
        var texture = DuplicateTexture(input);

        byte[] posterizeLUT = null;
        if (posterizeLevel > 1)
        {
            double p = posterizeLevel;
            posterizeLUT = Enumerable.Range(0, 256).Select(xInt =>
            {
                double x = xInt;
                double val = (x - x % (255.0 / p)) * (p / (p - 1.0));
                return (byte)Math.Round(Math.Clamp(val, 0.0, 255.0));
            }).ToArray();
        }
        else
        {
            posterizeLUT = Enumerable.Range(0, 256).Select(x => (byte)128).ToArray();
        }
        
        
        ApplyLUT(texture, posterizeLUT);

        return texture;
    }


    private static void Negate(ImageHolder source)
    {
        byte[] negateLUT = Enumerable.Range(0, 256).Select(x => (byte)(255 - x)).ToArray();
        var texture = DuplicateTexture(source);
        ApplyLUT(texture, negateLUT);
        Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, texture, source, source.GetComponent<DragableUIWindow>().WindowTitle + " - Negacja"));
    }

    private static void Duplicate(ImageHolder source)
    {
        var newTexture = DuplicateTexture(source);
        var window = source.GetComponent<DragableUIWindow>();
        ImageFiles.Instance.SpawnWithTexture(newTexture, window.WindowColor, window.WindowTitle);
    }

    private static Texture2D DuplicateTexture(ImageHolder source)
    {
        Texture2D newTexture = new Texture2D(source.Texture.width, source.Texture.height, source.Texture.format, false);
        Graphics.CopyTexture(source.Texture, newTexture);
        return newTexture;
    }

    private static void EqualizeHistogram(ImageHolder source)
    {
        var cumulativeDistribution = NormalizedCumulativeDistribution(GetHistogram(source));
        
        Texture2D texture = DuplicateTexture(source);
        ApplyLUT(texture, cumulativeDistribution);

        Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, texture, source, source.GetComponent<DragableUIWindow>().WindowTitle + " - Equalizacja histogramu"));
    }

    private static void ApplyLUT(Texture2D texture, double[] lut)
    {
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                var pixel = GetTexturePixelValue(texture, i, j);
                var newPixelValue = lut[pixel];
                var newPixelColorValue = (float)(newPixelValue / 255.0);
                Color newPixelColor = new Color(newPixelColorValue, newPixelColorValue, newPixelColorValue);
                texture.SetPixel(i, j, newPixelColor);
            }
        }

        texture.Apply();
    }
    
    private static void ApplyLUT(Texture2D texture, float[] lut)
    {
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                var pixel = GetTexturePixelValue(texture, i, j);
                var newPixelValue = lut[pixel];
                var newPixelColorValue = (newPixelValue / 255.0f);
                Color newPixelColor = new Color(newPixelColorValue, newPixelColorValue, newPixelColorValue);
                texture.SetPixel(i, j, newPixelColor);
            }
        }

        texture.Apply();
    }
    
    private static void ApplyLUT(Texture2D texture, byte[] lut)
    {
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                var pixel = GetTexturePixelValue(texture, i, j);
                var newPixelValue = lut[pixel];
                var newPixelColorValue = (newPixelValue / 255.0f);
                Color newPixelColor = new Color(newPixelColorValue, newPixelColorValue, newPixelColorValue);
                texture.SetPixel(i, j, newPixelColor);
            }
        }

        texture.Apply();
    }

    private static byte[] NormalizedCumulativeDistribution(byte[] input)
    {
        double sum = 0;
        double[] cumulativeSum = new double[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            sum += input[i];
            cumulativeSum[i] = sum;
        }
        
        byte[] output = new byte[256];
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = (byte)(cumulativeSum[i] / sum * 255.0);
        }

        return output;
    }
    
    private static long[] CumulativeDistribution(byte[] input)
    {
        long sum = 0;
        long[] output = new long[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            sum += input[i];
            output[i] = sum;
        }
        return output;
    }

    private static void StretchHistogram(ImageHolder source)
    {
        Texture2D texture = DuplicateTexture(source);
        var oldMin = GetTexturePixelValue(texture, 0, 0);
        var oldMax = GetTexturePixelValue(texture, 0, 0);
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                var pixel = GetTexturePixelValue(texture, i, j);
                if (pixel < oldMin)
                {
                    oldMin = pixel;
                }

                if (pixel > oldMax)
                {
                    oldMax = pixel;
                }
            }
        }

        float[] lut = new float[256];
        for (int i = 0; i < 256; i++)
        {
            lut[i] = Mathf.Lerp(0f, 255f, Mathf.InverseLerp(oldMin, oldMax, i));
        }
        
        ApplyLUT(texture, lut);
        
        Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, texture, source, source.GetComponent<DragableUIWindow>().WindowTitle + " - Rozciągnięcie histogramu"));
    }

    private static void ProfileLineTable(ImageHolder source)
    {
        Messenger.Default.Publish(new CreateProfileLineTableEvent(source));
    }

    private static void ProfileLinePlot(ImageHolder source)
    {
        Messenger.Default.Publish(new CreateProfileLinePlotEvent(source));
    }


    private const float UnityColorToHistogramAverage = 255f / 3f;
    public static byte[] GetHistogram(ImageHolder imageHolder)
    {
        byte[] histogram = new byte[256];
        var texture = imageHolder.Texture;
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                var gray = GetTexturePixelValue(texture, i, j);
                histogram[gray]++;
            }
        }

        return histogram;
    }

    private enum FloatPixelTextureValueMode
    {
        Sum,
        Avg,
        To255Max
    }
    
    private static float GetFloatTexturePixelValue(Texture2D texture, int i, int j, FloatPixelTextureValueMode mode)
    {
        var pixel = texture.GetPixel(i, j);
        float output = (pixel.r + pixel.g + pixel.b);
        switch (mode)
        {
            case FloatPixelTextureValueMode.Avg:
                output /= 3f;
                break;
            case FloatPixelTextureValueMode.To255Max:
                output *= UnityColorToHistogramAverage;
                break;
        }

        return output;
    }

    private static byte GetTexturePixelValue(Texture2D texture, int i, int j)
    {
        byte gray = (byte)Mathf.RoundToInt(GetFloatTexturePixelValue(texture, i, j, FloatPixelTextureValueMode.To255Max));
        return gray;
    }

    public static List<byte> GetProfileLine(ImageHolder source, Vector2 normalizedStart, Vector2 normalizedEnd)
    {
        using Mat mat = GetBlackAndWhiteMat(source);
        var start = new Point(normalizedStart.x * source.Texture.width,  (1f - normalizedStart.y) * source.Texture.height);
        var end = new Point(normalizedEnd.x * source.Texture.width,  (1f - normalizedEnd.y) * source.Texture.height);
        
        List<byte> profileLine = new List<byte>();
        foreach (var lip in new LineIterator(mat, start, end)) {
            byte v = lip.GetValue<byte>();
            profileLine.Add(v);
        }

        return profileLine;
    }

    private static void HistogramTable(ImageHolder source)
    {
        Messenger.Default.Publish(new CreateHistogramTableEvent(source));
    }

    private static void HistogramPlot(ImageHolder source)
    {
        Messenger.Default.Publish(new CreateHistogramPlotEvent(source));
    }

    private static void SplitLAB(ImageHolder source)
    {
        using Mat mat = OpenCvSharp.Unity.TextureToMat(source.Texture);
        using Mat lab = new Mat();
        Cv2.CvtColor(mat, lab, ColorConversionCodes.BGR2Lab);
        SplitMat(lab, new []{ColorHelper.ChangeValue(Color.white, 0.2f),  new Color(125f/255f, 61f/255f, 12f/255f, 1f), ColorHelper.ChangeValue(Color.cyan, 0.5f)}, new []
        {
            $"{source.GetComponent<DragableUIWindow>().WindowTitle} - Jasność", $"{source.GetComponent<DragableUIWindow>().WindowTitle} - A", $"{source.GetComponent<DragableUIWindow>().WindowTitle} - B"
        });
    }

    private static void SplitHSV(ImageHolder source)
    {
        using Mat mat = OpenCvSharp.Unity.TextureToMat(source.Texture);
        using Mat hsv = new Mat();
        Cv2.CvtColor(mat, hsv, ColorConversionCodes.BGR2HSV);
        SplitMat(hsv, new []{ColorHelper.ChangeValue(Color.magenta, 0.5f),  ColorHelper.ChangeValue(Color.yellow, 0.5f), Color.black}, new []
        {
            $"{source.GetComponent<DragableUIWindow>().WindowTitle} - Barwa", $"{source.GetComponent<DragableUIWindow>().WindowTitle} - Saturacja", $"{source.GetComponent<DragableUIWindow>().WindowTitle} - Wartość"
        });
    }
    
    private static void SplitRGB(ImageHolder source)
    {
        using Mat mat = OpenCvSharp.Unity.TextureToMat(source.Texture);

        string baseName = source.GetComponent<DragableUIWindow>().WindowTitle;

        SplitMat(mat, new []{ColorHelper.ChangeValue(Color.blue, 0.5f),  ColorHelper.ChangeValue(Color.green, 0.5f), ColorHelper.ChangeValue(Color.red, 0.5f)}, new []
        {
            $"{baseName} - Niebieski", $"{baseName} - Zielony", $"{baseName} - Czerwony"
        });
    }

    private static void SplitMat(Mat mat, Color[] colors = null, string[] titles = null)
    {
        Cv2.Split(mat, out Mat[] channels);
        for (int i = 0; i < channels.Length; i++)
        {
            Texture2D texture = MatToTexture(channels[i]);
            if (colors == null && titles == null)
            {
                ImageFiles.Instance.SpawnWithTexture(texture);
            }
            else if (colors != null && titles != null)
            {
                ImageFiles.Instance.SpawnWithTexture(texture, colors[i], titles[i]);
            }
            else if (colors != null)
            {
                ImageFiles.Instance.SpawnWithTexture(texture, colors[i]);
            }
            else
            {
                ImageFiles.Instance.SpawnWithTexture(texture, title: titles[i]);
            }
            channels[i].Dispose();
        }
    }

    public static Mat GetBlackAndWhiteMat(ImageHolder source)
    {
        using Mat mat = OpenCvSharp.Unity.TextureToMat(source.Texture);
        Mat grayMat = new Mat();
        Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);
        return grayMat;
    }
    
    public static Mat GetBinaryMat(ImageHolder source, int threshold = 128)
    {
        using Mat mat = OpenCvSharp.Unity.TextureToMat(source.Texture);
        using Mat tmpMat = new Mat();
        Cv2.Threshold(mat, tmpMat, threshold, 255, ThresholdTypes.Binary);
        return tmpMat.ExtractChannel(0);
    }
    
    public static Texture2D MatToTexture(Mat mat)
    {
        Texture2D texture = new Texture2D(mat.Width, mat.Height, DefaultFormat.LDR, 0, TextureCreationFlags.None);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        OpenCvSharp.Unity.MatToTexture(mat, texture);
        return texture;
    }
    
    private static void ToBlackAndWhite(ImageHolder source)
    {
        using var mat = GetBlackAndWhiteMat(source);
        Texture2D texture = MatToTexture(mat);
        Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, texture, source, source.GetComponent<DragableUIWindow>().WindowTitle + " - Odcienie szarości"));
    }

    public static Texture2D SelectiveStretchTexture(ImageHolder source, int p1, int p2, int q3, int q4)
    {
        byte[] lut = Enumerable.Range(0, 256).Select(x => (byte)x).ToArray();
        for (int i = p1; i < p2; i++)
        {
            float lerp = Mathf.InverseLerp(p1, p2, i);
            float value = Mathf.Lerp(q3, q4, lerp);
            lut[i] = (byte)Mathf.RoundToInt(value);
        }

        var duplicateTexture = DuplicateTexture(source);
        ApplyLUT(duplicateTexture, lut);
        return duplicateTexture;
    }
    
    public static Texture2D ConvolveTexture(ImageHolder source, double[,] kernel, BorderTypes borderType)
    {
        Debug.Log("filtering image with kernel: " + kernel.ToString() + " and border type: " + borderType.ToString());
        
        using InputArray kernelArray = InputArray.Create(kernel);
        using Mat inputMat = GetBlackAndWhiteMat(source);
        using Mat outputMat = new();
        double kernelSum = 0;
        for (int i = 0; i < kernel.GetLength(0); i++)
        {
            for (int j = 0; j < kernel.GetLength(1); j++)
            {
                kernelSum += kernel[i, j];
            }
        }

        kernelSum = kernelSum < 0.0 ? -kernelSum : kernelSum;
        
        if (kernelSum != 0.0)
        {
            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                for (int j = 0; j < kernel.GetLength(1); j++)
                {
                    kernel[i, j] /= kernelSum;
                }
            }
        }
        
        Cv2.Filter2D(inputMat, outputMat, MatType.MakeType(inputMat.Depth(), inputMat.Channels()), kernelArray, borderType: borderType);
        return MatToTexture(outputMat);
    }
    
    public static Texture2D CannyEdgeTexture(ImageHolder source, double threshold1, double threshold2, int sobelSize, bool fast)
    {
        using Mat inputMat = GetBlackAndWhiteMat(source);
        using Mat outputMat = new();
        Cv2.Canny(inputMat, outputMat, threshold1, threshold2, sobelSize, !fast);
        return MatToTexture(outputMat);
    }
    
    public static Texture2D MedianTexture(ImageHolder source, int kernelSize)
    {
        using Mat inputMat = GetBlackAndWhiteMat(source);
        using Mat outputMat = new();
        Cv2.MedianBlur(inputMat, outputMat, kernelSize);
        return MatToTexture(outputMat);
    }

    public static void Morph(ImageHolder source, bool allNeighbours, BorderTypes borderTypes, MorphTypes morphTypes)
    {
        switch (morphTypes)
        {
            case MorphTypes.Skeletonize:
                Skeletonize(source, allNeighbours, borderTypes);
                break;
            case MorphTypes.ERODE:
                Erode(source, allNeighbours, borderTypes);
                break;
            case MorphTypes.DILATE:
                Dilate(source, allNeighbours, borderTypes);
                break;
            case MorphTypes.Open:
                Open(source, allNeighbours, borderTypes);
                break;
            case MorphTypes.Close:
                Close(source, allNeighbours, borderTypes);
                break;
            default:
                Debug.LogError("Unknown morph type");
                return;
        }
        
    }

    public static Texture2D MakeBinaryTexture(ImageHolder source, float threshold)
    {
        using Mat binaryMat = GetBinaryMat(source, Mathf.RoundToInt(threshold * 255f));
        return MatToTexture(binaryMat);
    }
    
    public static void Erode(ImageHolder source, bool allNeighbours, BorderTypes borderType)
    {
        using Mat inputMat = GetBinaryMat(source);
        using Mat outputMat = new();
        Cv2.MorphologyEx(inputMat, outputMat, MorphTypes.ERODE, GetStructuringElement(allNeighbours), borderType: borderType);
        Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, MatToTexture(outputMat), source, source.GetComponent<DragableUIWindow>().WindowTitle + " - Erozja"));
    }
    
    public static void Dilate(ImageHolder source, bool allNeighbours, BorderTypes borderType)
    {
        using Mat inputMat = GetBinaryMat(source);
        using Mat outputMat = new();
        Cv2.MorphologyEx(inputMat, outputMat, MorphTypes.DILATE, GetStructuringElement(allNeighbours), borderType: borderType);
        Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, MatToTexture(outputMat), source, source.GetComponent<DragableUIWindow>().WindowTitle + " - Dylatacja"));
    }
    
    public static void Open(ImageHolder source, bool allNeighbours, BorderTypes borderType)
    {
        using Mat inputMat = GetBinaryMat(source);
        using Mat outputMat = new();
        Cv2.MorphologyEx(inputMat, outputMat, MorphTypes.Open, GetStructuringElement(allNeighbours), borderType: borderType);
        Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, MatToTexture(outputMat), source, source.GetComponent<DragableUIWindow>().WindowTitle + " - Otwarcie"));
    }
    
    public static void Close(ImageHolder source, bool allNeighbours, BorderTypes borderType)
    {
        using Mat inputMat = GetBinaryMat(source);
        using Mat outputMat = new();
        Cv2.MorphologyEx(inputMat, outputMat, MorphTypes.Close, GetStructuringElement(allNeighbours), borderType: borderType);
        Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, MatToTexture(outputMat), source, source.GetComponent<DragableUIWindow>().WindowTitle + " - Zamknięcie"));
    }
    
    public static void Skeletonize(ImageHolder source, bool allNeighbours, BorderTypes borderType)
    {
        Mat imgMat = GetBinaryMat(source);
        Mat skeletonMat = Mat.Zeros(imgMat.Size(), imgMat.Type()).ToMat();
        
        using Mat structuringElement = GetStructuringElement(allNeighbours);

        while (true)
        {
            Mat erodedMat = Mat.Zeros(imgMat.Size(), imgMat.Type()).ToMat();
            Mat tmpMat = Mat.Zeros(imgMat.Size(), imgMat.Type()).ToMat();
            
            Cv2.MorphologyEx(imgMat, erodedMat, MorphTypes.ERODE, structuringElement, borderType: borderType);
            Cv2.MorphologyEx(erodedMat, tmpMat, MorphTypes.DILATE, structuringElement, borderType: borderType);
            Cv2.Subtract(imgMat, tmpMat, tmpMat);
            Cv2.BitwiseOr(skeletonMat, tmpMat, skeletonMat);
            
            imgMat.Dispose();
            imgMat = erodedMat.Clone();

            erodedMat.Dispose();
            tmpMat.Dispose();
            
            if (Cv2.CountNonZero(imgMat) == 0)
            {
                break;
            }
        }
        imgMat.Dispose();
        
        Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, MatToTexture(skeletonMat), source, source.GetComponent<DragableUIWindow>().WindowTitle + " - Szkieletyzacja"));
        
        skeletonMat.Dispose();
    }
    
    private static Mat GetStructuringElement(bool allNeighbours)
    {
        return Cv2.GetStructuringElement(allNeighbours ? MorphShapes.Rect : MorphShapes.Cross, new Size(3,3));
    }
}
