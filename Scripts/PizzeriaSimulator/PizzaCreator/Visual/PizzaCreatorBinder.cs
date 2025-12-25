using Game.Root.ServicesInterfaces;
using Zenject;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual
{
    class PizzaCreatorBinder
    {
        readonly PizzeriaSceneReferences sceneReferences;
        readonly PizzaCreator pizzaCreator;
        readonly DiContainer diContainer;
        public PizzaCreatorBinder(PizzeriaSceneReferences _sceneReferences, PizzaCreator _pizzaCreator, DiContainer _diContainer)
        {
            sceneReferences = _sceneReferences;
            pizzaCreator = _pizzaCreator;
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
