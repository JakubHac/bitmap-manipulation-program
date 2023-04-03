using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveableUI : SerializedMonoBehaviour
{
	private const float HoldTimeThreshold = 0.5f;
	private const float HoldDeltaThreshold = 0.2f;

	private float holdTime = 0f;
	private Vector2? lastMousePos = null;
	[ShowInInspector] [ReadOnly] private Vector2 holdDelta = Vector2.zero;
	[ShowInInspector] [ReadOnly] private Vector2 MousePos = Vector2.zero;
	private RectTransform ChildRectTransform;
	private RectTransform WindowRectTransform;
	private GameObject Window;
	
	private GraphicRaycaster Raycaster;
	PointerEventData PointerEventData;
	[ShowInInspector] [ReadOnly] private MovableUIState State = MovableUIState.Idle;
	bool holdClicked = false;

	public enum MovableUIState
	{
		Idle,
		Dragged,
		Hold
	}

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
		var canvas = Window.AddComponent<Canvas>();
		Raycaster = Window.AddComponent<GraphicRaycaster>();
		var windowImage = Window.AddComponent<Image>();
		windowImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		ChildRectTransform.SetParent(WindowRectTransform);
		ChildRectTransform.anchoredPosition3D = new Vector3(0f, -yOffset/2f, 0f);
	}

	private void Update()
	{
		Vector3 screenPoint = Input.mousePosition;
		screenPoint.z = 99f; //distance of the plane from the camera
		MousePos = Camera.main.ScreenToWorldPoint(screenPoint);

		bool mouseOverUs = false;
		
		PointerEventData = new PointerEventData(EventSystem.current);
		PointerEventData.position = Input.mousePosition;
		List<RaycastResult> results = new List<RaycastResult>();
		Raycaster.Raycast(PointerEventData, results);

		if (results.Count > 0)
		{
			if (results[0].gameObject != null)
			{
				if (results[0].gameObject == Window)
				{
					mouseOverUs = true;
				}
				Debug.Log("Mouse over " + results[0].gameObject.name);
			}
		}
		

		switch (State)
		{
			case MovableUIState.Idle:
				HandleIdleState(mouseOverUs);
				break;
			case MovableUIState.Dragged:
				HandleDraggedState(mouseOverUs);
				break;
			case MovableUIState.Hold:
				HandleHoldState(mouseOverUs);
				break;
		}
	}

	private void HandleHoldState(bool mouseOverUs)
	{
		//TODO Invoke actions menu
		if (!holdClicked)
		{
			Debug.Log("Hold");
			holdClicked = true;
		}

		if (!Input.GetMouseButton(0))
		{
			holdTime = 0f;
			holdDelta = Vector2.zero;
			lastMousePos = null;
			holdClicked = false;
			State = MovableUIState.Idle;
		}
	}

	private void HandleDraggedState(bool mouseOverUs)
	{
		holdTime = 0f;
		if (Input.GetMouseButton(0))
		{
			if (lastMousePos == null)
			{
				lastMousePos = MousePos;
			}

			holdDelta += new Vector2(MousePos.x - lastMousePos.Value.x, MousePos.y - lastMousePos.Value.y);
			WindowRectTransform.position += new Vector3(holdDelta.x, holdDelta.y, 0f);
			holdDelta = Vector2.zero;
			lastMousePos = MousePos;
		}
		else
		{
			State = MovableUIState.Idle;
		}
	}

	private void HandleIdleState(bool mouseOverUs)
	{
		if (mouseOverUs && Input.GetMouseButtonDown(1))
		{
			State = MovableUIState.Hold;
			return;
		}

		if (mouseOverUs && Input.GetMouseButton(0))
		{
			holdTime += Time.deltaTime;
			if (lastMousePos == null)
			{
				lastMousePos = MousePos;
			}

			holdDelta += new Vector2(MousePos.x - lastMousePos.Value.x, MousePos.y - lastMousePos.Value.y);
			lastMousePos = MousePos;

			if (holdDelta.magnitude > HoldDeltaThreshold)
			{
				State = MovableUIState.Dragged;
			}

			if (holdTime > HoldTimeThreshold)
			{
				if (holdDelta.magnitude < HoldDeltaThreshold)
				{
					State = MovableUIState.Hold;
				}
				else
				{
					State = MovableUIState.Dragged;
				}
			}
		}
		else
		{
			holdTime = 0f;
			holdDelta = Vector2.zero;
			lastMousePos = null;
			State = MovableUIState.Idle;
		}
	}
}
