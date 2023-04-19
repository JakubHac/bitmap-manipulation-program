using System.Collections.Generic;
using Doozy.Engine.UI;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class DrawProfileLineView : MonoBehaviour
{
    [SerializeField] private UIView ProfileLineEditView;
    [SerializeField] private RectTransform StartPoint;
    [SerializeField] private RectTransform EndPoint;
    [SerializeField] private RectTransform Line;
    [SerializeField] private RawImage TargetImage;
    [SerializeField] private AspectRatioFitter TargetAspectRatioFitter;

    private Queue<RequestProfileLineUpdate> RequestQueue = new Queue<RequestProfileLineUpdate>();

    private RequestProfileLineUpdate CurrentRequest;

    private void Update()
    {
        if (ProfileLineEditView.Visibility == VisibilityState.NotVisible) return;
        
        var angle = MathHelper.AngleBetweenTwoPoints(StartPoint.position, EndPoint.position);
        EndPoint.rotation = Quaternion.Euler(0f, 0f, -angle);
        Line.rotation = Quaternion.Euler(0f, 0f, -angle);
        Line.position = (StartPoint.position + EndPoint.position) / 2f;
        Line.sizeDelta = new Vector2(Vector2.Distance(StartPoint.localPosition, EndPoint.localPosition) - 40f, Line.sizeDelta.y);
    }

    private void Start()
    {
        Messenger.Default.Subscribe<RequestProfileLineUpdate>(AddToQueue);
    }

    private void AddToQueue(RequestProfileLineUpdate obj)
    {
        RequestQueue.Enqueue(obj);
        if (CurrentRequest == null)
        {
            ShowNextRequest();
        }
    }

    private void ShowNextRequest()
    {
        ProfileLineEditView.Show();
        CurrentRequest = RequestQueue.Dequeue();
        StartPoint.GetComponent<ProfileLinePoint>().SetNormalizedPos(CurrentRequest.StartPoint);
        EndPoint.GetComponent<ProfileLinePoint>().SetNormalizedPos(CurrentRequest.EndPoint);
        TargetImage.texture = CurrentRequest.ImageHolder.Texture;
        TargetImage.texture.filterMode = FilterMode.Point;
        TargetAspectRatioFitter.aspectRatio = (float)TargetImage.texture.width / TargetImage.texture.height;
    }

    public void AcceptProfileLine()
    {
        Vector2 normalizedStart = StartPoint.GetComponent<ProfileLinePoint>().NormalizedPos;
        Vector2 normalizedEnd = EndPoint.GetComponent<ProfileLinePoint>().NormalizedPos;
        CurrentRequest.Callback?.Invoke(normalizedStart, normalizedEnd);
        CurrentRequest = null;
        
        if (RequestQueue.Count == 0)
        {
            ProfileLineEditView.Hide();
        }
        else
        {
            ShowNextRequest();
        }
    }

    public void AfterClose()
    {
        CurrentRequest = null;
        TargetImage.texture = null;
    }

    private void OnDestroy()
    {
        Messenger.Default.Unsubscribe<RequestProfileLineUpdate>(AddToQueue);
    }
}
