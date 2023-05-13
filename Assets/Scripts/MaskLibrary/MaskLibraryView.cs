using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Doozy.Engine.UI;
using OpenCvSharp;
using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;

public class MaskLibraryView : MonoBehaviour
{
	[SerializeField] private UIView LibraryUIView;
	[SerializeField] private TMP_InputField MaskX1Y1;
	[SerializeField] private TMP_InputField MaskX2Y1;
	[SerializeField] private TMP_InputField MaskX3Y1;
	[SerializeField] private TMP_InputField MaskX1Y2;
	[SerializeField] private TMP_InputField MaskX2Y2;
	[SerializeField] private TMP_InputField MaskX3Y2;
	[SerializeField] private TMP_InputField MaskX1Y3;
	[SerializeField] private TMP_InputField MaskX2Y3;
	[SerializeField] private TMP_InputField MaskX3Y3;
	[SerializeField] private TMP_Dropdown OperationDopdown;
	[SerializeField] private TMP_Dropdown EdgeDetectionMethodDopdown;
	[SerializeField] private TMP_Dropdown EdgeDetectionDirectionDopdown;
	[SerializeField] private TMP_Dropdown EdgeDetectionLaplacianDopdown;
	[SerializeField] private TMP_Dropdown SharpenDropdown;
	[SerializeField] private TMP_Dropdown BlurDropdown;

	private TMP_InputField[,] MaskTexts;

	private ConvolutionOperation operation;
	private ConvolutionEdgeDetectMethod edgeDetectionMethod;
	private ConvolutionEdgeDetectDirection edgeDetectionDirection;
	private ConvolutionSharpenType sharpenMethod;
	private ConvolutionBlurType blurMethod;
	private ConvolutionLaplacianEdgeDetectionType LaplacianEdgeDetectionType;

	private Queue<MaskLibraryRequest> RequestQueue = new();
	private MaskLibraryRequest CurrentRequest;

	private double[,] mask =
	{
		{ 0.0, 0.0, 0.0 },
		{ 0.0, 1.0, 0.0 },
		{ 0.0, 0.0, 0.0 }
	};

	bool uiChanged = false;
	bool dropdownChanged = false;

	private void SetupDropdown<T>(TMP_Dropdown dropdown, T enumType) where T : Type
	{
		dropdown.ClearOptions();
		var values = Enum.GetValues(enumType);

		if (enumType == typeof(ConvolutionOperation))
		{
			dropdown.AddOptions(values.Cast<ConvolutionOperation>().Select(ConvolutionDropdowns.GetDropdownValue)
				.ToList());
		}
		else if (enumType == typeof(ConvolutionEdgeDetectMethod))
		{
			dropdown.AddOptions(
				new[]
				{
					ConvolutionEdgeDetectMethod.Sobel, ConvolutionEdgeDetectMethod.Prewitt,
					ConvolutionEdgeDetectMethod.Laplacian
				}.Select(ConvolutionDropdowns.GetDropdownValue).ToList());
		}
		else if (enumType == typeof(ConvolutionEdgeDetectDirection))
		{
			dropdown.AddOptions(values.Cast<ConvolutionEdgeDetectDirection>()
				.Select(ConvolutionDropdowns.GetDropdownValue).ToList());
		}
		else if (enumType == typeof(ConvolutionLaplacianEdgeDetectionType))
		{
			dropdown.AddOptions(values.Cast<ConvolutionLaplacianEdgeDetectionType>()
				.Select(ConvolutionDropdowns.GetDropdownValue).ToList());
		}
		else if (enumType == typeof(ConvolutionSharpenType))
		{
			dropdown.AddOptions(values.Cast<ConvolutionSharpenType>().Select(ConvolutionDropdowns.GetDropdownValue)
				.ToList());
		}
		else if (enumType == typeof(ConvolutionBlurType))
		{
			dropdown.AddOptions(values.Cast<ConvolutionBlurType>().Select(ConvolutionDropdowns.GetDropdownValue)
				.ToList());
		}
		else
		{
			throw new Exception($"Unknown enum type {enumType}");
		}

		dropdown.RefreshShownValue();
		dropdown.onValueChanged.AddListener(_ =>
		{
			dropdownChanged = true;
			uiChanged = true;
		});
	}

