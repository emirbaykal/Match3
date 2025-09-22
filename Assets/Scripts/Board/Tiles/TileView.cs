using ScriptableObjects.Scripts;
using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

namespace Board.Tiles
{
    public class TileView : MonoBehaviour
    {
        [SerializeField] private SpriteAtlas TileSprites;
        [SerializeField] private SpriteAtlas IceSprites;

        [SerializeField] private GameObject Ice;

        private Sprite[] _allSprites;
        private void Awake()
        {
            _allSprites = new Sprite[TileSprites.spriteCount];
            TileSprites.GetSprites(_allSprites);
        }

        public void ChangeTileSprite()
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            var randomSprite = _allSprites[Random.Range(0, _allSprites.Length)];
            sprite.sprite = randomSprite;
        }

        public void ChangeTileType(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Normal:
                    break;
                case TileType.Ice:
                    Ice.SetActive(true);
                    break;
            }
        }
        
    }
}
