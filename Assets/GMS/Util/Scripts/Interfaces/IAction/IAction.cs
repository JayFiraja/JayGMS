namespace GMS
{
    /// <summary>
    /// Interface for executing actions
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Perform the action
        /// </summary>
        public void Do();
        
        /// <summary>
        /// Undo the action
        /// </summary>
        public void Undo();
    }
}
