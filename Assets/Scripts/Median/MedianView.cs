using System.Collections.Generic;
using Doozy.Engine.UI;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class MedianView : MonoBehaviour
{
	[SerializeField] private RawImage SmolImage;
	[SerializeField] private RawImage MediumImage;
	[SerializeField] private RawImage LargoImage;
	[SerializeField] private UIView MedianFilterUiView;
	
	
	private Queue<MedianFilterRequest> RequestQueue = new();
	private MedianFilterRequest CurrentRequest;
	private ImageHolder source => CurrentRequest.SourceImage;

	private void Start()
	{
		Messenger.Default.Subscribe<MedianFilterRequest>(AddToQueue);
	}

	private void ShowNextRequest()
	{
		MedianFilterUiView.Show();
		CurrentRequest = RequestQueue.Dequeue();
		SmolImage.texture = ImageActions.MedianTexture(source, 3);
		MediumImage.texture = ImageActions.MedianTexture(source, 5);
		LargoImage.texture = ImageActions.MedianTexture(source, 7);
	}

	private void AddToQueue(MedianFilterRequest obj)
	{
		RequestQueue.Enqueue(obj);
		if (CurrentRequest == null)
		{
			ShowNextRequest();
		}
	}

	public void ChooseSmol()
	{
		Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, (Texture2D)SmolImage.texture, source,
			source.GetComponent<DragableUIWindow>().WindowTitle + " - Mediana 3x3"));
		Finish();
	}

	public void ChooseMedium()
	{
		Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, (Texture2D)MediumImage.texture, source,
			source.GetComponent<DragableUIWindow>().WindowTitle + " - Mediana 5x5"));
		Finish();
	}

	public void ChooseLargo()
	{
		Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture, (Texture2D)LargoImage.texture, source,
			source.GetComponent<DragableUIWindow>().WindowTitle + " - Mediana 7x7"));
		Finish();
	}

	private void Finish()
	{
		CurrentRequest = null;

		if (RequestQueue.Count == 0)
		{
			Close();
			
		}
		else
		{
			ShowNextRequest();
		}
	}

	private void Close()
	{
		MedianFilterUiView.Hide();
		CurrentRequest = null;
		SmolImage.texture = null;
		MediumImage.texture = null;
		LargoImage.texture = null;
	}

	private void OnDestroy()
	{
		Messenger.Default.Unsubscribe<MedianFilterRequest>(AddToQueue);
	}
}
