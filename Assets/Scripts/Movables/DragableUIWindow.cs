using System.Collections.Generic;
using Doozy.Engine.UI;
using Sirenix.OdinInspector;
using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DragableUIWindow : SerializedMonoBehaviour
{
	private RectTransform ChildRectTransform;
	private RectTransform WindowRectTransform;
	private GameObject Window;
	private DragTarget self_DragTarget;
	
	private GameObject Arrow;

	public Color WindowColor
	{
		get => _windowColor;
		set
		{
			_windowColor = value;
			if (Window != null)
			{
				Window.GetComponent<Image>().color = value;
			}
		}
	}

	private Color _windowColor = new Color(0.5f,0.5f,0.5f,1f);
	
	public string WindowTitle
	{
		get => _windowTitle;
		set
		{
			_windowTitle = value;
			if (Window != null)
			{
				Window.GetComponentInChildren<TMP_Text>().text = value;
			}
		}
	}

	private string _windowTitle = string.Empty;

	[SerializeField] private bool IncludeEditButton;
	[SerializeField] private UnityEvent OnEditButton;
	[SerializeField] private Sprite CustomEditButtonSprite;
	
	
	private void Start()
	{
		CreateWindow();

		if (IncludeEditButton)
		{
			CreateEditButton();
		}
		
		CreateCloseButton();

		CreateWindowTitle();

		RegisterWindowAsDragTarget();
		
		Arrow = ArrowSpawner.Instance.SpawnArrow(WindowRectTransform);
	}

	private void CreateWindowTitle()
	{
		var titleGo = new GameObject($"Title - {gameObject.name}");
		var titleTransform = titleGo.AddComponent<RectTransform>();
		titleTransform.SetParent(WindowRectTransform);
		titleTransform.localScale = Vector3.one;
		titleTransform.pivot = new Vector2(0.5f, 1f);
		titleTransform.sizeDelta = new Vector2(0f, 0f);
		titleTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
		titleTransform.anchorMin = new Vector2(0f, 1f);
		titleTransform.anchorMax = new Vector2(1f, 1f);
		titleTransform.offsetMin = new Vector2(50f, 0f);
		titleTransform.offsetMax = new Vector2(-50f, 42f);
		titleTransform.anchoredPosition3D = new Vector3(0f, -5f, 0f);
		

		var titleText = titleGo.AddComponent<TextMeshProUGUI>();
		titleText.fontSizeMax = 72;
		titleText.fontSizeMin = 3;
		titleText.enableAutoSizing = true;
		titleText.alignment = TextAlignmentOptions.Center;
		titleText.verticalAlignment = VerticalAlignmentOptions.Geometry;
		titleText.color = Color.white;
		titleText.enableWordWrapping = true;
		titleText.overflowMode = TextOverflowModes.Masking;
		titleText.raycastTarget = false;
		
		titleText.text = WindowTitle;

		//buttonTransform.anchoredPosition3D = new Vector3(0, -4f, 0f);
	}

	private void RegisterWindowAsDragTarget()
	{
		self_DragTarget = new DragTarget(Window, OnDrag, DragTargetType.DraggableWindow);
		Messenger.Default.Publish(new RegisterDragTarget(self_DragTarget));
	}

	private void CreateWindow()
	{
		var yOffset = 50f;
		Vector2 tabOffset = new Vector2(2, yOffset + 2);
		ChildRectTransform = GetComponent<RectTransform>();
		Window = new GameObject($"Window - {gameObject.name}");
		WindowRectTransform = Window.AddComponent<RectTransform>();
		WindowRectTransform.SetParent(ChildRectTransform.parent);
		WindowRectTransform.localScale = Vector3.one;
		WindowRectTransform.sizeDelta = ChildRectTransform.sizeDelta + tabOffset;
		WindowRectTransform.anchoredPosition3D = ChildRectTransform.anchoredPosition3D;
		var windowImage = Window.AddComponent<Image>();
		windowImage.color = _windowColor;
		ChildRectTransform.SetParent(WindowRectTransform);
		ChildRectTransform.anchoredPosition3D = new Vector3(0f, -yOffset / 2f, 0f);
	}

	private void CreateCloseButton()
	{
		var buttonGo = new GameObject($"Close - {gameObject.name}");
		var buttonTransform = buttonGo.AddComponent<RectTransform>();
		buttonTransform.SetParent(WindowRectTransform);
		buttonTransform.localScale = Vector3.one;
		buttonTransform.sizeDelta = new Vector2(42f, 42f);
		buttonTransform.pivot = new Vector2(1f, 1f);
		buttonTransform.anchorMax = new Vector2(1f, 1f);
		buttonTransform.anchorMin = new Vector2(1f, 1f);
		buttonTransform.anchoredPosition3D = new Vector3(-4f, -4f, 0f);
		var buttonImage = buttonGo.AddComponent<Image>();
		buttonImage.sprite = Resources.Load<Sprite>("CloseButton");
		UIButton button = buttonGo.AddComponent<UIButton>();
		button.OnClick.OnTrigger.Event.AddListener(ExecuteClose);
		button.OnClick.Enabled = true;
		button.AllowMultipleClicks = false;
		button.DisableButtonBetweenClicksInterval = 1f;
		button.ButtonName = buttonGo.name;
		var colors = button.Button.colors;
		colors.highlightedColor = Color.red;
		button.Button.colors = colors;
	}

	private void CreateEditButton()
	{
		var buttonGo = new GameObject($"Edit - {gameObject.name}");
		var buttonTransform = buttonGo.AddComponent<RectTransform>();
		buttonTransform.SetParent(WindowRectTransform);
		buttonTransform.localScale = Vector3.one;
		buttonTransform.sizeDelta = new Vector2(42f, 42f);
		buttonTransform.pivot = new Vector2(0f, 1f);
		buttonTransform.anchorMax = new Vector2(0f, 1f);
		buttonTransform.anchorMin = new Vector2(0f, 1f);
		buttonTransform.anchoredPosition3D = new Vector3(4f, -4f, 0f);
		var buttonImage = buttonGo.AddComponent<Image>();
		buttonImage.sprite = CustomEditButtonSprite == null ? Resources.Load<Sprite>("EditButton") : CustomEditButtonSprite;
		UIButton button = buttonGo.AddComponent<UIButton>();
		button.OnClick.OnTrigger.Event.AddListener(ExecuteEdit);
		button.OnClick.Enabled = true;
		button.AllowMultipleClicks = false;
		button.DisableButtonBetweenClicksInterval = 1f;
		button.ButtonName = buttonGo.name;
		var colors = button.Button.colors;
		colors.highlightedColor = Color.yellow;
		button.Button.colors = colors;
	}

	private void ExecuteEdit()
	{
		OnEditButton?.Invoke();
	}

	public void ExecuteClose()
	{
		Destroy(Window);
	}

	private void OnDestroy()
	{
		Messenger.Default.Publish(new UnRegisterDragTarget(self_DragTarget));
		if (Arrow != null)
		{
			Arrow.SetActive(false);
			Destroy(Arrow);
		}
	}

	private void OnDrag(Vector2 dragValue, bool focus)
	{
		WindowRectTransform.position += new Vector3(dragValue.x, dragValue.y, 0f);
		if (focus && WindowRectTransform != null)
		{
			WindowRectTransform.SetAsLastSibling();
		}
	}
}
