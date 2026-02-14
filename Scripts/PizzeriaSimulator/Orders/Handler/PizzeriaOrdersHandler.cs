using Game.Root.ServicesInterfaces;
using Game.PizzeriaSimulator.Interactions;
using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.Orders.Items;
using Game.PizzeriaSimulator.PizzaHold;
using Game.PizzeriaSimulator.SodaMachine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Game.PizzeriaSimulator.Orders.Handle
{
    struct OrderItemData
    {
        public readonly PizzeriaOrderItemType ItemType;
        public bool Completed;
        public OrderItemData(PizzeriaOrderItemType _itemType, bool _completed)
        {
            ItemType = _itemType;
            Completed = _completed;
        }
    }
    sealed class OrderData
    {
        public readonly int Id;
        public readonly OrderItemData[] OrderItems;
        public bool OrderReady;
        public OrderData(int _id, OrderItemData[] _orderItems, bool _orderReady = false)
        {
            Id = _id;
            OrderItems = _orderItems;
            OrderReady = _orderReady;
        }
    }
    public sealed class PizzeriaOrdersHandler : IInittable, ISceneDisposable
    {
        public int InitPriority => 9;
        public event Action<int, List<PizzeriaOrderItemType>> OnNewOrder;
        public event Action<int, int> OnOrderItemReady;
        public event Action<int> OnOrderReady;
        public event Action<int> OnOrderCompleted;
        public event Action<int> OnOrderTaked;
        public event Action<byte> OnBellCallCanceled;
        readonly PizzaHolder pizzaHolder;
        readonly Interactor interactor;
        PizzeriaSodaMachine sodaMachine;
        readonly List<OrderData> activeOrders;
        readonly List<OrderData> completedOrders;
        readonly HashSet<int> reservedOrdersIds;
        int lastOrderId = -1;

        public const byte NoOrdersBellCancellCode = 0;
        public const byte NotReadyBellCancellCode = 1;
        public PizzeriaOrdersHandler(PizzaHolder _pizzaHolder, Interactor _interactor)
        {
            pizzaHolder = _pizzaHolder;
            interactor = _interactor;
            activeOrders = new List<OrderData>();
            completedOrders = new List<OrderData>();
            reservedOrdersIds = new HashSet<int>();
        }
        public void Init()
        {
            interactor.OnInteract += HandleInteractor;
            pizzaHolder.OnPizzaAdded += HandleNewPizzaInHold;
        }
        public void SetSodaMachine(PizzeriaSodaMachine _sodaMachine)
        {
            sodaMachine = _sodaMachine;
            sodaMachine.OnCupFilled += HandleCupFilled;
        }
        public void Dispose()
        {
            interactor.OnInteract -= HandleInteractor;
            pizzaHolder.OnPizzaAdded -= HandleNewPizzaInHold;
            if(sodaMachine !=null) sodaMachine.OnCupFilled -= HandleCupFilled; 
        }
        void HandleInteractor(InteractableType interactableType)
        {
            if (interactableType == InteractableType.OrderBell)
            {
                CallBell();
            }
        }
        public void CallBell()
        {
            for (int i = 0; i < activeOrders.Count; i++)
            {
                if (activeOrders[i].OrderReady)
                {
                    completedOrders.Add(activeOrders[i]);
                    activeOrders.RemoveAt(i);
                    OnOrderCompleted?.Invoke(completedOrders[^1].Id);
                    return;
                }
            }
            OnBellCallCanceled?.Invoke(activeOrders.Count > 0 ?NotReadyBellCancellCode : NoOrdersBellCancellCode);
        }
        void HandleNewPizzaInHold(int pizzaID)
        {
            UnityEngine.Debug.Log("Pizza in hold");
            if (TryGetOrderAndItemIndexOfType(PizzaIdToOrderItemType(pizzaID), out int orderIndex, out int itemIndex) && pizzaHolder.TryReservePizza(pizzaID))
            {
                UnityEngine.Debug.Log("Pizza complete");
                CompleteItemInOrder(orderIndex, itemIndex);
            }
        }
        void HandleCupFilled(int slotId)
        {
            if (TryGetOrderAndItemIndexOfType(PizzeriaOrderItemType.Soda, out int orderIndex, out int itemIndex) && sodaMachine.TryReserveSodaCup())
            {
                CompleteItemInOrder(orderIndex, itemIndex);
            }
        }
        bool TryGetOrderAndItemIndexOfType(PizzeriaOrderItemType itemType, out int orderIndex, out int itemIndex, bool searchForCompleted = false)
        {
            itemIndex = 0;
            for (orderIndex = 0; orderIndex < activeOrders.Count; orderIndex++)
            {
                for (itemIndex = 0; itemIndex < activeOrders[orderIndex].OrderItems.Length; itemIndex++)
                {
                    if (activeOrders[orderIndex].OrderItems[itemIndex].ItemType == itemType
                 && activeOrders[orderIndex].OrderItems[itemIndex].Completed == searchForCompleted) return true;
                }
            }
            return false;
        }
        void CompleteItemInOrder(int orderIndex, int itemIndex)
        {
            activeOrders[orderIndex].OrderItems[itemIndex].Completed = true;
            OnOrderItemReady?.Invoke(activeOrders[orderIndex].Id, itemIndex);
            if (CheckOrderReady(orderIndex))
            {
                activeOrders[orderIndex].OrderReady = true;
                OnOrderReady?.Invoke(activeOrders[orderIndex].Id);
            }
        }
        bool CheckOrderReady(int orderIndex)
        {
            OrderData orderData = activeOrders[orderIndex];   
            for (int i = 0; i < orderData.OrderItems.Length; i++)
            {
                if (!orderData.OrderItems[i].Completed) return false;
            }
            return true;
        }
        public int ReserveOrderID()
        {
            lastOrderId++;
            reservedOrdersIds.Add(lastOrderId);
            return lastOrderId;
        }
        public int Order(List<PizzeriaOrderItemType> orderItems)
        {
            lastOrderId++;
            OrderWithOrderId(lastOrderId, orderItems);
            return lastOrderId;
        }
        public void OrderWithReservedId(int reservedOrderId, List<PizzeriaOrderItemType> orderItems)
        {
            if (!reservedOrdersIds.Contains(reservedOrderId)) return;
            reservedOrdersIds.Remove(reservedOrderId);
            OrderWithOrderId(reservedOrderId, orderItems);
        }
        void OrderWithOrderId(int orderId, List<PizzeriaOrderItemType> orderItems)
        {
            OrderData orderData = new(orderId, new OrderItemData[orderItems.Count], false);
            for (int i = 0; i < orderItems.Count; i++)
            {
                orderData.OrderItems[i] = new OrderItemData(orderItems[i], TryCompleteItem(orderItems[i]));
            }
            activeOrders.Add(orderData);
            OnNewOrder?.Invoke(lastOrderId, orderItems);
            HandleLastOrder().Forget();
        }
        bool TryCompleteItem(PizzeriaOrderItemType itemType)
        {
            return itemType switch
            {
                PizzeriaOrderItemType.Soda => sodaMachine != null && sodaMachine.TryReserveSodaCup(),
                _ => pizzaHolder.TryReservePizza(OrderItemTypeToPizzaId(itemType))
            };
        }
        async UniTaskVoid HandleLastOrder()
        {
            OrderData orderData = activeOrders[^1];
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            int completedItems = 0;
            for (int i = 0; i < orderData.OrderItems.Length; i++)
            {
                if (orderData.OrderItems[i].Completed)
                {
                    completedItems++;
                    OnOrderItemReady?.Invoke(orderData.Id, i);
                }
            }
            if (completedItems == orderData.OrderItems.Length)
            {
                orderData.OrderReady = true;
                OnOrderReady?.Invoke(orderData.Id);
            }
        }
        public void TakeOrder(int orderID)
        {
            OrderData orderData = null;
            int orderIndex;
            for(orderIndex = 0; orderIndex < completedOrders.Count; orderIndex++)
            {
                if (completedOrders[orderIndex].Id == orderID) 
                {
                    orderData = completedOrders[orderIndex];
                    break;
                }
            }
            if (orderData == null) return;
            for (int i = 0; i < orderData.OrderItems.Length; i++)
            {
                TakeOrderItem(orderData.OrderItems[i].ItemType);
            }

            completedOrders.RemoveAt(orderIndex);
            OnOrderTaked?.Invoke(orderID);
        }
        void TakeOrderItem(PizzeriaOrderItemType itemType)
        {
            switch (itemType)
            {
                case PizzeriaOrderItemType.Soda:
                    sodaMachine?.RemoveReservedCup();
                    break;
                default:
                    pizzaHolder.RemoveReservedPizza(OrderItemTypeToPizzaId(itemType));
                    break;
            }
        }
        public static PizzeriaOrderItemType PizzaIdToOrderItemType(int pizzaID)
        {
            return pizzaID switch
            {
                0 => PizzeriaOrderItemType.CheesePizza,
                1 => PizzeriaOrderItemType.Pepperoni,
                2 => PizzeriaOrderItemType.TomatoPepperoni,
                3 => PizzeriaOrderItemType.ShrimpPizza,
                4 => PizzeriaOrderItemType.VillagePizza,
                5 => PizzeriaOrderItemType.VegeterianPizza,
                _ => PizzeriaOrderItemType.CheesePizza
            };
        }
        public static int OrderItemTypeToPizzaId(PizzeriaOrderItemType itemType)
        {
            return itemType switch
            {
                PizzeriaOrderItemType.CheesePizza => 0,
                PizzeriaOrderItemType.Pepperoni => 1,
                PizzeriaOrderItemType.TomatoPepperoni => 2,
                PizzeriaOrderItemType.ShrimpPizza => 3,
                PizzeriaOrderItemType.VillagePizza => 4,
                PizzeriaOrderItemType.VegeterianPizza => 5,
                _ => 0
            };
        }
    }
}