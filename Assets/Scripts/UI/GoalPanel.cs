using Board;
using ScriptableObjects.Scripts;
using Signals.Board;
using Signals.Managers;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class GoalPanel : MonoBehaviour
    {
        private SignalBus _bus;

        [Header("Ice Counter")] [SerializeField]
        private GameObject iceCounterGO;
        [SerializeField] private TMP_Text iceCounterText;

        [Header("Move Counter")] [SerializeField]
        private GameObject moveCounterGO;
        [SerializeField] private TMP_Text moveCountText;
        
        [Header("Score")] [SerializeField] 
        private GameObject scoreGO;
        [SerializeField] private TMP_Text scoreText;

        [Inject]
        public void Construct(SignalBus bus)
        {
            _bus = bus;
        }

        private void OnEnable()
        {
            _bus.Subscribe<LevelInitialized>(InitializeGoalPanel);
            _bus.Subscribe<IceTileCountChanged>(UpdateIceCounterText);
            _bus.Subscribe<TileMoveCountChanged>(UpdateMoveCounterText);
            _bus.Subscribe<BoardScoreChanged>(UpdateScoreCounterText);
        }

        private void OnDisable()
        {
            _bus.Unsubscribe<LevelInitialized>(InitializeGoalPanel);
            _bus.Unsubscribe<IceTileCountChanged>(UpdateIceCounterText);
            _bus.Unsubscribe<TileMoveCountChanged>(UpdateMoveCounterText);
            _bus.Unsubscribe<BoardScoreChanged>(UpdateScoreCounterText);
        }

        public void InitializeGoalPanel(LevelInitialized signal)
        {
            LevelData data = signal.LevelData;

            if (data.HasScore)
            {
                scoreGO.SetActive(true);
                _bus.Fire(new BoardScoreChanged
                {
                    Score = 0
                });
            }
            
            if (data.HasIceBreak)
            {
                iceCounterGO.SetActive(true);
                
                _bus.Fire(new IceTileCountChanged
                {
                    RemainingCount = 0
                });
            }

            if (data.HasMoveLimit)
            {
                moveCounterGO.SetActive(true);
                
                _bus.Fire(new TileMoveCountChanged
                {
                    MoveCount = data.MoveLimit
                });
            }
        }
        
        public void UpdateMoveCounterText(TileMoveCountChanged signal)
        {
            moveCountText.text = signal.MoveCount.ToString();
        }

        public void UpdateIceCounterText(IceTileCountChanged signal)
        {
            iceCounterText.text = signal.RemainingCount.ToString();
        }

        public void UpdateScoreCounterText(BoardScoreChanged signal)
        {
            scoreText.text = signal.Score.ToString();
        }
    }
}
