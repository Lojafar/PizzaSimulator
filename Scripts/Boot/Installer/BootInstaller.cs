using Game.Boot.LoadingSM;
using Game.Boot.LoadingSM.States;
using Zenject;

namespace Game.Boot.Installer
{
    class BootInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindLoadingStates();
            Container.BindInterfacesAndSelfTo<LoadingStatesFactory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LoadingStateMachine>().AsSingle().NonLazy();
        }
        void BindLoadingStates()
        {
            Container.BindInterfacesAndSelfTo<LoadingBootState>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ServicesPrepareLoadState>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameLoadingState>().AsSingle().NonLazy();
        }
    }
}
