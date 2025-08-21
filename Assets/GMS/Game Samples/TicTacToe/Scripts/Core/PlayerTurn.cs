namespace GMS.Samples
{
    /// <summary>
    /// Used for keeping track of the current player turn.
    /// </summary>
    public enum PlayerTurn
    {
        None,
        /// <summary>
        /// Starting player by definition
        /// </summary>
        PlayerA,
        /// <summary>
        /// AI controlled, when playing Player vsAI
        /// </summary>
        PlayerB
    }
}


