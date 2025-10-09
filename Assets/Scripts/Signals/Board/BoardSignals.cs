using System.Collections.Generic;
using Board.Node;
using UnityEngine;

namespace Signals.Board
{
    public class IceTileCountChanged
    {
        public int RemainingCount;
    }

    public class BushTileCountChanged
    {
        public int RemainingCount;
    }
    public class TileMoveCountChanged
    {
        public int MoveCount;
    }

    public class ScoreChanged
    {
        public int Score;
    }

    public class SpawnTile
    {
        public Vector2Int TargetPos;
    }
}