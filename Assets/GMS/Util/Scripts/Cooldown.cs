using UnityEngine;

namespace GMS
{
    /// <summary>
    /// Utility class for handling cooldowns.
    /// </summary>
    public static class Cooldown
    {
        /// <summary>
        /// Gets the current time since the level loaded.
        /// </summary>
        /// <returns>The current time since the level loaded.</returns>
        public static float Cool()
        {
            return Time.timeSinceLevelLoad;
        }
        
        /// <summary>
        /// Checks if the cooldown period has passed since a cached start time.
        /// </summary>
        /// <param name="cachedTimer">The start time of the cooldown, set with Time.timeSinceLevelLoad.</param>
        /// <param name="cooldown">The cooldown duration in seconds.</param>
        /// <returns>True if the cooldown period has passed; otherwise, false.</returns>
        public static bool IsCooled(this float  cachedTimer, float cooldown)
        {
            return Time.timeSinceLevelLoad > (cachedTimer + cooldown);
        }
        
        /// <summary>
        /// Checks if the cooldown period has passed since a cached start time and returns the pending time before it is cooled.
        /// </summary>
        /// <param name="cachedTimer">The start time of the cooldown, set with Time.timeSinceLevelLoad.</param>
        /// <param name="cooldown">The cooldown duration in seconds.</param>
        /// <param name="pendingTime">The pending time before the cooldown is finished. Useful for display purposes</param>
        /// <returns>True if the cooldown period has passed; otherwise, false.</returns>
        public static bool IsCooled(this float cachedTimer, float cooldown, out float pendingTime)
        {
            float currentTime = Time.timeSinceLevelLoad;
            pendingTime = (cachedTimer + cooldown) - currentTime;
            return pendingTime <= 0f;
        }
        
        /// <summary>
        /// Checks if the cooldown period has passed since a cached start time and returns the pending time and normalized waiting time.
        /// </summary>
        /// <param name="cachedTimer">The start time of the cooldown, set with Time.timeSinceLevelLoad.</param>
        /// <param name="cooldown">The cooldown duration in seconds.</param>
        /// <param name="pendingTime">The pending time before the cooldown is finished.</param>
        /// <param name="normalizedTime">The normalized waiting time (0 to 1) before the cooldown is finished.</param>
        /// <returns>True if the cooldown period has passed; otherwise, false.</returns>
        public static bool IsCooled(this float cachedTimer, float cooldown, out float pendingTime, out float normalizedTime)
        {
            IsCooled(cachedTimer,cooldown,out pendingTime);
            normalizedTime = Mathf.Clamp01((Time.timeSinceLevelLoad - cachedTimer) / cooldown);
            return pendingTime <= 0f;
        }
    }
}
