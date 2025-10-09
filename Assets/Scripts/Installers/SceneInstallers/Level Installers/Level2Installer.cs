using Signals.Board;
using Zenject;

namespace Installers.SceneInstallers.Level_Installers
{
    public class Level2Installer : BaseLevelInstaller
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            
            Container.DeclareSignal<IceTileCountChanged>();
        }
    }
}