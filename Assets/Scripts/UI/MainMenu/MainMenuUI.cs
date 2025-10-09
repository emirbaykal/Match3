using ScriptableObjects.Scripts.Level.LevelLoader;
using Signals.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.MainMenu
{
    public class MainMenuUI : MonoBehaviour
    {
        private LevelDatabase _levelDatabase;
        private SignalBus _bus;
        private DiContainer _container;
    
        [Inject] 
        public void Construct(LevelDatabase levelDatabase, SignalBus bus, DiContainer container)
        {
            _levelDatabase = levelDatabase;
            _bus = bus;
            _container = container;
        }

        [SerializeField]
        private Button _levelButtonPrefab;

        [SerializeField]
        private Transform buttonTransform;
    
    
        public void Start()
        {
            foreach (var level in _levelDatabase.AllLevels)
            {
                Button button =_container.InstantiatePrefabForComponent<Button>(_levelButtonPrefab, buttonTransform);
            
                button.onClick.AddListener((() => _bus.Fire(new RequestLoadLevel(level.LevelID))));
                button.GetComponentInChildren<TMP_Text>().text = level.LevelID.ToString();
            }
        }
    }
}
