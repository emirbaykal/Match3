using System.Collections.Generic;
using System.Linq;
using Board.Tiles;
using ScriptableObjects.Scripts;
using Signals.Board;
using Signals.Board.Tile;
using UnityEngine;
using Zenject;

namespace Board
{
    public class BoardModel
    {
        public struct BoardBounds
        {
            public int MinX;
            public int MaxX;
            public int MinY;
            public int MaxY;
        }

        public BoardBounds Bounds
        {
            get
            {
                return new BoardBounds
                {
                    MinX = _nodes.Keys.Min(pos => pos.x),
                    MaxX = _nodes.Keys.Max(pos => pos.x),
                    MinY = _nodes.Keys.Min(pos => pos.y),
                    MaxY = _nodes.Keys.Max(pos => pos.y),
                };
            }
        }
        
        private Vector2Int[] SideTilesPositions 
        {
            get
            {
                return new[]
                {
                    Vector2Int.left,
                    Vector2Int.down,
                    Vector2Int.right,
                    Vector2Int.up,
                };
            }
        }
        
        private readonly SignalBus _bus;
        
        public BoardModel(SignalBus bus)
        {
            _bus = bus;
        }
        
        private readonly Dictionary<Vector2Int, Node.Node> _nodes = new();
        public IReadOnlyDictionary<Vector2Int, Node.Node> Nodes => _nodes;
        
        public int IceTileCount { get; private set; }
        public int TileMoveCount { get; private set; }
        public int CurrentScore { get; private set; }

        private int _activeTweens;
        
        
        //GETTER FUNCTIONS
        public Node.Node GetNode(Vector2Int pos)
        {
            return _nodes[pos];
        }
        
        public TileController GetTile(Vector2Int pos)
        {
            if(_nodes.TryGetValue(pos, out var node))
                return node.currentTile;
            Debug.LogWarning("'_nodes' variable is null !!");
            return null;
        }
        
        public Vector3 BoardCenterPosition(int width, int height)
        {
            return new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f);
        }
        
        public bool IsWithinBounds(Vector2Int pos)
        {
            return _nodes.ContainsKey(pos);
        }
        
        public bool IsBarrier(Vector2Int pos)
        {
            if (_nodes.TryGetValue(pos, out var node))
                return node.currentTile.tileData.TileType != TileType.Normal;
            
            return false;
        }
        
        //SETTER FUNCTIONS

        public void RegisterFallsTweenStart()
        {
            _activeTweens++;
        }

        public void RegisterFallsTweenEnd()
        {
            _activeTweens--;
            if (_activeTweens <= 0)
            {
                _bus.Fire(new MatchControl());
            }
        }
        
        public void MoveTileToNode(TileController targetTile, Vector2Int targetPos)
        {
            if (_nodes.TryGetValue(targetTile.currentPosition, out var currentNode))
            {
                currentNode.currentTile = null;
            }
            
            if (_nodes.TryGetValue(targetPos, out var targetNode))
            {
                targetTile.currentPosition = targetPos;
                targetNode.currentTile = targetTile;
                targetTile.TileMoveTrigger(targetPos);
            }
        }
        public void AssignNode(Vector2Int pos, Node.Node node)
        {
            _nodes[pos] = node;
            node.currentPosition = pos;
        }
        
        public void AssignTileToNode(Vector2Int pos, TileController tile)
        {
            if (_nodes.TryGetValue(pos, out var node))
                node.currentTile = tile;
        }
        
        public void UnassignTileFromNode(Vector2Int pos, TileController tile)
        {
            if (_nodes.TryGetValue(pos, out var node))
            {
                if (node.currentTile == tile)
                    node.currentTile = null;
            }
        }
        public void IncreaseIceCount()
        {
            IceTileCount++;
            _bus.Fire(new IceTileCountChanged
            {
                RemainingCount = IceTileCount
            });
        }

        public void DecreaseIceCount()
        {
            if (IceTileCount != 0)
                IceTileCount--;
            
            _bus.Fire(new IceTileCountChanged
            {
                RemainingCount = IceTileCount
            });
        }
        
        public void DecreaseMoveCount()
        {
            if (TileMoveCount != 0)
                TileMoveCount--;
            
            _bus.Fire(new TileMoveCountChanged
            {
                MoveCount = TileMoveCount
            });
        }

