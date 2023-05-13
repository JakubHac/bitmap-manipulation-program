using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
	[SerializeField] private RectTransform ArrowsParent;
	[SerializeField] private GameObject ArrowPrefab;
	[SerializeField] private Camera Camera;
	
	
	public static ArrowSpawner Instance;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("Multiple ArrowSpawner instances!");
			return;
		}
		Instance = this;
	}

	public GameObject SpawnArrow(RectTransform target)
	{
		var go= Instantiate(ArrowPrefab, ArrowsParent);
		var arrow = go.GetComponent<Arrow>();
		arrow.Camera = Camera;
		arrow.Target = target;
		return go;
	} 
	
}
