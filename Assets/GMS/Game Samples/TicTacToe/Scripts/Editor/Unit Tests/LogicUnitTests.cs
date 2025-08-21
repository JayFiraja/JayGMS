using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

namespace GMS.Samples
{
    /// <summary>
    /// Class containing the unit tests for the critical logic for the game.
    /// </summary>
    public class LogicUnitTests : MonoBehaviour
    {
        [Test]
        public void CheckWinner_ShouldReturnTrue_ForWinningRow()
        {
            int[,] grid = new int[,]
            {
            { 1, 1, 1 },
            { 0, 0, 0 },
            { 2, 2, 0 }
            };

            bool result = GameLogic.CheckWinner(grid, 1, out List<Vector2> matchingCells);

            Assert.IsTrue(result);
        }

        [Test]
        public void CheckWinner_ShouldReturnTrue_ForWinningColumn()
        {
            int[,] grid = new int[,]
            {
            { 1, 0, 0 },
            { 1, 2, 0 },
            { 1, 0, 2 }
            };

            bool result = GameLogic.CheckWinner(grid, 1, out List<Vector2> matchingCells);

            Assert.IsTrue(result);
        }

        [Test]
        public void CheckWinner_ShouldReturnTrue_ForWinningDiagonal()
        {
            int[,] grid = new int[,]
            {
            { 1, 0, 0 },
            { 0, 1, 2 },
            { 2, 0, 1 }
            };

            bool result = GameLogic.CheckWinner(grid, 1, out List<Vector2> matchingCells);

            Assert.IsTrue(result);
        }

        [Test]
        public void CheckSequence_ShouldReturnTrue_ForPlayerTwoSequence()
        {
            int[,] grid = new int[,]
            {
            { 1, 2, 1 },
            { 0, 2, 0 },
            { 1, 2, 0 }
            };

            int target = 2;  // evaluate for player 2

            bool isWinner = GameLogic.CheckWinner(grid, target, out List<Vector2> matchingCells);

            Assert.IsTrue(isWinner);
        }

        [Test]
        public void CheckSequence_ShouldReturnFalse_IncompleteSequence()
        {
            int[,] grid = new int[,]
            {
            { 1, 1, 0 },
            { 0, 0, 0 },
            { 0, 2, 2 }
            };

            bool result = GameLogic.CheckSequence(grid, 0, 0, 0, 1, 2, out List<Vector2> matchingCells);

            Assert.IsFalse(result);
        }

        [Test]
        public void CheckSequence_ShouldReturnTrue_ForDiagonalSequence()
        {
            int[,] grid = new int[,]
            {
            { 1, 2, 2 },
            { 2, 1, 0 },
            { 0, 0, 1 }
            };

            bool result = GameLogic.CheckSequence(grid, 0, 0, 1, 1, 1, out List<Vector2> matchingCells);

            Assert.IsTrue(result);
        }

        [Test]
        public void TryGetRandomEmptyCell_GridWithEmptyCells_ReturnsTrueAndCoordinates()
        {
            // Arrange
            int[,] grid = new int[,]
            {
            { 1, 2, 2 },
            { 2, 1, 0 },
            { 2, 1, 1 }
            };

            // Act
            bool result = GameLogic.TryGetRandomEmptyCell(grid, out Vector2 emptyCell);

            // Assert
            Assert.IsTrue(result, "Expected to find an empty cell.");
            Assert.AreEqual(new Vector2(1, 2), emptyCell, "Expected the empty cell to be at (1, 2).");
        }

        [Test]
        public void TryGetRandomEmptyCell_GridWithoutEmptyCells_ReturnsFalse()
        {
            // Arrange
            int[,] grid = new int[,]
            {
            { 1, 2, 2 },
            { 2, 1, 1 },
            { 2, 1, 1 }
            };

            // Act
            bool result = GameLogic.TryGetRandomEmptyCell(grid, out Vector2 emptyCell);

            // Assert
            Assert.IsFalse(result, "Expected not to find any empty cells.");
            Assert.AreEqual(new Vector2(-1, -1), emptyCell, "Expected the coordinates to be (-1, -1) when no empty cells are found.");
        }
    }
}
