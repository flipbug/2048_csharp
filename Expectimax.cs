using System;
using System.Threading;
using System.Collections.Generic;

namespace GameOf2048
{
    public class Expectimax
    {
        private Engine engine;

        private int maxDepth;

        private double[,] TileWeights = new double[,] {{16, 12, 10, 8},
                                                        {1, 2, 4, 6},
                                                        {0.8, 0.6, 0.4, 0.2},
                                                        {0.04, 0.06, 0.08, 0.1}};


        public Expectimax(int maxDepth) {
            this.engine = new Engine();
            this.maxDepth = maxDepth;
        }

        public void TopLevelMove(Moves move, int[][] board, ref double score)
        {
            // Wrapper method to start the search tree for a single move.
            // This way we can use it together with mutlithreading.
            score = MaxNode(move, board, 0);
        }

        private double MaxNode(Moves move, int[][] board, int depth)
        {
            double score = 0, chance_one = 0, chance_two = 0;
            int[][] newBoard = Engine.CopyBoard(board);
            int mergedValue = engine.ExecuteMove(move, newBoard);

            // If the mergedValue is -1 the move was invalid.
            if (mergedValue < 0)
                return 0;

            depth++;

            // Stop if the state of the board has not changed or 
            // the maximum depth has been reached.
            if (Engine.BoardEquals(newBoard, board) || depth >= maxDepth)
                return CalculateScore(newBoard);

            // Go futher down the tree. Each empty field has a chance of spawing a number,
            // so we have to create a new branch for every possibility.
            // List<int[]> emptyTiles = new List<int[]>();
            for (int i = 0; i < newBoard.Length; i++)
            {
                for (int j = 0; j < newBoard[i].Length; j++)
                {
                    if (newBoard[i][j] == 0)
                    {
                        // The number 2 has a chance of 90% to appear next, while 4 has only 10%
                        newBoard[i][j] = 2;
                        chance_one = ChanceNode(0.9, Engine.CopyBoard(newBoard), depth);

                        // Optimization: Ignore the 4 branch for deep trees
                        if (maxDepth < 5) {
                            newBoard[i][j] = 4;
                            chance_two = ChanceNode(0.1, Engine.CopyBoard(newBoard), depth);
                        }

                        // Reset the field
                        newBoard[i][j] = 0;

                        // Use the highest score.
                        score = Math.Max(score, Math.Max(chance_one, chance_two));
                    }
                }
            }

            return score;
        }

        private double ChanceNode(double chance, int[][] board, int depth)
        {
            double score = 0;

            // Each new chance node has four possible moves. We sum the scores for each move 
            // together and multiply it by the chance value. 
            for (int i = 0; i < 4; i++)
            {
                score += MaxNode((Moves)i, board, depth);
            }

            return score * chance;
        }

        private double CalculateScore(int[][] board)
        {
            double score = 0;
            int emptyTiles = 0;

            // Calculate the heuristic value of the given board. 
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] == 0)
                        emptyTiles++;
                    else
                        score += board[i][j] * board[i][j] * TileWeights[i, j];
                }
            }
            return score * Math.Max(emptyTiles, 1);
        }
    }
}