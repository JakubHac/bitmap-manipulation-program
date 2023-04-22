using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Color = UnityEngine.Color;

public class ImageLoader : MonoBehaviour
{
    [SerializeField] private RectTransform ImagesParent;
    [SerializeField] GameObject ImageHolderPrefab;

    public static ImageLoader Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

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
        var fileName = Path.GetFileName(path);
        LoadFromBitmap(bitmap, fileName);
    }

    public void LoadFromBitmap(Image bitmap, string title = null)
    {
        using MemoryStream stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        Texture2D texture = new Texture2D(2, 2, DefaultFormat.LDR, 1, TextureCreationFlags.None);
        texture.LoadImage(stream.ToArray(), false);
        SpawnWithTexture(texture, title: title);
    }

    public void SpawnWithTexture(Texture2D texture, Color? color = null, string title = null)
    {
        GameObject imageHolder = Instantiate(ImageHolderPrefab, ImagesParent);
        imageHolder.GetComponent<ImageHolder>().Texture = texture;
        imageHolder.GetComponent<RectTransform>().localPosition = Vector3.zero;
        var window = imageHolder.GetComponent<DragableUIWindow>();
        if (color != null)
        {
            window.WindowColor = color.Value;
        }

        if (title != null)
        {
            window.WindowTitle = title;
        }

    }
}
