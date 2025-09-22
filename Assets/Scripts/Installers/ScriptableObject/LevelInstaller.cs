using ScriptableObjects.Scripts;
using UnityEngine;
using Zenject;

namespace Installers.ScriptableObject
{
    [CreateAssetMenu(fileName = "LevelInstaller", menuName = "Installers/LevelInstaller")]
    public class LevelInstaller : ScriptableObjectInstaller<LevelInstaller>
    {
        [SerializeField] private LevelData _levelData;
        public override void InstallBindings()
        {
            Container.Bind<LevelData>().FromInstance(_levelData).AsSingle();
        }
    }
}