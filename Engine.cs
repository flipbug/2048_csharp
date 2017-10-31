using System;
using System.Collections.Generic;

namespace GameOf2048
{
    public class Engine
    {
        public Boolean SpawnNumber(int[][] board)
        {
            // Spawn a 2 or 4 on a random empty field
            List<int[]> empty_tiles = new List<int[]>();
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] == 0)
                    {
                        empty_tiles.Add(new int[] { i, j });
                    }
                }
            }

            if (empty_tiles.Count > 0)
            {
                Random rnd = new Random();
                int index = rnd.Next(0, empty_tiles.Count);
                int number = rnd.Next(1, 10) < 9 ? 2 : 4;

                board[empty_tiles[index][0]][empty_tiles[index][1]] = number;

                return true;
            }
            else
            {
                return false;
            }
        }

        public int ExecuteMove(Moves move, int[][] board)
        {
            int mergedValue = 0;
            int[][] prevBoard = CopyBoard(board);

            switch (move)
            {
                case Moves.UP:
                    mergedValue = MergeUp(board);
                    break;
                case Moves.DOWN:
                    mergedValue = MergeDown(board);
                    break;
                case Moves.LEFT:
                    mergedValue = MergeLeft(board);
                    break;
                case Moves.RIGHT:
                    mergedValue = MergeRight(board);
                    break;
            }

            // The move is only valid if the state of the board has changed.
            if (BoardEquals(prevBoard, board))
            {
                return -1;
            }
            else 
            {
                // spawn a new number in case of a valid move
                SpawnNumber(board);
                return mergedValue;
            }
        }

        public Boolean NoMovesPossible(int[][] board)
        {
            var mergedValue = -1;
            foreach (Moves move in Enum.GetValues(typeof(Moves)))
            {
                int[][] tmpBoard = CopyBoard(board);
                mergedValue = Math.Max(mergedValue, ExecuteMove((Moves)move, tmpBoard));
            }
            return mergedValue < 0;
        }

        public Boolean MovePossible(Moves move, int[][] board)
        {
            int[][] newBoard = CopyBoard(board);
            ExecuteMove(move, newBoard);
            return !BoardEquals(newBoard, board);
        }

        public static Boolean BoardEquals(int[][] boardA, int[][] boardB)
        {
            for (int i = 0; i < boardA.Length; i++)
            {
                for (int j = 0; j < boardA[i].Length; j++)
                {
                    if (boardA[i][j] != boardB[i][j])
                        return false;
                }
            }
            return true;
        }

        public static int[][] CopyBoard(int[][] source)
        {
            int[][] dest = new int[source.Length][];
            for (int i = 0; i < source.Length; i++)
            {
                dest[i] = (int[])source[i].Clone();
            }
            return dest;
        }

        private int MergeLeft(int[][] board)
        {
            int merged = 0;
            for (int i = 0; i < board.Length; i++)
            {
                merged += MergeLeftRow(board[i], 0);
            }
            return merged;

        }

        private int MergeRight(int[][] board)
        {
            // reverse so we can merge left
            for (int i = 0; i < board.Length; i++)
            {
                Array.Reverse(board[i]);
            }

            var merged = MergeLeft(board);

            // reverse again
            for (int i = 0; i < board.Length; i++)
            {
                Array.Reverse(board[i]);
            }

            return merged;
        }

        private int MergeUp(int[][] board)
        {
            RotateCCW(board);
            var merged = MergeLeft(board);
            RotateCW(board);

            return merged;
        }

        private int MergeDown(int[][] board)
        {
            RotateCW(board);
            var merged = MergeLeft(board);
            RotateCCW(board);

            return merged;
        }

        private int MergeLeftRow(int[] row, int merged)
        {
            ShiftLeft(row);
            int initMerged = merged;
            for (int i = 0; i < row.Length - 1; i++)
            {
                int x = row[i];
                if (x == row[i + 1])
                {
                    x *= 2;
                    merged += x;
                    row[i] = x;
                    row[i + 1] = 0;
                }
            }

            if (initMerged == merged)
            {
                return merged;
            }
            else
            {
                return MergeLeftRow(row, merged);
            }
        }

        private void ShiftLeft(int[] row)
        {
            int zeroCount = 0;
            for (int i = 0; i < row.Length; i++)
            {
                if (row[i] == 0)
                {
                    // count how many times the row has to be shifted left
                    zeroCount++;
                }
                else if (zeroCount > 0)
                {
                    // reset main counter
                    i = i - zeroCount;

                    // shift array n times
                    for (int j = i; j < row.Length - zeroCount; j++)
                    {
                        row[j] = row[j + zeroCount];
                        row[j + zeroCount] = 0;
                    }
                    zeroCount = 0;
                }
            }
        }

        private void RotateCW(int[][] board)
        {
            int n = board.Length;
            int[][] tmpBoard = new int[4][];
            tmpBoard[0] = new int[4];
            tmpBoard[1] = new int[4];
            tmpBoard[2] = new int[4];
            tmpBoard[3] = new int[4];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    tmpBoard[i][j] = board[n - j - 1][i];
                }
            }

            for (int i = 0; i < n; i++)
            {
                board[i] = tmpBoard[i];
            }
        }

        private void RotateCCW(int[][] board)
        {
            int n = board.Length;
            int[][] tmpBoard = new int[4][];
            tmpBoard[0] = new int[4];
            tmpBoard[1] = new int[4];
            tmpBoard[2] = new int[4];
            tmpBoard[3] = new int[4];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    tmpBoard[i][j] = board[j][n - i - 1];
                }
            }

            for (int i = 0; i < n; i++)
            {
                board[i] = tmpBoard[i];
            }
        }
    }
}