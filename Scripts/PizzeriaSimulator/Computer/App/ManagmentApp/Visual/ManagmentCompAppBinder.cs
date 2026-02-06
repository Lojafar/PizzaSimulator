using Game.Root.ServicesInterfaces;
using Zenject;

namespace Game.PizzeriaSimulator.Computer.App.ManagmentApp.Visual
{
    sealed class ManagmentCompAppBinder
    {
        readonly ManagmentCompApp managmentCompApp;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly DiContainer diContainer;
        public ManagmentCompAppBinder(ManagmentCompApp _managmentCompApp, PizzeriaSceneReferences _sceneReferences, DiContainer _diContainer)
        {
            managmentCompApp = _managmentCompApp;
            sceneReferences = _sceneReferences;
            diContainer = _diContainer;
        }
        public void Bind()
        {
            ManagmentCompAppVM managmentVM = new(managmentCompApp);
            sceneReferences.ManagmentCompAppView.Bind(managmentVM);
            managmentVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(managmentVM);
        }
    }
}
