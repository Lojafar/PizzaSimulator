using Cysharp.Threading.Tasks;
using Game.PizzeriaSimulator.Boxes.Carry;
using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.Orders.Handle;
using Game.PizzeriaSimulator.SaveLoadHelp;
using Game.PizzeriaSimulator.SodaMachine;
using Game.PizzeriaSimulator.SodaMachine.Visual;
using UnityEngine;
using Zenject;

namespace Game.PizzeriaSimulator.Pizzeria.Furniture.Placement.Handler
{
    sealed class SodaMachineSubHandler : IFurnitureSubHandler
    {
        public const int SubHandlerTargetID = 1;
        readonly DiContainer diContainer;
        public SodaMachineSubHandler(DiContainer _diContainer)
        {
            diContainer = _diContainer;
        }
        public void HandleFurniture(GameObject furnitureObject)
        {
            if(furnitureObject.TryGetComponent(out PizzeriaSodaMachineViewBase sodaMachineView))
            {
                PizzeriaSodaMachine sodaMachine = new(diContainer.Resolve<Interactor>(), diContainer.Resolve<BoxesCarrier>(),
                    diContainer.Resolve<PizzeriaSaveLoadHelper>());
                new PizzeriaSodaMachineBinder(sodaMachine, sodaMachineView, diContainer).Bind();
                diContainer.Bind<PizzeriaSodaMachine>().FromInstance(sodaMachine);
                sodaMachine.Init().Forget();
                diContainer.Resolve<PizzeriaOrdersHandler>().SetSodaMachine(sodaMachine);
            }
        }
    }
}