public interface IPoolable
{
    void Reset();

    /// <summary>
    /// Returns true if dormant, 
    /// implemented in method as it might vary
    /// </summary>
    bool IsDormant();
}