using Game.PizzeriaSimulator.PizzaCreation;
using Game.PizzeriaSimulator.PizzaCreation.Config;
using Game.PizzeriaSimulator.PizzasConfig;
using Game.Root.ServicesInterfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PizzeriaSimulator.OrdersHandle.Visual
{
    using DeviceType = Game.Root.User.Environment.DeviceType;
    public class OrderVisualData
    {
        public readonly int OrderId;
        public readonly Sprite OrderIcon;
        public readonly Sprite[] IngredientsSprites;
        public readonly bool[] ReadyIngredients;
        public readonly bool InBake;
        public readonly bool InCut;
        public OrderVisualData(int _orderId, Sprite _orderIcon, Sprite[] _ingredientsSprites, bool[] _readyIngredients)
        {
            OrderId = _orderId;
            OrderIcon = _orderIcon;
            IngredientsSprites = _ingredientsSprites; 
            ReadyIngredients = _readyIngredients; 
        }
        public OrderVisualData(int _orderId, Sprite _orderIcon, bool _inBake = false, bool _inCut = false)
        {
            OrderId = _orderId;
            OrderIcon = _orderIcon;
            InBake = _inBake;
            InCut = _inCut;
        }
    }
    enum VMOrderState : byte
    {
        Gathering = 0, Baking = 1, Cutting = 2, Ready = 3 
    }
    readonly struct OrderVMData
    {
        public readonly int OrderIndex;
        public readonly VMOrderState OrderState;
        public OrderVMData(int _orderIndex, VMOrderState _orderState = VMOrderState.Gathering)
        {
            OrderIndex = _orderIndex;
            OrderState = _orderState;
        }
    }
    public class PizzeriaOrderHandlVM : ISceneDisposable
    {
        readonly PizzeriaOrdersHandler ordersHandler;
        readonly PizzaCreator pizzaCreator;
        readonly PizzaCreatorConfig pizzaCreatorConfig;
        public event Action<OrderVisualData> OnNewOrder;
        public event Action<int> CompleteOrder;
        public event Action<int, int> ComleteIngredientInOrder;
        public event Action<int> SetOrderBakeState;
        public event Action<int> SetOrderCutState;
        public event Action<int> SetOrderCompleted;
        public event Action ClearAllCompletedInBars;
        public event Action<string> OnBellCancelled;
        readonly int pizzaBaseIngredientAmount;
        int handledOrdersAmount;
        public readonly DeviceType DeviceType;
       
        readonly Dictionary<int, List<OrderVMData>> OrderIDToData;
        public PizzeriaOrderHandlVM(PizzeriaOrdersHandler _ordersHandler, PizzaCreator _pizzaCreator, DeviceType _deviceType)
        {
            OrderIDToData = new Dictionary<int, List<OrderVMData>>();
            ordersHandler = _ordersHandler;
            pizzaCreator = _pizzaCreator;
            DeviceType = _deviceType;
            pizzaCreatorConfig = pizzaCreator.GetPizzaCreatorConfig();
            pizzaBaseIngredientAmount = pizzaCreatorConfig.IngredientsForBase.Count;
        }
        public void Init()
        {
            ordersHandler.OnNewPizzaOrder += HandleNewOrder;
            ordersHandler.OnPizzaOrderReady += HandleOrderReady;
            ordersHandler.OnPizzaOrderCompleted += HandleOrderComplete;
            ordersHandler.OnBellCallCanceled += HandleBellCancell;
            pizzaCreator.OnIngredientSetted += HandleCreatorIngredientSet;
            pizzaCreator.OnPizzaCleared += HandleCreatorClear;
            pizzaCreator.OnPizzaBake += HandlePizzaBake;
            pizzaCreator.OnPizzaBaked += HandlePizzaCut;
        }
        public void Dispose()
        {
            ordersHandler.OnNewPizzaOrder -= HandleNewOrder;
            ordersHandler.OnPizzaOrderReady -= HandleOrderReady;
            ordersHandler.OnPizzaOrderCompleted -= HandleOrderComplete;
            ordersHandler.OnBellCallCanceled -= HandleBellCancell;
            pizzaCreator.OnIngredientSetted -= HandleCreatorIngredientSet;
            pizzaCreator.OnPizzaCleared -= HandleCreatorClear;
            pizzaCreator.OnPizzaBake -= HandlePizzaBake;
            pizzaCreator.OnPizzaBaked -= HandlePizzaCut;
        }
        void HandleNewOrder(int orderID)
        {
            if (!OrderIDToData.ContainsKey(orderID)) OrderIDToData.Add(orderID, new List<OrderVMData>());
            int orderIndex = handledOrdersAmount;
            handledOrdersAmount++;
            OrderVMData orderVMData = new(orderIndex);
            OrderVisualData orderVisualData;

            PizzaConfig pizzaConfig = pizzaCreator.GetPizzaConfigById(orderID);
            if (pizzaCreator.CurrentPizzaInCut == orderID &&
                !HasOrderOfState(VMOrderState.Cutting, OrderIDToData[orderID]))
            {
                orderVMData = new OrderVMData(orderIndex, VMOrderState.Cutting);
                orderVisualData = new OrderVisualData(orderIndex, pizzaConfig.PizzaIcon, _inCut: true);
            }
            else if (HasNotHandledPizzaInBake(orderID))
            {
                orderVMData = new OrderVMData(orderIndex, VMOrderState.Baking);
                orderVisualData = new OrderVisualData(orderIndex, pizzaConfig.PizzaIcon, _inBake: true);
            }
            else
            {
                Sprite[] ingredientsSprites = new Sprite[pizzaConfig.Ingredients.Count + pizzaBaseIngredientAmount];
                bool[] ingredientsComplete = new bool[ingredientsSprites.Length];
                int i;
                for (i = 0; i < pizzaBaseIngredientAmount; i++)
                {
                    ingredientsSprites[i] = pizzaCreatorConfig.GetIngredientConfigByType(pizzaCreatorConfig.IngredientsForBase[i]).IngredientIcon;
                    ingredientsComplete[i] = pizzaCreator.IsIngredientPlaced(pizzaCreatorConfig.IngredientsForBase[i]);
                }
                for (i = 0; i < pizzaConfig.Ingredients.Count; i++)
                {
                    ingredientsSprites[i + pizzaBaseIngredientAmount] = pizzaCreatorConfig.GetIngredientConfigByType(pizzaConfig.Ingredients[i]).IngredientIcon;
                    ingredientsComplete[i + pizzaBaseIngredientAmount] = pizzaCreator.IsIngredientPlaced(pizzaConfig.Ingredients[i]);
                }
                orderVisualData = new OrderVisualData(orderIndex, pizzaConfig.PizzaIcon, ingredientsSprites, ingredientsComplete);
            }
            OrderIDToData[orderID].Add(orderVMData);
            OnNewOrder?.Invoke(orderVisualData);
        }
        bool HasNotHandledPizzaInBake(int pizzaID)
        {
            int pizzasInBake = pizzaCreator.GetPizzaInBakeOfID(pizzaID);
            if (pizzasInBake > 0)
            {
                foreach (OrderVMData orderData in OrderIDToData[pizzaID])
                {
                    if (orderData.OrderState == VMOrderState.Baking) pizzasInBake--;
                }
                return pizzasInBake > 0;
            }
            return false;
        }
        void HandleOrderReady(int pizzaID)
        {
            if (OrderIDToData.TryGetValue(pizzaID, out List<OrderVMData> orders))
            {
                if(TryGetFirstOrderExcState(VMOrderState.Ready, orders, out int orderIndex))
                {
                    SetOrderCompleted?.Invoke(orders[orderIndex].OrderIndex);
                    orders[orderIndex] = new(orders[orderIndex].OrderIndex, VMOrderState.Ready);
                } 
            }
        }
        void HandleOrderComplete(int pizzaID)
        {
            if (OrderIDToData.TryGetValue(pizzaID, out List<OrderVMData> orders))
            {
                if (TryGetFirstOrderOfState(VMOrderState.Ready, orders, out int orderIndex))
                {
                    CompleteOrder?.Invoke(orders[orderIndex].OrderIndex);
                    orders.RemoveAt(orderIndex);
                }
            }
        }
        void HandleBellCancell()
        {
            if (ordersHandler.GetActiveOrdersAmount() > 0)
            {
                OnBellCancelled?.Invoke("The required pizza is not ready.");
            }
            else
            {
                OnBellCancelled?.Invoke("No orders.");
            }
        }
        void HandleCreatorIngredientSet(PizzaIngredientType ingredientType)
        {
            if (pizzaCreatorConfig.IsIngredientOfBaseAndGetIndex(ingredientType, out int baseIngredientIndex))
            {
                foreach (List<OrderVMData> orders in OrderIDToData.Values)
                {
                    foreach (OrderVMData order in orders)
                    {
                        ComleteIngredientInOrder?.Invoke(order.OrderIndex, baseIngredientIndex);
                    }
                }
                return;
            }
            foreach (KeyValuePair<int, List<OrderVMData>> ordersKeyValues in OrderIDToData)
            {
                int ingredientIndex = pizzaCreator.GetPizzaConfigById(ordersKeyValues.Key).GetIndexOfIngredient(ingredientType);
                if (ingredientIndex > -1)
                {
                    ingredientIndex += pizzaBaseIngredientAmount;
                    foreach (OrderVMData order in ordersKeyValues.Value)
                    {
                        ComleteIngredientInOrder?.Invoke(order.OrderIndex, ingredientIndex);
                    }
                }
            }
        }
        void HandleCreatorClear()
        {
            ClearAllCompletedInBars?.Invoke();
        }
        void HandlePizzaBake(int pizzaID)
        {
            if (OrderIDToData.TryGetValue(pizzaID, out List<OrderVMData> orders))
            {
                if (TryGetFirstOrderOfState(VMOrderState.Gathering, orders, out int orderIndex))
                {
                    SetOrderBakeState?.Invoke(orders[orderIndex].OrderIndex);
                    orders[orderIndex] = new(orders[orderIndex].OrderIndex, VMOrderState.Baking);
                }
            }
            ClearAllCompletedInBars?.Invoke();
        }
        void HandlePizzaCut(int pizzaID)
        {
            if (OrderIDToData.TryGetValue(pizzaID, out List<OrderVMData> orders) && TryGetFirstOrderOfState(VMOrderState.Baking, orders, out int orderIndex))
            {
                SetOrderCutState?.Invoke(orders[orderIndex].OrderIndex);
                orders[orderIndex] = new(orders[orderIndex].OrderIndex, VMOrderState.Cutting);
            }
        }
        bool TryGetFirstOrderExcState(VMOrderState exceptState, List<OrderVMData> orders, out int orderIndex)
        {
            for(orderIndex = 0; orderIndex < orders.Count; orderIndex++)
            {
                if (orders[orderIndex].OrderState != exceptState) 
                {
                    return true;
                }
            }
            return false;
        }
        bool TryGetFirstOrderOfState(VMOrderState state, List<OrderVMData> orders, out int orderIndex)
        {
            for (orderIndex = 0; orderIndex < orders.Count; orderIndex++)
            {
                if (orders[orderIndex].OrderState == state)
                {
                    return true;
                }
            }
            return false;
        }
        bool HasOrderOfState(VMOrderState state, List<OrderVMData> orders)
        {
            foreach (OrderVMData order in orders)
            {
                if (order.OrderState == state) return true;
            }
            return false;
        }
    }
}
