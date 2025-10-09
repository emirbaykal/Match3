using System.Linq;
using Board.Tiles;
using ScriptableObjects.Scripts.Level.LevelData;
using Signals.Board.Tile;
using Signals.Managers;
using UnityEngine;
using Zenject;

namespace Board
{
    public class BoardController : MonoBehaviour
    {
        //MVC Class
        private BoardModel _boardModel;
        private BoardView _boardView;
        
        //Public MVC Class
        public BoardModel BoardModel => _boardModel;
        
        //Container Class
        private SignalBus _bus;
        private DiContainer _container;
        private LevelData _levelData;
        private TileController.TilePool _tilePool;

    
        [SerializeField]
        private Node.Node nodePrefab;
        
        public Transform tilesTransform;
        
        [Inject]
        public void Construct(SignalBus bus, DiContainer container, BoardModel boardModel , LevelData levelData , TileController.TilePool tilePool)
        {
            _bus = bus;
            _container = container;
            _boardModel = boardModel;
            _levelData = levelData;
            _tilePool = tilePool;
            
            _boardView = GetComponent<BoardView>();
        }

        private void OnDisable()
        {
            _bus.Unsubscribe<MatchControl>(MatchingTileDestroyer);
            _bus.Unsubscribe<TileMoveCompleted>(MatchingTileDestroyer);
        }

        private void Start()
        {
            _bus.Subscribe<MatchControl>(MatchingTileDestroyer);
            _bus.Subscribe<TileMoveCompleted>(MatchingTileDestroyer);
            
            Initialize();
        }
        
        //LEVEL DATA INITIALIZE
        private void Initialize()
        {
            BoardModel.BoardBounds bounds = BoardModel.GetBoundsFromLevel();
            Vector3 boardSize = new Vector3(bounds.MaxX + 1,
                bounds.MaxY + 1);
            
            UpdateBackgroundPos(boardSize);
            
            UpdateCameraPos(boardSize);
            
            NodeGenerator();
            
           _bus.Fire(new LevelInitialized
           {
               LevelData = _levelData,
               BoardModel = BoardModel
           });
        }
        
        //Board Background Values Settings
        private void UpdateBackgroundPos(Vector3 boardSize)
        {
            gameObject.transform.position = _boardModel.GetBoardCenterPosition
                ((int)boardSize.x, (int)boardSize.y);
        
            _boardView.BoardBackground.transform.localScale =
                new Vector2( boardSize.x * _boardView._nodeSize, 
                    boardSize.y * _boardView._nodeSize);
        }
        
        //Focus on the camera center
        private void UpdateCameraPos(Vector3 boardSize)
        {
            _boardView.FocusCameraOnBoard(_boardModel.GetBoardCenterPosition
                ((int)boardSize.x,(int)boardSize.y));
        
            _boardView.UpdateCameraSize(boardSize);
        }
    
        //Creates the nodes on the board
        private void NodeGenerator()
        {
            foreach (var tileData in _levelData.Tiles)
            {
                 var node = _container.InstantiatePrefabForComponent<Node.Node>(nodePrefab, 
                    new Vector2(tileData.Position.x,tileData.Position.y),
                    Quaternion.identity, 
                    _boardView.NodesRoot);

                 _boardModel.SetNode(tileData.Position, node);
            }
        }
        
        //Matching tiles are collected here,
        //and the tiles in each group are sent to the pool one by one.
        
        //Afterwards, the top tiles are slid down
        //using the “FallingTiles” function.
        private void MatchingTileDestroyer()
        {
            var matches = _boardModel.MatchControl();

            foreach (var tile in matches.SelectMany(group => group))
            {
                _tilePool.Despawn(tile);
            }
            _boardModel.FallingTiles();
        }
    }
}
