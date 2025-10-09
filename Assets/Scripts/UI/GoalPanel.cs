using ScriptableObjects.Scripts.Level.LevelData;
using Signals.Managers;
using UnityEngine;
using Zenject;

namespace UI
{
    public class GoalPanel : MonoBehaviour
    {
        private SignalBus _bus;
        private DiContainer _container;

        [SerializeField]
        private GoalItem goalPrefab; 

        [Inject]
        public void Construct(SignalBus bus, DiContainer container)
        {
            _bus = bus;
            _container = container;
        }

        private void OnEnable()
        {
            _bus.Subscribe<LevelInitialized>(InitializeGoalPanel);
        }

        private void OnDisable()
        {
            _bus.Unsubscribe<LevelInitialized>(InitializeGoalPanel);
        }

        private void InitializeGoalPanel(LevelInitialized signal)
        {
            LevelData data = signal.LevelData;

            foreach (var goalData in data.ActiveGoals)
            {
                var goal = _container.InstantiatePrefabForComponent<GoalItem>
                    (goalPrefab,transform);
                goal.SetData(goalData);
                
            }
        }
    }
}
