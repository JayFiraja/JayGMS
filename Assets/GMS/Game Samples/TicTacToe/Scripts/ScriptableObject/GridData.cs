using UnityEngine;

namespace GMS.Samples
{
    /// <summary>
    /// Data scriptable object used for spawning the grid and having a dynamic file for swaping out and experimenting with different elements
    /// </summary>
    [CreateAssetMenu(fileName = "GridData", menuName = "GMS Samples/TicTacToe")]
    public class GridData : ScriptableObject
    {
        [Header("Grid parameters")]
        public float Offset = 3.0f;
        [Tooltip("Transform position for the Grid container.")]
        public Vector3 TransformCoordinates;

        [Header("Cell parameters")]
        public GameObject CellPrefab;
        // note: Downgrading unity versions can cause BoxColliders to lose serialized data.
        public Vector3 BoxColliderCenter;
        public Vector3 BoxColliderSize;

        [Header("Marker parameters")]
        [Tooltip("Marker prefab linked to the behaviour Marker")]
        public Marker MarkerPrefab;

        [Header("Grid transform names to create")]
        public string CellsTransformName;
        public string MarksTransformName;
        public string LightsTransformName;

        [Header("Lights parameters")]
        public Light SpotLightPrefab;
        [Tooltip("Default is 3 as 1 light for 1 winning marker")]
        public int LightsToSpawn = 3;
        [Tooltip("Seconds in between positioning the lights over winning sequence")]
        public float LightInterval = 0.5f;
    }
}

