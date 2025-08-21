namespace GMS
{
    /// <summary>
    /// Wrapper for being able to select Sub managers in the inspector for our Scriptable Objects.
    /// </summary>
    [System.Serializable]
    public struct SubManagerData
    {
        public string dataTypeName;

        /// <summary>
        /// If true this will be loaded by game manager.
        /// </summary>
        public bool Loads;

        /// <summary>
        /// If true this will be loaded as a base SubManager.
        /// </summary>
        public bool IsBase;
    }
}