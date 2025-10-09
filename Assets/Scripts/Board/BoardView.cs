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

        //The camera focuses on the center point of the board.
        public void FocusCameraOnBoard(Vector3 boardCenterPos)
        {
            
            if (Camera.main != null)
            {
                Camera.main.transform.position
                    = new Vector3(boardCenterPos.x, boardCenterPos.y, -10);
            }
        }

        //The size of one node is calculated.
        //The number of nodes is obtained from the board size,
        //and the target size is obtained.
        public void UpdateCameraSize(Vector3 boardSize)
        {
            var mainCamera = Camera.main;
            
            if (mainCamera != null)
            {
                var size = mainCamera.orthographicSize;
                var orthographicSize = size;
                float targetSize = (boardSize.x * (_nodeSize * 2) + orthographicSize);
                
                size += targetSize;
                mainCamera.orthographicSize = size;
            }
        }
        
    }
}
