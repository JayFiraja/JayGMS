using GMS;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GMS.Samples
{
    /// <summary>
    /// Example manager class that captures the inputs from InputActionReferences set on the Data struct.
    /// Feeds static properties with the current inputs, so any class is able to just read the inputs they need.
    /// </summary>
    public class Minimal_InputManager : ISubManager
    {
        /// <summary>
        /// Read the current input for player movement
        /// </summary>
        public static Vector2 PlayerMoveInput => _playerMoveInput;
        private static Vector2 _playerMoveInput;
        private InputAction _playerMoveAction;

        private Minimal_InputData _data;

        public Minimal_InputManager(Minimal_InputData data)
        {
            _data = data;
        }

        public bool Initialize(GMS.GameManager gameManager)
        {
            _playerMoveAction = _data.PlayerMove.action;
            _playerMoveAction.Enable();
            _playerMoveAction.performed += OnPlayerMove;
            _playerMoveAction.canceled += OnPlayerMove;
            return true;
        }

        public void UnInitialize()
        {
            _playerMoveAction.performed -= OnPlayerMove;
            _playerMoveAction.canceled -= OnPlayerMove;
            _playerMoveAction.Disable();
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

        private void OnPlayerMove(InputAction.CallbackContext context)
        {
            _playerMoveInput = context.ReadValue<Vector2>();
        }
    }
}
