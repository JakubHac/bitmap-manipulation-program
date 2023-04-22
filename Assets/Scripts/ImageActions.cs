using System;
using System.Collections.Generic;
using System.Linq;
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
        {"Duplikacja", Duplicate}
    };

    private static void Duplicate(ImageHolder source)
    {
        Texture2D newTexture = new Texture2D(source.Texture.width, source.Texture.height, source.Texture.format, false);
        Graphics.CopyTexture(source.Texture, newTexture);
        var window = source.GetComponent<DragableUIWindow>();
        ImageLoader.Instance.SpawnWithTexture(newTexture, window.WindowColor, window.WindowTitle);
    }

    private static void EqualizeHistogram(ImageHolder source)
    {
        var cumulativeDistribution = CumulativeDistribution(GetHistogram(source), 255.0);
        
        Texture2D texture = new Texture2D(source.Texture.width, source.Texture.height, source.Texture.format, false);
        Graphics.CopyTexture(source.Texture, texture);
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                var pixel = GetTexturePixelValue(texture, i, j);
                var newPixelValue = cumulativeDistribution[pixel];
                Color newPixelColor = new Color((float)(newPixelValue / 255.0), (float)(newPixelValue / 255.0), (float)(newPixelValue / 255.0));
                texture.SetPixel(i, j, newPixelColor);
            }
        }
        texture.Apply();
        
        Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, texture, source, source.GetComponent<DragableUIWindow>().WindowTitle + " - Equalizacja histogramu"));
    }

    private static double[] CumulativeDistribution(double[] input, double? normalizedMax = null)
    {
        double sum = 0;
        double[] output = new double[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            sum += input[i];
            output[i] = sum;
        }

        if (normalizedMax == null) return output;
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = output[i] / sum * normalizedMax.Value;
        }

        return output;
    }

    private static void StretchHistogram(ImageHolder source)
    {
        Texture2D texture = new Texture2D(source.Texture.width, source.Texture.height, source.Texture.format, false);
        Graphics.CopyTexture(source.Texture, texture);
        int oldMin = GetTexturePixelValue(texture, 0, 0);
        int oldMax = GetTexturePixelValue(texture, 0, 0);
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
        
        float newMin = 0f;
        float newMax = 255f;

        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                var pixel = GetTexturePixelValue(texture, i, j);
                float oldLerp = Mathf.InverseLerp(oldMin, oldMax, pixel);
                float newPixel = Mathf.Lerp(newMin, newMax, oldLerp);
                float newColorValue = newPixel / 255f;
                Color newPixelColor = new Color(newColorValue, newColorValue, newColorValue, 1f);
                texture.SetPixel(i, j, newPixelColor);
            }
        }
        texture.Apply();
        
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
    public static double[] GetHistogram(ImageHolder imageHolder)
    {
        double[] histogram = new double[256];
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

    private static int GetTexturePixelValue(Texture2D texture, int i, int j)
    {
        var pixel = texture.GetPixel(i, j);
        float avg = (pixel.r + pixel.g + pixel.b) * UnityColorToHistogramAverage;
        int gray = Mathf.RoundToInt(avg);
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
                ImageLoader.Instance.SpawnWithTexture(texture);
            }
            else if (colors != null && titles != null)
            {
                ImageLoader.Instance.SpawnWithTexture(texture, colors[i], titles[i]);
            }
            else if (colors != null)
            {
                ImageLoader.Instance.SpawnWithTexture(texture, colors[i]);
            }
            else
            {
                ImageLoader.Instance.SpawnWithTexture(texture, title: titles[i]);
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
    
    private static Texture2D MatToTexture(Mat mat)
    {
        Texture2D texture = new Texture2D(mat.Width, mat.Height, DefaultFormat.LDR, 0, TextureCreationFlags.None);
        OpenCvSharp.Unity.MatToTexture(mat, texture);
        return texture;
    }
    
    private static void ToBlackAndWhite(ImageHolder source)
    {
        using var mat = GetBlackAndWhiteMat(source);
        Texture2D texture = MatToTexture(mat);
        Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, texture, source, source.GetComponent<DragableUIWindow>().WindowTitle + " - Odcienie szarości"));
    }
}
