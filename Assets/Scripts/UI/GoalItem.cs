using Board;
using ScriptableObjects.Scripts.Level.LevelData;
using Signals.Board;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class GoalItem : MonoBehaviour
    {
        [Inject] private SignalBus _bus;
        [Inject] private LevelData _levelData;
        private BoardModel _boardModel;
        
        [SerializeField] private Image typeLogo;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private TMP_Text nameText;

        private int _count;

        [Inject]
        public void Construct(SignalBus bus, LevelData levelData, BoardModel boardModel)
        {
            _bus = bus;
            _levelData = levelData;
            _boardModel = boardModel;
        }

        private void OnEnable()
        {
            _bus.TryUnsubscribe<TileMoveCountChanged>(UpdateMoveCounterText);
            _bus.TryUnsubscribe<ScoreChanged>(UpdateScoreCounterText);
            _bus.TryUnsubscribe<BushTileCountChanged>(UpdateBushCounterText);
            _bus.TryUnsubscribe<IceTileCountChanged>(UpdateIceCounterText);
        }

        //We update values based on incoming data
        public void SetData(GoalTypeData typeData)
        {
            if (typeData.Sprite != null)
            {
                typeLogo.sprite = typeData.Sprite;
                typeLogo.gameObject.SetActive(true);
            }
            else
            {
                nameText.text = typeData.Name;
                nameText.gameObject.SetActive(true);
            }

            switch (typeData.Type)
            {
                case GoalType.MoveCounter:
                    countText.text = _levelData.MoveLimit.ToString();
                    _bus.Subscribe<TileMoveCountChanged>(UpdateMoveCounterText);
                    _boardModel.SetMoveCounter(newValue: _levelData.MoveLimit);
                    break;
                
                case GoalType.ScorePoint:
                    countText.text = 0.ToString();
                    _bus.Subscribe<ScoreChanged>(UpdateScoreCounterText);
                    break;
                
                case GoalType.IceCounter:
                    _bus.Subscribe<IceTileCountChanged>(UpdateIceCounterText);
                    break;
                
                case GoalType.BushCounter:
                    _bus.Subscribe<BushTileCountChanged>(UpdateBushCounterText);
                    break;
            }

        }

        //UI TEXT UPDATE
        private void UpdateMoveCounterText(TileMoveCountChanged signal)
        {
            countText.text = signal.MoveCount.ToString();
        }

        private void UpdateIceCounterText(IceTileCountChanged signal)
        {
            countText.text = signal.RemainingCount.ToString();
        }

        private void UpdateBushCounterText(BushTileCountChanged signal)
        {
            countText.text = signal.RemainingCount.ToString();
        }

        private void UpdateScoreCounterText(ScoreChanged signal)
        {
            countText.text = signal.Score.ToString();
        }
    }
}
