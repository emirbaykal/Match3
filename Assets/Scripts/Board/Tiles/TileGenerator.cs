using System;
using ScriptableObjects.Scripts;
using ScriptableObjects.Scripts.Level.LevelData;
using Signals.Board;
using Signals.Managers;
using UnityEngine;
using Zenject;

namespace Board.Tiles
{
    public class TileGenerator : IInitializable,IDisposable
    {
        private readonly TileController.TilePool _pool;
        private readonly SignalBus _bus;

        public TileGenerator(SignalBus bus, TileController.TilePool pool)
        {
            _bus = bus;
            _pool = pool;
        }

        public void Initialize()
        {
            _bus.Subscribe<SpawnTile>(TileSpawner);
            _bus.Subscribe<LevelInitialized>(SpawnInitialTiles);
        }

        public void Dispose()
        {
            _bus.Unsubscribe<SpawnTile>(TileSpawner);
            _bus.Unsubscribe<LevelInitialized>(SpawnInitialTiles);
        }

        
        //At the start of the game,
        //it generates the level based
        //on the level design we entered into the
        //“LevelData” scriptable object.
        private void SpawnInitialTiles(LevelInitialized signal)
        {
            foreach (var tileData in signal.LevelData.Tiles)
            {
                TileController tile = _pool.Spawn(tileData);
                signal.BoardModel.SetBarrierCounter(tile.TileModel.TileType, 1);

                //signal.BoardModel.AssignTileToNode(tileData.Position,tile);
                var targetNode = signal.BoardModel.Nodes[tileData.Position];

                targetNode.currentTile = tile;
            }
        }

        
        //Used for tiles that spawn individually after different explosions,
        //falling from above, unlike the SpawnInitialTiles function.
        private void TileSpawner(SpawnTile signal)
        {
            TileData spawnTileData = GetSpawnTileData(signal.TargetPos);

            _pool.Spawn(spawnTileData);
        }

        //The function that holds the default
        //data for tiles that will be dropped
        //to fill the gaps
        private TileData GetSpawnTileData(Vector2Int targetPos)
        {
            TileData data = new TileData
            {
                TileType = TileType.Normal,
                Position = targetPos,
                Strength = 1
            };

            return data;
        }
    }
}