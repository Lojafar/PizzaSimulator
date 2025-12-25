using Game.PizzeriaSimulator.PizzaCreation.Config;
using Game.PizzeriaSimulator.PizzaHold;
using System;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.PizzaCreation
{
    public class PizzaCreator
    {
        public event Action OnEnterCreate;
        public event Action OnLeaveCreate;
        public event Action OnEnterCut;
        public event Action OnLeaveCut;
        public event Action OnPizzaCleared;
        public event Action<int> OnPizzaReadyForBake;
        public event Action<int> OnPizzaBake;
        public event Action<int> OnPizzaBakeCancelled;
        public event Action<int> OnPizzaBaked;
        public event Action<PizzaIngredientType> OnIngredientSetted;
        public event Action<PizzaIngredientType, int> OnIngredientCancelled;
        public event Action<IEnumerable<PizzaIngredientType>> OnNewPossibleIngredients;

        readonly PizzaHolder pizzaHolder;
        readonly PizzaCreatorConfig pizzaCreatorConfig;
        readonly Stack<PizzaIngredientType> placedIngredients;
        readonly HashSet<PizzaIngredientType> possibleIngredients;
        readonly List<int> possiblePizzas;
        readonly List<int> pizzasInBake;

        public int CurrentPizzaInCut { get; private set; } = -1;
        readonly int baseIngredientsAmount;
        int readyPizza = -1;
        bool isBaseCreated;
        const int maxPizzasInBake = 4;
        public PizzaCreator(PizzaHolder _pizzaHolder, PizzaCreatorConfig _pizzaCreatorConfig)
        {
            pizzaHolder = _pizzaHolder;
            pizzaCreatorConfig = _pizzaCreatorConfig;
            placedIngredients = new Stack<PizzaIngredientType>();
            possibleIngredients = new HashSet<PizzaIngredientType>(5);
            possiblePizzas = new List<int>(pizzaCreatorConfig.Pizzas.Length);
            pizzasInBake = new List<int>(maxPizzasInBake);
            baseIngredientsAmount = pizzaCreatorConfig.IngredientsForBase.Length;
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
            if(pizzasInBake.Count >= maxPizzasInBake)
            {
                OnPizzaBakeCancelled?.Invoke(readyPizza);
                return; 
            }
            pizzasInBake.Add(readyPizza);
            OnPizzaBake?.Invoke(readyPizza);
            ClearAllPizzaValues();
            UpdatePossibleIngredients();
        }
        public void ClearPizzaInput()
        {
            if (placedIngredients.Count < 1) return;
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
            pizzasInBake.Remove(pizzaId);
            CurrentPizzaInCut = pizzaId;
            OnPizzaBaked?.Invoke(pizzaId);
        }
        public void PizzaCuttedInput(int pizzaId)
        {
            if (pizzaId != CurrentPizzaInCut) return;
            CurrentPizzaInCut = -1;
            pizzaHolder.AddPizza(pizzaId);
        }
        public void PlaceIngredient(PizzaIngredientType ingredientType)
        {
            if (!possibleIngredients.Contains(ingredientType))
            {
                OnIngredientCancelled?.Invoke(ingredientType, 0);
                return;
            }
            placedIngredients.Push(ingredientType);
            UpdatePossiblePizzas();
            if (!isBaseCreated && pizzaCreatorConfig.IngredientsForBase.Length <= placedIngredients.Count)
            {
                isBaseCreated = true;
                for (int i = 0; i < pizzaCreatorConfig.Pizzas.Length; i++) { possiblePizzas.Add(i); }
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
                if (pizzaCreatorConfig.Pizzas[pizzaID].Ingredients.Count == placedIngredients.Count - baseIngredientsAmount)
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
                if (!pizzaCreatorConfig.GetPizzaByID(possiblePizzas[i]).Ingredients.Contains(lastPlacedIngredient))
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
                if (pizzaCreatorConfig.IngredientsForBase.Length > placedIngredients.Count)
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
                    foreach (PizzaIngredientType ingredient in pizzaCreatorConfig.GetPizzaByID(possiblePizzaID).Ingredients)
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
            foreach(int pizzaInBake in pizzasInBake)
            {
                if (pizzaInBake == pizzaID) result++;
            }
            return result;
        }
        public bool CanCurrentPizzaBeBaked()
        {
            return readyPizza > -1;
        }
        public bool HasPizzaInBake(int pizzaID)
        {
            return pizzasInBake.Contains(pizzaID);
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
            return pizzaCreatorConfig.GetPizzaByID(id);
        }
        public PizzaCreatorConfig GetPizzaCreatorConfig()
        {
            return pizzaCreatorConfig;
        }
    }
}