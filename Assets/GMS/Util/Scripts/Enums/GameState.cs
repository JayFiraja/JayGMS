namespace GMS
{
    /// <summary>
    /// Used for keeping track of the current game state.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// Default state
        /// </summary>
        UnInitialized,
        /// <summary>
        /// Initial default value
        /// </summary>
        Loading,
        /// <summary>
        /// Game type selection, first landing interactive state
        /// </summary>
        MainMenu,
        /// <summary>
        /// Main game state where the game happens.
        /// </summary>
        GameLoop,
        /// <summary>
        /// End game display state
        /// </summary>
        Results
    }
}