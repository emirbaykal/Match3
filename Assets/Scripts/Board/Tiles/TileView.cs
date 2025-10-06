using System;
using DG.Tweening;
using ScriptableObjects.Scripts;
using Signals.Board.Tile;
using UnityEngine;
using UnityEngine.U2D;
using Zenject;
using Random = UnityEngine.Random;

namespace Board.Tiles
{
    public class TileView : MonoBehaviour
    {
        [Inject] private SignalBus _bus;
        
        [Header("Sprite Atlas")]
        [SerializeField] private SpriteAtlas TileSprites;
        [SerializeField] private SpriteAtlas BarrierSprites;
        
        [Header("Tile Tools")]
        [SerializeField] private SpriteRenderer tileSprite;
        [SerializeField] private GameObject barrier;

        private Sprite[] _allSprites;
        private Sprite[] _allBarrierSprites;
        
        private void Awake()
        {
            _allSprites = new Sprite[TileSprites.spriteCount];
            TileSprites.GetSprites(_allSprites);

            _allBarrierSprites = new Sprite[BarrierSprites.spriteCount];
            BarrierSprites.GetSprites(_allBarrierSprites);
        }

        public int SetRandomSprite()
        {
            int randomID = Random.Range(0, _allSprites.Length);
            tileSprite.sprite = _allSprites[randomID];
            return randomID;
        }

        public void ChangeTileType(TileData tileData, int strength)
        {
            if(tileData.TileType == TileType.Normal) return;
            
            barrier.SetActive(true);

            string spriteName = $"{tileData.TileType}{"0" + strength}";

            Sprite targetSprite = BarrierSprites.GetSprite(spriteName);
            Debug.Log(spriteName);

            barrier.GetComponent<SpriteRenderer>().sprite = targetSprite;

        }

        public void MoveAnimation(Vector2Int movePos)
        {
            DOTween.Kill(transform);

            Vector3 pos = new Vector3(movePos.x, movePos.y);
            gameObject.transform.DOMove(pos, 2f)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy).
                OnComplete(() =>
                {
                    _bus.Fire(new TileMoveCompleted());
                });
        }

        public void WrongMoveAnimation()
        {
            gameObject.transform
                .DOShakePosition(0.2f, 
                    new Vector3(0.1f, 0, 0)
                    ,10, 
                    0);
        }
        
    }
}
