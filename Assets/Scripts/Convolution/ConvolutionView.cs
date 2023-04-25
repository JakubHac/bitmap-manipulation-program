using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Doozy.Engine.UI;
using OpenCvSharp;
using Sirenix.OdinInspector;
using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;

public class ConvolutionView : SerializedMonoBehaviour
{
	[SerializeField] private UIView ConvolutionUIView;
	[SerializeField] private TMP_InputField MaskX1Y1;
	[SerializeField] private TMP_InputField MaskX2Y1;
	[SerializeField] private TMP_InputField MaskX3Y1;
	[SerializeField] private TMP_InputField MaskX1Y2;
	[SerializeField] private TMP_InputField MaskX2Y2;
	[SerializeField] private TMP_InputField MaskX3Y2;
	[SerializeField] private TMP_InputField MaskX1Y3;
	[SerializeField] private TMP_InputField MaskX2Y3;
	[SerializeField] private TMP_InputField MaskX3Y3;
	[SerializeField] private CanvasGroup MaskCanvasGroup;
	[SerializeField] private TMP_Dropdown OperationDopdown;
	[SerializeField] private TMP_Dropdown EdgeDetectionMethodDopdown;
	[SerializeField] private TMP_Dropdown EdgeDetectionDirectionDopdown;
	[SerializeField] private TMP_Dropdown EdgeDetectionLaplacianDopdown;
	[SerializeField] private TMP_Dropdown SharpenDropdown;
	[SerializeField] private TMP_Dropdown BlurDropdown;
	[SerializeField] private TMP_InputField CannyT1;
	[SerializeField] private TMP_InputField CannyT2;
	[SerializeField] private TMP_InputField CannySobel;

	private TMP_InputField[,] MaskTexts;
	private GameObject CannyPanel => CannyT1.transform.parent.gameObject;

	private ConvolutionOperation operation;
	private ConvolutionEdgeDetectMethod edgeDetectionMethod;
	private ConvolutionEdgeDetectDirection edgeDetectionDirection;
	private ConvolutionSharpenType sharpenMethod;
	private ConvolutionBlurType blurMethod;
	private ConvolutionLaplacianEdgeDetectionType LaplacianEdgeDetectionType;
	private BorderTypes BorderType = BorderTypes.Reflect101;
	private double CannyT1Value = 100.0;
	private double CannyT2Value = 200.0;
	private int CannySobelSize = 3;
	private bool CannyFast = true;

	private Queue<ConvolutionRequest> RequestQueue = new();
	private ConvolutionRequest CurrentRequest;
	private ImageHolder source => CurrentRequest.Source;

	private double[,] mask =
	{
		{ 0.0, 0.0, 0.0 },
		{ 0.0, 1.0, 0.0 },
		{ 0.0, 0.0, 0.0 }
	};
	
	bool uiChanged = false;
	bool dropdownChanged = false;

