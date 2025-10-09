using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;

namespace Board.Tiles.TileInput
{
    public class TileInputHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private TileInputs Inputs;
        private TileController _tile;
        
        [Inject]
        public void Construct(TileInputs inputs)
        {
            Inputs = inputs;
        }

        private Vector2 _swipeDirection;
        private float _swipeThreshold = 5f;
        
        public static TileInputHandler ActiveTile { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            ActiveTile = this;
            Inputs.Tile.MoveTile.performed += OnSwipeDelta;

            Inputs.Tile.Enable();
            _tile = eventData.pointerCurrentRaycast.
                gameObject.GetComponent<TileController>();

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Inputs.Tile.MoveTile.performed -= OnSwipeDelta;

            Inputs.Tile.Disable();
            ActiveTile = null;
            _tile = null;
        }

        private void OnSwipeDelta(InputAction.CallbackContext context)
        {
            Inputs.Tile.MoveTile.performed -= OnSwipeDelta;
            _swipeDirection = context.ReadValue<Vector2>();

            if (_swipeDirection.magnitude < _swipeThreshold)
                return;

            Vector2Int dir;

            if (Mathf.Abs(_swipeDirection.x) > Mathf.Abs(_swipeDirection.y))
                dir = _swipeDirection.x > 0 ? Vector2Int.right : Vector2Int.left;
            else
                dir = _swipeDirection.y > 0 ? Vector2Int.up : Vector2Int.down;

            _tile.OnMove(dir);
        }
    }
}
