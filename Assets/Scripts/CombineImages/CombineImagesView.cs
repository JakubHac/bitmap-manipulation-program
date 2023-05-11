using System;
using Doozy.Engine.UI;
using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class CombineImagesView : MonoBehaviour
{
    [SerializeField] private UIView View;
    [SerializeField] private RawImage ImagePreview;
    [SerializeField] private AspectRatioFitter ImagePreviewFitter;
	
    private ImageHolder SelectedHolder;
    [SerializeField] private TMP_InputField SearchField;
    [SerializeField] private RectTransform ButtonsParent;
    [SerializeField] private GameObject ButtonPrefab;
    
    private CombineImagesRequest Request;
    
    private void Start()
    {
	    Messenger.Default.Subscribe<CombineImagesRequest>(HandleRequest);
	    SearchField.onValueChanged.AddListener(OnSearchFieldChanged);
    }

    private void HandleRequest(CombineImagesRequest obj)
    {
	    Request = obj;
	    ImagePreview.texture = obj.OriginalImage.Texture;
	    ImagePreviewFitter.aspectRatio = (float)obj.OriginalImage.Texture.width / obj.OriginalImage.Texture.height;

	    ClearList();
	    
	    View.Show();
	    foreach (var holder in FindObjectsOfType<ImageHolder>(includeInactive: true))
	    {
		    var texture = holder.Texture;
		    if (texture.width != obj.OriginalImage.Texture.width || texture.height != obj.OriginalImage.Texture.height) return;
		    var buttonGo = Instantiate(ButtonPrefab, ButtonsParent);
		    var element = buttonGo.GetComponent<CombineImagesElement>();
		    element.SetSource(holder, this);
	    }
    }

    private void ClearList()
    {
	    foreach (var oldElement in ButtonsParent.GetComponentsInChildren<CombineImagesElement>(includeInactive: true))
	    {
		    Destroy(oldElement.gameObject);
		    oldElement.transform.SetParent(null);
		    oldElement.gameObject.SetActive(false);
	    }
    }

    public void SelectImage(ImageHolder otherImage)
    {
	    Request?.Combine(otherImage);
	    View.Hide();
    }
    
    private void OnSearchFieldChanged(string arg0)
    {
	    FilterImages();
    }
    
    private void FilterImages()
    {
	    foreach (var button in ButtonsParent.GetComponentsInChildren<UIButton>(includeInactive: true))
	    {
		    button.gameObject.SetActive(button.TextMeshProLabel.text.Contains(SearchField.text, StringComparison.OrdinalIgnoreCase));
	    }
    }
    
    public void OnClose()
    {
	    ClearList();
	    SelectedHolder = null;
	    ImagePreview.texture = null;
	    Request = null;
    }

    private void OnDestroy()
    {
	    Messenger.Default.Unsubscribe<CombineImagesRequest>(HandleRequest);
    }
}
