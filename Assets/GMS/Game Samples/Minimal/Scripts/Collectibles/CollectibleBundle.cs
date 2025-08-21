using System;
using System.Collections.Generic;

namespace GMS.Samples
{
    /// <summary>
    /// Bundles or "encapsulates"  data and logic related to collectibles
    /// </summary>
    public class CollectibleBundle
    {
        public CollectibleSO CollectibleSO { get; private set; }
        public int CollectedValue { get; private set; }
        public void Initialize(CollectibleSO collectibleSO, int initialValue = 0)
        {
            CollectibleSO = collectibleSO;
            CollectedValue = initialValue;
        }

        public void AddCollectedValue(int valueToAdd)
        {
            CollectedValue += valueToAdd;
        }
    }
}