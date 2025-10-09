using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Scripts.Level.LevelLoader
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/Level Database")]
    public class LevelDatabase : ScriptableObject
    {
        public List<LevelData.LevelData> AllLevels;

        public LevelData.LevelData GetLevelByID(int id)
        {
            return AllLevels.Find(level => level.LevelID == id);
        }

        public LevelData.LevelData GetNextLevel(LevelData.LevelData currentLevel)
        {
            int currentLevelIndex = AllLevels.IndexOf(currentLevel);
            if (currentLevelIndex < 0 || currentLevelIndex + 1 >= AllLevels.Count) return null;
            return AllLevels[currentLevelIndex + 1];
        }
    }
}