using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenCvSharp;
using SimpleFileBrowser;
using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;

public class ImageFiles : MonoBehaviour
{
	[SerializeField] private RectTransform ImagesParent;
	[SerializeField] GameObject ImageHolderPrefab;

	public static ImageFiles Instance;

	private static readonly IReadOnlyList<string> ImageExtensions = new List<string>()
		{ ".png", ".jpg", ".jpeg", ".bmp" };

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
		FileBrowser.SetFilters(false, new FileBrowser.Filter("Obrazy", ImageExtensions.ToArray()));
		FileBrowser.ShowHiddenFiles = true;
		FileBrowser.SingleClickMode = Application.platform == RuntimePlatform.Android;
	}

	public void LoadImage()
	{
		FileBrowser.ShowLoadDialog(OnLoadSuccess, OnLoadCancel, FileBrowser.PickMode.Files, true,
			Application.persistentDataPath, title: "Wybierz obrazy", loadButtonText: "Wybierz");
	}

	public void SaveImage(ImageHolder source)
	{
		FileBrowser.ShowSaveDialog(result => OnSaveSucces(result, source), OnSaveCancel, FileBrowser.PickMode.Files, allowMultiSelection: false, title:"Zapisz obraz", saveButtonText: "Zapisz", initialFilename: source.GetComponent<DragableUIWindow>().WindowTitle);
	}

	private void OnSaveCancel()
	{
		
	}

	private void OnSaveSucces(string[] paths, ImageHolder source)
	{
		string path = paths[0];
		if (!Path.HasExtension(path))
		{
			path += ".bmp";
		}

		string extension = Path.GetExtension(path);

		if (!ImageExtensions.Contains(extension)) return;

		

		switch (extension)
		{
			case ".jpg":
			case ".jpeg":
			case ".bmp":
				Messenger.Default.Publish(new SaveImageRequest(source, path));
				break;
			case ".png":
				SavePNG(source, path);
				break;
			default:
				return;
		}
	}

	private void SavePNG(ImageHolder source, string path)
	{
		byte[] pngData = source.Texture.EncodeToPNG();
		FileBrowserHelpers.WriteBytesToFile(path, pngData);
	}

	private void OnLoadCancel()
	{
	}

	private void OnLoadSuccess(string[] paths)
	{
		foreach (var path in paths)
		{
			Debug.Log("Path: " + path);
			LoadImageFromPath(path);
		}
	}

	private IEnumerator LoadFromURI(string uri)
	{
		string extension = Path.GetExtension(uri);
		string filename = Path.GetFileName(uri);
		if (!ImageExtensions.Contains(extension))
		{
			Debug.LogError($"Cannot read image format {extension}: " + uri);
			yield break;
		}

		var data = FileBrowserHelpers.ReadBytesFromFile(uri);
		
		Texture2D tex = new Texture2D(2, 2, DefaultFormat.LDR, 0, TextureCreationFlags.None);

		switch (extension)
		{
			case ".png":
			case ".jpg":
			case ".jpeg":
				tex.LoadImage(data);
				break;
			case ".bmp":
				using (var mat = Cv2.ImDecode(data, ImreadModes.Color))
				{
					tex = ImageActions.MatToTexture(mat);
				}
				break;
			case ".gif":
				break;
			default:
				DestroyImmediate(tex, true);
				yield break;
		}

		SpawnWithTexture(tex, title: Path.GetFileName(filename));
	}

	private void LoadImageFromPath(string path)
	{
		StartCoroutine(LoadFromURI(path));
	}

	public void SpawnWithTexture(Texture2D texture, Color? color = null, string title = null)
	{
		GameObject imageHolder = Instantiate(ImageHolderPrefab, ImagesParent);
		
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Point;
		
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