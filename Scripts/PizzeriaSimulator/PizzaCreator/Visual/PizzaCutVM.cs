using Game.Root.ServicesInterfaces;
using System;
using R3;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual
{
    public class PizzaCutVM : ISceneDisposable
    {
        readonly PizzaCreator pizzaCreator;
        public event Action EnterPizzaCut;
        public event Action LeavePizzaCut;
        public Subject<Unit> LeaveInput;
        public Subject<Unit> PizzaCuttedInput;
        int currentPizza;
        public PizzaCutVM(PizzaCreator _pizzaCreator)
        {
            pizzaCreator = _pizzaCreator;
            LeaveInput = new Subject<Unit>();
            PizzaCuttedInput = new Subject<Unit>();
        }
        public void Init()
        {
            LeaveInput.ThrottleFirst(TimeSpan.FromSeconds(0.1f)).Subscribe(_ => pizzaCreator.LeavePizzaCut());
            PizzaCuttedInput.Subscribe(_ => pizzaCreator.PizzaCuttedInput(currentPizza));
            pizzaCreator.OnEnterCut += OnEnterPizzaCut;
            pizzaCreator.OnLeaveCut += OnLeavePizzaCut;
            pizzaCreator.OnPizzaBaked += HandleNewPizzaToCut;
            pizzaCreator.ForcePizzaCreate += HandlePizzaForce;
        }
        public void Dispose()
        {
            LeaveInput.Dispose();
            PizzaCuttedInput.Dispose();
            pizzaCreator.OnEnterCut -= OnEnterPizzaCut;
            pizzaCreator.OnLeaveCut -= OnLeavePizzaCut;
            pizzaCreator.OnPizzaBaked -= HandleNewPizzaToCut;
            pizzaCreator.ForcePizzaCreate -= HandlePizzaForce;
        }
        void OnEnterPizzaCut()
        {
            EnterPizzaCut?.Invoke();
        }
        void OnLeavePizzaCut()
        {
            LeavePizzaCut?.Invoke();
        }
        void HandleNewPizzaToCut(int pizzaID)
        {
            currentPizza = pizzaID;
        }
        void HandlePizzaForce(int pizzaId, IEnumerable<PizzaIngredientType> ingredients, bool inCut)
        {
            if (inCut) currentPizza = pizzaId;
        }
    }
}
