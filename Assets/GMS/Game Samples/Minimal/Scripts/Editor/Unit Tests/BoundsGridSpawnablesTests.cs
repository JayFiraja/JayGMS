using NUnit.Framework;
using UnityEngine;

namespace GMS.Samples
{
    public class BoundsGridSpawnablesTests
    {
        private BoundsGridSpawnables<CollectibleItem> _boundsGrid;
        private float _cellSize = 10f;
        private int _maxEntriesPerCell = 5;
        private int _width = 100; // Total width of the grid in meters
        private int _height = 100; // Total height of the grid in meters

        [SetUp]
        public void Setup()
        {
            _boundsGrid = new BoundsGridSpawnables<CollectibleItem>();
            _boundsGrid.Initialize(_cellSize, _maxEntriesPerCell, _width, _height, Vector3.zero);
        }

        [Test]
        public void TestValidSpawnPointsFromGridOrigin()
        {
            Vector3 focusPoint = Vector3.zero; // Center point in the level
            int cellRange = 2; // Range around focus point to try spawning
            int spawnAttempts = 10; // Number of attempts to find a valid spawn point

            // Attempt to get valid spawn points multiple times
             for (int i = 0; i < 5; i++)
            {
                Vector3 spawnPosition;
                bool success = _boundsGrid.TryGetSpawnPoint(focusPoint, cellRange, spawnAttempts, out spawnPosition);

                Assert.IsTrue(success, "Failed to find a valid spawn position");
                Assert.IsTrue(IsWithinBounds(spawnPosition), $"Spawn position {spawnPosition} is outside expected bounds");
                Assert.IsTrue(IsCellRangeValid(focusPoint, spawnPosition, cellRange), $"Spawn position {spawnPosition} is not {cellRange} cells away from the focus point {focusPoint}");
            }
        }

        [Test]
        public void TestValidSpawnPointsAcrossGrid()
        {
            // Define various focus points across the grid to ensure comprehensive testing
            Vector3[] focusPoints = new Vector3[]
            {
                new Vector3(-50, 0, -50), // Bottom-left corner 
                new Vector3(0, 0, -49),   // Bottom-center adjusted
                new Vector3(40, 0, -40),  // Bottom-right corner adjusted
                new Vector3(-40, 0, 0),   // Middle-left adjusted
                new Vector3(0, 0, 0),     // Center
                new Vector3(40, 0, 0),    // Middle-right adjusted
                new Vector3(-40, 0, 40),  // Top-left corner adjusted
                new Vector3(0, 0, 40),    // Top-center adjusted
                new Vector3(40, 0, 40),   // Top-right corner adjusted
                new Vector3(20, 0, 20)    // Arbitrary point near center
            };

            int cellRange = 2; // Range around focus point to try spawning
            int spawnAttempts = 10;  // Number of attempts to find a valid spawn point

            foreach (var focusPoint in focusPoints)
            {
                Vector3 spawnPosition;
                bool success = _boundsGrid.TryGetSpawnPoint(focusPoint, cellRange, spawnAttempts, out spawnPosition);

                Assert.IsTrue(success, $"Failed to find a valid spawn position for focus point {focusPoint}");
                Assert.AreNotEqual(Vector3.zero, spawnPosition, $"Spawn position is unexpectedly at (0, 0, 0) for focus point {focusPoint}");
                Assert.IsTrue(IsWithinBounds(spawnPosition), $"Spawn position {spawnPosition} is outside expected bounds for focus point {focusPoint}");
                Assert.IsTrue(IsCellRangeValid(focusPoint, spawnPosition, cellRange), $"Spawn position {spawnPosition} is not {cellRange} cells away from the focus point {focusPoint}");
            }
        }

        // Helper method to verify that the position is within the grid bounds
        private bool IsWithinBounds(Vector3 position)
        {
            bool withinWidth = Mathf.Abs(position.x) <= _width / 2;
            bool withinHeight = Mathf.Abs(position.z) <= _height / 2;

            return withinWidth && withinHeight;
        }

        // Helper method to check if the spawn position is at least 'cellRange' cells away from the focus point
        private bool IsCellRangeValid(Vector3 focusPoint, Vector3 spawnPosition, int cellRange)
        {
            Vector2Int focusCellIndex = GetGridCellIndex(focusPoint);
            Vector2Int spawnCellIndex = GetGridCellIndex(spawnPosition);

            int distanceX = Mathf.Abs(spawnCellIndex.x - focusCellIndex.x);
            int distanceZ = Mathf.Abs(spawnCellIndex.y - focusCellIndex.y); // Corrected to use y for Vector2Int

            return distanceX >= cellRange && distanceZ >= cellRange;
        }

        // Calculates the grid cell index for the specified position
        private Vector2Int GetGridCellIndex(Vector3 position)
        {
            int xIndex = Mathf.FloorToInt((position.x + (_width / 2)) / _cellSize);
            int zIndex = Mathf.FloorToInt((position.z + (_height / 2)) / _cellSize);
            return new Vector2Int(xIndex, zIndex);
        }
    }
}