	private void Start()
	{
		MaskTexts = new[,]
		{
			{ MaskX1Y1, MaskX2Y1, MaskX3Y1 },
			{ MaskX1Y2, MaskX2Y2, MaskX3Y2 },
			{ MaskX1Y3, MaskX2Y3, MaskX3Y3 }
		};

		SetupDropdown(OperationDopdown, typeof(ConvolutionOperation));
		SetupDropdown(EdgeDetectionMethodDopdown, typeof(ConvolutionEdgeDetectMethod));
		SetupDropdown(EdgeDetectionDirectionDopdown, typeof(ConvolutionEdgeDetectDirection));
		SetupDropdown(EdgeDetectionLaplacianDopdown, typeof(ConvolutionLaplacianEdgeDetectionType));
		SetupDropdown(SharpenDropdown, typeof(ConvolutionSharpenType));
		SetupDropdown(BlurDropdown, typeof(ConvolutionBlurType));

		foreach (var maskText in MaskTexts)
		{
			maskText.onValueChanged.AddListener(_ => uiChanged = true);
		}

		SyncAllUI();

		Messenger.Default.Subscribe<MaskLibraryRequest>(AddToQueue);

		uiChanged = true;
		dropdownChanged = true;
	}

	private void AddToQueue(MaskLibraryRequest obj)
	{
		RequestQueue.Enqueue(obj);
		if (CurrentRequest == null)
		{
			ShowNextRequest();
		}
	}

	private void SyncAllUI()
	{
		SyncAllInputsToValues();
		SyncEnabledInputsWithValues();
	}

	private void SyncAllInputsToValues()
	{
		DropdownsFromSelectedValues();
		MaskTextsFromMask();
	}

	private void SyncEnabledInputsWithValues()
	{
		switch (operation)
		{
			case ConvolutionOperation.Custom:
				EdgeDetectionMethodDopdown.gameObject.SetActive(false);
				EdgeDetectionDirectionDopdown.gameObject.SetActive(false);
				EdgeDetectionLaplacianDopdown.gameObject.SetActive(false);
				SharpenDropdown.gameObject.SetActive(false);
				BlurDropdown.gameObject.SetActive(false);
				break;
			case ConvolutionOperation.Blur:
				EdgeDetectionMethodDopdown.gameObject.SetActive(false);
				EdgeDetectionDirectionDopdown.gameObject.SetActive(false);
				EdgeDetectionLaplacianDopdown.gameObject.SetActive(false);
				SharpenDropdown.gameObject.SetActive(false);
				BlurDropdown.gameObject.SetActive(true);
				break;
			case ConvolutionOperation.Sharpen:
				EdgeDetectionMethodDopdown.gameObject.SetActive(false);
				EdgeDetectionDirectionDopdown.gameObject.SetActive(false);
				EdgeDetectionLaplacianDopdown.gameObject.SetActive(false);
				SharpenDropdown.gameObject.SetActive(true);
				BlurDropdown.gameObject.SetActive(false);
				break;
			case ConvolutionOperation.EdgeDetection:
				EdgeDetectionMethodDopdown.gameObject.SetActive(true);
				EdgeDetectionDirectionDopdown.gameObject.SetActive(
					edgeDetectionMethod is ConvolutionEdgeDetectMethod.Sobel or ConvolutionEdgeDetectMethod.Prewitt);
				EdgeDetectionLaplacianDopdown.gameObject.SetActive(
					edgeDetectionMethod is ConvolutionEdgeDetectMethod.Laplacian);
				SharpenDropdown.gameObject.SetActive(false);
				BlurDropdown.gameObject.SetActive(false);
				break;
		}
	}

	private void DropdownsFromSelectedValues()
	{
		OperationDopdown.SetValueWithoutNotify((int)operation);
		EdgeDetectionMethodDopdown.SetValueWithoutNotify(EdgeDetectionMethodToDropdownIndex());
		EdgeDetectionDirectionDopdown.SetValueWithoutNotify((int)edgeDetectionDirection);
		EdgeDetectionLaplacianDopdown.SetValueWithoutNotify((int)LaplacianEdgeDetectionType);
		SharpenDropdown.SetValueWithoutNotify((int)sharpenMethod);
		BlurDropdown.SetValueWithoutNotify((int)blurMethod);
	}
	//{ ConvolutionEdgeDetectMethod.Sobel, ConvolutionEdgeDetectMethod.Prewitt, ConvolutionEdgeDetectMethod.Laplacian

	private int EdgeDetectionMethodToDropdownIndex()
	{
		switch (edgeDetectionMethod)
		{
			case ConvolutionEdgeDetectMethod.Sobel:
				return 0;
			case ConvolutionEdgeDetectMethod.Prewitt:
				return 1;
			case ConvolutionEdgeDetectMethod.Laplacian:
				return 2;
			default:
				return 0;
		}
	}

