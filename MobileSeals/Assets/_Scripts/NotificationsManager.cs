using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class NotificationsManager: MonoBehaviourSingleton<NotificationsManager>
{
    
    private bool _isPermissionGrantedCached = false;

    /// <summary>
    /// Returns the cached permission status. For a real-time hardware-level check, 
    /// call `await CheckPermissionStatusAsync()` instead.
    /// </summary>
    public bool CanUseNotifications => _isPermissionGrantedCached;
    
    public event Action<bool> OnNotificationPermissionChanged;
    
    protected override void Awake()
    {
        base.Awake();
        
#if UNITY_ANDROID
        RegisterAndroidChannel();
#endif
        
        InitPermissions();
    }

    private async void InitPermissions()
    {
        await CheckPermissionStatusAsync();
        if (!_isPermissionGrantedCached)
        {
            RequestNotifPermission();
        }
    }

    
    public async UniTask<bool> CheckPermissionStatusAsync()
    {
#if UNITY_ANDROID
        _isPermissionGrantedCached = AndroidNotificationCenter.UserPermissionToPost == PermissionStatus.Allowed;
        Debug.Log("Android permission check: " + _isPermissionGrantedCached);
#elif UNITY_IOS
        // iOS requires fetching settings settings natively
        var settings = iOSNotificationCenter.GetNotificationSettings();
        _isPermissionGrantedCached = settings.AuthorizationStatus == AuthorizationStatus.Authorized;
#else
        _isPermissionGrantedCached = false;
#endif
        
        OnNotificationPermissionChanged?.Invoke(_isPermissionGrantedCached);
        return _isPermissionGrantedCached;
    }
    
    public void RequestNotifPermission() => _ = RequestPermissionAsync();
    
    public void Send(string title, string body, System.TimeSpan delay)
    {
#if UNITY_ANDROID
        SendAndroid(title, body, delay);
#elif UNITY_IOS
        SendiOS(title, body, delay);
#endif
    }

    // --- Permission ---

    async UniTask RequestPermissionAsync()
    {
#if UNITY_ANDROID
        await RequestAndroidPermissionAsync();
#elif UNITY_IOS
        await RequestiOSPermissionAsync();
#endif

        await CheckPermissionStatusAsync();
    }

#if UNITY_ANDROID
    
    private void RegisterAndroidChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "default_channel", // This MUST match the ID in SendAndroid
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }
    async UniTask RequestAndroidPermissionAsync()
    {
        if (AndroidNotificationCenter.UserPermissionToPost == PermissionStatus.Allowed)
            return;

        Debug.Log("Android permission request: " + AndroidNotificationCenter.UserPermissionToPost);
        
        var request = new PermissionRequest();

        await UniTask.WaitUntil(() => request.Status != PermissionStatus.RequestPending);

        switch (request.Status)
        {
            case PermissionStatus.Allowed:
                Debug.Log("Notifications allowed.");
                break;
            case PermissionStatus.Denied:
                Debug.Log("Notifications denied.");
                break;
            case PermissionStatus.NotificationsBlockedForApp:
                Debug.Log("Blocked — user must enable in Settings.");
                break;
        }
    }

    static void SendAndroid(string title, string body, System.TimeSpan delay)
    {
        var notification = new AndroidNotification()
        {
            Title = title,
            Text = body,
            FireTime = System.DateTime.Now.Add(delay),
            SmallIcon = "default",
            LargeIcon = "default"
        };

        AndroidNotificationCenter.SendNotification(notification, "default_channel");
    }
#endif

#if UNITY_IOS
    async UniTask RequestiOSPermissionAsync()
    {
        using var request = new AuthorizationRequest(
            AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound,
            true
        );

        await UniTask.WaitUntil(() => request.IsFinished);

        if (request.Granted)
            Debug.Log("Notifications allowed.");
        else
            Debug.Log($"Notifications denied. Error: {request.Error}");
    }

    static void SendiOS(string title, string body, System.TimeSpan delay)
    {
        var notification = new iOSNotification()
        {
            Title = title,
            Body = body,
            ShowInForeground = true,
            ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
            Trigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = delay,
                Repeats = false
            }
        };

        iOSNotificationCenter.ScheduleNotification(notification);
    }
#endif
}