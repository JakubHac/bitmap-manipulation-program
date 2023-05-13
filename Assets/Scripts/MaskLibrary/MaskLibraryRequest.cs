using System;

public class MaskLibraryRequest
{
    public readonly Action<double[,]> Callback;

    public MaskLibraryRequest(Action<double[,]> callback)
    {
        Callback = callback;
    }
}
