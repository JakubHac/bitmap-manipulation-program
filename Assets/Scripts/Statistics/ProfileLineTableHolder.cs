using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;

public class ProfileLineTableHolder : MonoBehaviour
{
    [SerializeField] private Transform ContentTransform;
    [SerializeField] private GameObject TableCellPrefab;
    private ImageHolder ImageHolder = null;
    private Vector2 StartPoint = new Vector2(0.25f, 0.75f);
    private Vector2 EndPoint = new Vector2(0.75f, 0.25f);
    
    private TableCell[] Table;
    
    private class TableCell
    {
        public TMP_Text IndexText;
        public TMP_Text ValueText;
        public GameObject Parent;
    }

    public void AssignImageHolder(ImageHolder imageHolder)
    {
        if (ImageHolder != null)
        {
            Debug.LogError("ImageHolder already assigned", this.gameObject);
            return;
        }
        ImageHolder = imageHolder;
        PlotProfileLine();
        ImageHolder.OnTextureUpdated += PlotProfileLine;
        ImageHolder.OnCloseImage += DestroySelf;
        RequestProfileLineUpdate();
    }

    private void PlotProfileLine()
    {
        var window = GetComponent<DragableUIWindow>();
        var imageHolderWindow = ImageHolder.GetComponent<DragableUIWindow>();
        window.WindowTitle = $"Linia Profilu {imageHolderWindow.WindowTitle}";

        var line = ImageActions.GetProfileLine(ImageHolder, StartPoint, EndPoint);

        if (Table != null)
        {
            foreach (var cell in Table)
            {
                Destroy(cell.Parent);
                cell.Parent.SetActive(false);
            }
        }

        Table = new TableCell[line.Count];
        for (int i = 0; i < Table.Length; i++)
        {
            GameObject cellGO = Instantiate(TableCellPrefab, ContentTransform);
            TableCell cell = new TableCell
            {
                IndexText = cellGO.transform.Find("IndexText").GetComponent<TMP_Text>(),
                ValueText = cellGO.transform.Find("ValueText").GetComponent<TMP_Text>(),
                Parent = cellGO
            };
            Table[i] = cell;
            Table[i].IndexText.text = i.ToString();
            Table[i].ValueText.text = line[i].ToString();
        }
    }

    private void DestroySelf()
    {
        if (this != null)
        {
            if (GetComponent<DragableUIWindow>() is { } dragableUIWindow) 
            {
                dragableUIWindow.ExecuteClose();
            }
        }
    }
    
    private void OnDestroy()
    {
        if (ImageHolder != null)
        {
            ImageHolder.OnTextureUpdated -= PlotProfileLine;
            ImageHolder.OnCloseImage -= PlotProfileLine;
        }
    }
    
    public void RequestProfileLineUpdate()
    {
        Messenger.Default.Publish(new RequestProfileLineUpdate(ImageHolder, StartPoint, EndPoint, RequestProfileLineUpdateCallback));
    }
    
    private void RequestProfileLineUpdateCallback(Vector2 startPoint, Vector2 endPoint)
    {
        UpdateProfileLine(startPoint, endPoint);
    }
    
    public void UpdateProfileLine(Vector2 startPoint, Vector2 endPoint)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        PlotProfileLine();
    }
}
