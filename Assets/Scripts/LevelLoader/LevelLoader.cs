using ScriptableObjects.Scripts;
using Signals.Managers;
using UnityEngine;
using Zenject;

namespace LevelLoader
{
    public class LevelLoader : MonoBehaviour
    {
        private DiContainer _container;
        private LevelData _levelData;
        private SignalBus _bus;
        
        [Header("Prefabs")]
        [SerializeField]
        private GameObject boardPrefab;
        
        
        [Inject]
        public void Construct(SignalBus bus, DiContainer container, LevelData levelData)
        {
            _bus = bus;
            _levelData = levelData;
            _container = container;
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _container.InstantiatePrefab(boardPrefab);
            
            _bus.Fire(new LevelInitialized
            {
                LevelData = _levelData
            });
            
        }



    }
}
