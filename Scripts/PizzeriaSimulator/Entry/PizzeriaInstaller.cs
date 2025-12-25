using Game.PizzeriaSimulator.Exit;
using UnityEngine;
using Zenject;

namespace Game.PizzeriaSimulator.Entry
{
    class PizzeriaInstaller : MonoInstaller
    {
        [SerializeField] PizzeriaSceneReferences sceneReferences;
        public override void InstallBindings()
        {
            Container.Bind<PizzeriaSceneReferences>().FromInstance(sceneReferences).AsSingle();
            Container.BindInterfacesAndSelfTo<PizzeriaEntryPoint>().AsSingle();
            Container.BindInterfacesAndSelfTo<PizzariaExitPoint>().AsSingle();
        }
    }
}
