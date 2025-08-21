using UnityEngine;

namespace GMS
{
    public struct TransformData
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public static TransformData Default => new TransformData
        {
            Position = Vector3.zero,
            Rotation = Quaternion.identity,
            Scale = Vector3.one
        };

        public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        /// <summary>
        /// Constructor that converts rotation euler to Quaternion.
        /// </summary>
        public TransformData(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Position = position;
            Rotation = Quaternion.Euler(rotation);
            Scale = scale;
        }
    }
}