	private ConvolutionEdgeDetectMethod DropdownIndexToEdgeDetectionMethod()
	{
		switch (EdgeDetectionMethodDopdown.value)
		{
			case 0:
				return ConvolutionEdgeDetectMethod.Sobel;
			case 1:
				return ConvolutionEdgeDetectMethod.Prewitt;
			case 2:
				return ConvolutionEdgeDetectMethod.Laplacian;
			default:
				return ConvolutionEdgeDetectMethod.Sobel;
		}
	}

	private void MaskTextsFromMask()
	{
		for (int i = 0; i < MaskTexts.GetLength(0); i++)
		{
			for (int j = 0; j < MaskTexts.GetLength(1); j++)
			{
				var text = MaskTexts[i, j];
				double valueInText = 0;
				ConvolutionView.TryReadValue(text.text, ref valueInText, double.MinValue, double.MaxValue);
				text.text = mask[i, j].ToString(CultureInfo.InvariantCulture);
				text.readOnly = operation != ConvolutionOperation.Custom;
			}
		}
	}

	private void ShowNextRequest()
	{
		LibraryUIView.Show();
		CurrentRequest = RequestQueue.Dequeue();
		operation = ConvolutionOperation.Custom;
		edgeDetectionMethod = ConvolutionEdgeDetectMethod.Sobel;
		edgeDetectionDirection = ConvolutionEdgeDetectDirection.North;
		LaplacianEdgeDetectionType = ConvolutionLaplacianEdgeDetectionType.FourConnected;
		sharpenMethod = ConvolutionSharpenType.Laplacian1;
		blurMethod = ConvolutionBlurType.All;

		mask = GetMask();
		SyncAllUI();
	}

	private double[,] GetMask()
	{
		return ConvolutionMasks.GetMaskFromEnums(operation, blurMethod, edgeDetectionMethod, edgeDetectionDirection,
			sharpenMethod, LaplacianEdgeDetectionType);
	}

	public void AcceptMask()
	{
		SyncAllInputsToValues();

		double[,] kernel = operation == ConvolutionOperation.Custom ? mask : GetMask();

		CurrentRequest.Callback(kernel);

		CurrentRequest = null;

		if (RequestQueue.Count == 0)
		{
			LibraryUIView.Hide();
		}
		else
		{
			ShowNextRequest();
		}
	}

	public void AfterClose()
	{
		CurrentRequest = null;
	}

	private void OnDestroy()
	{
		Messenger.Default.Unsubscribe<MaskLibraryRequest>(AddToQueue);
	}

	private void Update()
	{
		if (LibraryUIView.Visibility == VisibilityState.NotVisible) return;

		if (uiChanged)
		{
			SyncValuesWithInputs();
			SyncEnabledInputsWithValues();
			if (dropdownChanged)
			{
				SyncTextsOnChange();
				dropdownChanged = false;
			}

			uiChanged = false;
		}
	}

	private void SyncTextsOnChange()
	{
		var custom = operation == ConvolutionOperation.Custom;
		var tmpMask = GetMask();


		for (int i = 0; i < MaskTexts.GetLength(0); i++)
		{
			for (int j = 0; j < MaskTexts.GetLength(1); j++)
			{
				var text = MaskTexts[i, j];
				text.readOnly = !custom;
				if (!custom)
				{
					text.text = tmpMask[i, j].ToString(CultureInfo.InvariantCulture);
				}
			}
		}
	}

	private void SyncValuesWithInputs()
	{
		operation = (ConvolutionOperation)OperationDopdown.value;
		edgeDetectionMethod = DropdownIndexToEdgeDetectionMethod();
		edgeDetectionDirection = (ConvolutionEdgeDetectDirection)EdgeDetectionDirectionDopdown.value;
		LaplacianEdgeDetectionType = (ConvolutionLaplacianEdgeDetectionType)EdgeDetectionLaplacianDopdown.value;
		sharpenMethod = (ConvolutionSharpenType)SharpenDropdown.value;
		blurMethod = (ConvolutionBlurType)BlurDropdown.value;

		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				double value = mask[i, j];
				ConvolutionView.TryReadValue(MaskTexts[i, j].text, ref value, double.MinValue, double.MaxValue);
				mask[i, j] = value;
			}
		}
	}
}