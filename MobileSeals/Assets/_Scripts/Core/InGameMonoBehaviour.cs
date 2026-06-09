using System;
using System.Threading;
using UnityEngine;

/// <summary>
///     Class that implements custom initialization pipeline
/// </summary>
public abstract class InGameMonobehaviour : MonoBehaviour, IInitializable
{
    public Action onDestroy;
    protected CancellationToken CancellationToken => destroyCancellationToken;

    private void Start()
    {
        if (IsInitialized) return;

        IsInitialized = true;
        Initialize();
    }

    private void OnDestroy()
    {
        onDestroy?.Invoke();

        if (this is IAttachListeners attachListeners)
            attachListeners.DetachListeners();

        CleanUp();

        if (this is IWithSetUp setup)
            setup.TearDown();
    }

    protected virtual void OnValidate()
    {
    }

    public bool IsInitialized { get; private set; }

    public void Initialize()
    {
        if (this is IWithSetUp setup)
            setup.SetUp();

        if (this is IAttachListeners attachListeners) attachListeners.AttachListeners();
    }

    protected virtual void CleanUp()
    {
    }
}