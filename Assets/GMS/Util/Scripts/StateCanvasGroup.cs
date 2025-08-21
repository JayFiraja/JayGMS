using UnityEngine;

namespace GMS
{
    /// <summary>
    /// Data drwapper for grouping canvas group with game state
    /// </summary>
    [System.Serializable]
    public class StateCanvasGroup : CanvasGroupTransition
    {
        public GameState GameState;
    }
}