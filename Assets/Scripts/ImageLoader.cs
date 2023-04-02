using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using OpenCvSharp;
using SimpleFileBrowser;
using UnityEngine;

public class ImageLoader : MonoBehaviour
{
    [SerializeField] private RectTransform ImagesParent;
    [SerializeField] GameObject ImageHolderPrefab;

    private void Start()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Obrazy", ".png", ".jpg", ".jpeg", ".bmp"));
        FileBrowser.ShowHiddenFiles = true;
        FileBrowser.SingleClickMode = Application.platform == RuntimePlatform.Android;
    }

    public void LoadImage()
    {
        FileBrowser.ShowLoadDialog(OnSuccess, OnCancel, FileBrowser.PickMode.Files, true, Application.persistentDataPath, title:"Wybierz obrazy", loadButtonText: "Wybierz");
    }

    private void OnCancel()
    {
        
    }

    private void OnSuccess(string[] paths)
    {
        foreach (var path in paths)
        {
            Debug.Log("Path: " + path);
            LoadImageFromPath(path);
        }
    }

    private void LoadImageFromPath(string path)
    {
        var bitmap = Image.FromFile(path);
        LoadFromBitmap(bitmap);
    }

    public void LoadFromBitmap(Image bitmap)
    {
        using MemoryStream stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(stream.ToArray());
        SpawnWithTexture(texture);
    }

    private void SpawnWithTexture(Texture2D texture)
    {
        GameObject imageHolder = Instantiate(ImageHolderPrefab, ImagesParent);
        imageHolder.GetComponent<ImageHolder>().Texture = texture;
        imageHolder.GetComponent<RectTransform>().localPosition = Vector3.zero;
    }
}
