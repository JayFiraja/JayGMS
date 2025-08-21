using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace GMS.Samples
{
    /// <summary>
    /// This contains the critical game logic.
    /// </summary>
    public static class GameLogic
    {
        /// <summary>
        /// The number of sequencial marks that define a winner.
        /// </summary>
        public const int MATCH_COUNT = 3;
        /// <summary>
        /// Grid dimension for Tic Tac Toe this is a 3x3
        /// </summary>
        public const int GRID_DIMENSION = 3;

        /// <summary>
        /// Checks the given grid and validates if the target has achieved a tic Tac Toe Winner
        /// by Matching a sequence of 3 marks in any direction.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="target">marker number, 0 as empty cell</param>
        /// <returns></returns>
        public static bool CheckWinner(int[,] grid, int target, out List<Vector2> matchingCells)
        {
            int rowCount = grid.GetLength(0);
            int colCount = grid.GetLength(1);
            matchingCells = new List<Vector2>();

            // Check rows
            for (int i = 0; i < rowCount; i++)
            {
                if (CheckSequence(grid, startRow: i, startCol: 0, rowStep: 0, colStep: 1, target, out matchingCells))
                {
                    return true;
                }
            }

            // Check columns
            for (int j = 0; j < colCount; j++)
            {
                if (CheckSequence(grid, startRow: 0, startCol: j, rowStep: 1, colStep: 0, target, out matchingCells))
                {
                    return true;
                }
            }

            // Check diagonals
            if (CheckSequence(grid, startRow: 0, startCol: 0, rowStep: 1, colStep: 1, target, out matchingCells) || CheckSequence(grid, startRow: 0, startCol: colCount - 1, rowStep: 1, colStep: -1, target, out matchingCells))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the given sequence step checks meet the match count <see cref="MATCH_COUNT"/>
        /// </summary>
        /// <param name="grid">Given grid</param>
        /// <param name="startRow"></param>
        /// <param name="startCol"></param>
        /// <param name="rowStep">step in which the sequence is calculated</param>
        /// <param name="colStep">step in which the sequence is calculated</param>
        /// <param name="target">target value to find a match from</param>
        /// <returns></returns>
        public static bool CheckSequence(int[,] grid, int startRow, int startCol, int rowStep, int colStep, int target, out List<Vector2> matchingCells)
        {
            matchingCells = new List<Vector2>();
            int count = 0;
            int rowCount = grid.GetLength(dimension: 0);
            int colCount = grid.GetLength(dimension: 1);

            for (int i = 0; i < rowCount && i < colCount; i++)
            {
                int row = startRow + i * rowStep;
                int col = startCol + i * colStep;

                if (row >= rowCount || col >= colCount || row < 0 || col < 0)
                {
                    break;
                }

                if (grid[row, col] == target)
                {
                    count++;
                    matchingCells.Add(new Vector2(row, col));
                    if (count == MATCH_COUNT)
                    {
                        return true;
                    }
                }
                else
                {
                    count = 0;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a random grid element that has a 0.
        /// </summary>
        /// <param name="grid">The grid to search for an empty cell.</param>
        /// <param name="emptyCell">The coordinates of the found empty cell.</param>
        /// <returns>True if an empty cell was found; otherwise, false.</returns>
        public static bool TryGetRandomEmptyCell(int[,] grid, out Vector2 emptyCell)
        {
            List<Vector2> emptyCells = new List<Vector2>();

            int rowCount = grid.GetLength(0);
            int colCount = grid.GetLength(1);

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    if (grid[i, j] == 0)
                    {
                        emptyCells.Add(new Vector2(i, j));
                    }
                }
            }

            if (emptyCells.Count == 0)
            {
                emptyCell = new Vector2(-1, -1); // Return an invalid position
                return false; // No empty cells found
            }

            Random random = new Random();
            int randomIndex = random.Next(emptyCells.Count);
            emptyCell = emptyCells[randomIndex];
            return true; // Empty cell found
        }

        /// <summary>
        /// Creates a new 3x3 grid with all values set to 0.
        /// </summary>
        /// <returns>A new 3x3 grid with all values set to 0</returns>
        public static int[,] CreateNewGrid()
        {
            int[,] grid = new int[GRID_DIMENSION, GRID_DIMENSION];
            for (int i = 0; i < GRID_DIMENSION; i++)
            {
                for (int j = 0; j < GRID_DIMENSION; j++)
                {
                    grid[i, j] = 0;
                }
            }
            return grid;
        }
    }
}
