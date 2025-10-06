using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Scripts
{
    [System.Serializable]
    public class TileData
    {
        public Vector2Int Position;
        public TileType TileType;
        public int Strength;
    }
    public enum TileType
    {
        Normal,
        Ice,
        Bush
    }
    
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/Level Data")]
    public class LevelData : ScriptableObject
    {
        public int LevelID;
     
        [Header("Board Data")]
        //Grid Data
        //KALDIRILACAK
        public int Rows;
        public int Columns;
        public List<TileData> Tiles;
        
        [Header("Level Goal")]
        //Level Type
        public bool HasScore;
        public bool HasIceBreak;
        public bool HasMoveLimit;
        
        [Header("Score Goal")]
        //Params
        public int TargetScore;
        
        [Header("Move Goal")]
        public int MoveLimit;
        
    }
}