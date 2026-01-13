using Game.Root.ServicesInterfaces;
using Zenject;

namespace Game.PizzeriaSimulator.PizzaCreation.IngredientsHold.Visual
{
    class PizzaIngredientsHolderBinder
    {
        readonly PizzeriaSceneReferences sceneReferences;
        readonly PizzaIngredientsHolder ingredientsHolder;
        readonly DiContainer diContainer;
        public PizzaIngredientsHolderBinder(PizzaIngredientsHolder _ingredientsHolder, PizzeriaSceneReferences _sceneReferences,  DiContainer _diContainer)
        {
            ingredientsHolder = _ingredientsHolder;
            sceneReferences = _sceneReferences;
            diContainer = _diContainer;
        }
        public void Bind()
        {
            PizzaIngredientsHolderVM ingredientsHolderVM = new(ingredientsHolder);
            sceneReferences.PizzaIngredientsHoldView.Bind(ingredientsHolderVM);
            ingredientsHolderVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(ingredientsHolderVM);
        }
    }
}
