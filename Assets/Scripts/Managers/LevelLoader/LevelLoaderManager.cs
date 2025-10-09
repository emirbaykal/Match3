using System;
using System.Threading.Tasks;
using DG.Tweening;
using ScriptableObjects.Scripts.Level.LevelData;
using ScriptableObjects.Scripts.Level.LevelLoader;
using Signals.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Managers.LevelLoader
{
    public class LevelLoaderManager : IInitializable, IDisposable
    {
        private readonly SignalBus _bus;
        private readonly LevelDatabase _database;

        private LevelData _currentLevel;
        
        public LevelLoaderManager(SignalBus bus, LevelDatabase database)
        {
            _bus = bus;
            _database = database;
        }
        
        public void Initialize()
        {
            _bus.Subscribe<RequestLoadLevel>(OnLoadLevel);
            _bus.Subscribe<RequestNextLevel>(OnNextLevel);
            _bus.Subscribe<RequestRetryLevel>(OnRetryLevel);
            _bus.Subscribe<RequestMainMenu>(OnMainMenu);
        }

        private void OnLoadLevel(RequestLoadLevel signal)
        {
            DOTween.KillAll();

            _currentLevel = _database.GetLevelByID(signal.LevelIndex);
            if(_currentLevel == null) return;

            LoadSceneAsync(GetLevelName());
        }
        private void OnNextLevel()
        {
            var next = _database.GetNextLevel(_currentLevel);

            if (next != null)
            {
                _currentLevel = next;
                LoadSceneAsync(GetLevelName());
            }
            else
            {
                _bus.Fire(new RequestMainMenu());
            }
        }
        private void OnRetryLevel()
        {
            if (_currentLevel != null)
                LoadSceneAsync(GetLevelName());
        }
        private void OnMainMenu()
        {
            LoadSceneAsync("MainMenu");
        }
        
        public async void LoadSceneAsync(string sceneName)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName);

            while (!operation.isDone)
                await Task.Yield();
        }
        
        private string GetLevelName()
        {
            return "Level" + _currentLevel.LevelID;
        }
        
        public void Dispose()
        {
            _bus.TryUnsubscribe<RequestLoadLevel>(OnLoadLevel);
            _bus.TryUnsubscribe<RequestNextLevel>(OnNextLevel);
            _bus.TryUnsubscribe<RequestRetryLevel>(OnRetryLevel);
            _bus.TryUnsubscribe<RequestMainMenu>(OnMainMenu);
        }


    }
}