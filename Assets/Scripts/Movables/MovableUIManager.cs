using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class MovableUIManager : MonoBehaviour
{
	[Inject] Camera MainCamera;
	private const float ImageCanvasPlaneDistance = 99f;

	private Vector2? lastMousePos;
	[ShowInInspector] [ReadOnly] private Vector2 holdDelta = Vector2.zero;
	[ShowInInspector] [ReadOnly] private Vector2 MousePos = Vector2.zero;
	[ShowInInspector] [ReadOnly] private GameObject LastDragTarget = null;
	[SerializeField] private GraphicRaycaster Raycaster;

	[FormerlySerializedAs("DebugUnderMouse")] [SerializeField]
	private bool DebugLastDragTarget;

	PointerEventData PointerEventData;

	List<DragTarget> DragTargets = new();

	private void Update()
	{
		Vector3 screenPoint = Input.mousePosition;
		screenPoint.z = ImageCanvasPlaneDistance;
		MousePos = MainCamera.ScreenToWorldPoint(screenPoint);
		if (lastMousePos == null)
		{
			lastMousePos = MousePos;
		}

		if (Input.GetMouseButton(0))
		{
			List<RaycastResult> results = new List<RaycastResult>();
			if (LastDragTarget == null)
			{
				PointerEventData = new PointerEventData(EventSystem.current)
				{
					position = Input.mousePosition
				};
				Raycaster.Raycast(PointerEventData, results);
			}

			if (results.Count > 0 && results[0].gameObject is { } hit)
			{
				if (LastDragTarget != null)
				{
					HandleDragSpecific(LastDragTarget);
				}
				else
				{
					HandleDragSpecific(hit);
				}
			}
			else
			{
				if (LastDragTarget == null)
				{
					HandleDragAll();
				}
				else
				{
					HandleDragSpecific(LastDragTarget);
				}
			}
		}
		else
		{
			LastDragTarget = null;
		}

		lastMousePos = MousePos;
	}

	private void HandleDragAll()
	{
		var delta = new Vector2(MousePos.x - lastMousePos.Value.x, MousePos.y - lastMousePos.Value.y);
		foreach (var dragTarget in DragTargets)
		{
			dragTarget.OnDrag(delta);
		}
	}

	private void HandleDragSpecific(GameObject hit)
	{
		foreach (var dragTarget in DragTargets)
		{
			if (dragTarget.GameObjects.Any(x => x == hit))
			{
				var delta = new Vector2(MousePos.x - lastMousePos.Value.x, MousePos.y - lastMousePos.Value.y);
				dragTarget.OnDrag(delta);
				LastDragTarget = hit;
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
		DragTargets.Remove(obj.DragTarget);
	}

	private void HandleRegisterDragTarget(RegisterDragTarget registerDragTarget)
	{
		DragTargets.Add(registerDragTarget.DragTarget);
	}

	private void OnDisable()
	{
		Messenger.Default.Unsubscribe<RegisterDragTarget>(HandleRegisterDragTarget);
		Messenger.Default.Unsubscribe<UnRegisterDragTarget>(HandleUnRegisterDragTarget);
	}
}