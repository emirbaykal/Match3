using Board.Tiles;
using ScriptableObjects.Scripts;
using Signals.Managers;
using UnityEngine;
using Zenject;

namespace Board
{
    public class BoardController : MonoBehaviour
    {
        private BoardModel _boardModel;
        private BoardView _boardView;
        private SignalBus _bus;
        private DiContainer _container;
        private TileController.TilePool _tilePool;
    
        [SerializeField]
        private Node.Node nodePrefab;

        [SerializeField] 
        private TileController tilePrefab;

        public int IceTileCount => _boardModel.IceTileCount;
    
        [Inject]
        public void Construct(SignalBus bus, DiContainer container, BoardModel boardModel, TileController.TilePool tilePool)
        {
            _tilePool = tilePool;
            _bus = bus;
            _container = container;
            _boardModel = boardModel;
            _boardView = GetComponent<BoardView>();
        }

        private void OnEnable()
        {
            _bus.Subscribe<LevelInitialized>(Initialize);
        }

        private void OnDisable()
        {
            _bus.Unsubscribe<LevelInitialized>(Initialize);
        }

        public void Initialize(LevelInitialized signal)
        {
        
            Vector3 boardSize = new Vector3(signal.LevelData.Columns,
                signal.LevelData.Rows);


            UpdateBackgroundPos(boardSize);
            
            UpdateCameraPos(signal, boardSize);
            
            NodeGenerator(signal);
            TileGenerator(signal);
        }

        private void UpdateBackgroundPos(Vector3 boardSize)
        {
            gameObject.transform.position = _boardModel.BoardCenterPosition
                ((int)boardSize.x, (int)boardSize.y);
        
            _boardView.BoardBackground.transform.localScale =
                new Vector2( boardSize.x * _boardView._nodeSize, 
                    boardSize.y * _boardView._nodeSize);
        }
        private void UpdateCameraPos(LevelInitialized signal, Vector3 boardSize)
        {
            _boardView.FocusCameraOnBoard(_boardModel.BoardCenterPosition
                (signal.LevelData.Columns, signal.LevelData.Rows));
        
            _boardView.UpdateCameraSize(boardSize);
        }
    
        private void NodeGenerator(LevelInitialized signal)
        {
            foreach (var tileData in signal.LevelData.Tiles)
            {
                _container.InstantiatePrefab(nodePrefab, 
                    new Vector2(tileData.Position.x,tileData.Position.y),
                    Quaternion.identity, 
                    _boardView.NodesRoot);
            }
        }
        
        private void TileGenerator(LevelInitialized signal)
        {
            var data = signal.LevelData;

            foreach (var tileData in data.Tiles)
            {
                _tilePool.Spawn(tileData, _boardView.TilesRoot);
                if (tileData.TileType == TileType.Ice)
                {
                    _boardModel.IncreaseIceCount();
                }
            }
            
        }
    }
}
