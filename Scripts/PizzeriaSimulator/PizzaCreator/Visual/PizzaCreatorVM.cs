using Game.PizzeriaSimulator.PizzasConfig;
using Game.PizzeriaSimulator.PizzaCreation.Config;
using Game.PizzeriaSimulator.PizzaCreation.Visual.IngredientOnPizza;
using Game.Root.ServicesInterfaces;
using System;
using System.Collections.Generic;
using R3;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual
{
    public class PizzaCreatorVM : ISceneDisposable
    {
        public event Action EnterConstruction;
        public event Action LeaveConstruction;
        public event Action OnPizzaStarted;
        public event Action RemovePizza;
        public event Action DehighlightContainers;
        public event Action ConfirmIngredientInput;
        public event Action<BakedPizzaObject, Action> ForceCurrentPizzaToBake;
        public event Action<BakedPizzaObject, Action> BakePizza;
        public event Action<PizzaIngredientType> HighlightContainer;
        public event Action<IngredientOnPizzaObjectBase> ConfirmIngredientPlace;
        public event Action<IngredientOnPizzaObjectBase> ForceIngredientPlace;
        public event Action<string> CancelIngredientPlace;
        public event Action<string> CancellPizzaBake;
        public event Action<string> PizzaAssembled;
        public event Action<bool> ActivateBakeInput;
        public Subject<Unit> LeaveInput;
        public Subject<Unit> ClearPizzaInput;
        public Subject<Unit> BakeInput;
        public Subject<PizzaIngredientType> IngredientInput;
        public Subject<PizzaIngredientType> IngredientPlaceInput;
        readonly PizzaCreator pizzaCreator;
        public PizzaCreatorVM(PizzaCreator _pizzaCreator)
        {
            pizzaCreator = _pizzaCreator;
            LeaveInput = new Subject<Unit>();
            ClearPizzaInput = new Subject<Unit>();
            BakeInput = new Subject<Unit>();
            IngredientInput = new Subject<PizzaIngredientType>();
            IngredientPlaceInput = new Subject<PizzaIngredientType>();
        }
        public void Init()
        {
            LeaveInput.ThrottleFirst(TimeSpan.FromSeconds(0.1f)).Subscribe(_ => pizzaCreator.LeavePizzaCreate());
            IngredientInput.ThrottleFirst(TimeSpan.FromSeconds(0.1f)).Subscribe(OnIngredientInput);
            IngredientPlaceInput.ThrottleFirst(TimeSpan.FromSeconds(0.1f)).Subscribe(pizzaCreator.PlaceIngredient);
            ClearPizzaInput.ThrottleFirst(TimeSpan.FromSeconds(0.1f)).Subscribe(_ => pizzaCreator.ClearPizzaInput());
            BakeInput.ThrottleFirst(TimeSpan.FromSeconds(0.1f)).Subscribe(_ => pizzaCreator.BakePizzaInput());
            pizzaCreator.OnEnterCreate += OnEnterPizzaCreate;
            pizzaCreator.OnLeaveCreate += OnLeavePizzaCreate;
            pizzaCreator.ForcePizzaCreate += HandlePizzaForceCreate;
            pizzaCreator.OnPizzaReadyForBake += OnPizzaReadyForBake;
            pizzaCreator.OnIngredientSetted += HandleIngredientPlace;
            pizzaCreator.OnIngredientCancelled += HandleIngredientPlaceCancel;
            pizzaCreator.OnNewPossibleIngredients += HandlePossibleIngredients;
            pizzaCreator.OnPizzaCleared += HandlePizzaClear;
            pizzaCreator.OnPizzaBake += HandlePizzaBake;
            pizzaCreator.OnPizzaBakeCancelled += HandlePizzaBakeCancell;
        }
        public void Dispose()
        {
            LeaveInput.Dispose();
            IngredientInput.Dispose();
            IngredientPlaceInput.Dispose();
            ClearPizzaInput.Dispose();
            pizzaCreator.OnEnterCreate -= OnEnterPizzaCreate;
            pizzaCreator.OnLeaveCreate -= OnLeavePizzaCreate;
            pizzaCreator.ForcePizzaCreate -= HandlePizzaForceCreate;
            pizzaCreator.OnPizzaReadyForBake -= OnPizzaReadyForBake;
            pizzaCreator.OnIngredientSetted -= HandleIngredientPlace;
            pizzaCreator.OnIngredientCancelled -= HandleIngredientPlaceCancel;
            pizzaCreator.OnNewPossibleIngredients -= HandlePossibleIngredients; 
            pizzaCreator.OnPizzaCleared -= HandlePizzaClear;
            pizzaCreator.OnPizzaBake -= HandlePizzaBake;
            pizzaCreator.OnPizzaBakeCancelled -= HandlePizzaBakeCancell;
        }
        void OnIngredientInput(PizzaIngredientType pizzaIngredientType)
        {
            if (pizzaCreator.IsIngredientAvailable(pizzaIngredientType)) ConfirmIngredientInput?.Invoke();
        }
        void OnEnterPizzaCreate()
        {
            ActivateBakeInput?.Invoke(pizzaCreator.CanCurrentPizzaBeBaked());
            EnterConstruction?.Invoke();
        }
        void OnLeavePizzaCreate()
        {
            DehighlightContainers?.Invoke();
            LeaveConstruction?.Invoke();
        }
        void HandlePizzaForceCreate(int pizzaId, IEnumerable<PizzaIngredientType> ingredients, bool pizzaInCut)
        {
            OnPizzaStarted?.Invoke();
            IngredientConfig ingredientConfig;
            foreach (PizzaIngredientType ingredientType in ingredients)
            {
                ingredientConfig = pizzaCreator.GetIngredientConfigByType(ingredientType);
                if (ingredientConfig == null) continue;
                ForceIngredientPlace?.Invoke(ingredientConfig.OnPizzaPrefab);
            }
            ForceCurrentPizzaToBake?.Invoke(pizzaCreator.GetPizzaConfigById(pizzaId).BakedPizzaPrefab, pizzaInCut? null : () => pizzaCreator.PizzaBakedInput(pizzaId));
        }
        void OnPizzaReadyForBake(int pizzaId)
        {
            PizzaConfig pizzaConfig = pizzaCreator.GetPizzaConfigById(pizzaId);
            if (pizzaConfig != null) PizzaAssembled?.Invoke(pizzaConfig.Name);
        }
        void HandleIngredientPlace(PizzaIngredientType ingredientType)
        {
            if (ingredientType == PizzaIngredientType.Dough) OnPizzaStarted?.Invoke();
            ActivateBakeInput?.Invoke(pizzaCreator.CanCurrentPizzaBeBaked());
            IngredientConfig ingredientConfig = pizzaCreator.GetIngredientConfigByType(ingredientType);
            if (ingredientConfig == null) return;
            ConfirmIngredientPlace?.Invoke(ingredientConfig.OnPizzaPrefab);
        }
        void HandleIngredientPlaceCancel(PizzaIngredientType ingredientType, int code)
        {
            string message = "";
            switch (code)
            {
                case 0:
                    message = "Wrong Ingredient";
                    break;
            }
            CancelIngredientPlace?.Invoke(message);
        }
        void HandlePossibleIngredients(IEnumerable<PizzaIngredientType> possibleIngredients)
        {
            DehighlightContainers?.Invoke();
            foreach (PizzaIngredientType ingredientType in possibleIngredients)
            {
                HighlightContainer?.Invoke(ingredientType);
            }
        }
        void HandlePizzaClear()
        {
            ActivateBakeInput?.Invoke(false);
            RemovePizza?.Invoke();
        }
        void HandlePizzaBake(int pizzaID)
        {
            ActivateBakeInput?.Invoke(false);
            BakePizza?.Invoke(pizzaCreator.GetPizzaConfigById(pizzaID).BakedPizzaPrefab, () => pizzaCreator.PizzaBakedInput(pizzaID));
        }
        void HandlePizzaBakeCancell(int pizzaID)
        {
            CancellPizzaBake?.Invoke("Too much pizza on the conveyor");
        }
    }
}
