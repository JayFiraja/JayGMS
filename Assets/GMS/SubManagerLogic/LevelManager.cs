using UnityEngine;

namespace GMS
{
    public class LevelManager: ISubManager
    {
    
        private LevelData _data;
        
        public LevelManager(LevelData data)
        {
            _data = data;
        }
        
        public bool Initialize(GameManager gameManager)
        {
            Debug.Log("LevelManager, Initialized");
            return true;
        }
        
        public void UnInitialize()
        {
           
        }
        
        public void OnUpdate()
        {
         
        }
        
        public bool Equals(ISubManager other)
        {
            // Check if other is null
            if (other == null)
            {
                return false;
            }
        
            // Compare the runtime types of the current instance and the other instance
            return GetType() == other.GetType();
        }
    
        public override bool Equals(object obj)
        {
            if (obj is ISubManager other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the fields that contribute to equality
            return GetType().GetHashCode();
        }
    }
}
