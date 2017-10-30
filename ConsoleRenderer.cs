using System;
using System.Text;

namespace GameOf2048
{
    public class ConsoleRenderer : IGameRenderer
    {
        
        public void DrawBoard(int[][] board, int score)
        {
            Console.Clear();

            StringBuilder output = new StringBuilder();
            int cellWidth = 8;
            int numOfCells = board.Length;
            output.Append(CreateLine(cellWidth, numOfCells));
            // draw numbers
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    output.Append("|");
                    string number = board[i][j].ToString();
                    output.Append(number.PadLeft(cellWidth - 2));
                    output.Append(" ");

                }
                output.Append("|\n");
                output.Append(CreateLine(cellWidth, numOfCells));
            }
            output.Append("\n");
            Console.Write(output);
            Console.WriteLine($"Score: {score}");
        }

        private string CreateLine(int cellWidth, int numOfCells)
        {
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < numOfCells; i++)
            {
                string separator = "+";
                output.Append(separator.PadRight(cellWidth, '-'));
            }
            output.Append("+\n");
            return output.ToString();
        }

    }
}