/// <summary>
///     Interface for classes that require additional initialization at Awake
/// </summary>
public interface IInitializable
{
    public bool IsInitialized { get; }
    public void Initialize();
}