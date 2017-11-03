using System;
using System.Threading;
using System.Collections.Generic;

namespace GameOf2048
{
    public class Expectimax
    {
        private Engine engine;

        private int maxDepth;

        private double[,] TileWeights = new double[,] {{16,11,10,9},
                                                        {1,2,3,4},
                                                        {0.8,0.7,0.6,0.5},
                                                        {0.1,0.2,0.3,0.4}};

        public Expectimax(int maxDepth)
        {
            this.engine = new Engine();
            this.maxDepth = maxDepth;
        }

        public void TopLevelMove(Moves move, int[][] board, ref double score)
        {
            // Wrapper method to start the search tree for a single move.
            // This way we can use it together with mutlithreading.
            int[][] newBoard = Engine.CopyBoard(board);
            int mergedValue = engine.ExecuteMove(move, newBoard);

            if (!Engine.BoardEquals(newBoard, board))
                score = Math.Max(score, ChanceNode(newBoard, 0));
            else
                score = 0;
        }

        private double ChanceNode(int[][] board, int depth)
        {
            double score = 0;

            // Stop if the state of the board has not changed or 
            // the maximum depth has been reached.
            if (depth >= maxDepth)
                return CalculateScore(board);

            // Go futher down the tree. Each empty field has a chance of spawing a number,
            // so we have to create a new branch for every possibility. 
            // For performance reasons we ignore the chance factor of individual tiles.
            // e.g. 1/emptyTiles
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    
                    if (board[i][j] == 0)
                    {
                        // The number 2 has a chance of 90% to appear next, while 4 has only 10%
                        board[i][j] = 2;
                        score += MoveNode(Engine.CopyBoard(board), depth) * 0.9;

                        board[i][j] = 4;
                        score += MoveNode(Engine.CopyBoard(board), depth) * 0.1;

                        // Reset the field
                        board[i][j] = 0;
                    }
                }
            }

            return score;
        }

        private double MoveNode(int[][] board, int depth)
        {
            double score = 0;
            depth++;
            // Execute all possible moves and give the new state to the chance node. 
            // The biggest score will be returned or 0 if no moves are possible.
            for (int i = 0; i < 4; i++)
            {
                int[][] newBoard = Engine.CopyBoard(board);
                int mergedValue = engine.ExecuteMove((Moves)i, newBoard);
                if (!Engine.BoardEquals(newBoard, board))
                    score = Math.Max(score, ChanceNode(newBoard, depth));
            }

            return score;
        }

        private double CalculateScore(int[][] board)
        {
            double score = 0;
            double value = 0;
            int emptyTiles = 0;

            // Calculate the heuristic value of the given board. 
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    value = 0;
                    if (board[i][j] == 0)
                    {
                        emptyTiles++;
                    }
                    else
                    {
                        value = board[i][j];
                        
                        // bonus if prev > current > next
                        if (j > 0 && board[i][j - 1] > value)
                            value *= 2;
                        if (j < board[i].Length - 1 && board[i][j + 1] < value)
                            value *= board[i][j + 1] == board[i][j] / 2 ? 4 : 2;

                        // bonus if above > current > below
                        if (i > 0 && board[i - 1][j] > value)
                            value *= 2;
                        if (i < board.Length - 1 && board[i + 1][j] < value)
                            value *= 2;                        

                        score += value * TileWeights[i, j] * TileWeights[i, j];

                    }
                }
            }
            return score * emptyTiles * emptyTiles;
        }
    }
}