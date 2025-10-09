namespace Signals.Managers
{
    public class RequestLoadLevel
    {
        public int LevelIndex;

        public RequestLoadLevel(int levelIndex)
        {
            LevelIndex = levelIndex;
        }
    }
    public class RequestNextLevel { }
    public class RequestRetryLevel { }
    public class RequestMainMenu { }
}