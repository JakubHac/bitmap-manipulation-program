using System;
using UnityEngine;

public class DragTarget
{
	public readonly GameObject[] GameObjects;
	public readonly Action<Vector2, bool> OnDrag;

	public DragTarget(GameObject[] gameObjects, Action<Vector2, bool> onDrag)
	{
		GameObjects = gameObjects;
		OnDrag = onDrag;
	}
	
	public DragTarget(GameObject gameObject, Action<Vector2, bool> onDrag)
	{
		GameObjects = new[] {gameObject};
		OnDrag = onDrag;
	}
}