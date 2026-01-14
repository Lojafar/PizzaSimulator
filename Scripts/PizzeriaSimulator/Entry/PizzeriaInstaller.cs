using Game.PizzeriaSimulator.Exit;
using Game.PizzeriaSimulator.SaveLoadHelp;
using UnityEngine;
using Zenject;

namespace Game.PizzeriaSimulator.Entry
{
    class PizzeriaInstaller : MonoInstaller
    {
        [SerializeField] PizzeriaSceneReferences sceneReferences;
        public override void InstallBindings()
        {
            Container.Bind<PizzeriaSaveLoadHelper>().AsSingle().NonLazy();
            Container.Bind<PizzeriaSceneReferences>().FromInstance(sceneReferences).AsSingle();
            Container.BindInterfacesAndSelfTo<PizzeriaEntryPoint>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PizzariaExitPoint>().AsSingle().NonLazy();
        }
    }
}
