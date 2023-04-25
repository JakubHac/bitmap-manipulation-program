public static class ConvolutionMasks
{
	public static double[,] GetMaskFromEnums(ConvolutionOperation operation, ConvolutionBlurType blur,
		ConvolutionEdgeDetectMethod edgeDetectMethod, ConvolutionEdgeDetectDirection edgeDetectDirection,
		ConvolutionSharpenType sharpen, ConvolutionLaplacianEdgeDetectionType laplacianEdgeDetectionType)
	{
		switch (operation)
		{
			case ConvolutionOperation.Custom:
				return new[,]
				{
					{ 0.0, 0.0, 0.0 },
					{ 0.0, 1.0, 0.0 },
					{ 0.0, 0.0, 0.0 }
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

	private static double[,] HandleEdgeDetection(ConvolutionEdgeDetectMethod edgeDetectMethod,
		ConvolutionEdgeDetectDirection edgeDetectDirection,
		ConvolutionLaplacianEdgeDetectionType laplacianEdgeDetectionType)
	{
		return edgeDetectMethod switch
		{
			ConvolutionEdgeDetectMethod.Sobel => HandleSobelEdgeDetect(edgeDetectDirection),
			ConvolutionEdgeDetectMethod.Laplacian => HandleLaplacianEdgeDetect(laplacianEdgeDetectionType),
			ConvolutionEdgeDetectMethod.Canny =>
				new[,]
				{ 
					{ 85.0, 255.0, 3.0} 
				},
			ConvolutionEdgeDetectMethod.Prewitt => HandlePrewittEdgeDetect(edgeDetectDirection),
			_ => new[,]
			{
				{ 0.0, 0.0, 0.0 },
				{ 0.0, 1.0, 0.0 },
				 { 0.0, 0.0, 0.0 }
			},
		};
	}

	private static double[,] HandlePrewittEdgeDetect(ConvolutionEdgeDetectDirection edgeDetectDirection)
	{
		return edgeDetectDirection switch
		{
			ConvolutionEdgeDetectDirection.North => new[,]
			{
				{ 1.0, 1.0, 1.0 },
				{ 0.0, 0.0, 0.0 },
				{ -1.0, -1.0, -1.0 }
			},
			ConvolutionEdgeDetectDirection.South => new[,]
			{
				{ -1.0, -1.0, -1.0 },
				{ 0.0, 0.0, 0.0 },
				{ 1.0, 1.0, 1.0 }
			},
			ConvolutionEdgeDetectDirection.East => new[,]
			{
				{ -1.0, 0.0, 1.0 },
				{ -1.0, 0.0, 1.0 },
				{ -1.0, 0.0, 1.0 }
			},
			ConvolutionEdgeDetectDirection.West => new[,]
			{
				{ 1.0, 0.0, -1.0 },
				{ 1.0, 0.0, -1.0 },
				{ 1.0, 0.0, -1.0 }
			},
			ConvolutionEdgeDetectDirection.NorthEast => new[,]
			{
				{ 0.0, 1.0, 1.0 },
				{ -1.0, 0.0, 1.0 }, 
				{ -1.0, -1.0, 0.0 }
			},
			ConvolutionEdgeDetectDirection.NorthWest => new[,]
			{
				{ 0.0, 1.0, 1.0 }, 
				{ -1.0, 0.0, 1.0 }, 
				{ -1.0, -1.0, 0.0 }
			},
			ConvolutionEdgeDetectDirection.SouthEast => new[,]
			{
				{ -1.0, -1.0, 0.0 }, 
				{ -1.0, 0.0, 1.0 }, 
				{ 0.0, 1.0, 1.0 }
			},
			ConvolutionEdgeDetectDirection.SouthWest => new[,]
			{
				{ 0.0, -1.0, -1.0 }, 
				{ 1.0, 0.0, -1.0 }, 
				{ 1.0, 1.0, 0.0 }
			},
			_ => new[,]
			{
				{ 0.0, 0.0, 0.0 },
				{ 0.0, 1.0, 0.0 },
				{ 0.0, 0.0, 0.0 }
			},
		};
	}

	private static double[,] HandleLaplacianEdgeDetect(ConvolutionLaplacianEdgeDetectionType laplacianEdgeDetectionType)
	{
		return laplacianEdgeDetectionType switch
		{
			ConvolutionLaplacianEdgeDetectionType.FourConnected => new[,]
			{
				{ 0.0, -1.0, 0.0 },
				{ -1.0, 4.0, -1.0 },
				{ 0.0, -1.0, 0.0 }
			},
			ConvolutionLaplacianEdgeDetectionType.EightConnected => new[,]
			{
				{ -1.0, -1.0, -1.0 },
				{ -1.0, 8.0, -1.0 },
				{ -1.0, -1.0, -1.0 }
			},
			_ => new[,]
			{
				{ 0.0, 0.0, 0.0 },
				{ 0.0, 1.0, 0.0 },
				{ 0.0, 0.0, 0.0 }
			},
		};
	}

	private static double[,] HandleSobelEdgeDetect(ConvolutionEdgeDetectDirection edgeDetectDirection)
	{
		return edgeDetectDirection switch
		{
			ConvolutionEdgeDetectDirection.North => new[,]
			{
				{ 1.0, 2.0, 1.0 },
				{ 0.0, 0.0, 0.0 },
				{ -1.0, -2.0, -1.0 }
			},
			ConvolutionEdgeDetectDirection.South => new[,]
			{
				{ -1.0, -2.0, -1.0 },
				{ 0.0, 0.0, 0.0 },
				{ 1.0, 2.0, 1.0 }
			},
			ConvolutionEdgeDetectDirection.East => new[,]
			{
				{ -1.0, 0.0, 1.0 },
				{ -2.0, 0.0, 2.0 },
				{ -1.0, 0.0, 1.0 }
			},
			ConvolutionEdgeDetectDirection.West => new[,]
			{
				{ 1.0, 0.0, -1.0 },
				{ 2.0, 0.0, -2.0 },
				{ 1.0, 0.0, -1.0 }
			},
			ConvolutionEdgeDetectDirection.NorthEast => new[,]
			{
				{ 0.0, 1.0, 2.0 },
				{ -1.0, 0.0, 1.0 }, 
				{ -2.0, -1.0, 0.0 }
			},
			ConvolutionEdgeDetectDirection.NorthWest => new[,]
			{
				{ 0.0, 1.0, 2.0 }, 
				{ -1.0, 0.0, 1.0 }, 
				{ -2.0, -1.0, 0.0 }
			},
			ConvolutionEdgeDetectDirection.SouthEast => new[,]
			{
				{ -2.0, -1.0, 0.0 }, 
				{ -1.0, 0.0, 1.0 }, 
				{ 0.0, 1.0, 2.0 }
			},
			ConvolutionEdgeDetectDirection.SouthWest => new[,]
			{
				{ 0.0, -1.0, -2.0 }, 
				{ 1.0, 0.0, -1.0 }, 
				{ 2.0, 1.0, 0.0 }
			},
			_ => new[,]
			{
				{ 0.0, 0.0, 0.0 },
				{ 0.0, 1.0, 0.0 },
				{ 0.0, 0.0, 0.0 }
			},
		};
	}

	private static double[,] HandleSharpen(ConvolutionSharpenType sharpen)
	{
		return sharpen switch
		{
			ConvolutionSharpenType.Laplacian1 => new[,]
			{
				{ 0.0, -1.0, 0.0 }, 
				{ -1.0, 5.0, -1.0 }, 
				{ 0.0, -1.0, 0.0 }
			},
			ConvolutionSharpenType.Laplacian2 => new[,]
			{
				{ -1.0, -1.0, -1.0 }, 
				{ -1.0, 9.0, -1.0 }, 
				{ -1.0, -1.0, -1.0 }
			},
			ConvolutionSharpenType.Laplacian3 => new[,]
			{
				{ 1.0, -2.0, 1.0 }, 
				{ -2.0, 5.0, -2.0 }, 
				{ 1.0, -2.0, 1.0 }
			},
			_ => new[,]
			{
				{ 0.0, 0.0, 0.0 },
				{ 0.0, 1.0, 0.0 },
				{ 0.0, 0.0, 0.0 }
			},
		};
	}

	private static double[,] HandleBlur(ConvolutionBlurType blur)
	{
		return blur switch
		{
			ConvolutionBlurType.Neighbours => new[,]
			{
				{ 0.0, 1.0, 0.0 }, 
				{ 1.0, 1.0, 1.0 }, 
				{ 0.0, 1.0, 0.0 }
			},
			ConvolutionBlurType.Neighbours_Weighted => new[,]
			{
				{ 0.0, 1.0, 0.0 }, 
				{ 1.0, 4.0, 1.0 }, 
				{ 0.0, 1.0, 0.0 }
			},
			ConvolutionBlurType.All => new[,]
			{
				{ 1.0, 1.0, 1.0 }, 
				{ 1.0, 1.0, 1.0 }, 
				{ 1.0, 1.0, 1.0 }
			},
			ConvolutionBlurType.All_Weighted => new[,]
			{
				{ 1.0, 1.0, 1.0 }, 
				{ 1.0, 8.0, 1.0 }, 
				{ 1.0, 1.0, 1.0 }
			},
			ConvolutionBlurType.Gauss => new[,]
			{
				{ 1.0, 2.0, 1.0 }, 
				{ 2.0, 4.0, 2.0 }, 
				{ 1.0, 2.0, 1.0 }
			},
			_ => new[,]
			{
				{ 0.0, 0.0, 0.0 },
				{ 0.0, 1.0, 0.0 },
				{ 0.0, 0.0, 0.0 }
			}
		};
	}
}