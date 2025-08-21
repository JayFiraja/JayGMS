using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS.Samples
{
    /// <summary>
    /// Wrapper for being able to design a minimal character.
    /// </summary>
    [System.Serializable]
    public struct MinimalCharacterData
    {
        public bool IsPlayerControlled;

        public MinimalCharacterView characterPrefab;

        [Header("Movement Attributes")]
        public string MoveAnimatorKey;
        public float MaxSpeed;
        public AnimationCurve Acceleration;
        public bool MoveInRelationToMainCamera;

        [Header("Rotation Attributes")]
        public bool LookAtMovementDirection;
        public float RotationSpeed;
        public AnimationCurve LookAtAcceleration;

        [Header("SFX")]
        public AudioClip[] JumpClips;
    }
}
