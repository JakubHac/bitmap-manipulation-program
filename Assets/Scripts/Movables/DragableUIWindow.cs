using System.Collections;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Animation;
using Sirenix.OdinInspector;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DragableUIWindow : SerializedMonoBehaviour
{
	private RectTransform ChildRectTransform;
	private RectTransform WindowRectTransform;
	private GameObject Window;
	private DragTarget self_DragTarget;

	[SerializeField] private bool IncludeEditButton;
	[SerializeField] private UnityEvent OnEditButton;
	
	private void Start()
	{
		CreateWindow();

		if (IncludeEditButton)
		{
			CreateEditButton();
		}
		
		CreateCloseButton();

		RegisterWindowAsDragTarget();
	}

	private void RegisterWindowAsDragTarget()
	{
		self_DragTarget = new DragTarget(Window, OnDrag);
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
		windowImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
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
		buttonImage.sprite = Resources.Load<Sprite>("EditButton");
		UIButton button = buttonGo.AddComponent<UIButton>();
		button.OnClick.OnTrigger.Event.AddListener(ExecuteEdit);
		button.OnClick.Enabled = true;
		button.AllowMultipleClicks = false;
		button.DisableButtonBetweenClicksInterval = 1f;
		button.ButtonName = buttonGo.name;
	}

	private void ExecuteEdit()
	{
		OnEditButton?.Invoke();
	}

	private void ExecuteClose()
	{
		Destroy(Window);
	}

	private void OnDestroy()
	{
		Messenger.Default.Publish(new UnRegisterDragTarget(self_DragTarget));
	}

	private void OnDrag(Vector2 dragValue)
	{
		WindowRectTransform.position += new Vector3(dragValue.x, dragValue.y, 0f);
	}
}
