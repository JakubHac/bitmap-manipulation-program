using UnityEngine;
using UnityEngine.UI;

public class BackgroundShaderOffset : MonoBehaviour
{
	[SerializeField] private Image Background;
	
	[SerializeField] private MovableUIManager movableUI;
	[SerializeField] private Vector2 OffsetMultiplier;
	[SerializeField] private float AllOffset;
	

	private int id;

	private void Start()
	{
		id = Shader.PropertyToID("_Offset");
	}

	private void LateUpdate()
	{
		Background.material.SetVector(id, new Vector2(movableUI.AllOffset.x * ((Screen.width / 10f * -OffsetMultiplier.x) / Screen.width), movableUI.AllOffset.y * ((Screen.height / 10f * OffsetMultiplier.y) / Screen.height)) * (Screen.height * AllOffset));
	}
}
