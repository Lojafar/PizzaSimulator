using Game.Root.ServicesInterfaces;
using Zenject;

namespace Game.PizzeriaSimulator.PizzaHold.Visual
{
    class PizzaHolderBinder
    {
        readonly PizzaHolder pizzaHolder;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly DiContainer diContainer;
        public PizzaHolderBinder(PizzaHolder _pizzaHolder, PizzeriaSceneReferences _sceneReferences, DiContainer _diContainer) 
        {
            pizzaHolder = _pizzaHolder;
            sceneReferences = _sceneReferences;
            diContainer = _diContainer;
        }
        public void Bind()
        {
            PizzaHolderVM pizzaHolderVM = new(pizzaHolder);
            sceneReferences.PizzaHolderViewBase.Bind(pizzaHolderVM);
            pizzaHolderVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(pizzaHolderVM);
        }
    }
}
