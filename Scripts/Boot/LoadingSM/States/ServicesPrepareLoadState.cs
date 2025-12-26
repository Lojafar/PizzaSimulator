using Cysharp.Threading.Tasks;
using Game.Root.AssetsManagment;
using Game.Root.User.Environment;
using Game.Root.User.Environment.Editor;
using Zenject;

namespace Game.Boot.LoadingSM.States
{
    class ServicesPrepareLoadState : INonParamLoadingState
    {
        readonly LoadingStateMachine loadingStateMachine;
        readonly DiContainer diContainer;
        public ServicesPrepareLoadState(LoadingStateMachine _loadingStateMachine, DiContainer _diContainer)
        {
            loadingStateMachine = _loadingStateMachine;
            diContainer = _diContainer.ParentContainers[0];
        }
        public async void Enter() 
        {
            await PrepareServices();
            loadingStateMachine.EnterState<GameLoadingState>();
        }
        async UniTask PrepareServices()
        {
            IEnvironmentHandler environmentHandler = new EditorEnvironmentHandler(diContainer.Resolve<IAssetsProvider>());
            await environmentHandler.Init();
            diContainer.Bind<IEnvironmentHandler>().FromInstance(environmentHandler).AsSingle().NonLazy();
        }
    }
}
