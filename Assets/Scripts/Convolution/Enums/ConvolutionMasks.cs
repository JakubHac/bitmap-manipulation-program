public static class ConvolutionMasks
{
	public static double[][] GetMaskFromEnums(ConvolutionOperation operation, ConvolutionBlurType blur,
		ConvolutionEdgeDetectMethod edgeDetectMethod, ConvolutionEdgeDetectDirection edgeDetectDirection,
		ConvolutionSharpenType sharpen, ConvolveLaplacianEdgeDetectionType laplacianEdgeDetectionType)
	{
		switch (operation)
		{
			case ConvolutionOperation.Custom:
				return new[]
				{
					new[] { 0.0, 0.0, 0.0 },
					new[] { 0.0, 1.0, 0.0 },
					new[] { 0.0, 0.0, 0.0 }
				};
			case ConvolutionOperation.Blur:
				return HandleBlur(blur);
			case ConvolutionOperation.Sharpen:
				return HandleSharpen(sharpen);
			case ConvolutionOperation.EdgeDetection:
				return HandleEdgeDetection(edgeDetectMethod, edgeDetectDirection, laplacianEdgeDetectionType);
			default:
				goto case ConvolutionOperation.Custom;
		}
	}

	private static double[][] HandleEdgeDetection(ConvolutionEdgeDetectMethod edgeDetectMethod,
		ConvolutionEdgeDetectDirection edgeDetectDirection,
		ConvolveLaplacianEdgeDetectionType laplacianEdgeDetectionType)
	{
		return edgeDetectMethod switch
		{
			ConvolutionEdgeDetectMethod.Sobel => HandleSobelEdgeDetect(edgeDetectDirection),
			ConvolutionEdgeDetectMethod.Laplacian => HandleLaplacianEdgeDetect(laplacianEdgeDetectionType),
			ConvolutionEdgeDetectMethod.Canny =>
				new[]
				{
					new[] { 100.0, 200.0 }
				},
			ConvolutionEdgeDetectMethod.Prewitt => HandlePrewittEdgeDetect(edgeDetectDirection),
			_ => new[]
			{
				new[] { 0.0, 0.0, 0.0 },
				new[] { 0.0, 1.0, 0.0 },
				new[] { 0.0, 0.0, 0.0 }
			},
		};
	}

	private static double[][] HandlePrewittEdgeDetect(ConvolutionEdgeDetectDirection edgeDetectDirection)
	{
		return edgeDetectDirection switch
		{
			ConvolutionEdgeDetectDirection.North => new[]
			{
				new[] { 1.0, 1.0, 1.0 },
				new[] { 0.0, 0.0, 0.0 },
				new[] { -1.0, -1.0, -1.0 }
			},
			ConvolutionEdgeDetectDirection.South => new[]
			{
				new[] { -1.0, -1.0, -1.0 },
				new[] { 0.0, 0.0, 0.0 },
				new[] { 1.0, 1.0, 1.0 }
			},
			ConvolutionEdgeDetectDirection.East => new[]
			{
				new[] { -1.0, 0.0, 1.0 },
				new[] { -1.0, 0.0, 1.0 },
				new[] { -1.0, 0.0, 1.0 }
			},
			ConvolutionEdgeDetectDirection.West => new[]
			{
				new[] { 1.0, 0.0, -1.0 },
				new[] { 1.0, 0.0, -1.0 },
				new[] { 1.0, 0.0, -1.0 }
			},
			ConvolutionEdgeDetectDirection.NorthEast => new[]
			{
				new[] { 0.0, 1.0, 1.0 },
				new[] { -1.0, 0.0, 1.0 }, 
				new[] { -1.0, -1.0, 0.0 }
			},
			ConvolutionEdgeDetectDirection.NorthWest => new[]
			{
				new[] { 0.0, 1.0, 1.0 }, 
				new[] { -1.0, 0.0, 1.0 }, 
				new[] { -1.0, -1.0, 0.0 }
			},
			ConvolutionEdgeDetectDirection.SouthEast => new[]
			{
				new[] { -1.0, -1.0, 0.0 }, 
				new[] { -1.0, 0.0, 1.0 }, 
				new[] { 0.0, 1.0, 1.0 }
			},
			ConvolutionEdgeDetectDirection.SouthWest => new[]
			{
				new[] { 0.0, -1.0, -1.0 }, 
				new[] { 1.0, 0.0, -1.0 }, 
				new[] { 1.0, 1.0, 0.0 }
			},
			_ => new[]
			{
				new[] { 0.0, 0.0, 0.0 },
				new[] { 0.0, 1.0, 0.0 },
				new[] { 0.0, 0.0, 0.0 }
			},
		};
	}

	private static double[][] HandleLaplacianEdgeDetect(ConvolveLaplacianEdgeDetectionType laplacianEdgeDetectionType)
	{
		return laplacianEdgeDetectionType switch
		{
			ConvolveLaplacianEdgeDetectionType.FourConnected => new[]
			{
				new[] { 0.0, -1.0, 0.0 },
				new[] { -1.0, 4.0, -1.0 },
				new[] { 0.0, -1.0, 0.0 }
			},
			ConvolveLaplacianEdgeDetectionType.EightConnected => new[]
			{
				new[] { -1.0, -1.0, -1.0 },
				new[] { -1.0, 8.0, -1.0 },
				new[] { -1.0, -1.0, -1.0 }
			},
			_ => new[]
			{
				new[] { 0.0, 0.0, 0.0 },
				new[] { 0.0, 1.0, 0.0 },
				new[] { 0.0, 0.0, 0.0 }
			},
		};
	}

	private static double[][] HandleSobelEdgeDetect(ConvolutionEdgeDetectDirection edgeDetectDirection)
	{
		return edgeDetectDirection switch
		{
			ConvolutionEdgeDetectDirection.North => new[]
			{
				new[] { 1.0, 2.0, 1.0 },
				new[] { 0.0, 0.0, 0.0 },
				new[] { -1.0, -2.0, -1.0 }
			},
			ConvolutionEdgeDetectDirection.South => new[]
			{
				new[] { -1.0, -2.0, -1.0 },
				new[] { 0.0, 0.0, 0.0 },
				new[] { 1.0, 2.0, 1.0 }
			},
			ConvolutionEdgeDetectDirection.East => new[]
			{
				new[] { -1.0, 0.0, 1.0 },
				new[] { -2.0, 0.0, 2.0 },
				new[] { -1.0, 0.0, 1.0 }
			},
			ConvolutionEdgeDetectDirection.West => new[]
			{
				new[] { 1.0, 0.0, -1.0 },
				new[] { 2.0, 0.0, -2.0 },
				new[] { 1.0, 0.0, -1.0 }
			},
			ConvolutionEdgeDetectDirection.NorthEast => new[]
			{
				new[] { 0.0, 1.0, 2.0 },
				new[] { -1.0, 0.0, 1.0 }, 
				new[] { -2.0, -1.0, 0.0 }
			},
			ConvolutionEdgeDetectDirection.NorthWest => new[]
			{
				new[] { 0.0, 1.0, 2.0 }, 
				new[] { -1.0, 0.0, 1.0 }, 
				new[] { -2.0, -1.0, 0.0 }
			},
			ConvolutionEdgeDetectDirection.SouthEast => new[]
			{
				new[] { -2.0, -1.0, 0.0 }, 
				new[] { -1.0, 0.0, 1.0 }, 
				new[] { 0.0, 1.0, 2.0 }
			},
			ConvolutionEdgeDetectDirection.SouthWest => new[]
			{
				new[] { 0.0, -1.0, -2.0 }, 
				new[] { 1.0, 0.0, -1.0 }, 
				new[] { 2.0, 1.0, 0.0 }
			},
			_ => new[]
			{
				new[] { 0.0, 0.0, 0.0 },
				new[] { 0.0, 1.0, 0.0 },
				new[] { 0.0, 0.0, 0.0 }
			},
		};
	}

	private static double[][] HandleSharpen(ConvolutionSharpenType sharpen)
	{
		return sharpen switch
		{
			ConvolutionSharpenType.Laplacian1 => new[]
			{
				new[] { 0.0, -1.0, 0.0 }, 
				new[] { -1.0, 5.0, -1.0 }, 
				new[] { 0.0, -1.0, 0.0 }
			},
			ConvolutionSharpenType.Laplacian2 => new[]
			{
				new[] { -1.0, -1.0, -1.0 }, 
				new[] { -1.0, 9.0, -1.0 }, 
				new[] { -1.0, -1.0, -1.0 }
			},
			ConvolutionSharpenType.Laplacian3 => new[]
			{
				new[] { 1.0, -2.0, 1.0 }, 
				new[] { -2.0, 5.0, -2.0 }, 
				new[] { 1.0, -2.0, 1.0 }
			},
			_ => new[]
			{
				new[] { 0.0, 0.0, 0.0 },
				new[] { 0.0, 1.0, 0.0 },
				new[] { 0.0, 0.0, 0.0 }
			},
		};
	}

	private static double[][] HandleBlur(ConvolutionBlurType blur)
	{
		return blur switch
		{
			ConvolutionBlurType.Neighbours => new[]
			{
				new[] { 0.0, 1.0, 0.0 }, 
				new[] { 1.0, 1.0, 1.0 }, 
				new[] { 0.0, 1.0, 0.0 }
			},
			ConvolutionBlurType.Neighbours_Weighted => new[]
			{
				new[] { 0.0, 1.0, 0.0 }, 
				new[] { 1.0, 4.0, 1.0 }, 
				new[] { 0.0, 1.0, 0.0 }
			},
			ConvolutionBlurType.All => new[]
			{
				new[] { 1.0, 1.0, 1.0 }, 
				new[] { 1.0, 1.0, 1.0 }, 
				new[] { 1.0, 1.0, 1.0 }
			},
			ConvolutionBlurType.All_Weighted => new[]
			{
				new[] { 1.0, 1.0, 1.0 }, 
				new[] { 1.0, 8.0, 1.0 }, 
				new[] { 1.0, 1.0, 1.0 }
			},
			ConvolutionBlurType.Gauss => new[]
			{
				new[] { 1.0, 2.0, 1.0 }, 
				new[] { 2.0, 4.0, 2.0 }, 
				new[] { 1.0, 2.0, 1.0 }
			},
			_ => new[]
			{
				new[] { 0.0, 0.0, 0.0 },
				new[] { 0.0, 1.0, 0.0 },
				new[] { 0.0, 0.0, 0.0 }
			}
		};
	}
}