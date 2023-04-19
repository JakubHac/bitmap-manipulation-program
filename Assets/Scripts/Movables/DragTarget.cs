using System;
using UnityEngine;

public class DragTarget
{
	public readonly GameObject[] GameObjects;
	public readonly Action<Vector2, bool> OnDrag;
	public readonly DragTargetType DragTargetType;

	public DragTarget(GameObject[] gameObjects, Action<Vector2, bool> onDrag, DragTargetType type)
	{
		GameObjects = gameObjects;
		OnDrag = onDrag;
		DragTargetType = type;
	}
	
	public DragTarget(GameObject gameObject, Action<Vector2, bool> onDrag, DragTargetType type)
	{
		GameObjects = new[] {gameObject};
		OnDrag = onDrag;
		DragTargetType = type;
	}
}