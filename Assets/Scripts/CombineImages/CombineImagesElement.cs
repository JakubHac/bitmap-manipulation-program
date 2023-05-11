using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombineImagesElement : MonoBehaviour
{
	[SerializeField] private TMP_Text SourceNameText;
	[SerializeField] private RawImage SourceImagePreview;
	[SerializeField] private AspectRatioFitter AspectRatio;
	
	private ImageHolder Source;
	private CombineImagesView View;
	
	public void SetSource(ImageHolder source, CombineImagesView view)
	{
		Source = source;
		View = view;
		SourceNameText.text = source.GetComponent<DragableUIWindow>().WindowTitle;
		SourceImagePreview.texture = source.Texture;
		AspectRatio.aspectRatio = (float)source.Texture.width / source.Texture.height;
	}

	public void Click()
	{
		View.SelectImage(Source);
	}

}
