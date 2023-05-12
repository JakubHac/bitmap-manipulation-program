using System.Linq;
using Doozy.Engine.UI;
using OpenCvSharp;
using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;

public class MorphologyView : MonoBehaviour
{
	[SerializeField] private UIView MorphologyUIView;
	[SerializeField] private TMP_Dropdown OperationDopdown;
	[SerializeField] private TMP_Dropdown StructuringElementDopdown;
	[SerializeField] private TMP_Dropdown BorderTypeDropdown;
	
	private BorderTypes BorderType = BorderTypes.Reflect101;
	private bool AllNeigghbours = false;
	private MorphTypes MorphOperation = MorphTypes.Close;
	
	private MorphologyRequest CurrentRequest;
	
	private ImageHolder source => CurrentRequest.Source;
	
	bool dropdownChanged = false;

	private void SetupDropdowns()
	{
		BorderTypeDropdown.ClearOptions();
		BorderTypeDropdown.AddOptions(new[] { BorderTypes.Isolated, BorderTypes.Reflect, BorderTypes.Reflect101, BorderTypes.Replicate}.Select(MorphologyDropdowns.GetDropdownValue).ToList());
		BorderTypeDropdown.RefreshShownValue();
		BorderTypeDropdown.onValueChanged.AddListener(_ =>
		{
			dropdownChanged = true;
		});
		
		OperationDopdown.ClearOptions();
		OperationDopdown.AddOptions(new[] { MorphTypes.Skeletonize, MorphTypes.Close, MorphTypes.Open, MorphTypes.ERODE, MorphTypes.DILATE}.Select(MorphologyDropdowns.GetDropdownValue).ToList());
		OperationDopdown.RefreshShownValue();
		OperationDopdown.onValueChanged.AddListener(_ =>
		{
			dropdownChanged = true;
		});
		
		StructuringElementDopdown.ClearOptions();
		StructuringElementDopdown.AddOptions(new[] { true, false}.Select(MorphologyDropdowns.GetDropdownValue).ToList());
		StructuringElementDopdown.RefreshShownValue();
		StructuringElementDopdown.onValueChanged.AddListener(_ =>
		{
			dropdownChanged = true;
		});
		
		DropdownsFromSelectedValues();
		dropdownChanged = true;
	}

	private void Start()
	{
		SetupDropdowns();
		Messenger.Default.Subscribe<MorphologyRequest>(HandleRequest);
	}
	
	private void DropdownsFromSelectedValues()
	{
		OperationDopdown.SetValueWithoutNotify(MorphOperationToDropdownIndex());
		StructuringElementDopdown.SetValueWithoutNotify(AllNeigghbours ? 0 : 1);
		BorderTypeDropdown.SetValueWithoutNotify(BorderTypeToDropdownIndex());
	}
	
	private int MorphOperationToDropdownIndex()
	{
		switch (MorphOperation)
		{
			case MorphTypes.Skeletonize:
				return 0;
			case MorphTypes.Close:
				return 1;
			case MorphTypes.Open:
				return 2;
			case MorphTypes.ERODE:
				return 3;
			case MorphTypes.DILATE:
				return 4;
			default:
				return 0;
		}
	}
	
	private int BorderTypeToDropdownIndex()
	{
		switch (BorderType)
		{
			case BorderTypes.Isolated:
				return 0;
			case BorderTypes.Reflect:
				return 1;
			case BorderTypes.Reflect101:
				return 2;
			case BorderTypes.Replicate:
				return 3;
			default:
				return 0;
		}
	}

	private void HandleRequest(MorphologyRequest obj)
	{
		CurrentRequest = obj;
		MorphologyUIView.Show();
		MorphOperation = obj.MorphologyOperation;
		AllNeigghbours = false;
		BorderType = BorderTypes.Reflect101;
		
		DropdownsFromSelectedValues();
	}
	
	public void AfterClose()
	{
		CurrentRequest = null;
	}

	private void OnDestroy()
	{
		Messenger.Default.Unsubscribe<MorphologyRequest>(HandleRequest);
	}
	
	private void Update()
	{
		if (MorphologyUIView.Visibility == VisibilityState.NotVisible) return;

		if (dropdownChanged)
		{
			SyncValuesWithInputs();
			DropdownsFromSelectedValues();
			dropdownChanged = false;
		}
	}

	private void SyncValuesWithInputs()
	{
		BorderType = DropdownIndexToBorderType();
		MorphOperation = DropdownIndexToMorphOperation();
		AllNeigghbours = StructuringElementDopdown.value == 0;
	}
	
	private BorderTypes DropdownIndexToBorderType()
	{
		switch (BorderTypeDropdown.value)
		{
			case 0:
				return BorderTypes.Isolated;
			case 1:
				return BorderTypes.Reflect;
			case 2:
				return BorderTypes.Reflect101;
			case 3:
				return BorderTypes.Replicate;
			default:
				return BorderTypes.Isolated;
		}
	}
	
	private MorphTypes DropdownIndexToMorphOperation()
	{
		switch (OperationDopdown.value)
		{
			case 0:
				return MorphTypes.Skeletonize;
			case 1:
				return MorphTypes.Close;
			case 2:
				return MorphTypes.Open;
			case 3:
				return MorphTypes.ERODE;
			case 4:
				return MorphTypes.DILATE;
			default:
				return MorphTypes.Skeletonize;
		}
	}

	public void AcceptMorphology()
	{
		DropdownsFromSelectedValues();
		ImageActions.Morph(source, AllNeigghbours, BorderType, MorphOperation);
		CurrentRequest = null;
		MorphologyUIView.Hide();
	}
}
