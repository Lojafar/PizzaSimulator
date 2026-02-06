using Game.Root.ServicesInterfaces;
using Zenject;

namespace Game.PizzeriaSimulator.Pizzeria.Managment.Visual
{
    sealed class PizzeriaManagerBinder
    {
        readonly PizzeriaManager pizzeriaManager;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly DiContainer diContainer;
        public PizzeriaManagerBinder(PizzeriaManager _pizzeriaManager, PizzeriaSceneReferences _sceneReferences, DiContainer _diContainer) 
        {
            pizzeriaManager = _pizzeriaManager;
            sceneReferences = _sceneReferences;
            diContainer = _diContainer;
        }
        public void Bind()
        {
            PizzeriaManagerVM viewModel = new(pizzeriaManager);
            sceneReferences.PizzeriaManagerView.Bind(viewModel);
            viewModel.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(viewModel);
        }
        
    }
}
