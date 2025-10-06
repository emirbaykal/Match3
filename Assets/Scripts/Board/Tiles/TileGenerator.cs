using System;
using ScriptableObjects.Scripts;
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

        private void SpawnInitialTiles(LevelInitialized signal)
        {
            foreach (var tileData in signal.LevelData.Tiles)
            {
                TileController tile = _pool.Spawn(tileData);
                if (tileData.TileType == TileType.Ice)
                {
                    signal.BoardModel.IncreaseIceCount();
                }

                //signal.BoardModel.AssignTileToNode(tileData.Position,tile);
                var targetNode = signal.BoardModel.Nodes[tileData.Position];

                targetNode.currentTile = tile;
            }
            
        }

        private void TileSpawner(SpawnTile signal)
        {
            TileData spawnTileData = GetSpawnTileData(signal.TargetPos);

            _pool.Spawn(spawnTileData);
        }

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