	private void Start()
	{
		MaskTexts = new[,]
		{
			{ MaskX1Y1, MaskX2Y1, MaskX3Y1 },
			{ MaskX1Y2, MaskX2Y2, MaskX3Y2 },
			{ MaskX1Y3, MaskX2Y3, MaskX3Y3 }
		};

		OperationDopdown.ClearOptions();
		OperationDopdown.AddOptions(Enum.GetValues(typeof(ConvolutionOperation)).Cast<ConvolutionOperation>()
			.Select(ConvolutionDropdowns.GetDropdownValue).ToList());
		OperationDopdown.RefreshShownValue();
		EdgeDetectionMethodDopdown.ClearOptions();
		EdgeDetectionMethodDopdown.AddOptions(Enum.GetValues(typeof(ConvolutionEdgeDetectMethod))
			.Cast<ConvolutionEdgeDetectMethod>().Select(ConvolutionDropdowns.GetDropdownValue).ToList());
		EdgeDetectionMethodDopdown.RefreshShownValue();
		EdgeDetectionDirectionDopdown.ClearOptions();
		EdgeDetectionDirectionDopdown.AddOptions(Enum.GetValues(typeof(ConvolutionEdgeDetectDirection))
			.Cast<ConvolutionEdgeDetectDirection>().Select(ConvolutionDropdowns.GetDropdownValue).ToList());
		EdgeDetectionDirectionDopdown.RefreshShownValue();
		EdgeDetectionLaplacianDopdown.ClearOptions();
		EdgeDetectionLaplacianDopdown.AddOptions(Enum.GetValues(typeof(ConvolutionLaplacianEdgeDetectionType))
			.Cast<ConvolutionLaplacianEdgeDetectionType>().Select(ConvolutionDropdowns.GetDropdownValue).ToList());
		EdgeDetectionLaplacianDopdown.RefreshShownValue();
		SharpenDropdown.ClearOptions();
		SharpenDropdown.AddOptions(Enum.GetValues(typeof(ConvolutionSharpenType)).Cast<ConvolutionSharpenType>()
			.Select(ConvolutionDropdowns.GetDropdownValue).ToList());
		SharpenDropdown.RefreshShownValue();
		BlurDropdown.ClearOptions();
		BlurDropdown.AddOptions(Enum.GetValues(typeof(ConvolutionBlurType)).Cast<ConvolutionBlurType>()
			.Select(ConvolutionDropdowns.GetDropdownValue).ToList());
		BlurDropdown.RefreshShownValue();
		
		OperationDopdown.onValueChanged.AddListener(_ =>
		{
			dropdownChanged = true;
			uiChanged = true;
		});
		EdgeDetectionMethodDopdown.onValueChanged.AddListener(_ =>
		{
			dropdownChanged = true;
			uiChanged = true;
		});
		EdgeDetectionDirectionDopdown.onValueChanged.AddListener(_ =>
		{
			dropdownChanged = true;
			uiChanged = true;
		});
		EdgeDetectionLaplacianDopdown.onValueChanged.AddListener(_ =>
		{
			dropdownChanged = true;
			uiChanged = true;
		});
		SharpenDropdown.onValueChanged.AddListener(_ =>
		{
			dropdownChanged = true;
			uiChanged = true;
		});
		BlurDropdown.onValueChanged.AddListener(_ =>
		{
			dropdownChanged = true;
			uiChanged = true;
		});
		CannyT1.onValueChanged.AddListener(_ => uiChanged = true);
		CannyT2.onValueChanged.AddListener(_ => uiChanged = true);
		CannySobel.onValueChanged.AddListener(_ => uiChanged = true);
		foreach (var maskText in MaskTexts)
		{
			maskText.onValueChanged.AddListener(_ => uiChanged = true);
		}

		SyncAllUI();

		Messenger.Default.Subscribe<ConvolutionRequest>(AddToQueue);
		
		uiChanged = true;
		dropdownChanged = true;
	}

