namespace GameOf2048
{
    public interface IPlayer
    {
        Moves MakeMove(int[][] board);
    }
}