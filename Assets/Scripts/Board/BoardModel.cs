using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Board.Tiles;
using ScriptableObjects.Scripts;
using ScriptableObjects.Scripts.Level.LevelData;
using Signals.Board;
using Signals.Board.Tile;
using Signals.Managers;
using UnityEngine;
using Zenject;

namespace Board
{
    public class BoardModel
    {
        private readonly LevelData _levelData;
        private readonly SignalBus _bus;
        
        public BoardModel(SignalBus bus, LevelData levelData)
        {
            _bus = bus;
            _levelData = levelData;
        }
        public struct BoardBounds
        {
            public int MinX;
            public int MaxX;
            public int MinY;
            public int MaxY;
        }
        public BoardBounds GetBoundsFromLevel()
        {
            if (_levelData.Tiles == null || _levelData.Tiles.Count == 0)
                return new BoardBounds();

            return new BoardBounds
            {
                MinX = _levelData.Tiles.Min(t => t.Position.x),
                MaxX = _levelData.Tiles.Max(t => t.Position.x),
                MinY = _levelData.Tiles.Min(t => t.Position.y),
                MaxY = _levelData.Tiles.Max(t => t.Position.y),
            };
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
        
        
        private readonly Dictionary<Vector2Int, Node.Node> _nodes = new();
        public IReadOnlyDictionary<Vector2Int, Node.Node> Nodes => _nodes;
        
        public int IceTileCount { get; private set; }
        public int BushTileCount { get; private set; }
        public int TileMoveCount { get; private set; }
        public int CurrentScore { get; private set; }

        private int _activeTweens;
        
        
        //GETTER FUNCTIONS
        
        //Tiles are taken from the nodes
        //on the board according to the given position.
        public TileController GetTile(Vector2Int pos)
        {
            if(_nodes.TryGetValue(pos, out var node))
                return node.currentTile;
            Debug.LogWarning("'_nodes' variable is null !!");
            return null;
        }
        
        public Vector3 GetBoardCenterPosition(int width, int height)
        {
            return new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f);
        }
        
        //CHECK FUNCTIONS
        
        //Is the control performed within the board boundaries?
        //Is the target position checked to see if it is outside the board?
        public bool IsWithinBounds(Vector2Int pos)
        {
            return _nodes.ContainsKey(pos);
        }
        
        //The target position is checked to see if it is a barrier. 
        //Used to prevent and immobilize during a match
        public bool IsBarrier(Vector2Int pos)
        {
            if (_nodes.TryGetValue(pos, out var node))
            {
                return node.currentTile.TileModel.TileType != TileType.Normal;
            }
            return false;
        }
        
        //SETTER FUNCTIONS
        private void FallingMoveTile(TileController targetTile, Vector2Int targetPos)
        {
            if (_nodes.TryGetValue(targetTile.currentPosition, out var currentNode))
            {
                currentNode.currentTile = null;
            }

            if (!_nodes.TryGetValue(targetPos, out var targetNode)) return;
            targetTile.UnAsyncTileMove(targetPos);
            targetTile.currentPosition = targetPos;
            targetNode.currentTile = targetTile;
        }
        
        //determines the node's location and assigns it within the held “nodes”
        public void SetNode(Vector2Int pos, Node.Node node)
        {
            _nodes[pos] = node;
            node.currentPosition = pos;
        }
        
        //Assigns the desired tile value to node a
        public void AssignTileToNode(Vector2Int pos, TileController tile)
        {
            if (_nodes.TryGetValue(pos, out var node))
                node.currentTile = tile;
        }
        
        //clears the tile value from the node
        public void UnAssignTileFromNode(Vector2Int pos, TileController tile)
        {
            if (_nodes.TryGetValue(pos, out var node))
            {
                if (node.currentTile == tile)
                    node.currentTile = null;
            }
        }

