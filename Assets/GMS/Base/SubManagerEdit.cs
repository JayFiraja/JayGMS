namespace GMS
{
    /// <summary>
    /// Wrapper for editing SubManager into a list.
    /// </summary>
    public struct SubManagerEdit
    {
        public ISubManager SubManager;
        public bool ADD;
        public bool IsBase;
        public SubManagerEdit(ISubManager subManager, bool add, bool isBase)
        {
            SubManager = subManager;
            ADD = add;
            IsBase = isBase;
        }
    }
}