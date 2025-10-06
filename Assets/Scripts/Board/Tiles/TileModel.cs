using System.Collections.Generic;
using ScriptableObjects.Scripts;
using UnityEngine;

namespace Board.Tiles
{
    public class TileModel
    {
        public enum MoveSource
        {
            PlayerSwap,
            Falling
        }
        public int ColorID { get; private set; }
        public int Strength { get; private set; }
        
        public Vector2Int GetMovePosition(Vector2Int currentPos, Vector2Int dir)
        {
            return currentPos + dir;
        }

        public void SetStrength(int strength)
        {
            Strength = strength;
        }

        public int GetStrength()
        {
            return Strength;
        }

        public void SetColorID(int colorID)
        {
            ColorID = colorID;
        }

        public void ApplyDamage(TileData data, int amount)
        {
            if(data.TileType != TileType.Normal)
                Strength -= amount;
        }
    }
}
