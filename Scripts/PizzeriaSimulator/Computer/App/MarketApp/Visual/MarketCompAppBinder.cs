using Game.Root.ServicesInterfaces;
using Zenject;

namespace Game.PizzeriaSimulator.Computer.App.Market.Visual
{
    class MarketCompAppBinder
    {
        readonly MarketCompApp marketCompApp;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly DiContainer diContainer;
        public MarketCompAppBinder(MarketCompApp _marketCompApp, PizzeriaSceneReferences _sceneReferences, DiContainer _diContainer)
        {
            marketCompApp = _marketCompApp;
            sceneReferences = _sceneReferences;
            diContainer = _diContainer;
        }
        public void Bind()
        {
            MarketCompAppVM marketVM = new(marketCompApp);
            sceneReferences.MarketCompAppView.Bind(marketVM);
            marketVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(marketVM);
        }
    }
}
