using System;
using Sirenix.OdinInspector;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class ImageHolder : SerializedMonoBehaviour
{
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
				GetComponent<RectTransform>().sizeDelta = new Vector2(Texture.width, Texture.height);
				OnTextureUpdated?.Invoke();
			}
		}
	}

	private void Start()
	{
		GetComponent<RectTransform>().sizeDelta = new Vector2(Texture.width, Texture.height);
	}

	public void EditImage()
	{
		Messenger.Default.Publish(new EditImageHolder(this));
	}

	public void ReplaceTexture(Texture2D newImageTexture)
	{
		Texture = newImageTexture;
	}

	private void OnDestroy()
	{
		OnCloseImage?.Invoke();
	}
}