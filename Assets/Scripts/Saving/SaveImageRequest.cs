using System.IO;

public class SaveImageRequest
{
    public readonly ImageHolder Source;
    public readonly string FilePath;
    public bool isJPG => Path.GetExtension(FilePath) is ".jpg" or ".jpeg";
    public bool isPNG => Path.GetExtension(FilePath) is ".png";
    public bool isBMP => Path.GetExtension(FilePath) is ".bmp";
    
    public SaveImageRequest(ImageHolder source, string filePath)
	{
		Source = source;
		FilePath = filePath;
	}
}