        public void IncreaseScore(int amount)
        {
            CurrentScore += amount;
                
            _bus.Fire(new BoardScoreChanged
            {
                Score = CurrentScore 
            });
        }
        
        public void SwapTiles(TileController movingTile, TileController targetTile)
        {
            Vector2Int movingTilePos = movingTile.currentPosition;
            Vector2Int targetTilePos = targetTile.currentPosition;

            _nodes[movingTilePos].currentTile = targetTile;
            _nodes[targetTilePos].currentTile = movingTile;

            movingTile.currentPosition = targetTilePos;
            targetTile.currentPosition = movingTilePos;
            
            movingTile.TileMoveTrigger(targetTilePos);
            targetTile.TileMoveTrigger(movingTilePos);
        }
        
        public List<List<TileController>> MatchControl()
        {
            List<List<TileController>> allMatches = new List<List<TileController>>();
            HashSet<TileController> visited = new HashSet<TileController>();
            
            foreach (var nodePair in Nodes)
            {
                TileController startTile = GetTile(nodePair.Key);
                if(startTile == null || visited.Contains(startTile))
                    continue;
                
                if(startTile.tileData.TileType != TileType.Normal)
                    continue;

                List<TileController> connected = FindConnectedTiles(startTile);

                foreach (var tile in connected)
                    visited.Add(tile);

                if (connected.Count >= 3)
                    allMatches.Add(connected);
            }
            
            if(allMatches.Count > 0)
                DamageBarriers(allMatches);
            
            return allMatches;
        }
        
        private List<TileController> FindConnectedTiles(TileController startTile)
        {
            List<TileController> connected = new List<TileController>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            int targetColorID = startTile.GetColorID();

            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(startTile.currentPosition);

            while (queue.Count > 0)
            {
                Vector2Int pos = queue.Dequeue();
                if (!IsWithinBounds(pos) || visited.Contains(pos))
                    continue;

                visited.Add(pos);

                TileController tile = GetTile(pos);
                if (tile == null) continue;

                if (tile.tileData.TileType != TileType.Normal) continue;
                if (tile.GetColorID() != targetColorID) continue;

                connected.Add(tile);

                foreach (var dir in SideTilesPositions)
                {
                    Vector2Int neighbor = pos + dir;
                    if (IsWithinBounds(neighbor) && !visited.Contains(neighbor))
                        queue.Enqueue(neighbor);
                }
            }

            return connected;
        }

        private void DamageBarriers(List<List<TileController>> allMatches)
        {
            List<TileController> damagedTiles = new List<TileController>();
            
            foreach (var group in allMatches)
            {
                foreach (var tile in group)
                {
                    foreach (var dir in SideTilesPositions)
                    {
                        Vector2Int sideTilePos = tile.currentPosition + dir;
                        if (!IsWithinBounds(sideTilePos)) continue;
                        
                        TileController sideTile = GetTile(sideTilePos);
                        if(sideTile == null) continue;
                        if(sideTile.tileData.TileType != TileType.Normal)
                            damagedTiles.Add(sideTile);
                    }
                }
            }
            
            foreach (var tile in damagedTiles)
            {
                tile.DamageBarrier(1);
            }
        }

        public void FallingTiles()
        {
            for (int x = Bounds.MinX; x <= Bounds.MaxX; x++)
            {
                for (int y = Bounds.MinY; y <= Bounds.MaxY; y++)
                {
                    Vector2Int nodePos = new Vector2Int(x, y);
                    
                    if(!_nodes.TryGetValue(nodePos, out var node)) continue;

                    if (node.currentTile == null)
                    {
                        bool found = false;
                        for (int aboveY = y + 1; aboveY <= Bounds.MaxY; aboveY++)
                        {
                            Vector2Int abovePos = new Vector2Int(x, aboveY);
                            if(!_nodes.TryGetValue(abovePos, out var aboveNode))
                                continue;
                            
                            if (aboveNode.currentTile != null && aboveNode.currentTile.tileData.TileType == TileType.Normal)
                            {
                                TileController fallingTile = aboveNode.currentTile;
                                MoveTileToNode(fallingTile, nodePos);

                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            _bus.Fire(new SpawnTile
                            {
                                TargetPos = new Vector2Int(x, y)
                            });
                        }
                    }
                }
            }
        }
    }
}
