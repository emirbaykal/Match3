using Board.Tiles.TileInput;
using UnityEngine;
using Zenject;
using TileData = ScriptableObjects.Scripts.TileData;

namespace Board.Tiles
{
    public class TileController : MonoBehaviour
    {
        public Vector2Int currentPosition;
        public TileData tileData;
        
        private TileView _tileView;
        private TileModel _tileModel; 
        private BoardController _boardController;
        private SignalBus _bus;

        public TileModel TileModel => _tileModel;
        public TileView TileView => _tileView;
        
        [Inject]
        public void Construct(SignalBus bus, TileModel tileModel, BoardController boardController)
        {
            _bus = bus;
            _tileModel = tileModel;
            _boardController = boardController;
            
            _tileView = GetComponent<TileView>();
        }
        
        public void OnMove(Vector2Int dir)
        {
            if (TileInputHandler.ActiveTile != gameObject.GetComponent<TileInputHandler>() 
                || _boardController.BoardModel.IsBarrier(currentPosition))
                return;
            
            Vector2Int movePos = _tileModel.GetMovePosition(currentPosition, dir);

            if (!_boardController.BoardModel.IsWithinBounds(movePos) || _boardController.BoardModel.IsBarrier(movePos))
                _tileView.WrongMoveAnimation();
            else
            {
                TileController targetTile = _boardController.BoardModel.GetTile(movePos);

                _boardController.BoardModel.SwapTiles(this, targetTile);
            }
        }
        
        public void TileMoveTrigger(Vector2Int targetPos)
        {
            _tileView.MoveAnimation(targetPos);
        }
        
        public void DamageBarrier(int amount)
        {
            _tileModel.ApplyDamage(tileData, amount);
            _tileView.ChangeTileType(tileData, _tileModel.GetStrength());
        }
        public void InitializeRandomTile()
        {
            int colorID = _tileView.SetRandomSprite();
            _tileModel.SetColorID(colorID);
        }
        public int GetColorID()
        {
            return _tileModel.ColorID; 
        }
        
        private void SetPositionOnGrid(Vector2Int position)
        {
            var tileTransform = transform;
            
            Vector2Int spawnPos = new Vector2Int(position.x, _boardController.BoardModel.Bounds.MaxY + 1);
            tileTransform.position = new Vector3(spawnPos.x, spawnPos.y);
            
            currentPosition = position;
            tileTransform.parent = _boardController.tilesTransform;

            TileMoveTrigger(position);
            
        }
        
        public class TilePool : MonoMemoryPool<TileData, TileController>
        {
            protected override void Reinitialize(TileData tileData,TileController item)
            {
                
                item.tileData = tileData;
                item.InitializeRandomTile();
                
                //view and strength
                item._tileModel.SetStrength(tileData.Strength);
                item._tileView.ChangeTileType(tileData, item._tileModel.GetStrength());
                
                //Position
                Vector2Int targetPos = tileData.Position;
                item.SetPositionOnGrid(targetPos);
                item._boardController.BoardModel.AssignTileToNode(targetPos, item);
            }

            protected override void OnDespawned(TileController item)
            {
                if (item._boardController != null)
                {
                    var boardModel = item._boardController.BoardModel;
                    boardModel.UnassignTileFromNode(item.currentPosition, item);
                }
                
                //Reset view
                item.currentPosition = Vector2Int.zero;

                item.gameObject.SetActive(false);
            }
        }
    }
    
}

