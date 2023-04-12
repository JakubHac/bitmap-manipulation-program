using System.Linq;
using DG.Tweening;
using Doozy.Engine.UI;
using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageActionView : MonoBehaviour
{
	[SerializeField] private UIView View;
	[SerializeField] private RawImage ImagePreview;
	[SerializeField] private AspectRatioFitter ImagePreviewFitter;
	
	private ImageHolder SelectedHolder;
	[SerializeField] private TMP_InputField SearchField;
	[SerializeField] private RectTransform ButtonsParent;
	[SerializeField] private GameObject ButtonPrefab;
	
	private void Start()
	{
		Messenger.Default.Subscribe<EditImageHolder>(OnEditImageHolder);

		foreach (var actionName in ImageActions.Actions.Keys.OrderBy(x => x))
		{
			var buttonGo = Instantiate(ButtonPrefab, ButtonsParent);
			buttonGo.name = actionName;
			var button = buttonGo.GetComponent<UIButton>();
			button.ButtonName = actionName;
			button.OnClick.OnTrigger.Event.AddListener(() =>
			{
				ImageActions.Actions[actionName].Invoke(SelectedHolder);
				View.Hide();
			});
			button.SetLabelText(actionName);
		}
	}

	private void OnEditImageHolder(EditImageHolder obj)
	{
		SelectedHolder = obj.ImageHolder;
		ImagePreview.texture = SelectedHolder.Texture;
		ImagePreviewFitter.aspectRatio = (float)SelectedHolder.Texture.width / SelectedHolder.Texture.height;
		SearchField.text = string.Empty;
		View.Show();
	}

	public void OnClose()
	{
		SelectedHolder = null;
		ImagePreview.texture = null;
	}

	private void OnDestroy()
	{
		Messenger.Default.Unsubscribe<EditImageHolder>(OnEditImageHolder);
	}
}