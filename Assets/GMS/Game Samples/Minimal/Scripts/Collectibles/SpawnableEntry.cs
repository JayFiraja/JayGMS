namespace GMS.Samples
{
    /// <summary>
    /// Wrapper for a view side spawnable item with it's transform data.
    /// </summary>
    public class SpawnableEntry<T>
    {
        /// <summary>
        /// View item completely separated from data
        /// </summary>
        public T ViewItem;
        /// <summary>
        /// Transform data governed by a data driven class.
        /// </summary>
        public TransformData TransformData;

        /// <summary>
        /// Preserved starting transform data, set when Spawned <see cref="SpawnableEntry"/>
        /// </summary>
        public TransformData StartingTransformData { get; private set; }

        /// <summary>
        /// Sets the view item and sets transform data, preserving a copy in StartingTransformData
        /// </summary>
        /// <param name="viewItem"></param>
        /// <param name="starttransformData"></param>
        public SpawnableEntry(T viewItem, TransformData starttransformData)
        {
            ViewItem = viewItem;
            TransformData = starttransformData;
            StartingTransformData = starttransformData;
        }
    }
}
