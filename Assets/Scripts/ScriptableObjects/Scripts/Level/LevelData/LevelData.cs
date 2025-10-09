using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Scripts.Level.LevelData
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
    
    
    [CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/Level Data")]
    public class LevelData : ScriptableObject
    {
        public int LevelID;
     
        [Header("Board Data")]
        //Grid Data
        public List<TileData> Tiles;

        [Header("Level Goal")] 
        
        public List<GoalTypeData> ActiveGoals;
        
        [Header("Score Goal")]
        //Params
        public int TargetScore;
        
        [Header("Move Goal")]
        public int MoveLimit;
        
    }
}