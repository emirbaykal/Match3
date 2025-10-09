using Board;
using ScriptableObjects.Scripts;
using ScriptableObjects.Scripts.Level.LevelData;

namespace Signals.Managers
{
    public class LevelInitialized
    {
        public LevelData LevelData;
        public BoardModel BoardModel;
    }

    public class LevelFailed {}
    
    public class LevelSuccessful {}
}