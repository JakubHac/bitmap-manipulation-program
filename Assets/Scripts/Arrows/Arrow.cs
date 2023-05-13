using UnityEngine;

public class Arrow : MonoBehaviour
{

	public Camera Camera;
	[SerializeField] private RectTransform RectTransform;
	[SerializeField] private CanvasGroup CanvasGroup;
	public RectTransform Target;
	
	private void Start()
	{
		RectTransform = GetComponent<RectTransform>();
	}
	
	private bool WorldPointOutsideScreen(Vector3 point)
	{
		var viewportPoint = Camera.WorldToViewportPoint(point);
		return viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1;
	}

	private bool AllTargetPointsOutsideScreen()
	{
		Vector3[] points = new Vector3[4];
		Target.GetWorldCorners(points);
		foreach (var point in points)
		{
			if (!WorldPointOutsideScreen(point))
			{
				return false;
			}
		}
		return true;
	}

	private void LateUpdate()
	{
		if (AllTargetPointsOutsideScreen())
		{
			CanvasGroup.alpha = 1f;
			var direction = (((Vector2)(Target.position - Camera.transform.position)).normalized / new Vector2(Screen.width, Screen.height)).normalized;
			var rot = RectTransform.localEulerAngles;
			rot.z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			RectTransform.localEulerAngles = rot;
			var squarePos = CircleToSquare(direction) / 2f + new Vector2(0.5f, 0.5f);
			RectTransform.anchorMin = squarePos;
			RectTransform.anchorMax = squarePos;
		}
		else
		{
			CanvasGroup.alpha = 0f;
		}
	}
	
	private Vector2 CircleToSquare(Vector2 point)
	{
		var x = point.x;
		var y = point.y;
		return x*x >= y*y ? new Vector2(Mathf.Sign(x), y /x * Mathf.Sign(x)) : new Vector2(x / y * Mathf.Sign(y), Mathf.Sign(y));
	}
}
