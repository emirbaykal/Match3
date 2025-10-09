using Signals.Board;
using Zenject;

namespace Installers.SceneInstallers.Level_Installers
{
    public class Level3Installer : BaseLevelInstaller
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            
            Container.DeclareSignal<BushTileCountChanged>();
            Container.DeclareSignal<IceTileCountChanged>();
        }
    }
}