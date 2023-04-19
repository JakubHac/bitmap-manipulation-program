using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class ProfileLinePoint : MonoBehaviour
{
    private DragTarget SelfDragTarget;
    private RectTransform RectTrans;
    [SerializeField] private RawImage TargetImage;
    public Vector2 NormalizedPos;
    
    void Start()
    {
        RectTrans = GetComponent<RectTransform>();
        SelfDragTarget = new DragTarget(gameObject, OnDrag, DragTargetType.ProfileLinePoint);
        Messenger.Default.Publish(new RegisterDragTarget(SelfDragTarget));
    }

    public void SetNormalizedPos(Vector2 normalizedPos)
    {
        float minX = -TargetImage.rectTransform.rect.width/2f;
        float maxX = TargetImage.rectTransform.rect.width/2f;
        float actualX = Mathf.Lerp(minX, maxX, normalizedPos.x);
        float minY = -TargetImage.rectTransform.rect.height/2f;
        float maxY = TargetImage.rectTransform.rect.height/2f;
        float actualY = Mathf.Lerp(minY, maxY, normalizedPos.y);
        RectTrans.anchoredPosition = new Vector2(actualX, actualY);
    }

    private void Update()
    {
        float minX = -TargetImage.rectTransform.rect.width/2f;
        float maxX = TargetImage.rectTransform.rect.width/2f;
        float actualX = Mathf.Clamp(RectTrans.anchoredPosition.x, minX, maxX);
        float minY = -TargetImage.rectTransform.rect.height/2f;
        float maxY = TargetImage.rectTransform.rect.height/2f;
        float actualY = Mathf.Clamp(RectTrans.anchoredPosition.y, minY, maxY);
        RectTrans.anchoredPosition = new Vector2(actualX, actualY);
        NormalizedPos = new Vector2(Mathf.InverseLerp(minX, maxX, actualX), Mathf.InverseLerp(minY, maxY, actualY));
    }
    
    private void OnDrag(Vector2 dragValue, bool focus)
    {
        RectTrans.position += new Vector3(dragValue.x, dragValue.y, 0f);
        if (focus && RectTrans != null)
        {
            RectTrans.SetAsLastSibling();
        }
    }
}
