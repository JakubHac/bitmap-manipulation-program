using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenCvSharp;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoader : MonoBehaviour
{
	[SerializeField] private RectTransform ImagesParent;
	[SerializeField] GameObject ImageHolderPrefab;

	public static ImageLoader Instance;

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
		FileBrowser.ShowLoadDialog(OnSuccess, OnCancel, FileBrowser.PickMode.Files, true,
			Application.persistentDataPath, title: "Wybierz obrazy", loadButtonText: "Wybierz");
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
		
		Texture2D tex = new Texture2D(2, 2);

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


	private byte[] ImportFromIntent(string importPath)
	{
		byte[] data = null;
		try
		{
			// Get the current activity
			AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activityObject = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

			// Get the current intent
			AndroidJavaObject intent = activityObject.Call<AndroidJavaObject>("getIntent");

			// Get the intent data using AndroidJNI.CallObjectMethod so we can check for null
			//IntPtr method_getData = AndroidJNIHelper.GetMethodID(intent.GetRawClass(), "getData", "()Ljava/lang/Object;");
			// IntPtr getDataResult = AndroidJNI.CallObjectMethod(intent.GetRawObject(), method_getData, AndroidJNIHelper.CreateJNIArgArray(new object[0]));
			// if (getDataResult.ToInt32() != 0)
			// {
				// Now actually get the data. We should be able to get it from the result of AndroidJNI.CallObjectMethod, but I don't know how so just call again
				AndroidJavaObject intentURI = intent.Call<AndroidJavaObject>("getData");

				// Open the URI as an input channel
				AndroidJavaObject contentResolver = activityObject.Call<AndroidJavaObject>("getContentResolver");
				AndroidJavaObject inputStream = contentResolver.Call<AndroidJavaObject>("openInputStream", intentURI);
				
				//Read
				data = inputStream.Call<byte[]>("readAllBytes");
				
				Debug.Log($"Read {data.Length} bytes from {importPath}");

				// Close the stream
				inputStream.Call("close");
			// }
			// else
			// {
			// 	Debug.LogError("getDataResult was null");
			// }
		}
		catch (System.Exception ex)
		{
			// Handle error
		}

		return data;
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