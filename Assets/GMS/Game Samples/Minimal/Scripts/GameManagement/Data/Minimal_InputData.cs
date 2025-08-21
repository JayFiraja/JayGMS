using GMS;
using UnityEngine.InputSystem;
using UnityEngine;
namespace GMS.Samples
{
    [System.Serializable]
    [LinkDataLogic(typeof(Minimal_InputData), dataDisplayName: "Minimal_InputData", typeof(Minimal_InputManager), displayName: "Minimal_InputManager")]
    public struct Minimal_InputData : ISubManagerData
    {
        public InputActionReference PlayerMove;
    }
}
