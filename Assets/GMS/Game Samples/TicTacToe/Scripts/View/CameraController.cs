using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GMS.Samples
{
    /// <summary>
    /// This class allows to smoothly orbit around the camera's target.
    /// </summary>
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraController : MonoBehaviour
    {
        // Members
        [Header("Parameters")]
        public float rotationSpeed = 10f;
        [Range(0.01f, 0.99f)]
        public float momentumDecay = 0.95f;
        // Minimum Y angle in degrees
        [Range(-25f, -40f)]
        public float minYAngle = -30;
        // Maximum Y angle in degrees
        [Range(-10f, -24f)]
        public float maxYAngle = -20;

        [Header("Assign in Inspector")]
        public InputActionReference orbitInputRef;
        private InputAction _orbitInput;
        public InputActionReference cursorDeltaRef;
        private InputAction _cursorDelta;

        private CinemachineVirtualCamera _cineMachineCamera;

        // variables
        private Vector2 _lastMousePosition;
        private Vector2 _momentum;
        private bool _isDragging;
        private float _currentYAngle = 0f;
        public bool IsCameraInputAllowed { get; set;}

        public void OnUpdate()
        {
            if (!IsCameraInputAllowed)
            {
                return;
            }
            _isDragging = _orbitInput.IsPressed();
            HandleMouseInput();
            ApplyMomentum();
        }

        public void Initialize()
        {
            _isDragging = false;

            TryGetComponent(out _cineMachineCamera);
            // Initialize current Y angle based on initial camera orientation
            Vector3 direction = (transform.position - _cineMachineCamera.Follow.position).normalized;
            _currentYAngle = Vector3.Angle(Vector3.up, direction) - 90f;


            if (orbitInputRef == null)
            {
                Debug.LogError("Input Action Assignment orbitInputRef is missing");
            }
            else
            {
                _orbitInput = orbitInputRef.action;
                _orbitInput.Enable();
                _orbitInput.started += OnDraggingStarted;
                _orbitInput.performed += OnDraggingEnded;
            }

            if (cursorDeltaRef == null)
            {
                Debug.LogError("Input Action Assignment cursorDeltaRef is missing");
            }
            else
            {
                _cursorDelta = cursorDeltaRef.action;
                _cursorDelta.Enable();
            }
        }

        public void UnInitialize()
        {
            if (_orbitInput != null)
            {
                _orbitInput.started -= OnDraggingStarted;
            }
        }

        private void OnDraggingStarted(InputAction.CallbackContext context)
        {
            _isDragging = true;
        }

        private void OnDraggingEnded(InputAction.CallbackContext context)
        {
            _isDragging = false;
        }

        void HandleMouseInput()
        {
            if (!_isDragging)
            {
                return;
            }

            Vector2 delta = _cursorDelta.ReadValue<Vector2>();
            delta *= rotationSpeed * Time.deltaTime;

            _momentum = delta;
            ApplyLookCameraAxisValue(delta);
        }

        private void ApplyMomentum()
        {
            if (_isDragging)
            {
                return;
            }

            _momentum *= momentumDecay;
            ApplyLookCameraAxisValue(_momentum);
        }

        private void ApplyLookCameraAxisValue(Vector2 delta)
        {
            if (delta.sqrMagnitude < 0.001f)
            {
                return;
            }

            transform.RotateAround(_cineMachineCamera.Follow.position, Vector3.up, delta.x);

            float newYAngle = _currentYAngle - delta.y; // Subtract delta.y to align with natural dragging direction
            newYAngle = Mathf.Clamp(newYAngle, minYAngle, maxYAngle);

            float angleChange = newYAngle - _currentYAngle;
            _currentYAngle = newYAngle;

            transform.RotateAround(_cineMachineCamera.Follow.position, transform.right, angleChange);
        }
    }
}