        //The calculation we see on the goal panel is performed.
        //How many ice cubes are left on the board, for example.        
        public void SetBarrierCounter(TileType type, int increaseCount)
        {
            switch (type)
            {
                case TileType.Ice:
                    IceTileCount += increaseCount;
                    
                    _bus.Fire(new IceTileCountChanged
                    {
                        RemainingCount = IceTileCount
                    });
                    break;
                case TileType.Bush:
                    BushTileCount += increaseCount;
                    
                    
                    _bus.Fire(new BushTileCountChanged
                    {
                        RemainingCount = BushTileCount
                    });
                    break;
            }

            if (CheckLevelCompletion())
                _bus.Fire(new LevelSuccessful());
        }

        
        //It ensures that the “LevelData” scriptable objects
        //we create check whether the tasks we want in that level have been
        //completed from the “ActiveGoals” list we assign to those tasks.    
        
        //At this level, if we assign a certain number of tasks,
        //it is controlled using a local dictionary in this way to ensure
        //that all of them are completed.  
        private bool CheckLevelCompletion()
        {
            var activeGoals = _levelData.ActiveGoals;

            Dictionary<GoalTypeData, bool> conditions = new Dictionary<GoalTypeData, bool>();
            
            foreach (var goal in activeGoals)
            {
                conditions.TryAdd(goal, false);
                if (goal.Type == GoalType.IceCounter)
                {
                    if (IceTileCount == 0)
                        conditions[goal] = true;
                }

                if (goal.Type == GoalType.BushCounter)
                {
                    if (BushTileCount == 0)
                        conditions[goal] = true;
                }
                
                if (goal.Type == GoalType.MoveCounter)
                {
                    if (TileMoveCount != 0)
                        conditions[goal] = true;
                }
                
                if (goal.Type == GoalType.ScorePoint)
                {
                    if (CurrentScore >= _levelData.TargetScore)
                        conditions[goal] = true;
                }
            }

            if (conditions.ContainsValue(false))
                return false;

            return true;
        }

        //In this function, we can either directly change the value or add or remove elements. 
        public void SetMoveCounter(int? newValue = null, int IncreaseCount = 0)
        {
            if (newValue.HasValue)
                TileMoveCount = newValue.Value;
            else
                TileMoveCount += IncreaseCount;
            
            _bus.Fire(new TileMoveCountChanged
            {
                MoveCount = TileMoveCount
            });
            
            if(TileMoveCount <= 0)
                _bus.Fire(new LevelFailed());
                
        }

        //In this function, we can either directly change the value or add or remove elements. 
        public void SetScoreCounter(int? newValue = null, int IncreaseCount = 0)
        {
            if (newValue.HasValue)
                CurrentScore = newValue.Value;
            else
                CurrentScore += IncreaseCount;
            
            _bus.Fire(new ScoreChanged
            {
                Score = CurrentScore
            });
            
            if(CheckLevelCompletion())
                _bus.Fire(new LevelSuccessful());
        }
        
        //In this function, we change the two tiles that match the move we made.
        
