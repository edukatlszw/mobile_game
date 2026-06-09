using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableComponent : Selectable
{
    [SerializeField] private float _selectionDelay;
    public event Action<BaseEventData> OnSelectEvent;
    public event Action<BaseEventData> OnDeselectEvent;

    private CancellationTokenSource _manualCts;
    
    public bool IsSelected { get; private set; } = false;
    
    protected override void Awake()
    {
        base.Awake();
        _manualCts = new CancellationTokenSource();
        
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        AwaitSelectionDelayAsync(eventData);
    }

    private async void AwaitSelectionDelayAsync(BaseEventData eventData)
    {
        try
        {
            if (_selectionDelay != 0)
            {
                await AwaitDelay(_selectionDelay, _manualCts.Token);
            }
        }
        catch (OperationCanceledException cancelEx)
        {
            return;
        }
        
        if(_manualCts.Token.IsCancellationRequested) return;
        
        IsSelected = true;
        OnSelectEvent?.Invoke(eventData);

    }

    private async Task AwaitDelay(float delay, CancellationToken cts)
    { 
        await Task.Delay((int)(delay*1000), cts);
    }
    
    public void DeselectObject()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        ResetToken();
        IsSelected = false;
        OnDeselectEvent?.Invoke(eventData);
    }

    private void ResetToken()
    {
        _manualCts?.Cancel();
        _manualCts?.Dispose();
        _manualCts = new CancellationTokenSource();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _manualCts?.Cancel();
        _manualCts?.Dispose();
        OnSelectEvent = null;
        OnDeselectEvent = null;
    }
}
