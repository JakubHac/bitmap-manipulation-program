using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;
using SuperMaxim.Messaging;
using UnityEngine;

public static class ImageActions
{
    public static readonly IReadOnlyDictionary<string, Action<ImageHolder>> Actions = new Dictionary<string, Action<ImageHolder>>()
    {
        {"Odcienie Szarości", ToBlackAndWhite},
        {"Podziel na RGB", SplitRGB},
        {"Podziel na HSV", SplitHSV},
        {"Podziel na LAB", SplitLAB},
        {"Histogram (wykres)", HistogramPlot},
        {"Histogram (tablica)", HistogramTable},
        {"Linia profilu (wykres)", ProfileLinePlot},
        {"Linia profilu (tablica)", ProfileLineTable}
    };

    private static void ProfileLineTable(ImageHolder obj)
    {
        
    }

    private static void ProfileLinePlot(ImageHolder obj)
    {
        
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
                var pixel = texture.GetPixel(i, j);
                float avg = (pixel.r + pixel.g + pixel.b) * UnityColorToHistogramAverage;
                int gray = Mathf.RoundToInt(avg);
                histogram[gray]++;
            }
        }

        return histogram;
    }

    public static double[] GetProfileLine(ImageHolder source, Vector2 normalizedStart, Vector2 normalizedEnd)
    {
        using Mat mat = GetBlackAndWhiteMat(source);
        LineIterator iterator = new LineIterator(mat, 
            new Point(normalizedStart.x * source.Texture.width, normalizedStart.y * source.Texture.height), 
            new Point(normalizedEnd.x * source.Texture.width, normalizedEnd.y * source.Texture.height));
        
        double[] profileLine = new double[iterator.ElemSize];
        var enumerator = iterator.GetEnumerator();
        for (int i = 0; i < profileLine.Length; i++)
        {
            profileLine[i] = enumerator.Current.Value.ToInt32();
            if (i + 1 < profileLine.Length)
            {
                enumerator.MoveNext();
            }
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
            Texture2D texture = OpenCvSharp.Unity.MatToTexture(channels[i]);
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
    
    private static void ToBlackAndWhite(ImageHolder source)
    {
        using var mat = GetBlackAndWhiteMat(source);
        Texture2D texture = OpenCvSharp.Unity.MatToTexture(mat);
        Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, texture, source, source.GetComponent<DragableUIWindow>().WindowTitle + " - Odcienie Szarości"));
    }
}
