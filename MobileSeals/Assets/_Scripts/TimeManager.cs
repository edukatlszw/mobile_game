using System;
using UnityEngine;

public class TimeManager : PersistentMonoSingleton<TimeManager>
{


    // 50 = every 1 second at default timestep
    [SerializeField] private int _fixedUpdatesPerTick = 5; 
    private int _fixedUpdateCount;


    private float _tickTimer;
    private DateTime _lastSessionTime;
    private const string LastSessionKey = "LastSessionTime";
    
    public event Action OnTick;
    public event Action<int> OnOfflineTicks;
    
    public float TickIntervalSeconds => _fixedUpdatesPerTick * Time.fixedDeltaTime;

    private void Start()
    {
        LoadLastSessionTime();
    }


    private void FixedUpdate()
    {
        _fixedUpdateCount++;

        if (_fixedUpdateCount >= _fixedUpdatesPerTick)
        {
            _fixedUpdateCount = 0;
            OnTick?.Invoke();
        }
    }

    private void OnApplicationPause(bool pausing)
    {
        if (pausing) SaveSessionTime();
        else ProcessOfflineTime();
    }

    private void OnApplicationQuit() => SaveSessionTime();

    
    private void SaveSessionTime()
    {
        PlayerPrefs.SetString(LastSessionKey, DateTime.UtcNow.ToString("o"));
        PlayerPrefs.Save();
    }

    private void LoadLastSessionTime()
    {
        string saved = PlayerPrefs.GetString(LastSessionKey, string.Empty);

        if (!string.IsNullOrEmpty(saved) &&
            DateTime.TryParse(saved, null,
                System.Globalization.DateTimeStyles.RoundtripKind,
                out DateTime lastTime))
        {
            _lastSessionTime = lastTime;
            ProcessOfflineTime();
        }
        else
        {
            _lastSessionTime = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Returns elapsed ticks since last session
    /// </summary>
    public int ProcessOfflineTime()
    {
        TimeSpan elapsed = DateTime.UtcNow - _lastSessionTime;
        float tickIntervalSeconds = _fixedUpdatesPerTick * Time.fixedDeltaTime;
        int missedTicks = (int)(elapsed.TotalSeconds / tickIntervalSeconds);

        OnOfflineTicks?.Invoke(missedTicks);

        _lastSessionTime = DateTime.UtcNow;
        return missedTicks;
    }
}
