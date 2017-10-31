using System;

namespace GameOf2048
{
    public class ConsolePlayer : IPlayer
    {
        public Moves MakeMove(int[][] board)
        {
            while(true)
            {
                var ch = Console.ReadKey(false).Key;
                switch(ch)
                {
                    case ConsoleKey.LeftArrow:
                        return Moves.LEFT;
                    case ConsoleKey.RightArrow:
                        return Moves.RIGHT;
                    case ConsoleKey.UpArrow:
                        return Moves.UP;
                    case ConsoleKey.DownArrow:
                        return Moves.DOWN;
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break; 
                }
            }
        } 
    }
}