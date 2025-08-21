using UnityEngine;

namespace GMS.Samples
{
    /// <summary>
    /// View representation of the minimal character.
    /// </summary>
    public class MinimalCharacterView : MonoBehaviour
    {
        [Header("Components")]
        public Animator animator;
        private string _moveAnimatorKey;
        private float _currentDelta = 0;
        private Vector3 _lastPosition;
        private MinimalCharacterData _data;
        private AudioManager _audioManager;

        public void Initialize(MinimalCharacterData data)
        {
            _data = data;
            GameInstance.TryGetSubManager(out _audioManager);

            if (animator == null)
            {
                Debug.LogError("Animator expected");
            }
            _moveAnimatorKey = data.MoveAnimatorKey;
            _lastPosition = transform.position;
        }

        public void SetAllTransforms(TransformData transformData)
        {
            transform.position = transformData.Position;
            transform.rotation = transformData.Rotation;
            transform.localScale = transformData.Scale;
        }

        public void SetTransformPosition(TransformData transformData, bool updateAnimator = true)
        {
            _lastPosition = transform.position;
            _currentDelta = (_lastPosition - transformData.Position).magnitude;
            transform.position = transformData.Position;
            transform.localRotation = transformData.Rotation;

            if (updateAnimator)
            {
                UpdateAnimator();
            }
        }

        public void UpdateAnimator()
        {
            animator.SetFloat(_moveAnimatorKey, _currentDelta);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IPickUp pickUp))
            {
                return;
            }
            
            MinimalGameEvents.ItemPickedUp itemPickupEvent = GameEventService.GetEvent<MinimalGameEvents.ItemPickedUp>();
            itemPickupEvent.PickupItemUp = pickUp;
            itemPickupEvent.PickedByPlayableCharacter = _data.IsPlayerControlled;
            GameEventService.TriggerEvent(itemPickupEvent);
        }

        /// <summary>
        /// Plays the jump audio clip from data using AudioManager 
        /// <see cref="AudioManager"/>
        /// </summary>
        public void PlayJumpSound()
        {
            _audioManager?.PlayClipAt(_data.JumpClips, transform.position);
        }
    }
}
