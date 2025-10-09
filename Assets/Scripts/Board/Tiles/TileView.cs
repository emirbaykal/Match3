using System.Threading.Tasks;
using DG.Tweening;
using ScriptableObjects.Scripts.Level.LevelData;
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
        [SerializeField] private SpriteAtlas tileSprites;
        [SerializeField] private SpriteAtlas barrierSprites;
        
        [Header("Tile Tools")]
        [SerializeField] private SpriteRenderer tileSprite;
        [SerializeField] private GameObject barrier;

        private Sprite[] _allSprites;
        private Sprite[] _allBarrierSprites;
        
        private void Awake()
        {
            //SPRITE ATLAS
            _allSprites = new Sprite[tileSprites.spriteCount];
            tileSprites.GetSprites(_allSprites);

            _allBarrierSprites = new Sprite[barrierSprites.spriteCount];
            barrierSprites.GetSprites(_allBarrierSprites);
        }

        //SETTER FUNCTION
        public int SetRandomSprite()
        {
            int randomID = Random.Range(0, _allSprites.Length);
            tileSprite.sprite = _allSprites[randomID];
            return randomID;
        }
        public void SetTileType(TileData tileData, int strength)
        {
            if(tileData.TileType == TileType.Normal) return;
            
            barrier.SetActive(true);

            string spriteName = $"{tileData.TileType}{"0" + strength}";

            Sprite targetSprite = barrierSprites.GetSprite(spriteName);

            barrier.GetComponent<SpriteRenderer>().sprite = targetSprite;

        }

        
        //DO TWEEN ANIMATIONS
        public void MoveAnimation(Vector2Int movePos)
        {
            DOTween.Kill(transform);

            Vector3 pos = new Vector3(movePos.x, movePos.y);
            gameObject.transform.DOMove(pos, 0.5f)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy).
                OnComplete(() =>
                {
                    _bus.Fire(new TileMoveCompleted());
                });
        }
        public async Task MoveAnimationAsync(Vector2Int movePos)
        {
            var tcs = new TaskCompletionSource<bool>();
            
            DOTween.Kill(transform);
            Vector3 pos = new Vector3(movePos.x, movePos.y);

            gameObject.transform.DOMove(pos, 0.5f)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(() =>
                {
                    _bus.Fire(new TileMoveCompleted());
                    tcs.TrySetResult(true);
                });

            await tcs.Task;
        }
        public void TileShakeAnimation()
        {
            gameObject.transform
                .DOShakePosition(0.2f, 
                    new Vector3(0.1f, 0, 0)
                    ,10, 
                    0);
        }
        
    }
}
