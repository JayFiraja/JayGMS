using System.Collections.Generic;
using UnityEngine;

namespace GMS.Samples
{
    /// <summary>
    /// This is a self-contained helper class for allowing us to get SpawnPoints within a grid, with a max number of elements in it.
    /// Constructing a grid with the useful Bounds struct.
    /// </summary>
    /// <typeparam name="T">The type of the spawnable items.</typeparam>
    public class BoundsGridSpawnables<T>
    {
        // Maximum number of attempts to find a valid offset
        private const int MAX_ATTEMPTS_ON_RANDOM_CELL_WIHTIN_RANGE = 100;

        public int _width;
        public int _height;

        private float _cellSize = 10f; // Each cell is 10x10 units
        private int _maxEntriesPerCell = 5;

        private Dictionary<Vector2Int, Bounds> _gridCells = new Dictionary<Vector2Int, Bounds>();
        private Dictionary<Vector2Int, int> _gridSpawnableCounts = new Dictionary<Vector2Int, int>();

        // Dictionary to store spawnable entries using a unique key (InstanceID)
        private Dictionary<int, SpawnableEntry<T>> _spawnables = new Dictionary<int, SpawnableEntry<T>>();
        private bool _isInitialized = false;

        /// <summary>
        /// Initializes the grid to cover a specified area with cells of a given size.
        /// </summary>
        /// <param name="cellSize">The size of each grid cell in meters.</param>
        /// <param name="maxEntriesPerCell">The maximum number of entries allowed per cell.</param>
        /// <param name="areaWidth">The total width of the area to be covered by the grid.</param>
        /// <param name="areaHeight">The total height of the area to be covered by the grid.</param>
        /// <param name="centerPosition">The center position of the grid in world space.</param>
        public void Initialize(float cellSize, int maxEntriesPerCell, int width, int height, Vector3 centerPosition)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _maxEntriesPerCell = maxEntriesPerCell;
            GenerateGrid(width, height, centerPosition);
            _isInitialized = true;
        }

