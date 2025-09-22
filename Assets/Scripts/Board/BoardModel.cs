using Signals.Board;
using UnityEngine;
using Zenject;

namespace Board
{
    public class BoardModel
    {
        private SignalBus _bus;

        public BoardModel(SignalBus bus)
        {
            _bus = bus;
        }

        public int IceTileCount { get; private set; }
        public int TileMoveCount { get; private set; }
        
        public int CurrentScore { get; private set; }
        
        public Vector3 BoardCenterPosition(int width, int height)
        {
            return new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f);
        }

        public void IncreaseIceCount()
        {
            IceTileCount++;
            _bus.Fire(new IceTileCountChanged
            {
                RemainingCount = IceTileCount
            });
        }

        public void DecreaseIceCount()
        {
            if (IceTileCount != 0)
                IceTileCount--;
            
            _bus.Fire(new IceTileCountChanged
            {
                RemainingCount = IceTileCount
            });
        }

        public void DecreaseMoveCount()
        {
            if (TileMoveCount != 0)
                TileMoveCount--;
            
            _bus.Fire(new TileMoveCountChanged
            {
                MoveCount = TileMoveCount
            });
        }

        public void IncreaseScore(int amount)
        {
            CurrentScore += amount;
                
            _bus.Fire(new BoardScoreChanged
            {
                Score = CurrentScore 
            });
        }
    }
}
