using Game.Root.ServicesInterfaces;
using Zenject;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual
{
    class PizzaCreatorBinder
    {
        readonly PizzeriaSceneReferences sceneReferences;
        readonly PizzaCreator pizzaCreator;
        readonly DiContainer diContainer;
        public PizzaCreatorBinder(PizzaCreator _pizzaCreator, PizzeriaSceneReferences _sceneReferences,DiContainer _diContainer)
        {
            pizzaCreator = _pizzaCreator;
            sceneReferences = _sceneReferences;
            diContainer = _diContainer;
        }
        public void Bind()
        {
            PizzaCreatorVM pizzaCreatorVM = new(pizzaCreator);
            sceneReferences.PizzaCreatorViewBase.Bind(pizzaCreatorVM);
            pizzaCreatorVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(pizzaCreatorVM);

            PizzaCutVM pizzaCutVM = new(pizzaCreator);
            sceneReferences.PizzaCutViewBase.Bind(pizzaCutVM);
            pizzaCutVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(pizzaCutVM);
        }
    }
}
