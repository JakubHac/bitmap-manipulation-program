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
	private const float HoldTimeThreshold = 0.5f;

	private RightClickTarget HoldTarget;
	private float holdTime;
	private bool HoldExecuted = false;
	private Vector2? lastMousePos;
	[ShowInInspector] [ReadOnly] private Vector2 holdDelta = Vector2.zero;
	[ShowInInspector] [ReadOnly] private Vector2 MousePos = Vector2.zero;
	[ShowInInspector] [ReadOnly] private GameObject LastDragTarget = null;
	[SerializeField] private GraphicRaycaster Raycaster;
	[FormerlySerializedAs("DebugUnderMouse")] [SerializeField] private bool DebugLastDragTarget;
	
	PointerEventData PointerEventData;

	List<DragTarget> DragTargets = new();
	List<RightClickTarget> RightClickTargets = new();

	private void Update()
	{
		Vector3 screenPoint = Input.mousePosition;
		screenPoint.z = ImageCanvasPlaneDistance;
		MousePos = MainCamera.ScreenToWorldPoint(screenPoint);
		if (lastMousePos == null)
		{
			lastMousePos = MousePos;
		}

		PointerEventData = new PointerEventData(EventSystem.current)
		{
			position = Input.mousePosition
		};
		List<RaycastResult> results = new List<RaycastResult>();
		if (LastDragTarget == null)
		{
			Raycaster.Raycast(PointerEventData, results);
		}
		
		if (results.Count > 0 && results[0].gameObject is { } hit)
		{
			if (Input.GetMouseButton(0))
			{
				if (LastDragTarget != null)
				{
					HandleDragSpecific(LastDragTarget);
				}
				else if (!HandleHold(hit))
				{
					HandleDragSpecific(hit);
				}
			}
			else if (Input.GetMouseButtonDown(1))
			{
				HandleRightClick(hit);
			}
		}
		else
		{
			if (Input.GetMouseButton(0))
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
			else
			{
				HoldExecuted = false;
				LastDragTarget = null;
			}
		}

		if (DebugLastDragTarget && LastDragTarget != null)
		{
			Debug.Log("Last Drag Target: " + LastDragTarget);
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

	private bool HandleHold(GameObject hit)
	{
		foreach (var rightClickTarget in RightClickTargets)
		{
			if (rightClickTarget.GameObjects.Any(x => x == hit))
			{
				if (HoldTarget != rightClickTarget)
				{
					HoldTarget = rightClickTarget;
					holdTime = 0f;
				}
				else
				{
					holdTime += Time.deltaTime;
					if (holdTime > HoldTimeThreshold && !HoldExecuted)
					{
						HoldExecuted = true;
						HoldTarget?.OnRightClick();
					}
				}
				return true;
			}
		}

		return false;
	}

	private void HandleRightClick(GameObject hit)
	{
		foreach (var rightClickTarget in RightClickTargets)
		{
			if (rightClickTarget.GameObjects.Any(x => x == hit))
			{
				rightClickTarget.OnRightClick();
				break;
			}
		}
	}

	private void OnEnable()
	{
		Messenger.Default.Subscribe<RegisterDragTarget>(HandleRegisterDragTarget);
		Messenger.Default.Subscribe<RegisterRightClickTarget>(HandleRegisterRightClickTarget);
		Messenger.Default.Subscribe<UnRegisterDragTarget>(HandleUnRegisterDragTarget);
		Messenger.Default.Subscribe<UnRegisterRightClickTarget>(HandleUnRegisterRightClickTarget);
	}

	private void HandleUnRegisterRightClickTarget(UnRegisterRightClickTarget obj)
	{
		RightClickTargets.Remove(obj.RightClickTarget);
	}

	private void HandleUnRegisterDragTarget(UnRegisterDragTarget obj)
	{
		DragTargets.Remove(obj.DragTarget);
	}

	private void HandleRegisterRightClickTarget(RegisterRightClickTarget registerRightClickTarget)
	{
		RightClickTargets.Add(registerRightClickTarget.RightClickTarget);
	}

	private void HandleRegisterDragTarget(RegisterDragTarget registerDragTarget)
	{
		DragTargets.Add(registerDragTarget.DragTarget);
	}

	private void OnDisable()
	{
		Messenger.Default.Unsubscribe<RegisterDragTarget>(HandleRegisterDragTarget);
		Messenger.Default.Unsubscribe<RegisterRightClickTarget>(HandleRegisterRightClickTarget);
		Messenger.Default.Unsubscribe<UnRegisterDragTarget>(HandleUnRegisterDragTarget);
		Messenger.Default.Unsubscribe<UnRegisterRightClickTarget>(HandleUnRegisterRightClickTarget);
	}
}