using System;
using System.Text;

namespace GameOf2048
{

    public enum GameState { PLAY, WIN, GAMEOVER }

    public enum Moves { UP = 0, DOWN, LEFT, RIGHT }

    class Game
    {
        private int[][] Board;
        private int Score;
        private IGameRenderer renderer;
        private GameState State;
        private Engine engine;
        private IPlayer player;

        public Game(IGameRenderer renderer, IPlayer player)
        {
            this.renderer = renderer;
            this.player = player;
            this.engine = new Engine();
        }

        public void InitGame()
        {
            State = GameState.PLAY;
            Board = new int[4][];
            Board[0] = new int[] { 0, 0, 0, 0 };
            Board[1] = new int[] { 0, 0, 0, 0 };
            Board[2] = new int[] { 0, 0, 0, 0 };
            Board[3] = new int[] { 0, 0, 0, 0 };

            Score = 0;

            // The board starts with two numbers.
            engine.SpawnNumber(Board);
            engine.SpawnNumber(Board);
        }

        public void Start()
        {
            InitGame();

            while (State == GameState.PLAY)
            {
                renderer.DrawBoard(Board, Score);
                
                var move = player.MakeMove(Board);

                // If the returned value equals -1, the move was invalid.
                var mergedValue = engine.ExecuteMove(move, Board);

                if (mergedValue >= 0) {
                    Score += mergedValue;
                } else if (engine.NoMovesPossible(Board)) {
                    State = GameState.GAMEOVER;
                }
            }
        }

        static void Main(string[] args)
        {
            IGameRenderer renderer = new ConsoleRenderer();
            // IPlayer player = new ConsolePlayer();

            IPlayer ai = new AiPlayer();
            Game game = new Game(renderer, ai);

            game.Start();
        }
    }
}
