using ScriptableObjects.Scripts.Level.LevelData;
using UnityEngine;
using Zenject;

namespace Board.Tiles
{
    public class TileModel
    {
        private SignalBus _bus;
        private BoardController _boardController;

        [Inject]
        public void Construct(SignalBus bus, BoardController boardController)
        {
            _bus = bus;
            _boardController = boardController;
        }
        public int ColorID { get; private set; }
        public int Strength { get; private set; }

        public TileType TileType;
        
        //GETTER FUNCTIONS
        
        public int GetStrength()
        {
            return Strength;
        }
        public Vector2Int GetMovePosition(Vector2Int currentPos, Vector2Int dir)
        {
            return currentPos + dir;
        }
        
        //SETTER FUNCTIONS

        public void SetStrength(int strength)
        {
            Strength = strength;
        }
        public void SetType(TileType newType)
        {
            TileType = newType;
        }
        public void SetColorID(int colorID)
        {
            ColorID = colorID;
        }
        
        
        //The “DamageBarrier” function is triggered within the tile controller.
        //If there is a barrier, it performs health calculations and reduces it.
        public void ApplyDamage(TileData data, int amount)
        {
            if (data.TileType != TileType.Normal)
            {
                Strength -= amount;

                if (Strength <= 0)
                {
                    _boardController.BoardModel.SetBarrierCounter(TileType, -1);
                    SetType(TileType.Normal);
                }      
            }
        }
    }
}
