using Board;
using Board.Tiles;
using Signals.Board;
using Signals.Board.Tile;
using Signals.Managers;
using UI;
using UnityEngine;
using Zenject;

namespace Installers.SceneInstallers
{
    public class BaseLevelInstaller : MonoInstaller
    {
        [SerializeField] private TileController tilePrefab;
        [SerializeField] private BoardController boardPrefab;
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<LevelInitialized>();
            Container.DeclareSignal<MatchControl>();
            Container.DeclareSignal<SpawnTile>();
            Container.DeclareSignal<TileMoveCompleted>();
            Container.Bind<TileInputs>().AsSingle();
            Container.Bind<TileModel>().AsTransient();

            Container.BindMemoryPool<TileController, TileController.TilePool>()
                .WithInitialSize(16)
                .FromComponentInNewPrefab(tilePrefab);
            
            Container.BindInterfacesAndSelfTo<TileGenerator>().AsSingle();
            
            Container.Bind<BoardModel>().AsSingle();

            Container.Bind<BoardController>().FromComponentInNewPrefab(boardPrefab).AsSingle().NonLazy();
            
            Container.Bind<GoalPanel>().FromComponentInHierarchy().AsTransient();
        }
    }
}