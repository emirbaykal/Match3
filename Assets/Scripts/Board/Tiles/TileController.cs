using ScriptableObjects.Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using TileData = ScriptableObjects.Scripts.TileData;

namespace Board.Tiles
{
    public class TileController : MonoBehaviour
    {
        protected TileView _tileView;

        public void SetGridPosition(int x, int y, Transform parent)
        {
            gameObject.transform.position = new Vector3(x,y);
            gameObject.transform.parent = parent;
        }

        public class TilePool : MonoMemoryPool<TileData, Transform, TileController>
        {
            protected override void Reinitialize(TileData tileData, Transform parent,TileController item)
            {
                var view = item.GetComponent<TileView>();
                view.ChangeTileSprite();
                view.ChangeTileType(tileData.TileType);
                
                item.SetGridPosition(tileData.Position.x,tileData.Position.y,parent);
            }

            protected override void OnDespawned(TileController item)
            {
                
            }
        }
    }
    
}

