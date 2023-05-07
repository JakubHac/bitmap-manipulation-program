using System;
using Sirenix.OdinInspector;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class ImageHolder : SerializedMonoBehaviour
{
	private const int MinWindowWidth = 1920 / 10;
	private const int MinWindowHeight = 1080 / 10;
	private const int MaxWindowWidth = 1920 / 2;
	private const int MaxWindowHeight = 1080 / 2;
	[SerializeField] private RawImage Image;

	public Action OnTextureUpdated;
	public Action OnCloseImage;
	
	public Texture2D Texture
	{
		get => (Texture2D)Image.texture;
		set
		{
			if (value == null || value.height == 0 || value.width == 0)
			{
				Debug.Log("Texture is null or size 0");
				Destroy(gameObject);
				gameObject.SetActive(false);
			}
			else
			{
				Image.texture = value;
				value.filterMode = FilterMode.Point;
				
				GetComponent<RectTransform>().sizeDelta = GetHolderSizeFromTexture(value);
				Image.GetComponent<AspectRatioFitter>().aspectRatio = (float)value.width / value.height;
				OnTextureUpdated?.Invoke();
			}
		}
	}

	private static Vector2 GetHolderSizeFromTexture(Texture2D texture)
	{
		float width = Math.Clamp(texture.width, MinWindowWidth, MaxWindowWidth);
		float height = Math.Clamp(texture.height * ((float)width / texture.width), MinWindowHeight, MaxWindowHeight);
		width = Math.Clamp(texture.width * ((float)height / texture.height), MinWindowWidth, MaxWindowWidth);

		return new Vector2(width, height);
	}
	
	

	public void EditImage()
	{
		Messenger.Default.Publish(new EditImageHolder(this));
	}

	private void OnDestroy()
	{
		OnCloseImage?.Invoke();
	}
}