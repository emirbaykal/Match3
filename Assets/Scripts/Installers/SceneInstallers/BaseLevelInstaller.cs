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
        [SerializeField] private GameObject goalPanel;
        public override void InstallBindings()
        {
            Container.DeclareSignal<LevelInitialized>();
            
            Container.DeclareSignal<MatchControl>();
            Container.DeclareSignal<TileMoveCompleted>();
            Container.DeclareSignal<TileMoveCountChanged>();
            Container.DeclareSignal<SpawnTile>();

            Container.DeclareSignal<LevelFailed>();
            Container.DeclareSignal<LevelSuccessful>();
            
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