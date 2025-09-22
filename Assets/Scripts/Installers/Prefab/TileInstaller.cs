using Board.Tiles;
using Zenject;

namespace Installers.Prefab
{
    public class TileInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<TileModel>().AsTransient();
            Container.Bind<TileView>().FromComponentOnRoot().AsSingle();
            Container.Bind<TileController>().FromComponentOnRoot().AsSingle();
        }
    }
}