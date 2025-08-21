namespace GMS
{
    /// <summary>
    /// Logic class that manages NPC pooling and spawning.
    /// </summary>
    public class NPCManager: ISubManager
    {
        private NPCManagerData _data;

        public NPCManager(NPCManagerData data)
        {
            _data = data;
        }
        
        #region ISubManager
        
        /// <inheritdoc/>
        public bool Initialize(GameManager gameManager)
        {

            return true;
        }
        
        /// <inheritdoc/>
        public void UnInitialize()
        {
          
        }
        
        /// <inheritdoc/>
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

        #endregion ISubManager
    }
}