using System;
using Signals.Managers;
using UnityEngine;
using Zenject;

namespace Managers.GameFlow
{
    public class UIManager : MonoBehaviour
    {
        [Inject] private SignalBus _bus;

        [SerializeField] 
        private GameResultPanel _gameResultPanel;

        private void OnEnable()
        {
            _gameResultPanel.Hide();
            
            _bus.Subscribe<LevelSuccessful>(OnLevelSuccess);
            _bus.Subscribe<LevelFailed>(OnLevelFailed);
            
            _bus.Subscribe<RequestNextLevel>(CloseGameResultPanel);
            _bus.Subscribe<RequestRetryLevel>(CloseGameResultPanel);
            _bus.Subscribe<RequestMainMenu>(CloseGameResultPanel);
        }

        private void OnDisable()
        {
            _bus.Unsubscribe<LevelSuccessful>(OnLevelSuccess);
            _bus.Unsubscribe<LevelFailed>(OnLevelFailed);
            
            _bus.Unsubscribe<RequestNextLevel>(CloseGameResultPanel);
            _bus.Unsubscribe<RequestRetryLevel>(CloseGameResultPanel);
            _bus.Unsubscribe<RequestMainMenu>(CloseGameResultPanel);
        }

        private void OnLevelSuccess()
        {
            _gameResultPanel.Show();
            _gameResultPanel.UpdateResultElements(true);
        }

        private void OnLevelFailed()
        {
            _gameResultPanel.Show();
            _gameResultPanel.UpdateResultElements(false);
        }
        

        private void CloseGameResultPanel()
        {
            _gameResultPanel.Hide();
        }
    }
}