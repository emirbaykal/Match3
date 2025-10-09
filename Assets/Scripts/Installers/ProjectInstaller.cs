using Managers.LevelLoader;
using ScriptableObjects.Scripts.Level.LevelLoader;
using Signals.Managers;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public LevelDatabase LevelDatabase;
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.BindInstance(LevelDatabase).AsSingle();
            
            Container.DeclareSignal<RequestLoadLevel>();
            Container.DeclareSignal<RequestMainMenu>();
            Container.DeclareSignal<RequestNextLevel>();
            Container.DeclareSignal<RequestRetryLevel>();

            Container.BindInterfacesAndSelfTo<LevelLoaderManager>().AsSingle();
        }
    }
}