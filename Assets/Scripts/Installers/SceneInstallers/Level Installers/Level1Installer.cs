using Signals.Board;
using Zenject;

namespace Installers.SceneInstallers.Level_Installers
{
    public class Level1Installer : BaseLevelInstaller
    {
        public override void InstallBindings()
        {
            //BASE LEVEL INSTALLER
            base.InstallBindings();
            
            Container.DeclareSignal<ScoreChanged>();
            
        }
    }
}