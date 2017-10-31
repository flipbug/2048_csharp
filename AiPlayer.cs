using System;
using System.Threading;
using System.Collections.Generic;

namespace GameOf2048
{
    public class AiPlayer : IPlayer
    {

        public Moves MakeMove(int[][] board)
        {
            // Find the best move using the expectimax algorithm:
            // Generate a tree for each possible move and calculate the score,
            // once we reach a leaf. Then find the max score by backtracking through all nodes.
            // After each move, the game spawns a 2 or 4 on a random empty tile. So following 
            // a max node we add a chance node with the possibility for a number to appear on that tile.
            Expectimax algo = new Expectimax(GetMaxDepth(board));
            Engine engine = new Engine();
            Moves move = 0;

            // For parallel performance and deeper callstack a thread is created for each top level move. 
            List<Thread> threads = new List<Thread>();

            // Increase the default stack size per thread. Default is 1 MB.
            int maxStackSize = 1000 * 1000 * 1000; // 10 MB

            // Define a variable for each thread individually and pass it by reference to get a return value.
            // Maybe there is a nicer way?
            double score1 = 0;
            double score2 = 0;
            double score3 = 0;
            double score4 = 0;

            threads.Add(new Thread(() => algo.TopLevelMove(Moves.UP, Engine.CopyBoard(board), ref score1), maxStackSize));
            threads.Add(new Thread(() => algo.TopLevelMove(Moves.DOWN, Engine.CopyBoard(board), ref score2), maxStackSize));
            threads.Add(new Thread(() => algo.TopLevelMove(Moves.LEFT, Engine.CopyBoard(board), ref score3), maxStackSize));
            threads.Add(new Thread(() => algo.TopLevelMove(Moves.RIGHT, Engine.CopyBoard(board), ref score4), maxStackSize));

            // start threads
            threads.ForEach(x => x.Start());
            // wait for threads to finish
            threads.ForEach(x => x.Join());

            double bestScore = 0;
            double[] scores = new double[] { score1, score2, score3, score4 };

            for (int i = 0; i < scores.Length; i++)
            {
                // Todo: movepossible check should not be neccessary
                if (bestScore < scores[i] && engine.MovePossible((Moves)i, board))
                {
                    bestScore = scores[i];
                    move = (Moves)i;
                }

                Console.WriteLine($"Calculated Score: {scores[i]}");
                Console.WriteLine($"Move: {(Moves)i}");
            }

            Console.WriteLine($"Best Score: {bestScore}");

            // Random move as ugly fallback to make the last move.
            if (bestScore == 0)
            {
                Random rd = new Random();
                move = (Moves)rd.Next(0, 3);
            }

            return move;
        }

        private int GetMaxDepth(int[][] board)
        {
            int emptyTiles = 0;
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] == 0)
                        emptyTiles++;
                }
            }

            /*if (emptyTiles < 2)
                return 6;
            */
            if (emptyTiles < 4)
                return 4;

            if (emptyTiles < 6)
                return 3;

            return 2;
        }
    }
}