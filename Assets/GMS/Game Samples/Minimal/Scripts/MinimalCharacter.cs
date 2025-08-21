using UnityEngine;

namespace GMS.Samples
{
    /// <summary>
    /// Data representation object for a minimal character
    /// </summary>
    public class MinimalCharacter
    {
        private MinimalCharacterData _data;
        private MinimalCharacterView _minimalCharacterView;

        private TransformData _transformData;

        public MinimalCharacter(MinimalCharacterData data, MinimalCharacterView minimalCharacterView, TransformData startTransformData)
        {
            _data = data;
            _minimalCharacterView = minimalCharacterView;
            _transformData = startTransformData;
            _minimalCharacterView.SetAllTransforms(_transformData);
        }

        public void OnUpdate()
        {
            MoveAround();
        }

        private void MoveAround()
        {
            if (!_data.IsPlayerControlled)
            {
                return;
            }

            // Get the player move input
            Vector2 moveInput = Minimal_InputManager.PlayerMoveInput;

            // Calculate the movement speed using the animation curve
            float moveSpeed = _data.Acceleration.Evaluate(moveInput.magnitude) * _data.MaxSpeed;

            // Create a movement vector
            Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);

            if (_data.MoveInRelationToMainCamera && Camera.main.TryGetCameraRelativeDirectionsXZPlane(out Vector3 cameraForward, out Vector3 cameraRight))
            {
                // Calculate the desired direction relative to the camera in XZ plane
                Vector3 newDirection = cameraRight * moveInput.x + cameraForward * moveInput.y;

                // Apply the new direction to the movement
                movement = newDirection;
            }

            // Scale the movement by speed and deltaTime
            movement *= moveSpeed * Time.deltaTime;

            _transformData.Position += movement;
            _transformData.Rotation = RotationTowardsMovementDirection(_transformData.Rotation, movement);

            if (movement.magnitude > 0)
            {
                MinimalGameEvents.OnControllableCharacterMoved characterMovedEvent = GameEventService.GetEvent<MinimalGameEvents.OnControllableCharacterMoved>();
                characterMovedEvent.NewPosition = _transformData.Position;
                GameEventService.TriggerEvent(characterMovedEvent);
            }

            // Add movement to the character
            _minimalCharacterView.SetTransformPosition(_transformData);
        }

        // this rotation is smoothed, it uses the data's LookAtAcceleration
        private Quaternion RotationTowardsMovementDirection(Quaternion currentRotation, Vector3 movement)
        {
            // return If rotation towards movement direction is not enabled, or if there is no movement registered
            if (!_data.LookAtMovementDirection || movement.magnitude <= 0)
            {
                return currentRotation;
            }

            // Calculate the desired rotation based on the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(movement.normalized);

            // Calculate the angle difference between the current and target rotation
            float angleDifference = Quaternion.Angle(currentRotation, targetRotation);

            // Normalize the angle difference to a range of 0 to 1
            float normalizedAngleDifference = angleDifference / 180f; // 180 is the maximum possible angle difference

            // Calculate the smoothing factor from the animation curve based on the normalized angle difference
            float smoothingFactor = _data.LookAtAcceleration.Evaluate(normalizedAngleDifference);

            // Determine the maximum rotation step for this frame based on the specified rotation speed
            float maxRotationStep = _data.RotationSpeed * Time.deltaTime;

            // Calculate the actual rotation step by combining the smoothing factor and max rotation speed
            float rotationStep = Mathf.Min(smoothingFactor * angleDifference, maxRotationStep);

            // Smoothly rotate towards the desired direction using the calculated rotation step
            return Quaternion.RotateTowards(currentRotation, targetRotation, rotationStep);

        }
    }
}
