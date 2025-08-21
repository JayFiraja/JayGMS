using System.Collections.Generic;

namespace GMS
{
    /// <summary>
    /// Utilities class containing useful methods for quick reusage.
    /// </summary>
    public static class CollectionUtilities
    {
        /// <summary>
        /// Shuffles the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">Reference list.</param>
        public static void ShuffleList<T>(List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}