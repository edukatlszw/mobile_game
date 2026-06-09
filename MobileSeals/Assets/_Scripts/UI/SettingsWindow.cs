using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : InGameMonobehaviour, IAttachListeners
{
    [SerializeField] private Toggle _notifPermissionToggle;
    [SerializeField] private Button _testNotifButton;
    [SerializeField] private Button _downScaleButton;
    [SerializeField] private Button _upScaleButton;
    [SerializeField] private TMP_Text _scaleText;

    [SerializeField] private List<float> _scales = new List<float>(){0.8f, 0.9f, 1f, 1.1f, 1.2f};
    
    private int _currentScaleIndex;
    
    private void Awake()
    {
        _downScaleButton.onClick.AddListener(DecreaseScale);
        _upScaleButton.onClick.AddListener(IncreaseScale);
        _testNotifButton.onClick.AddListener(SendTestNotif);
        _notifPermissionToggle.onValueChanged.AddListener(OnNotifPermissionToggled);
        _currentScaleIndex = _scales.Count/2;
        UpdateScale();
        _notifPermissionToggle.SetIsOnWithoutNotify(NotificationsManager.Instance.CanUseNotifications);
    }

    private void DecreaseScale()
    {
        if(_currentScaleIndex <= 0) return;
        _currentScaleIndex--;
        UpdateScale();
    }
    
    private void IncreaseScale()
    {
        if(_currentScaleIndex >= _scales.Count -1) return;
        _currentScaleIndex++;
        UpdateScale();
    }


    private void UpdateScale()
    {
        UIManager.Instance.SetNewScale(_scales[_currentScaleIndex]);
        _downScaleButton.interactable = _currentScaleIndex > 0;
        _upScaleButton.interactable = _currentScaleIndex < _scales.Count-1;
        _scaleText.text = $"{_currentScaleIndex+1}";
    }


    
    
    private void SendTestNotif()
    {
        NotificationsManager.Instance.Send("Test Notification", "Hello, this is a test!", TimeSpan.Zero);
    }
    
    private void OnNotifPermissionToggled(bool value)
    {
        NotificationsManager.Instance.RequestNotifPermission();
    }

    public void AttachListeners()
    {
        NotificationsManager.Instance.OnNotificationPermissionChanged += UpdateNotifToggle;
    }

    private void UpdateNotifToggle(bool isAllowed)
    {
        _notifPermissionToggle.SetIsOnWithoutNotify(isAllowed);
    }
    
    public void DetachListeners()
    {
        if (NotificationsManager.Instance != null)
        {
            NotificationsManager.Instance.OnNotificationPermissionChanged -= UpdateNotifToggle;

        }
    }
}
