using System.Collections.Generic;
using UnityEngine;

namespace GMS
{
    /// <summary>
    /// Keeps track of game time.
    /// Helper class for adding cooldown delegates for wait actions without Coroutines.
    /// </summary>
    public class TimeManager: ISubManager
    {
        private TimeManagerData _data;

        private float _gameSeconds;
        
        // Lists for looping and invoking the delegate when cooled.
        private List<ICooldownDelegate> _cooldownDelegates = new List<ICooldownDelegate>(0);
        private List<ICooldownDelegate> _parametrizedCooldownDelegates = new List<ICooldownDelegate>(0);

        // Pools to avoiding generating garbage by creating new Delegates as necessary, and reuse free ones.
        private Stack<ICooldownDelegate> _cooldownDelegatePool = new Stack<ICooldownDelegate>(0);
        private Stack<ICooldownDelegate> _parametrizedCooldownDelegatePool = new Stack<ICooldownDelegate>(0);

        public TimeManager(TimeManagerData data)
        {
            _data = data;
            _cooldownDelegates.Clear();
            _parametrizedCooldownDelegates.Clear();
            _cooldownDelegatePool.Clear();
            _parametrizedCooldownDelegatePool.Clear();
        }
    
        #region ISubManager

        public bool Initialize(GameManager gameManager)
        {
            return true;
        }

        public void UnInitialize()
        {
           
        }

        public void OnUpdate()
        {
            _gameSeconds += Time.deltaTime;
            UpdateActions();
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

        /// <summary>
        /// Updates the cooldowns list.
        /// </summary>
        public void UpdateActions()
        {
            for (int i = _cooldownDelegates.Count - 1; i >= 0; i--)
            {
                ICooldownDelegate cooldownDelegate = _cooldownDelegates[i];
                if (cooldownDelegate.IsCooled())
                {
                    cooldownDelegate.Invoke();
                    cooldownDelegate.CancelDelegate();
                    _cooldownDelegates.RemoveAt(i);
                    _cooldownDelegatePool.Push(cooldownDelegate); // Return to pool
                }
            }

            for (int i = _parametrizedCooldownDelegates.Count - 1; i >= 0; i--)
            {
                ICooldownDelegate paramCooldownDelegate = _parametrizedCooldownDelegates[i];
                if (paramCooldownDelegate.IsCooled())
                {
                    paramCooldownDelegate.Invoke();
                    paramCooldownDelegate.CancelDelegate();
                    _parametrizedCooldownDelegates.RemoveAt(i);
                    _parametrizedCooldownDelegatePool.Push(paramCooldownDelegate); // Return to pool
                }
            }
        }
    
        /// <summary>
        /// Get the time in different formats with the given total seconds
        /// </summary>
        /// <param name="toString">minutes, or hours format</param>
        public void GetFormatedTime(float totalSeconds, out int hours, out int minutes, out string toString)
        {
            hours = (int)totalSeconds / 3600;
            minutes = (int)totalSeconds / 60;
            int displaySeconds = (int)totalSeconds - 60 * minutes;
    
            if (hours <= 0)
            {
                toString = string.Format("{0:00}:{1:00}", minutes, displaySeconds);
            }
            else
            {
                toString = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, displaySeconds);
            }
        }

        /// <summary>
        /// Returns a delegate so that when the cooldown is finished the delegate is called and the cooldown callback is met.
        /// Example:
        /// CooldownDelegate cooldownFinished = GameManager.S.timeManager.AddCooldown(0.1f);
        /// </summary>
        /// <param name="cooldown">cooldown time</param>
        /// <param name="cooldownFinished">delegate to call upon cooled</param>
        /// <returns>Returns the reference to the cooldownDelegate so it can be set to null in order to stop it, use <see cref="CooldownDelegate.CancelDelegate"/></returns>
        public CooldownDelegate AddCooldown(float cooldown, cooldownFinished cooldownFinished = null)
        {
            CooldownDelegate cooldownDelegate;
            if (_cooldownDelegatePool.Count > 0)
            {
                cooldownDelegate = (CooldownDelegate)_cooldownDelegatePool.Pop();
                cooldownDelegate.CoolTime = cooldown;
                cooldownDelegate.CoolTime_cache = Cooldown.Cool();
                cooldownDelegate.CooldownFinished = cooldownFinished;
            }
            else
            {
                cooldownDelegate = new CooldownDelegate(cooldown, cooldownFinished);
            }

            _cooldownDelegates.Add(cooldownDelegate);
            return cooldownDelegate;
        }

        /// <summary>
        /// Adds a parameterized cooldown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cooldown">cooldown time</param>
        /// <param name="cooldownFinished">delegate that receives the parameter</param>
        /// <param name="parameter">will be fed into the delegate</param>
        /// <returns>Returns the reference to the cooldownDelegate so it can be set to null in order to stop it, use <see cref="CooldownDelegate.CancelDelegate"/> </returns>
        public CooldownDelegate<T> AddCooldown<T>(float cooldown, CooldownFinished<T> cooldownFinished, T parameter)
        {
            CooldownDelegate<T> cooldownDelegate;
            if (_parametrizedCooldownDelegatePool.Count > 0 && _parametrizedCooldownDelegatePool.Peek() is CooldownDelegate<T>)
            {
                cooldownDelegate = (CooldownDelegate<T>)_parametrizedCooldownDelegatePool.Pop();
                cooldownDelegate.CoolTime = cooldown;
                cooldownDelegate.CoolTime_cache = Cooldown.Cool();
                cooldownDelegate.CooldownFinished = cooldownFinished;
                cooldownDelegate.Parameter = parameter;
            }
            else
            {
                cooldownDelegate = new CooldownDelegate<T>(cooldown, cooldownFinished, parameter);
            }

            _parametrizedCooldownDelegates.Add(cooldownDelegate);
            return cooldownDelegate;
        }

        /// <summary>
        /// Removes the CooldownFinished delegate to call and forces it's cooldown to finish.
        /// </summary>
        public void StopCooldownDelegate(CooldownDelegate cooldownDelegate)
        {
            if (cooldownDelegate == null)
            {
                return;
            }
            cooldownDelegate.CancelDelegate();
            // Finish the timer, so next tick this is automatically cleaned
            cooldownDelegate.CoolTime_cache = cooldownDelegate.CoolTime + Cooldown.Cool();
        }

        /// <summary>
        /// Removes the CooldownFinished delegate to call and forces it's cooldown to finish.
        /// </summary>
        public void StopCooldownDelegate<T>(CooldownDelegate<T> cooldownDelegate)
        {
            if (cooldownDelegate == null)
            {
                return;
            }
            cooldownDelegate.CancelDelegate();
            cooldownDelegate.CoolTime_cache = cooldownDelegate.CoolTime + Cooldown.Cool();
        }
    }
}
