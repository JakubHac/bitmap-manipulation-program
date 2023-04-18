using System;
using UnityEngine;

public class ImageReplaceOrNewEvent
{
    public readonly Texture2D OldTexture;
    public readonly Texture2D NewTexture;
    public readonly ImageHolder CurrentImageHolder;
    public readonly string NewName;

    public ImageReplaceOrNewEvent(Texture2D oldTexture, Texture2D newTexture, ImageHolder currentImageHolder, string newName)
    {
        OldTexture = oldTexture;
        NewTexture = newTexture;
        CurrentImageHolder = currentImageHolder;
        NewName = newName;
    }
}
