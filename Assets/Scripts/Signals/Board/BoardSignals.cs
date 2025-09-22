namespace Signals.Board
{
    public class IceTileCountChanged
    {
        public int RemainingCount;
    }

    public class TileMoveCountChanged
    {
        public int MoveCount;
    }

    public class BoardScoreChanged
    {
        public int Score;
    }
}