        /// <summary>
        /// Debug method - Visualizes the grid by creating cube primitives for each grid cell.
        /// </summary>
        /// <param name="centerPosition">The center position of the grid in world space.</param>
        public void CreateGridVisualization(Vector3 centerPosition)
        {
            // Iterate through each grid cell and create a cube
            foreach (var cell in _gridCells.Values)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = cell.center;
                cube.transform.localScale = new Vector3(_cellSize, 0.1f, _cellSize); // Set the height to a small value for visualization

                // Set the cube's color and transparency
                var renderer = cube.GetComponent<Renderer>();
                renderer.material.color = new Color(0f, 1f, 1f, 0.5f); // Cyan with transparency

                // Optionally, disable the collider for visualization purposes
                var collider = cube.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }
        }

        /// <summary>
        /// Generates the grid based on the given area dimensions.
        /// </summary>
        /// <param name="areaWidth">The width of the area.</param>
        /// <param name="areaHeight">The height of the area.</param>
        /// <param name="centerPosition">The center position of the grid in world space.</param>
        private void GenerateGrid(int areaWidth, int areaHeight, Vector3 centerPosition)
        {
            // Calculate the number of cells along each dimension
            int numCellsX = Mathf.CeilToInt((float)areaWidth / _cellSize);
            int numCellsZ = Mathf.CeilToInt((float)areaHeight / _cellSize);

            //Debug.Log($"Number of cells: X = {numCellsX}, Z = {numCellsZ}");

            // Calculate offsets to position the grid centered around the specified center position
            float xOffset = centerPosition.x - (areaWidth / 2f);
            float zOffset = centerPosition.z - (areaHeight / 2f);

            // Initialize grid cells and counts
            for (int x = 0; x < numCellsX; x++)
            {
                for (int z = 0; z < numCellsZ; z++)
                {
                    // Calculate the center of each grid cell
                    Vector3 cellCenter = new Vector3(
                        x * _cellSize + _cellSize / 2f + xOffset,
                        0,
                        z * _cellSize + _cellSize / 2f + zOffset
                    );

                    Bounds cellBounds = new Bounds(cellCenter, new Vector3(_cellSize, 1, _cellSize));

                    Vector2Int gridIndex = new Vector2Int(x, z);
                    _gridCells[gridIndex] = cellBounds;
                    _gridSpawnableCounts[gridIndex] = 0;

                   // Debug.Log($"Initialized cell at {gridIndex} with center {cellCenter}");
                }
            }
        }

        /// <summary>
        /// Tries to get a spawn position near a focus point within grid bounds.
        /// </summary>
        /// <param name="focusPoint">The center point from which to find a spawn location.</param>
        /// <param name="cellRange">The cell range away from the focus point to search for a spawn location.</param>
        /// <param name="attempts">The number of spawn attempts to try.</param>
        /// <param name="spawnPosition">The output parameter for a valid spawn position, if found.</param>
        /// <returns>Returns true if a valid spawn position is found; otherwise, false.</returns>
        public bool TryGetSpawnPoint(Vector3 focusPoint, int cellRange, int attempts, out Vector3 spawnPosition)
        {
            spawnPosition = Vector3.zero;
            bool validPositionFound = false;

            if (!_isInitialized)
            {
                Debug.LogError("Initialization is needed!");
                return false;
            }

            Vector2Int focusCellIndex = GetGridCellIndex(focusPoint);

            for (int i = 0; i < attempts; i++)
            {
                int attemptCount = 0;

                do
                {
                    // Generate a random offset that meets the minimum cell range requirement
                    Vector2Int randomOffset = GetRandomOffset(cellRange);
                    Vector2Int targetCellIndex = focusCellIndex + randomOffset;

                    // Validate if the target cell is within bounds
                    if (_gridCells.ContainsKey(targetCellIndex))
                    {
                        // Retrieve cell bounds and find a random position within the target cell
                        Bounds targetBounds = _gridCells[targetCellIndex];
                        spawnPosition = GetRandomPositionWithinBounds(targetBounds);

                        // Ensure no spawnables are spawned too close to each other
                        validPositionFound = IsPositionValid(spawnPosition, targetCellIndex);
                    }

                    attemptCount++;
                } while (!validPositionFound && attemptCount < 10); // Limit attempts per position

                if (validPositionFound)
                {
                    Vector2Int gridIndex = GetGridCellIndex(spawnPosition);
                    _gridSpawnableCounts[gridIndex]++;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Generates a random offset that is at least a specified number of cells away from the origin.
        /// </summary>
        /// <param name="cellRange">The minimum number of cells for the offset.</param>
        /// <returns>A random Vector2Int offset within the specified range.</returns>
        private Vector2Int GetRandomOffset(int cellRange) 
        {
            int attempts = 0;
            int maxAttempts = MAX_ATTEMPTS_ON_RANDOM_CELL_WIHTIN_RANGE;

            int randomX, randomZ;

            do
            {
                randomX = Random.Range(-cellRange, cellRange + 1);
                randomZ = Random.Range(-cellRange, cellRange + 1);
                attempts++;
            }
            while ((Mathf.Abs(randomX) < cellRange || Mathf.Abs(randomZ) < cellRange) && attempts < maxAttempts);

            // Check if maxAttempts was reached and if so, fallback to a default safe offset
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Max attempts reached while trying to find a valid offset. Defaulting to safe offset.");
                randomX = cellRange;
                randomZ = cellRange;
            }

            return new Vector2Int(randomX, randomZ);
        }

        /// <summary>
        /// Gets a random position within the specified bounds.
        /// </summary>
        /// <param name="bounds">The bounds within which to generate a random position.</param>
        /// <returns>A random Vector3 position within the bounds.</returns>
        private Vector3 GetRandomPositionWithinBounds(Bounds bounds)
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);
            return new Vector3(randomX, 0, randomZ);
        }

        /// <summary>
        /// Checks if the specified position is valid for spawning.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <param name="gridIndex">The grid index corresponding to the position.</param>
        /// <returns>Returns true if the position is valid; otherwise, false.</returns>
        private bool IsPositionValid(Vector3 position, Vector2Int gridIndex)
        {
            float minDistanceBetweenSpawnables = 5f; // Minimum distance between spawnables

            // Check the grid cell and surrounding cells for existing spawnables
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    Vector2Int adjacentIndex = gridIndex + new Vector2Int(x, z);
                    if (_gridSpawnableCounts.TryGetValue(adjacentIndex, out int count) && count > 0)
                    {
                        // Ensure the current cell is not overcrowded
                        if (count >= _maxEntriesPerCell)
                        {
                            return false;
                        }

                        // Check distance to existing spawnables in this cell
                        foreach (var entry in _spawnables.Values)
                        {
                            Vector3 elementPosition = entry.TransformData.Position;
                            if (_gridCells.TryGetValue(adjacentIndex, out Bounds bounds))
                            {
                                if (bounds.Contains(position) && Vector3.Distance(elementPosition, position) < minDistanceBetweenSpawnables)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Calculates the grid cell index for the specified position.
        /// </summary>
        /// <param name="position">The position to convert to a grid index.</param>
        /// <returns>Returns the grid cell index as a Vector2Int.</returns>
        private Vector2Int GetGridCellIndex(Vector3 position)
        {
            int xIndex = Mathf.FloorToInt((position.x + (_width / 2)) / _cellSize);
            int zIndex = Mathf.FloorToInt((position.z + (_height / 2)) / _cellSize);
            return new Vector2Int(xIndex, zIndex);
        }


        /// <summary>
        /// Adds a new spawnable to the grid and updates the grid data.
        /// </summary>
        /// <param name="instanceID">View side gameobject's InstanceID</param>
        /// <param name="spawnable">The spawnable item.</param>
        /// <param name="transformData">The transform data for the spawnable item.</param>
        public void AddSpawnable(int instanceID, T spawnable, TransformData transformData)
        {
            _spawnables[instanceID] = new SpawnableEntry<T>(spawnable, transformData);
            Vector2Int gridIndex = GetGridCellIndex(transformData.Position);
            _gridSpawnableCounts[gridIndex]++;
        }

        /// <summary>
        /// Removes a spawnable from the grid.
        /// </summary>
        /// <param name="instanceID">View side gameobject's InstanceID</param>
        /// <param name="spawnable">The spawnable item to remove.</param>
        public void RemoveSpawnable(int instanceID, T spawnable)
        {
            SpawnableEntry<T> entry = _spawnables[instanceID];
            Vector2Int gridIndex = GetGridCellIndex(entry.StartingTransformData.Position);
            _gridSpawnableCounts[gridIndex]--;

            _spawnables.Remove(instanceID);
        }
    }
}
