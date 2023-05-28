using System.Collections;
using System.Globalization;
using Doozy.Engine.UI;
using OpenCvSharp;
using Sirenix.OdinInspector;
using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TwoStepView : SerializedMonoBehaviour
{
	[SerializeField] private UIView TwoStepUIView;

	#region FirstInputMaskField
	private TMP_InputField[,] FirstMaskTexts;
	[BoxGroup("First Mask")][SerializeField] private TMP_InputField FirstMaskX1Y1;
	[BoxGroup("First Mask")][SerializeField] private TMP_InputField FirstMaskX2Y1;
	[BoxGroup("First Mask")][SerializeField] private TMP_InputField FirstMaskX3Y1;
	[BoxGroup("First Mask")][SerializeField] private TMP_InputField FirstMaskX1Y2;
	[BoxGroup("First Mask")][SerializeField] private TMP_InputField FirstMaskX2Y2;
	[BoxGroup("First Mask")][SerializeField] private TMP_InputField FirstMaskX3Y2;
	[BoxGroup("First Mask")][SerializeField] private TMP_InputField FirstMaskX1Y3;
	[BoxGroup("First Mask")][SerializeField] private TMP_InputField FirstMaskX2Y3;
	[BoxGroup("First Mask")][SerializeField] private TMP_InputField FirstMaskX3Y3;
	private double[,] firstMask;
	#endregion
	
	#region FinalMaskFields
	private TMP_InputField[,] FinalMaskTexts;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX1Y1;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX2Y1;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX3Y1;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX4Y1;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX5Y1;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX1Y2;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX2Y2;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX3Y2;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX4Y2;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX5Y2;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX1Y3;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX2Y3;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX3Y3;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX4Y3;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX5Y3;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX1Y4;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX2Y4;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX3Y4;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX4Y4;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX5Y4;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX1Y5;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX2Y5;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX3Y5;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX4Y5;
	[BoxGroup("Final Mask")][SerializeField] private TMP_InputField FinalMaskX5Y5;
	private double[,] finalMask; 
	#endregion
	
	#region SecondInputMaskField
	private TMP_InputField[,] SecondMaskTexts;
	[BoxGroup("Second Mask")][SerializeField] private TMP_InputField SecondMaskX1Y1;
	[BoxGroup("Second Mask")][SerializeField] private TMP_InputField SecondMaskX2Y1;
	[BoxGroup("Second Mask")][SerializeField] private TMP_InputField SecondMaskX3Y1;
	[BoxGroup("Second Mask")][SerializeField] private TMP_InputField SecondMaskX1Y2;
	[BoxGroup("Second Mask")][SerializeField] private TMP_InputField SecondMaskX2Y2;
	[BoxGroup("Second Mask")][SerializeField] private TMP_InputField SecondMaskX3Y2;
	[BoxGroup("Second Mask")][SerializeField] private TMP_InputField SecondMaskX1Y3;
	[BoxGroup("Second Mask")][SerializeField] private TMP_InputField SecondMaskX2Y3;
	[BoxGroup("Second Mask")][SerializeField] private TMP_InputField SecondMaskX3Y3;
	private double[,] secondMask;
	#endregion
	
	private TwoStepRequest Request;
	private ImageHolder Source;

	private void Start()
	{
		FirstMaskTexts = new[,]
		{
			{ FirstMaskX1Y1, FirstMaskX2Y1, FirstMaskX3Y1 },
			{ FirstMaskX1Y2, FirstMaskX2Y2, FirstMaskX3Y2 },
			{ FirstMaskX1Y3, FirstMaskX2Y3, FirstMaskX3Y3 }
		};
		
		SecondMaskTexts = new[,]
		{
			{ SecondMaskX1Y1, SecondMaskX2Y1, SecondMaskX3Y1 },
			{ SecondMaskX1Y2, SecondMaskX2Y2, SecondMaskX3Y2 },
			{ SecondMaskX1Y3, SecondMaskX2Y3, SecondMaskX3Y3 }
		};
		
		FinalMaskTexts = new[,]
		{
			{ FinalMaskX1Y1, FinalMaskX2Y1, FinalMaskX3Y1, FinalMaskX4Y1, FinalMaskX5Y1 },
			{ FinalMaskX1Y2, FinalMaskX2Y2, FinalMaskX3Y2, FinalMaskX4Y2, FinalMaskX5Y2 },
			{ FinalMaskX1Y3, FinalMaskX2Y3, FinalMaskX3Y3, FinalMaskX4Y3, FinalMaskX5Y3 },
			{ FinalMaskX1Y4, FinalMaskX2Y4, FinalMaskX3Y4, FinalMaskX4Y4, FinalMaskX5Y4 },
			{ FinalMaskX1Y5, FinalMaskX2Y5, FinalMaskX3Y5, FinalMaskX4Y5, FinalMaskX5Y5 }
		};
		
		DefaultMasks();

		Messenger.Default.Subscribe<TwoStepRequest>(HandleRequest);
	}

	private void OnDestroy()
	{
		Messenger.Default.Unsubscribe<TwoStepRequest>(HandleRequest);
	}

	private void DefaultMasks()
	{
		firstMask = new[,]
		{
			{ 1.0, 1.0, 1.0 },
			{ 1.0, 1.0, 1.0 },
			{ 1.0, 1.0, 1.0 }
		};

		secondMask = new[,]
		{
			{ 1.0, 1.0, 1.0 },
			{ 1.0, 1.0, 1.0 },
			{ 1.0, 1.0, 1.0 }
		};

		CalculateFinalMask();
		SyncAllTexts();
	}

	private void HandleRequest(TwoStepRequest obj)
	{
		Request = obj;
		Source = obj.Source;
		DefaultMasks();
		TwoStepUIView.Show();
	}

	private void CalculateFinalMask()
	{
		finalMask = ImageActions.MixKernels(firstMask, secondMask);
	}

	private void SyncAllTexts()
	{
		SyncTextsToMask(FirstMaskTexts, firstMask);
		SyncTextsToMask(SecondMaskTexts, secondMask);
		SyncTextsToMask(FinalMaskTexts, finalMask);

		StartCoroutine(rebuildAfterFrames());
		IEnumerator rebuildAfterFrames()
		{
			for (int i = 0; i < 5; i++)
			{
				yield return null;
			}
			for (int i = 0; i < 5; i++)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
				yield return null;
			}
			
		}
	}

	private void SyncTextsToMask(TMP_InputField[,] texts, double[,] mask)
	{
		for (int i = 0; i < texts.GetLength(0); i++)
		{
			for (int j = 0; j < texts.GetLength(1); j++)
			{
				var text = texts[i, j];
				text.text = mask[i, j].ToString(CultureInfo.InvariantCulture);
				text.readOnly = true;
			}
		}
	}

	public void RequestChangeFirstMask()
	{
		TwoStepUIView.Hide();
		Messenger.Default.Publish(new MaskLibraryRequest(UpdateFirstMask));
	}
	
	public void RequestChangeSecondMask()
	{
		TwoStepUIView.Hide();
		Messenger.Default.Publish(new MaskLibraryRequest(UpdateSecondMask));
	}
	
	private void UpdateSecondMask(double[,] mask)
	{
		secondMask = mask;
		CalculateFinalMask();
		SyncAllTexts();
		TwoStepUIView.Show();
	}

	private void UpdateFirstMask(double[,] mask)
	{
		firstMask = mask;
		CalculateFinalMask();
		SyncAllTexts();
		TwoStepUIView.Show();
	}
	
	public void Accept()
	{
		Messenger.Default.Publish(new ImageReplaceOrNewEvent(Source.Texture,
			ImageActions.ConvolveTexture(Source, finalMask, BorderTypes.Reflect101), Source,
			Source.GetComponent<DragableUIWindow>().WindowTitle + " - Konwolucja 5x5"));
		Request = null;
		TwoStepUIView.Hide();
	}
}
