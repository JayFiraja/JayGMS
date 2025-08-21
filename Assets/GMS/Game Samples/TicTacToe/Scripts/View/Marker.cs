using System.Collections;
using UnityEngine;

namespace GMS.Samples
{
    /// <summary>
    /// Contains the View behaviour for the markers, independent of player.
    /// Positions the marker to the target location and applies rotation effects to the marker.
    /// Fires action <see cref="GlobalActions.MarkDrawCompletedOnCell"/> to process next player turn & win condition check.
    /// </summary>
    public class Marker : MonoBehaviour
    {
        [Header("Set in Inspector")]
        [SerializeField]
        private Transform model_T;
        [SerializeField]
        private GameObject groundSmashDecal;
        [SerializeField]
        private float initialHeight = 10;
        [SerializeField, Tooltip("Animation curve to control fall speed")]
        private AnimationCurve fallCurve;
        [SerializeField, Tooltip("Intensity of the vibration")]
        private float vibrationIntensity = 3.0f;
        [SerializeField, Tooltip("Number of rotations during the fall")]
        private int rotations = 3;

        // variable
        private Coroutine _fallingCo;
        private Vector3 _initialPosition;
        private Vector3 _targetFallPosition;
        private float _elapsedTime;
        private CellData _cellData;
        private bool _isInitialized;
        private TicTacToeAudio _audio;

        public void Initialize(PlayerData playerData, CellData cellData)
        {
            GameInstance.TryGetSubManager(out _audio);
            _cellData = cellData;
            _targetFallPosition = _cellData.LocalPosition;

            groundSmashDecal.SetActive(false);

            _initialPosition = _targetFallPosition;
            _initialPosition.y = initialHeight;
            transform.localPosition = _initialPosition;

            GameObject prefab = playerData.usingAlternativeMark ? playerData.AlternativeMarkPrefab : playerData.DefaultMarkPrefab;
            SpawnModel(prefab);
        }

        private void SpawnModel(GameObject prefabToSpawn)
        {
            if (prefabToSpawn == null)
            {
                return;
            }

            Instantiate(prefabToSpawn, model_T);

            if (_fallingCo != null)
            {
                StopCoroutine(_fallingCo);
            }
            _fallingCo = StartCoroutine(FallToTargetPosition());
        }

        private IEnumerator FallToTargetPosition()
        {
            _elapsedTime = 0.0f;
            float fallDuration = fallCurve[fallCurve.keys.Length - 1].time;

            Quaternion originalRotation = model_T.localRotation;
            Quaternion startRotation = originalRotation;
            int currentRotationStep = -1; // Initialize to -1 to ensure the first rotation occurs immediately
            Quaternion endRotation = GetRandomRotation(originalRotation, vibrationIntensity);

            float rotationSegmentDuration = fallDuration / rotations;
            float rotationSegmentElapsedTime = 0.0f;

            _audio?.PlayFallingMarker(transform.position);

            while (_elapsedTime < fallDuration)
            {
                _elapsedTime += Time.deltaTime;
                float t = _elapsedTime / fallDuration;
                float curveValue = fallCurve.Evaluate(t);

                // Calculate new position
                Vector3 newPosition = transform.localPosition;
                // Apply curve to Y-axis only
                newPosition.y = Mathf.Lerp(_initialPosition.y, _targetFallPosition.y, curveValue);

                // Update rotation segment elapsed time
                rotationSegmentElapsedTime += Time.deltaTime;

                // Check if we need to change the rotation
                if (rotationSegmentElapsedTime >= rotationSegmentDuration)
                {
                    currentRotationStep++;
                    rotationSegmentElapsedTime = 0.0f;

                    // If this is the last rotation step, transition to the original rotation
                    if (currentRotationStep == rotations)
                    {
                        endRotation = originalRotation;
                    }
                    else
                    {
                        startRotation = model_T.localRotation; // Use the current rotation as the start for the next
                        endRotation = GetRandomRotation(originalRotation, vibrationIntensity);
                    }
                }

                // Apply rotation effect
                float rotationProgress = rotationSegmentElapsedTime / rotationSegmentDuration; // Progress within the current rotation segment

                // set transforms
                model_T.localRotation = Quaternion.Lerp(startRotation, endRotation, rotationProgress);
                transform.localPosition = newPosition;
                yield return null;
            }

            // Ensure the object lands flat and at the exact target position
            transform.localPosition = _targetFallPosition;
            model_T.localRotation = originalRotation;

            FallImpact();

            _fallingCo = null;
        }

        private Quaternion GetRandomRotation(Quaternion baseRotation, float angleRange)
        {
            float rotationAngleX = Random.Range(-angleRange, angleRange);
            float rotationAngleY = Random.Range(-angleRange, angleRange);
            float rotationAngleZ = Random.Range(-angleRange, angleRange);
            return baseRotation * Quaternion.Euler(rotationAngleX, rotationAngleY, rotationAngleZ);
        }

        private void FallImpact()
        {
            // Play impact sound
            _audio?.PlayMarkerFallImpact(transform.position);

            // Activate ground smash decal
            groundSmashDecal.SetActive(true);

            TTCEvents.MarkDrawCompletedOnCell markDrawCompletedEvent = GameEventService.GetEvent<TTCEvents.MarkDrawCompletedOnCell>();
            markDrawCompletedEvent.CellData = _cellData;
            GameEventService.TriggerEvent(markDrawCompletedEvent);
        }

        /// <summary>
        /// Check if this marker is set as marked, aka sitting on a grid.
        /// </summary>
        /// <returns>true if marked and sitting on a grid.</returns>
        public bool IsMarked()
        {
            return _cellData.IsMarked;
        }

        /// <summary>
        /// Clean up model, hide effects
        /// </summary>
        public void MakeDormant()
        {
            _cellData.IsMarked = false;
            // clean up current loaded model
            if (model_T.childCount > 0)
            {
                for (int i = model_T.childCount - 1; i >= 0; i--)
                {
                    Destroy(model_T.GetChild(i).gameObject);
                }
            }
            groundSmashDecal.SetActive(false);
        }
    }
}
    
