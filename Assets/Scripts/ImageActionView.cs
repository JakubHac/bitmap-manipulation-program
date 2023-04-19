using System;
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
		SearchField.onValueChanged.AddListener(OnSearchFieldChanged);

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

	private void OnSearchFieldChanged(string arg0)
	{
		FilterActions();
	}

	private void FilterActions()
	{
		foreach (var button in ButtonsParent.GetComponentsInChildren<UIButton>(includeInactive: true))
		{
			button.gameObject.SetActive(button.TextMeshProLabel.text.Contains(SearchField.text, StringComparison.OrdinalIgnoreCase));
		}
	}

	private void OnEditImageHolder(EditImageHolder obj)
	{
		SelectedHolder = obj.ImageHolder;
		ImagePreview.texture = SelectedHolder.Texture;
		ImagePreview.texture.filterMode = FilterMode.Point;
		ImagePreviewFitter.aspectRatio = (float)SelectedHolder.Texture.width / SelectedHolder.Texture.height;
		SearchField.text = string.Empty;
		View.Show();
		FilterActions();
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
