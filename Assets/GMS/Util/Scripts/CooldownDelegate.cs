namespace GMS
{
    /// <summary>
    /// Delegate used for timed method calls.
    /// </summary>
    public delegate void cooldownFinished();
    /// <summary>
    /// generic delegate used for time method calls that require an input parameter
    /// </summary>
    /// <typeparam name="T">Type required</typeparam>
    /// <param name="parameter">parameter for the delegate</param>
    public delegate void CooldownFinished<T>(T parameter);

    /// <summary>
    /// Data for non-parameterized cooldown
    /// </summary>
    public class CooldownDelegate : ICooldownDelegate
    {
        /// <summary>
        /// Delegate to call
        /// </summary>
        public cooldownFinished CooldownFinished;
        /// <summary>
        /// Cooldown wait time to meet
        /// </summary>
        public float CoolTime;
        /// <summary>
        /// Working variable for catching the time since the wait started.
        /// </summary>
        public float CoolTime_cache;

        public CooldownDelegate(float cooldown, cooldownFinished cooldownFinished)
        {
            CoolTime = cooldown;
            CoolTime_cache = Cooldown.Cool();
            CooldownFinished = cooldownFinished;
        }

        /// <inheritdoc> </inheritdoc>
        public bool IsCooled()
        {
            return Cooldown.IsCooled(CoolTime_cache, CoolTime);
        }

        /// <inheritdoc> </inheritdoc>
        public void Invoke()
        {
            CooldownFinished?.Invoke();
        }

        /// <inheritdoc> </inheritdoc>
        public void CancelDelegate()
        {
            CooldownFinished = null;
        }
    }

    /// <summary>
    /// Interface for cooldown delegates
    /// </summary>
    public interface ICooldownDelegate
    {
        /// <summary>
        /// Check if the delegate is cooldown time has been met.
        /// </summary>
        /// <returns></returns>
        bool IsCooled();
        /// <summary>
        /// Tries to Invoke the delegate in case it's not null
        /// </summary>
        void Invoke();
        /// <summary>
        /// Cancels the delegate, in case we want to cancel action even when timed.
        /// </summary>
        void CancelDelegate();
    }

    /// <summary>
    /// Data for parameterized cooldown
    /// </summary>
    public class CooldownDelegate<T> : ICooldownDelegate
    {
        /// <summary>
        /// Generic Delegate to call
        /// </summary>
        public CooldownFinished<T> CooldownFinished;
        /// <summary>
        /// Generic parameter for the delegate
        /// </summary>
        public T Parameter;
        /// <summary>
        /// Cooldown wait time to meet
        /// </summary>
        public float CoolTime;
        /// <summary>
        /// Working variable for catching the time since the wait started.
        /// </summary>
        public float CoolTime_cache;

        public CooldownDelegate(float cooldown, CooldownFinished<T> cooldownFinished, T parameter)
        {
            CoolTime = cooldown;
            CoolTime_cache = Cooldown.Cool();
            CooldownFinished = cooldownFinished;
            Parameter = parameter;
        }

        /// <inheritdoc> </inheritdoc>
        public bool IsCooled()
        {
            return Cooldown.IsCooled(CoolTime_cache, CoolTime);
        }

        /// <inheritdoc> </inheritdoc>
        public void Invoke()
        {
            CooldownFinished?.Invoke(Parameter);
        }

        /// <inheritdoc> </inheritdoc>
        public void CancelDelegate()
        {
            CooldownFinished = null;
        }
    }
}
