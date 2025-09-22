using Board;
using Board.Tiles;
using Signals.Board;
using Signals.Managers;
using UI;
using UnityEngine;
using Zenject;

namespace Installers.SceneInstallers
{
    public class BaseLevelInstaller : MonoInstaller
    {
        [SerializeField] private TileController tilePrefab;
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<LevelInitialized>();
            
            Container.Bind<BoardModel>().AsTransient();

            Container.Bind<GoalPanel>().FromComponentInHierarchy().AsTransient();


            Container.BindMemoryPool<TileController, TileController.TilePool>()
                .WithInitialSize(16)
                .FromComponentInNewPrefab(tilePrefab);
        }
    }
}