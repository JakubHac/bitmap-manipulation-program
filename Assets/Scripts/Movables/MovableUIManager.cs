using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.UI;
using Sirenix.OdinInspector;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class MovableUIManager : SerializedMonoBehaviour
{
	[Inject] Camera MainCamera;
	private const float ImageCanvasPlaneDistance = 99f;

	private Vector2? lastMousePos;
	private Vector2 MousePos = Vector2.zero;
	private GameObject LastDragTargetGameObject = null;
	[SerializeField] private Dictionary<DragTargetType, GraphicRaycaster> RaycastersByType = new Dictionary<DragTargetType, GraphicRaycaster>();
	[SerializeField] private List<UIView> ForbiddenUIViews = new List<UIView>();
	[SerializeField] private UIView DrawProfileLineView;

	[FormerlySerializedAs("DebugUnderMouse")] [SerializeField]
	private bool DebugLastDragTarget;

	PointerEventData PointerEventData;
	
	Dictionary<DragTargetType, List<DragTarget>> DragTargetsByType = new Dictionary<DragTargetType, List<DragTarget>>();

	private void Awake()
	{
		foreach (DragTargetType dragTargetType in Enum.GetValues(typeof(DragTargetType)))
		{
			DragTargetsByType.Add(dragTargetType, new List<DragTarget>());
		}
	}

	private void Update()
	{
		if (!Application.isFocused || SimpleFileBrowser.FileBrowser.IsOpen)
		{
			lastMousePos = null;
			return;
		}
		
		Vector3 screenPoint = Input.mousePosition;
		screenPoint.z = ImageCanvasPlaneDistance;
		MousePos = MainCamera.ScreenToWorldPoint(screenPoint);

		if (lastMousePos == null)
		{
			lastMousePos = MousePos;
		}

		DragTargetType allowedDragTargetType = DragTargetType.NONE;

		if (ForbiddenUIViews.All(x => x.Visibility == VisibilityState.NotVisible))
		{
			allowedDragTargetType = DragTargetType.DraggableWindow;
		}
		else if (DrawProfileLineView.Visibility == VisibilityState.Visible)
		{
			allowedDragTargetType = DragTargetType.ProfileLinePoint;
		}

		if (allowedDragTargetType == DragTargetType.NONE)
		{
			LastDragTargetGameObject = null;
			lastMousePos = null;
			return;
		}

		List<DragTarget> allowedDragTargets = DragTargetsByType[allowedDragTargetType];

		if (!allowedDragTargets.Any(x => x.GameObjects.Contains(LastDragTargetGameObject)))
		{
			LastDragTargetGameObject = null;
		}

		if (Input.GetMouseButton(0))
		{
			List<RaycastResult> results = new List<RaycastResult>();
			if (LastDragTargetGameObject == null)
			{
				PointerEventData = new PointerEventData(EventSystem.current)
				{
					position = Input.mousePosition
				};
				RaycastersByType[allowedDragTargetType].Raycast(PointerEventData, results);
			}

			if (results.Count > 0 && results[0].gameObject is { } hit)
			{
				if (LastDragTargetGameObject != null)
				{
					HandleDragSpecific(LastDragTargetGameObject, allowedDragTargets);
				}
				else
				{
					HandleDragSpecific(hit, allowedDragTargets);
				}
			}
			else
			{
				if (LastDragTargetGameObject == null)
				{
					if (allowedDragTargetType == DragTargetType.DraggableWindow)
					{
						HandleDragAll(allowedDragTargets);
					}
				}
				else
				{
					HandleDragSpecific(LastDragTargetGameObject, allowedDragTargets);
				}
			}
		}
		else
		{
			LastDragTargetGameObject = null;
		}

		lastMousePos = MousePos;
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus)
		{
			lastMousePos = null;
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			lastMousePos = null;
		}
	}

	private void HandleDragAll(List<DragTarget> allowedDragTargets)
	{
		var delta = new Vector2(MousePos.x - lastMousePos.Value.x, MousePos.y - lastMousePos.Value.y);
		foreach (var dragTarget in allowedDragTargets)
		{
			dragTarget.OnDrag(delta, false);
		}
	}

	private void HandleDragSpecific(GameObject hit, List<DragTarget> allowedDragTargets)
	{
		foreach (var dragTarget in allowedDragTargets)
		{
			if (dragTarget.GameObjects.Any(x => x == hit))
			{
				var delta = new Vector2(MousePos.x - lastMousePos.Value.x, MousePos.y - lastMousePos.Value.y);
				dragTarget.OnDrag(delta, true);
				LastDragTargetGameObject = hit;
				break;
			}
		}
	}

	private void OnEnable()
	{
		Messenger.Default.Subscribe<RegisterDragTarget>(HandleRegisterDragTarget);
		Messenger.Default.Subscribe<UnRegisterDragTarget>(HandleUnRegisterDragTarget);
	}

	private void HandleUnRegisterDragTarget(UnRegisterDragTarget obj)
	{
		DragTargetsByType[obj.DragTarget.DragTargetType].Remove(obj.DragTarget);
	}

	private void HandleRegisterDragTarget(RegisterDragTarget registerDragTarget)
	{
		DragTargetsByType[registerDragTarget.DragTarget.DragTargetType].Add(registerDragTarget.DragTarget);
	}

	private void OnDisable()
	{
		Messenger.Default.Unsubscribe<RegisterDragTarget>(HandleRegisterDragTarget);
		Messenger.Default.Unsubscribe<UnRegisterDragTarget>(HandleUnRegisterDragTarget);
	}
}