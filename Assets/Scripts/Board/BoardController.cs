using System;
using System.Collections;
using Board.Tiles;
using DG.Tweening;
using ScriptableObjects.Scripts;
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

        public int IceTileCount => _boardModel.IceTileCount;
    
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
        private void Initialize()
        {
            Vector3 boardSize = new Vector3(_levelData.Columns,
                _levelData.Rows);
            
            UpdateBackgroundPos(boardSize);
            
            UpdateCameraPos(boardSize);
            
            NodeGenerator();
            
            _bus.Fire(new LevelInitialized
            {
                LevelData = _levelData,
                BoardModel = BoardModel
            });
        }
        
        private void UpdateBackgroundPos(Vector3 boardSize)
        {
            gameObject.transform.position = _boardModel.BoardCenterPosition
                ((int)boardSize.x, (int)boardSize.y);
        
            _boardView.BoardBackground.transform.localScale =
                new Vector2( boardSize.x * _boardView._nodeSize, 
                    boardSize.y * _boardView._nodeSize);
        }
        private void UpdateCameraPos(Vector3 boardSize)
        {
            _boardView.FocusCameraOnBoard(_boardModel.BoardCenterPosition
                (_levelData.Columns, _levelData.Rows));
        
            _boardView.UpdateCameraSize(boardSize);
        }
    
        private void NodeGenerator()
        {
            foreach (var tileData in _levelData.Tiles)
            {
                 var node = _container.InstantiatePrefabForComponent<Node.Node>(nodePrefab, 
                    new Vector2(tileData.Position.x,tileData.Position.y),
                    Quaternion.identity, 
                    _boardView.NodesRoot);

                 _boardModel.AssignNode(tileData.Position, node);
            }
        }
        
        public void MatchingTileDestroyer()
        {
            var matches = _boardModel.MatchControl();

            foreach (var group in matches)
            {
                foreach (var tile in group)
                {
                    _tilePool.Despawn(tile);
                }
            }
            _boardModel.FallingTiles();
            
        }
    }
}