	private void SyncValuesWithInputs()
	{
		operation = (ConvolutionOperation)OperationDopdown.value;
		edgeDetectionMethod = (ConvolutionEdgeDetectMethod)EdgeDetectionMethodDopdown.value;
		edgeDetectionDirection = (ConvolutionEdgeDetectDirection)EdgeDetectionDirectionDopdown.value;
		LaplacianEdgeDetectionType = (ConvolutionLaplacianEdgeDetectionType)EdgeDetectionLaplacianDopdown.value;
		sharpenMethod = (ConvolutionSharpenType)SharpenDropdown.value;
		blurMethod = (ConvolutionBlurType)BlurDropdown.value;
		
		TryReadValue(CannyT1.text, ref CannyT1Value, 0.0, 255.0);
		TryReadValue(CannyT2.text, ref CannyT2Value, 0.0, 255.0);
		if (CannyT2Value < CannyT1Value)
		{
			CannyT2Value = CannyT1Value;
		}
		TryReadValue(CannySobel.text, ref CannySobelSize, 3, int.MaxValue);
		if (CannySobelSize % 2 == 0)
		{
			CannySobelSize++;
		}
		
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				double value = mask[i, j];
				TryReadValue(MaskTexts[i, j].text, ref value, double.MinValue, double.MaxValue);
				mask[i, j] = value;
			}
		}
	}

	private void Update()
	{
		if (ConvolutionUIView.Visibility == VisibilityState.NotVisible) return;

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

	private void SyncAllUI()
	{
		SyncAllInputsToValues();
		SyncEnabledInputsWithValues();
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
				CannyPanel.SetActive(false);
				MaskCanvasGroup.alpha = 1.0f;
				break;
			case ConvolutionOperation.Blur:
				EdgeDetectionMethodDopdown.gameObject.SetActive(false);
				EdgeDetectionDirectionDopdown.gameObject.SetActive(false);
				EdgeDetectionLaplacianDopdown.gameObject.SetActive(false);
				SharpenDropdown.gameObject.SetActive(false);
				BlurDropdown.gameObject.SetActive(true);
				CannyPanel.SetActive(false);
				MaskCanvasGroup.alpha = 1.0f;
				break;
			case ConvolutionOperation.Sharpen:
				EdgeDetectionMethodDopdown.gameObject.SetActive(false);
				EdgeDetectionDirectionDopdown.gameObject.SetActive(false);
				EdgeDetectionLaplacianDopdown.gameObject.SetActive(false);
				SharpenDropdown.gameObject.SetActive(true);
				BlurDropdown.gameObject.SetActive(false);
				CannyPanel.SetActive(false);
				MaskCanvasGroup.alpha = 1.0f;
				break;
			case ConvolutionOperation.EdgeDetection:
				EdgeDetectionMethodDopdown.gameObject.SetActive(true);
				EdgeDetectionDirectionDopdown.gameObject.SetActive(
					edgeDetectionMethod is ConvolutionEdgeDetectMethod.Sobel or ConvolutionEdgeDetectMethod.Prewitt);
				EdgeDetectionLaplacianDopdown.gameObject.SetActive(
					edgeDetectionMethod is ConvolutionEdgeDetectMethod.Laplacian);
				SharpenDropdown.gameObject.SetActive(false);
				BlurDropdown.gameObject.SetActive(false);
				CannyPanel.SetActive(edgeDetectionMethod is ConvolutionEdgeDetectMethod.Canny);
				MaskCanvasGroup.alpha = edgeDetectionMethod == ConvolutionEdgeDetectMethod.Canny ? 0f : 1f;
				break;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns>if any changes were made</returns>
	private void SyncAllInputsToValues()
	{
		DropdownsFromSelectedValues();
		MaskTextsFromMask();
		CannyFromValues();
	}

	private void CannyFromValues()
	{
		double valueInsideText = 0.0;
		TryReadValue(CannyT1.text, ref valueInsideText, double.MinValue, double.MaxValue);
		CannyT1.text = CannyT1Value.ToString(CultureInfo.InvariantCulture);
		TryReadValue(CannyT2.text, ref valueInsideText, double.MinValue, double.MaxValue);
		CannyT2.text = CannyT2Value.ToString(CultureInfo.InvariantCulture);
		int valueInsideTextInt = 0;
		TryReadValue(CannySobel.text, ref valueInsideTextInt, int.MinValue, int.MaxValue);
		CannySobel.text = CannySobelSize.ToString();
	}

	private void DropdownsFromSelectedValues()
	{
		OperationDopdown.SetValueWithoutNotify((int)operation);
		EdgeDetectionMethodDopdown.SetValueWithoutNotify((int)edgeDetectionMethod);
		EdgeDetectionDirectionDopdown.SetValueWithoutNotify((int)edgeDetectionDirection);
		EdgeDetectionLaplacianDopdown.SetValueWithoutNotify((int)LaplacianEdgeDetectionType);
		SharpenDropdown.SetValueWithoutNotify((int)sharpenMethod);
		BlurDropdown.SetValueWithoutNotify((int)blurMethod);
	}

	private void SyncTextsOnChange()
	{
		var custom = operation == ConvolutionOperation.Custom;
		var tmpMask = GetMask();

		if (operation == ConvolutionOperation.EdgeDetection && edgeDetectionMethod == ConvolutionEdgeDetectMethod.Canny)
		{
			CannyT1.text = CannyT1Value.ToString(CultureInfo.InvariantCulture);
			CannyT2.text = CannyT2Value.ToString(CultureInfo.InvariantCulture);
			CannySobel.text = CannySobelSize.ToString(CultureInfo.InvariantCulture);
		}
		else
		{
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
	}

	private void MaskTextsFromMask()
	{
		for (int i = 0; i < MaskTexts.GetLength(0); i++)
		{
			for (int j = 0; j < MaskTexts.GetLength(1); j++)
			{
				var text = MaskTexts[i, j];
				double valueInText = 0;
				TryReadValue(text.text, ref valueInText, double.MinValue, double.MaxValue);
				text.text = mask[i, j].ToString(CultureInfo.InvariantCulture);
				text.readOnly = operation != ConvolutionOperation.Custom;
			}
		}
	}

	private void AddToQueue(ConvolutionRequest obj)
	{
		RequestQueue.Enqueue(obj);
		if (CurrentRequest == null)
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
		Messenger.Default.Unsubscribe<ConvolutionRequest>(AddToQueue);
	}

	private void TryReadValue(string textvalue, ref double target, double min, double max)
	{
		var value = ReadValue(textvalue, min, max);
		if (value is null || Math.Abs(target - value.Value) < double.Epsilon) return;
		target = value.Value;
	}

	private void TryReadValue(string textvalue, ref int target, int min, int max)
	{
		var value = ReadValue(textvalue, min, max);
		if (value is null || target != value.Value) return;
		target = value.Value;
	}

	private double? ReadValue(string text, double min, double max)
	{
		if (string.IsNullOrEmpty(text)) return null;
		IFormatProvider formatProvider = new CultureInfo("en-US");
		if (double.TryParse(text.Replace(",", "."), NumberStyles.Number, formatProvider, out double value))
		{
			if (value >= min && value <= max)
			{
				return value;
			}
		}

		return null;
	}

	private int? ReadValue(string text, int min, int max)
	{
		if (string.IsNullOrEmpty(text)) return null;
		if (int.TryParse(text, out int value))
		{
			if (value >= min && value <= max)
			{
				return value;
			}
		}

		return null;
	}

	public void AcceptConvolution()
	{
		SyncAllInputsToValues();
		if (operation == ConvolutionOperation.EdgeDetection && edgeDetectionMethod == ConvolutionEdgeDetectMethod.Canny)
		{
			Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture,
				ImageActions.CannyEdgeTexture(source, CannyT1Value, CannyT2Value, CannySobelSize, CannyFast), source,
				source.GetComponent<DragableUIWindow>().WindowTitle + " - Canny"));
		}
		else
		{
			double[,] kernel = operation == ConvolutionOperation.Custom ? mask : GetMask();
			Messenger.Default.Publish(new ImageReplaceOrNewEvent(source.Texture,
				ImageActions.ConvolveTexture(source, kernel, BorderType), source,
				source.GetComponent<DragableUIWindow>().WindowTitle + " - Konwolucja"));
		}

		CurrentRequest = null;

		if (RequestQueue.Count == 0)
		{
			ConvolutionUIView.Hide();
		}
		else
		{
			ShowNextRequest();
		}
	}

	private void ShowNextRequest()
	{
		ConvolutionUIView.Show();
		CurrentRequest = RequestQueue.Dequeue();
		operation = CurrentRequest.Operation;
		edgeDetectionMethod = CurrentRequest.EdgeDetectMethod;
		edgeDetectionDirection = CurrentRequest.EdgeDetectDirection;
		LaplacianEdgeDetectionType = CurrentRequest.LaplacianEdgeDetectionType;
		sharpenMethod = CurrentRequest.SharpenType;
		blurMethod = CurrentRequest.BlurType;
		CannyT1Value = 85;
		CannyT2Value = 255;
		CannySobelSize = 3;

		mask = GetMask();
		SyncAllUI();
	}

	private double[,] GetMask()
	{
		return ConvolutionMasks.GetMaskFromEnums(operation, blurMethod, edgeDetectionMethod, edgeDetectionDirection,
			sharpenMethod, LaplacianEdgeDetectionType);
	}
}