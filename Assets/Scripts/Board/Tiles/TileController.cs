using System.Threading.Tasks;
using Board.Tiles.TileInput;
using DG.Tweening;
using ScriptableObjects.Scripts.Level.LevelData;
using UnityEngine;
using Zenject;

namespace Board.Tiles
{
    public class TileController : MonoBehaviour
    {
        public Vector2Int currentPosition;
        public TileData tileData;
        
        private TileView _tileView;
        private TileModel _tileModel; 
        private BoardController _boardController;
        public TileModel TileModel => _tileModel;
        public TileView TileView => _tileView;
        
        [Inject]
        public void Construct(TileModel tileModel, BoardController boardController)
        {
            _tileModel = tileModel;
            _boardController = boardController;
            
            _tileView = GetComponent<TileView>();
        }
        
        //SETTER FUNCTION
        
        private void InitializeRandomTileData()
        {
            var tileModel = _tileModel;
            
            int colorID = _tileView.SetRandomSprite();
            
            tileModel.SetColorID(colorID);
            tileModel.SetStrength(tileData.Strength);
            tileModel.SetType(tileData.TileType);
            
            _tileView.SetTileType(tileData, tileModel.GetStrength());
        }
        
        private void SetPositionOnGrid(Vector2Int position)
        {
            var tileTransform = transform;
            var bounds = _boardController.BoardModel.GetBoundsFromLevel();
            
            Vector2Int spawnPos = new Vector2Int(position.x, bounds.MaxY + 1);
            tileTransform.position = new Vector3(spawnPos.x, spawnPos.y);
            
            currentPosition = position;
            tileTransform.parent = _boardController.tilesTransform;

            UnAsyncTileMove(position);
        }
        
        
        //GETTER
        public int GetColorID()
        {
            return _tileModel.ColorID; 
        }
        
        //INTERACTION
        
        //It is a function that runs when the motion input is triggered.
        public void OnMove(Vector2Int dir)
        {
            if (TileInputHandler.ActiveTile != gameObject.GetComponent<TileInputHandler>() 
                || _boardController.BoardModel.IsBarrier(currentPosition))
                return;
            Vector2Int movePos = _tileModel.GetMovePosition(currentPosition, dir);

            if (!_boardController.BoardModel.IsWithinBounds(movePos) || _boardController.BoardModel.IsBarrier(movePos))
                _tileView.TileShakeAnimation();
            else
            {
                TileController targetTile = _boardController.BoardModel.GetTile(movePos);

                _boardController.BoardModel.SwapTiles(this, targetTile);
            }
        }
        
        
        //If there are barriers (ice, bushes) around exploding tiles,
        //they reduce their health.  
        public void DamageBarrier(int amount)
        {
            _tileModel.ApplyDamage(tileData, amount);
            _tileView.SetTileType(tileData, _tileModel.GetStrength());
        }
        
        //ASYNC
        
        //TILE MOVE ANIMATIONS TRIGGER
        public async Task AsyncTileMove(Vector2Int movePos)
        {
            await _tileView.MoveAnimationAsync(movePos);
        }
        public void UnAsyncTileMove(Vector2Int targetPos)
        {
            _tileView.MoveAnimation(targetPos);
        }
        
        public class TilePool : MonoMemoryPool<TileData, TileController>
        {
            protected override void Reinitialize(TileData tileData,TileController item)
            {
                DOTween.Kill(item.transform);
                item.tileData = tileData;
                
                item.InitializeRandomTileData();
                
                //view and strength
                
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
                    boardModel.UnAssignTileFromNode(item.currentPosition, item);
                }
                
                //Reset view
                DOTween.Kill(item.transform);
                
                item.currentPosition = Vector2Int.zero;

                item.gameObject.SetActive(false);
            }
        }
    }
    
}

