using UnityEngine;

namespace Board
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private Transform _nodesRoot;
        [SerializeField] private Transform _tilesRoot;
        [SerializeField] private SpriteRenderer _background;

        public float _nodeSize;

        public Transform NodesRoot => _nodesRoot;
        public Transform TilesRoot => _tilesRoot;
        public SpriteRenderer BoardBackground => _background;

        public void FocusCameraOnBoard(Vector3 boardCenterPos)
        {
            if (Camera.main != null)
            {
                Camera.main.transform.position
                    = new Vector3(boardCenterPos.x, boardCenterPos.y, -10);
            }
        }

        public void UpdateCameraSize(Vector3 boardSize)
        {
            float targetSize = boardSize.x * (_nodeSize * 2);
            if (Camera.main != null)
            {
                Camera.main.orthographicSize += targetSize;
            }
        }
        
    }
}
