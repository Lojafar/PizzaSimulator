using Game.PizzeriaSimulator.PizzasConfig;
using Game.Root.ServicesInterfaces;
using Zenject;

namespace Game.PizzeriaSimulator.PizzaHold.Visual
{
    sealed class PizzaHolderBinder
    {
        readonly PizzaHolder pizzaHolder;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly AllPizzaConfig allPizzaConfig;
        readonly DiContainer diContainer;
        public PizzaHolderBinder(PizzaHolder _pizzaHolder, PizzeriaSceneReferences _sceneReferences, AllPizzaConfig _allPizzaConfig, DiContainer _diContainer) 
        {
            pizzaHolder = _pizzaHolder;
            sceneReferences = _sceneReferences;
            allPizzaConfig = _allPizzaConfig;
            diContainer = _diContainer;
        }
        public void Bind()
        {
            PizzaHolderVM pizzaHolderVM = new(pizzaHolder, allPizzaConfig);
            sceneReferences.PizzaHolderViewBase.Bind(pizzaHolderVM);
            pizzaHolderVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(pizzaHolderVM);
        }
    }
}
