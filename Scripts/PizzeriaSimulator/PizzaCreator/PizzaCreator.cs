using Game.PizzeriaSimulator.Boxes.Carry;
using Game.PizzeriaSimulator.Interactions;
using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.PizzaCreation.Config;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold;
using Game.PizzeriaSimulator.PizzaHold;
using Game.PizzeriaSimulator.PizzasConfig;
using Game.Root.ServicesInterfaces;
using System;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.PizzaCreation
{
    public class PizzaCreator : IInittable, ISceneDisposable
    {
        public event Action OnEnterCreate;
        public event Action OnLeaveCreate;
        public event Action OnEnterCut;
        public event Action OnLeaveCut;
        public event Action OnPizzaCleared;
        public event Action<int, IEnumerable<PizzaIngredientType>, bool> ForcePizzaCreate;
        public event Action<int> OnPizzaReadyForBake;
        public event Action<int> OnPizzaBake;
        public event Action<int> OnPizzaBakeCancelled;
        public event Action<int> OnPizzaBaked;
        public event Action<PizzaIngredientType> OnIngredientSetted;
        public event Action<PizzaIngredientType, int> OnIngredientCancelled;
        public event Action<IEnumerable<PizzaIngredientType>> OnNewPossibleIngredients;
        readonly PizzaHolder pizzaHolder;
        readonly PizzaIngredientsHolder ingredientsHolder;
        readonly BoxesCarrier boxesCarrier;
        readonly Interactor interactor;
        readonly AllPizzaConfig allPizzaConfig;
        readonly PizzaCreatorConfig pizzaCreatorConfig;
        readonly Stack<PizzaIngredientType> placedIngredients;
        readonly HashSet<PizzaIngredientType> possibleIngredients;
        readonly List<int> possiblePizzas;
        readonly PizzaCreatorData pizzaCreatorData;
        public PizzaCreatorData PizzaCreatorData => pizzaCreatorData;
        public int CurrentPizzaInCut  => pizzaCreatorData.PizzaInCut;
        public int PizzasInBakeCount  => pizzaCreatorData.PizzasInBake.Count;
        readonly int baseIngredientsAmount;
        int readyPizza = -1;
        bool isBaseCreated;
        const int maxPizzasInBake = 4;
        const int averageIngredientsCount = 5;
        public PizzaCreator(PizzaCreatorData _pizzaCreatorData, PizzaIngredientsHolder _ingredientsHolder, PizzaHolder _pizzaHolder, BoxesCarrier _boxesCarrier, 
            Interactor _interactor, AllPizzaConfig _allPizzaConfig, PizzaCreatorConfig _pizzaCreatorConfig)
        {
            ingredientsHolder = _ingredientsHolder;
            pizzaHolder = _pizzaHolder;
            boxesCarrier = _boxesCarrier;
            interactor = _interactor;
            allPizzaConfig = _allPizzaConfig;
            pizzaCreatorConfig = _pizzaCreatorConfig;
            placedIngredients = new Stack<PizzaIngredientType>();
            possibleIngredients = new HashSet<PizzaIngredientType>(averageIngredientsCount);
            possiblePizzas = new List<int>(allPizzaConfig.PizzasCount);
            baseIngredientsAmount = pizzaCreatorConfig.IngredientsForBase.Count;
            pizzaCreatorData = _pizzaCreatorData ?? new PizzaCreatorData();
        }
        public void Init()
        {
            if (pizzaCreatorData.PizzasInBake.Count > 0)
            {
                List<PizzaIngredientType> pizzaIngredients = new (averageIngredientsCount);
                int ingredientIndex;
                int pizzaId;
                PizzaConfig pizzaConfig;
                for (int i = 0 - ((pizzaCreatorData.PizzaInCut == -1) ? 0 : 1); i < pizzaCreatorData.PizzasInBake.Count; i++)
                {
                    pizzaId = (i != -1) ? pizzaCreatorData.PizzasInBake[i] : pizzaCreatorData.PizzaInCut;
                    pizzaConfig = allPizzaConfig.GetPizzaByID(pizzaId);
                   
                    if (pizzaConfig == null) continue;
                    for(ingredientIndex = 0; ingredientIndex < pizzaCreatorConfig.IngredientsForBase.Count; ingredientIndex++)
                    {
                        pizzaIngredients.Add(pizzaCreatorConfig.IngredientsForBase[ingredientIndex]);
                    }
                    for(ingredientIndex = 0; ingredientIndex < pizzaConfig.Ingredients.Count; ingredientIndex++)
                    {
                        pizzaIngredients.Add(pizzaConfig.Ingredients[ingredientIndex]);
                    }
                    ForcePizzaCreate?.Invoke(pizzaId, pizzaIngredients, i == -1);
                    pizzaIngredients.Clear();
                }
            }
            interactor.OnInteract += HandleInteractor;
        }
        public void Dispose()
        {
            interactor.OnInteract -= HandleInteractor;
        }
        void HandleInteractor(InteractableType interactableType)
        {
            if (boxesCarrier.IsCarryingBox) return;
            if (interactableType == InteractableType.PizzaCreateTable)
            {
                EnterPizzaCreate();
            }
            else if(interactableType == InteractableType.PizzaCutTable)
            {
                EnterPizzaCut();
            }
        }
        public void EnterPizzaCreate()
        {
            OnEnterCreate?.Invoke();
            UpdatePossibleIngredients();
        }
        public void LeavePizzaCreate()
        {
            OnLeaveCreate?.Invoke();
        }
        public void EnterPizzaCut()
        {
            OnEnterCut?.Invoke();
        }
        public void LeavePizzaCut()
        {
            OnLeaveCut?.Invoke();
        }
        public void BakePizzaInput()
        {
            if (readyPizza < 0) return;
            if(pizzaCreatorData.PizzasInBake.Count >= maxPizzasInBake)
            {
                OnPizzaBakeCancelled?.Invoke(readyPizza);
                return; 
            }
            pizzaCreatorData.PizzasInBake.Add(readyPizza);
            OnPizzaBake?.Invoke(readyPizza);
            ClearAllPizzaValues();
            UpdatePossibleIngredients();
        }
        public void ClearPizzaInput()
        {
            if (placedIngredients.Count < 1) return;
            foreach(PizzaIngredientType ingredient in placedIngredients)
            {
                ingredientsHolder.TryAddIngredient(ingredient);
            }
            ClearAllPizzaValues();
            OnPizzaCleared?.Invoke();
            UpdatePossibleIngredients();
        }
        void ClearAllPizzaValues()
        {
            possibleIngredients.Clear();
            possiblePizzas.Clear();
            placedIngredients.Clear();
            readyPizza = -1;
            isBaseCreated = false;
        }
        public void PizzaBakedInput(int pizzaId)
        {
            pizzaCreatorData.PizzasInBake.Remove(pizzaId);
            pizzaCreatorData.PizzaInCut = pizzaId;
            OnPizzaBaked?.Invoke(pizzaId);
        }
        public void PizzaCuttedInput(int pizzaId)
        {
            if (pizzaId != pizzaCreatorData.PizzaInCut) return;
            pizzaCreatorData.PizzaInCut = -1;
            pizzaHolder.AddPizza(pizzaId);
        }
        public void PlaceIngredient(PizzaIngredientType ingredientType)
        {
            if (!possibleIngredients.Contains(ingredientType) || !ingredientsHolder.TryRemoveIngredient(ingredientType))
            {
                OnIngredientCancelled?.Invoke(ingredientType, 0);
                return;
            }
            placedIngredients.Push(ingredientType);
            UpdatePossiblePizzas();
            if (!isBaseCreated && pizzaCreatorConfig.IngredientsForBase.Count <= placedIngredients.Count)
            {
                isBaseCreated = true;
                for (int i = 0; i < allPizzaConfig.PizzasCount; i++) { possiblePizzas.Add(i); }
            }
            UpdatePossibleIngredients();
            CheckPizzaComplete();
            OnIngredientSetted?.Invoke(ingredientType);
        }
        void CheckPizzaComplete()
        {
            readyPizza = -1;
            if (!isBaseCreated) return;
            foreach (int pizzaID in possiblePizzas)
            {
                if (allPizzaConfig.GetPizzaByID(pizzaID).Ingredients.Count == placedIngredients.Count - baseIngredientsAmount)
                {
                    readyPizza = pizzaID;
                    OnPizzaReadyForBake?.Invoke(pizzaID);
                    break;
                }
            }
        }
        void UpdatePossiblePizzas()
        {
            if (!isBaseCreated || placedIngredients.Count < 1) return;
            PizzaIngredientType lastPlacedIngredient = placedIngredients.Peek();
            for (int i = 0; i < possiblePizzas.Count; i++)
            {
                if (!allPizzaConfig.GetPizzaByID(possiblePizzas[i]).ContainsIngredient(lastPlacedIngredient))
                {
                    possiblePizzas.RemoveAt(i);
                    i--;
                }
            }
        }
        void UpdatePossibleIngredients()
        {
            possibleIngredients.Clear();

            if (!isBaseCreated)
            {
                if (pizzaCreatorConfig.IngredientsForBase.Count > placedIngredients.Count)
                {
                    possibleIngredients.Add(pizzaCreatorConfig.IngredientsForBase[placedIngredients.Count]);
                }
                else
                {
                    UnityEngine.Debug.LogError("Error on update possible ingredients. Base isn't created, but placed is beyond array of pizza base.");
                }
            }
            else
            {
                foreach (int possiblePizzaID in possiblePizzas)
                {
                    foreach (PizzaIngredientType ingredient in allPizzaConfig.GetPizzaByID(possiblePizzaID).Ingredients)
                    {
                        if(!possibleIngredients.Contains(ingredient) && !placedIngredients.Contains(ingredient)) possibleIngredients.Add(ingredient);
                    }
                }
            }

            OnNewPossibleIngredients?.Invoke(possibleIngredients);
        }

        public int GetPizzaInBakeOfID(int pizzaID) 
        {
            int result = 0;
            foreach(int pizzaInBake in pizzaCreatorData.PizzasInBake)
            {
                if (pizzaInBake == pizzaID) result++;
            }
            return result;
        }
        public bool IsIngredientAvailable(PizzaIngredientType ingredientType)
        {
            return ingredientsHolder.HasIngredient(ingredientType);
        }
        public bool CanCurrentPizzaBeBaked()
        {
            return readyPizza > -1;
        }
        public bool HasPizzaInBake(int pizzaID)
        {
            return pizzaCreatorData.PizzasInBake.Contains(pizzaID);
        }
        public bool IsIngredientPlaced(PizzaIngredientType pizzaIngredientType)
        {
            return placedIngredients.Contains(pizzaIngredientType);
        }
        public IEnumerable<int> GetPossiblePizzas()
        {
            return possiblePizzas;
        }
        public IngredientConfig GetIngredientConfigByType(PizzaIngredientType pizzaIngredientType)
        {
            return pizzaCreatorConfig.GetIngredientConfigByType(pizzaIngredientType);
        }
        public PizzaConfig GetPizzaConfigById(int id) 
        {
            return allPizzaConfig.GetPizzaByID(id);
        }
        public PizzaCreatorConfig GetPizzaCreatorConfig()
        {
            return pizzaCreatorConfig;
        }
    }
}