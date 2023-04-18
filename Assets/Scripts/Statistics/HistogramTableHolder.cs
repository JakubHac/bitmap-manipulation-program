using System;
using TMPro;
using UnityEngine;

public class HistogramTableHolder : MonoBehaviour
{
    [SerializeField] private Transform ContentTransform;
    [SerializeField] private GameObject TableCellPrefab;
    
    private TableCell[] Table;
    private ImageHolder ImageHolder = null;

    private class TableCell
    {
        public TMP_Text IndexText;
        public TMP_Text ValueText;
    }
    
    public void AssignImageHolder(ImageHolder imageHolder)
    {
        if (ImageHolder != null)
        {
            Debug.LogError("ImageHolder already assigned", this.gameObject);
            return;
        }
        
        Table = new TableCell[256];
        for (int i = 0; i < Table.Length; i++)
        {
            GameObject cellGO = Instantiate(TableCellPrefab, ContentTransform);
            TableCell cell = new TableCell
            {
                IndexText = cellGO.transform.Find("IndexText").GetComponent<TMP_Text>(),
                ValueText = cellGO.transform.Find("ValueText").GetComponent<TMP_Text>()
            };
            Table[i] = cell;
            Table[i].IndexText.text = i.ToString();
        }
        ImageHolder = imageHolder;
        FillOutTable();
        ImageHolder.OnTextureUpdated += FillOutTable;
        ImageHolder.OnCloseImage += DestroySelf;
    }

    private void FillOutTable()
    {
        var window = GetComponent<DragableUIWindow>();
        var imageHolderWindow = ImageHolder.GetComponent<DragableUIWindow>();
        window.WindowTitle = $"Histogram {imageHolderWindow.WindowTitle}";
        var histogram = ImageActions.GetHistogram(ImageHolder);
        for (int i = 0; i < histogram.Length; i++)
        {
            Table[i].ValueText.text = Mathf.RoundToInt((float)histogram[i]).ToString();
        }
    }

    private void DestroySelf()
    {
        try
        {
            GetComponent<DragableUIWindow>().ExecuteClose();
        }
        catch (Exception e)
        {
            
        }
        
    }

    private void OnDestroy()
    {
        if (ImageHolder != null)
        {
            ImageHolder.OnTextureUpdated -= FillOutTable;
            ImageHolder.OnCloseImage -= FillOutTable;
        }
    }
}
