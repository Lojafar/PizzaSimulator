using Game.PizzeriaSimulator.Interactions;
using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.PizzaHold;
using Game.Root.ServicesInterfaces;
using System;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.OrdersHandle
{
    readonly struct OrderData
    {
        public readonly int PizzaID;
        public readonly bool OrderReady;
        public OrderData(int _pizzaID, bool _orderReady = false)
        {
            PizzaID = _pizzaID;
            OrderReady = _orderReady;
        }
    }
    public class PizzeriaOrdersHandler : IInittable, ISceneDisposable
    {
        public int InitPriority => 9;
        readonly PizzaHolder pizzaHolder;
        readonly Interactor interactor;
        public event Action<int> OnNewPizzaOrder;
        public event Action<int> OnPizzaOrderReady;
        public event Action<int> OnPizzaOrderCompleted;
        public event Action<int> OnOrderTaked;
        public event Action OnBellCallCanceled;
        readonly List<OrderData> activeOrders;
        readonly List<OrderData> completedOrders;
        public PizzeriaOrdersHandler(PizzaHolder _pizzaHolder, Interactor _interactor) 
        {
            pizzaHolder = _pizzaHolder;
            interactor = _interactor;
            activeOrders = new List<OrderData>();
            completedOrders = new List<OrderData>();
        }
        public void Init()
        {
            interactor.OnInteract += HandleInteractor;
            pizzaHolder.OnPizzaAdded += HandleNewPizzaInHold;
        }
        public void Dispose()
        {
            interactor.OnInteract -= HandleInteractor;
            pizzaHolder.OnPizzaAdded -= HandleNewPizzaInHold;
        }
        void HandleInteractor(InteractableType interactableType)
        {
            if (interactableType == InteractableType.OrderBell)
            {
                CallBell();
            }
        }
        void HandleNewPizzaInHold(int pizzaID)
        {
            if (TryGetOrderIndexOfPizza(pizzaID, out int orderIndex) && pizzaHolder.TryReservePizza(pizzaID))
            {
                activeOrders[orderIndex] = new OrderData(pizzaID, true);
                OrderReady(orderIndex);
            }
        }   
        void OrderReady(int orderIndex)
        {
            OnPizzaOrderReady?.Invoke(activeOrders[orderIndex].PizzaID);
        }
        public void Order(int pizzaID)
        {
            OnNewPizzaOrder?.Invoke(pizzaID);
            if (pizzaHolder.TryReservePizza(pizzaID))
            {
                activeOrders.Add(new OrderData(pizzaID, true));
                OrderReady(activeOrders.Count - 1);
                return;
            }
            activeOrders.Add(new OrderData(pizzaID));
        }

        public void CallBell()
        {
            for (int i = 0; i < activeOrders.Count; i++) 
            {
                if (activeOrders[i].OrderReady)
                {
                    OnPizzaOrderCompleted?.Invoke(activeOrders[i].PizzaID);
                    completedOrders.Add(activeOrders[i]);
                    activeOrders.RemoveAt(i);
                    return;
                }
            }
            OnBellCallCanceled?.Invoke();
        }
        public void TakeOrder(int orderID)
        {
            for (int i = 0; i < completedOrders.Count; i++)
            {
                if (completedOrders[i].PizzaID == orderID)
                {
                    pizzaHolder.RemoveReservedPizza(completedOrders[i].PizzaID);
                    completedOrders.RemoveAt(i);
                    OnOrderTaked?.Invoke(completedOrders[i].PizzaID);
                }
            }
        }
        bool TryGetOrderIndexOfPizza(int pizzaID, out int index, bool searchForReady = false)
        {
            for (index = 0; index < activeOrders.Count; index++) 
            {
                if (activeOrders[index].PizzaID == pizzaID && activeOrders[index].OrderReady == searchForReady) return true;
            }
            return false;
        }
        public int GetActiveOrdersAmount()
        {
            return activeOrders.Count;
        }
    }
}
