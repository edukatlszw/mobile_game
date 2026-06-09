using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragObject : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] Canvas _canvas;
    [SerializeField] RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData e) => SetGhostPosition(e);

    public void OnEndDrag(PointerEventData e)
    {
        
    }

    void SetGhostPosition(PointerEventData e)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.GetComponent<RectTransform>(),
            e.position, _canvas.worldCamera, out var pos);
        _rectTransform.localPosition = pos;
    }
}