        //We wait for the end of the move animation with “task”.
        //When it is complete, a match check is performed. (Only checked for those we swapped) 
        public async void SwapTiles(TileController movingTile, TileController targetTile)
        {
            //First, we perform the swap movement.
            //
            Vector2Int movingTilePos = movingTile.currentPosition;
            Vector2Int targetTilePos = targetTile.currentPosition;

            movingTile.currentPosition = targetTilePos;
            targetTile.currentPosition = movingTilePos;

            //We send the move operations as a task and wait for them to complete.
            Task movingTileMoveTask = movingTile.AsyncTileMove(targetTilePos);
            Task targetTileMoveTask = targetTile.AsyncTileMove(movingTilePos);
            await Task.WhenAll(movingTileMoveTask, targetTileMoveTask);

            _nodes[movingTilePos].currentTile = targetTile;
            _nodes[targetTilePos].currentTile = movingTile;

            //After completion, if there are matches, it detonates the matches. 
            bool haveMatch = false;
            foreach (var matches in MatchControl())
            {
                foreach (var tile in matches)
                {
                    if (tile == movingTile || tile == targetTile)
                    {
                        haveMatch = true;
                        break;
                    }
                }
                if (haveMatch)
                    break;
            }
            
            //If there is no match,
            //the return move tween is called for these two tiles,
            //and we create a task to wait for their completion.
            if (!haveMatch)
            {
                Task movingTileWrongTask = movingTile.AsyncTileMove(movingTilePos);
                Task targetTileWrongTask = targetTile.AsyncTileMove(targetTilePos);

                await Task.WhenAll(movingTileWrongTask, targetTileWrongTask);

                _nodes[movingTilePos].currentTile = movingTile;
                _nodes[targetTilePos].currentTile = targetTile;

                movingTile.currentPosition = movingTilePos;
                targetTile.currentPosition = targetTilePos;
            }
            else
            {
                //If there is no match,
                //the explosion signal is fired.
                _bus.Fire(new TileMoveCompleted());
                
                //Decrease move count
                SetMoveCounter(IncreaseCount: -1);
            }
        }
        
        
        //The entire board is scanned, the area around that tile is checked,
        //and matches corresponding to that tile are added to the “allMatches” list.
        public List<List<TileController>> MatchControl()
        {
            List<List<TileController>> allMatches = new List<List<TileController>>();
            HashSet<TileController> visited = new HashSet<TileController>();
            
            foreach (var nodePair in Nodes)
            {
                TileController startTile = GetTile(nodePair.Key);
                if(startTile == null || visited.Contains(startTile))
                    continue;
                
                if(startTile.TileModel.TileType != TileType.Normal)
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
        
        //In the MatchControl function,
        //each selected tile is sent to this function and checked here to see if it is associated with a match.
        //If it is associated, it is assigned to the “allmatches” list and the other tile is checked.
        
        //(If a tile is already in the matches, this is not done again for it.)    
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

                if (tile.TileModel.TileType != TileType.Normal) continue;
                if (tile.GetColorID() != targetColorID) continue;

                connected.Add(tile);

                foreach (var dir in SideTilesPositions)
                {
                    Vector2Int neighbor = pos + dir;
                    if (IsWithinBounds(neighbor) && !visited.Contains(neighbor))
                        queue.Enqueue(neighbor);
                }
            }
            //Add Score
            if (_levelData.ActiveGoals.Any(data => data.Type == GoalType.ScorePoint))
                SetScoreCounter(IncreaseCount: connected.Count * 2);

            return connected;
        }

        
        //The most important and seemingly complex function of the project. 
        
        
        private void DamageBarriers(List<List<TileController>> allMatches)
        {
            //IN ORDER
            
            //make all matches into a single tile list
            //create 4 directions for each tile
            //remove those not on the board from the list
            //take the tile in the target position
            //select those who are barriers
            //If it has been added twice, delete it.

            var damagedTiles = allMatches
                .SelectMany(group => group)//
                .SelectMany(tile => SideTilesPositions
                    .Select(dir => tile.currentPosition + dir))
                .Where(sidePos => IsWithinBounds(sidePos))
                .Select(sidePos => GetTile(sidePos))
                .Where(tile => tile != null && tile.TileModel.TileType != TileType.Normal)
                .Distinct()
                .ToList();
            
            foreach (var tile in damagedTiles)
                tile.DamageBarrier(1);
        }

        
        //The exploding tile pushes the tile above it down. 
        public void FallingTiles()
        {
            BoardBounds bounds = GetBoundsFromLevel();
            
            for (int x = bounds.MinX; x <= bounds.MaxX; x++)
            {
                for (int y = bounds.MinY; y <= bounds.MaxY; y++)
                {
                    Vector2Int nodePos = new Vector2Int(x, y);
                    
                    if(!_nodes.TryGetValue(nodePos, out var node))
                        continue;

                    if (node.currentTile != null)
                        continue;
                    
                    var fallingTile = FindFallingTileAbove(nodePos, bounds);
                    
                    if(fallingTile != null)
                        FallingMoveTile(fallingTile, nodePos);
                    else
                        _bus.Fire(new SpawnTile
                        {
                            TargetPos =  new Vector2Int(x,y)
                        });
                }
            }
        }

        private TileController FindFallingTileAbove(Vector2Int nodePos, BoardBounds bounds)
        {
            for (int aboveY = nodePos.y + 1; aboveY <= bounds.MaxY; aboveY++)
            {
                var abovePos = new Vector2Int(nodePos.x, aboveY);
                if(!_nodes.TryGetValue(abovePos, out var aboveNode))
                    continue;

                if (aboveNode.currentTile != null &&
                    aboveNode.currentTile.TileModel.TileType == TileType.Normal)
                {
                    return aboveNode.currentTile;
                }
            }

            return null;
        }
    }
}
