namespace GMS
{
    /// <summary>
    /// To be implemented by submanagers
    /// Makes it easy to Main stream behaviour for all submanagers by GameManager
    /// </summary>
    public interface ISubManager
    {
        /// <summary>
        /// Initializes this subManager
        /// </summary>
        /// <returns></returns>
        bool Initialize(GameManager gameManager);
        
        /// <summary>
        /// Run unsubscriptions, despawning, cleanup
        /// </summary>
        void UnInitialize();
        
        /// <summary>
        /// Tick called by GameManager.Update method.
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// Checks if the passed-in object is equal to this instance.
        /// </summary>
        /// <param name="other">The other subManager to compare against.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        bool Equals(ISubManager other);
    }
}
