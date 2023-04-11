using Sirenix.OdinInspector;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class DragableUIWindow : SerializedMonoBehaviour
{
	
	private RectTransform ChildRectTransform;
	private RectTransform WindowRectTransform;
	private GameObject Window;

	private DragTarget self_DragTarget;
	
	
	private void Start()
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
		ChildRectTransform.anchoredPosition3D = new Vector3(0f, -yOffset/2f, 0f);
		
		self_DragTarget = new DragTarget(Window, OnDrag);
		Messenger.Default.Publish(new RegisterDragTarget(self_DragTarget));
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
