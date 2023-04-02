using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageHolder : SerializedMonoBehaviour
{
	[SerializeField] private RawImage Image;

	private const float HoldTimeThreshold = 0.5f;
	private const float HoldDeltaThreshold = 0.2f;

	private float holdTime = 0f;
	private Vector2? lastMousePos = null;
	[ShowInInspector] [ReadOnly] private Vector2 holdDelta = Vector2.zero;
	[ShowInInspector] [ReadOnly] private Vector2 MousePos = Vector2.zero;
	public RectTransform RectTransform;
	public GraphicRaycaster Raycaster;
	PointerEventData PointerEventData;
	[ShowInInspector] [ReadOnly] private ImageHolderState State = ImageHolderState.Idle;
	bool holdClicked = false;

	public enum ImageHolderState
	{
		Idle,
		Dragged,
		Hold
	}

	public Texture2D Texture
	{
		get => (Texture2D)Image.texture;
		set
		{
			if (value == null || value.height == 0 || value.width == 0)
			{
				Debug.Log("Texture is null or size 0");
				Destroy(gameObject);
				gameObject.SetActive(false);
			}
			else
			{
				Image.texture = value;
			}
		}
	}

	private void Start()
	{
		RectTransform = GetComponent<RectTransform>();
		Raycaster = GetComponent<GraphicRaycaster>();
		RectTransform.sizeDelta = new Vector2(Texture.width, Texture.height);
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
		
		foreach (RaycastResult result in results)
		{
			if (result.gameObject == gameObject)
			{
				mouseOverUs = true;
				break;
			}
		}

		switch (State)
		{
			case ImageHolderState.Idle:
				HandleIdleState(mouseOverUs);
				break;
			case ImageHolderState.Dragged:
				HandleDraggedState(mouseOverUs);
				break;
			case ImageHolderState.Hold:
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
			State = ImageHolderState.Idle;
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
			RectTransform.position += new Vector3(holdDelta.x, holdDelta.y, 0f);
			holdDelta = Vector2.zero;
			lastMousePos = MousePos;
		}
		else
		{
			State = ImageHolderState.Idle;
		}
	}

	private void HandleIdleState(bool mouseOverUs)
	{
		if (mouseOverUs && Input.GetMouseButtonDown(1))
		{
			State = ImageHolderState.Hold;
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
				State = ImageHolderState.Dragged;
			}

			if (holdTime > HoldTimeThreshold)
			{
				if (holdDelta.magnitude < HoldDeltaThreshold)
				{
					State = ImageHolderState.Hold;
				}
				else
				{
					State = ImageHolderState.Dragged;
				}
			}
		}
		else
		{
			holdTime = 0f;
			holdDelta = Vector2.zero;
			lastMousePos = null;
			State = ImageHolderState.Idle;
		}
	}
}