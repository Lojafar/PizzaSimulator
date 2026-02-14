using Game.Root.ServicesInterfaces;
using Game.PizzeriaSimulator.PizzaCreation;
using Game.PizzeriaSimulator.PizzaCreation.Config;
using Game.PizzeriaSimulator.PizzasConfig;
using Game.PizzeriaSimulator.Orders.Items;
using Game.PizzeriaSimulator.Orders.Items.Config;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PizzeriaSimulator.Orders.Handle.Visual
{
    using DeviceType = Game.Root.User.Environment.DeviceType;
    public class OrderVisualData
    {
        public readonly int OrderId;
        public readonly Sprite[] OrderItemsIcons;
        public OrderVisualData(int _orderId, Sprite[] _orderItemsIcons)
        {
            OrderId = _orderId;
            OrderItemsIcons = _orderItemsIcons;
        }
    }
    struct OrderItemVMData
    {
        public PizzeriaOrderItemType ItemType;
        public byte ExtraData;
        public OrderItemVMData(PizzeriaOrderItemType _itemType, byte _extraData = 0)
        {
            ItemType = _itemType;
            ExtraData = _extraData;
        }
    }
    class OrderVMData
    {
        public readonly int OrderID;
        public readonly OrderItemVMData[] Items;
        public int CurrentItemIndex;
        public OrderVMData(int _orderId, OrderItemVMData[] _items, int _currentIndicatsItem)
        {
            OrderID = _orderId;
            Items = _items;
            CurrentItemIndex = _currentIndicatsItem;
        }
    }
    public class PizzeriaOrderHandlVM : ISceneDisposable
    {
        public event Action<OrderVisualData> AddNewOrder;
        public event Action<int, Sprite> SetOrderIndicatorIcon;
        public event Action<int, Sprite[]> SetOrderIndicatorsIcons;
        public event Action<int, int> SetIndicDoneInOrder;
        public event Action<int, int> SetItemReadyInOrder;
        public event Action<int> ClearIndicsDonesInOrder;
        public event Action<int> SetOrderAsReady;
        public event Action<int> CompleteOrder;
        public event Action<string> ShowBellCancel;
        public readonly DeviceType DeviceType;

        readonly PizzeriaOrdersHandler ordersHandler;
        readonly PizzaCreator pizzaCreator;
        readonly OrdersConfig ordersConfig;
        readonly PizzaCreatorConfig pizzaCreatorConfig;
        readonly int pizzaBaseIngredientAmount;
        readonly Dictionary<int, OrderVMData> OrderIDToData;
        int notReadyOrdersAmount;

        const byte startItemExtra = 0;
        const byte pizzaInBakeExtra = 1;
        const byte pizzaInCutExtra = 2;
        const byte completedItemExtra = byte.MaxValue;
        public PizzeriaOrderHandlVM(PizzeriaOrdersHandler _ordersHandler, PizzaCreator _pizzaCreator, OrdersConfig _ordersConfig, DeviceType _deviceType)
        {
            ordersHandler = _ordersHandler;
            pizzaCreator = _pizzaCreator;
            ordersConfig = _ordersConfig;
            DeviceType = _deviceType;
            pizzaCreatorConfig = pizzaCreator.GetPizzaCreatorConfig();
            pizzaBaseIngredientAmount = pizzaCreatorConfig.IngredientsForBase.Count;
            OrderIDToData = new Dictionary<int, OrderVMData>(); // construct with size of max orders amount
        }
        public void Init()
        {
            ordersHandler.OnNewOrder += HandleNewOrder;
            ordersHandler.OnOrderItemReady += HandleOrderItemReady;
            ordersHandler.OnOrderReady += HandleOrderReady;
            ordersHandler.OnOrderCompleted += HandleOrderComplete;
            ordersHandler.OnBellCallCanceled += HandleBellCancell;
            pizzaCreator.OnIngredientSetted += HandleCreatorIngredientSet;
            pizzaCreator.OnPizzaCleared += ClearGatheringPizzasIndicsDones;
            pizzaCreator.OnPizzaBake += HandlePizzaBake;
            pizzaCreator.OnPizzaBaked += HandlePizzaInCut;
        }
        public void Dispose()
        {
            ordersHandler.OnNewOrder -= HandleNewOrder;
            ordersHandler.OnOrderItemReady -= HandleOrderItemReady;
            ordersHandler.OnOrderReady -= HandleOrderReady;
            ordersHandler.OnOrderCompleted -= HandleOrderComplete;
            ordersHandler.OnBellCallCanceled -= HandleBellCancell;
            pizzaCreator.OnIngredientSetted -= HandleCreatorIngredientSet;
            pizzaCreator.OnPizzaCleared -= ClearGatheringPizzasIndicsDones;
            pizzaCreator.OnPizzaBake -= HandlePizzaBake;
            pizzaCreator.OnPizzaBaked -= HandlePizzaInCut;
        }
        void HandleNewOrder(int orderID, List<PizzeriaOrderItemType> items)
        {
            if (OrderIDToData.ContainsKey(orderID)) return;

            notReadyOrdersAmount++;
            OrderItemVMData[] orderItemsVMData = new OrderItemVMData[items.Count];
            OrderVMData orderVMData = new(orderID, orderItemsVMData, 0);
            OrderIDToData.Add(orderID, orderVMData);

            Sprite[] orderItemsIcons = new Sprite[items.Count];

            for (int i = 0; i < items.Count; i++)
            {
                orderItemsVMData[i] = new OrderItemVMData(items[i], GetExtraForItem(items[i]));
                if (ordersConfig.GetOrderItemByType(items[i]) is OrderItemConfig itemConfig)
                {
                    orderItemsIcons[i] = itemConfig.Icon;
                }
            }
            AddNewOrder?.Invoke(new OrderVisualData(orderID, orderItemsIcons));
            UpdateIndicatorsInOrder(orderVMData);
        }
        byte GetExtraForItem(PizzeriaOrderItemType itemType)
        {
            if (IsPizzaOrderItem(itemType))
            {
                int pizzaId = PizzeriaOrdersHandler.OrderItemTypeToPizzaId(itemType); 
                if (pizzaCreator.CurrentPizzaInCut == pizzaId && !HasItemOfTypeAndExtra(itemType, pizzaInCutExtra))
                {
                    return pizzaInCutExtra;
                }
                else if (HasNotHandledPizzaInBake(itemType, pizzaId))
                {
                    return pizzaInBakeExtra;
                }
            }
            return startItemExtra;
        }
        bool HasItemOfTypeAndExtra(PizzeriaOrderItemType targetType, byte targetExtra)
        {
            int i;
            foreach (OrderVMData order in OrderIDToData.Values)
            {
                for (i = 0; i < order.Items.Length; i++)
                {
                    if (order.Items[i].ItemType == targetType && order.Items[i].ExtraData == targetExtra) return true;
                }
            }
            return false;
        }
        bool HasNotHandledPizzaInBake(PizzeriaOrderItemType targetType, int pizzaId)
        {
            int pizzasInBake = pizzaCreator.GetPizzasInBakeOfID(pizzaId);
            if (pizzasInBake < 1) return false; 
            int i;
            foreach (OrderVMData order in OrderIDToData.Values)
            {
                for (i = 0; i < order.Items.Length; i++)
                {
                    if (order.Items[i].ItemType == targetType && order.Items[i].ExtraData == pizzaInBakeExtra)
                    {
                        pizzasInBake--;
                        if (pizzasInBake < 1) return false;
                    }
                }
            }
            return pizzasInBake > 0;
        }
        void UpdateIndicatorsInOrder(OrderVMData orderVMData)
        {
            OrderItemVMData orderItem = orderVMData.Items[orderVMData.CurrentItemIndex];
            if (IsPizzaOrderItem(orderItem.ItemType))
            {
                if (orderItem.ExtraData == startItemExtra)
                {
                    PizzaConfig pizzaConfig = pizzaCreator.GetPizzaConfigById(PizzeriaOrdersHandler.OrderItemTypeToPizzaId(orderItem.ItemType));
                    if (pizzaConfig == null) return;

                    Sprite[] indicatorsSprites = new Sprite[pizzaBaseIngredientAmount + pizzaConfig.Ingredients.Count];
                    bool[] ingredientsCompletes = new bool[indicatorsSprites.Length];
                    int i;
                    for (i = 0; i < pizzaBaseIngredientAmount; i++)
                    {
                        indicatorsSprites[i] = pizzaCreatorConfig.GetIngredientConfigByType(pizzaCreatorConfig.IngredientsForBase[i]).IngredientIcon; // init inside pizzaCreatorConfig
                        ingredientsCompletes[i] = pizzaCreator.IsIngredientPlaced(pizzaCreatorConfig.IngredientsForBase[i]);
                    }
                    for (i = 0; i < pizzaConfig.Ingredients.Count; i++)
                    {
                        indicatorsSprites[i + pizzaBaseIngredientAmount] = pizzaCreatorConfig.GetIngredientConfigByType(pizzaConfig.Ingredients[i]).IngredientIcon;
                        ingredientsCompletes[i + pizzaBaseIngredientAmount] = pizzaCreator.IsIngredientPlaced(pizzaConfig.Ingredients[i]);
                    }
                    SetOrderIndicatorsIcons?.Invoke(orderVMData.OrderID, indicatorsSprites);
                    for (i = 0; i < ingredientsCompletes.Length; i++)
                    {
                        if (ingredientsCompletes[i])
                            SetIndicDoneInOrder?.Invoke(orderVMData.OrderID, i);
                    }
                    return;
                }
                else
                {
                    SetOrderIndicatorIcon?.Invoke(orderVMData.OrderID, orderItem.ExtraData == pizzaInBakeExtra ? ordersConfig.PizzaBakeIndicator : ordersConfig.PizzaCutIndicator);
                }
            }
        }
        void HandleOrderItemReady(int orderId, int itemId)
        {
            if (OrderIDToData.TryGetValue(orderId, out OrderVMData orderData))
            {
                if (orderData.Items.Length <= itemId) return;
                orderData.Items[itemId].ExtraData = completedItemExtra;
                if (orderData.CurrentItemIndex == itemId)
                {
                    for (int i = itemId + 1; i < orderData.Items.Length; i++)
                    {
                        if (orderData.Items[i].ExtraData != completedItemExtra)
                        {
                            orderData.CurrentItemIndex = i;
                            break;
                        }
                    }
                }
                SetItemReadyInOrder?.Invoke(orderId, itemId);
                UpdateIndicatorsInOrder(orderData);
            }
        }
        void HandleOrderReady(int orderID)
        {
            if (OrderIDToData.ContainsKey(orderID))
            {
                notReadyOrdersAmount--;
                SetOrderIndicatorIcon?.Invoke(orderID, ordersConfig.OrderReadyIndicator);
                SetOrderAsReady?.Invoke(orderID);
            }
        }
        void HandleOrderComplete(int orderID)
        {
            if (OrderIDToData.ContainsKey(orderID))
            {
                OrderIDToData.Remove(orderID);
                CompleteOrder?.Invoke(orderID);
            }
        }
        void HandleBellCancell(byte cancellCode)
        {
            if (cancellCode == PizzeriaOrdersHandler.NotReadyBellCancellCode)
            {
                ShowBellCancel?.Invoke("The required orders isn't ready.");
            }
            else
            {
                ShowBellCancel?.Invoke("No orders.");
            }
        }
        void HandleCreatorIngredientSet(PizzaIngredientType ingredientType)
        {
            if (notReadyOrdersAmount < 1) return;
            if (pizzaCreatorConfig.IsIngredientOfBaseAndGetIndex(ingredientType, out int baseIngredientIndex))
            {
                foreach (OrderVMData order in OrderIDToData.Values)
                {
                    if (order.Items[order.CurrentItemIndex].ExtraData == startItemExtra && IsPizzaOrderItem(order.Items[order.CurrentItemIndex].ItemType))
                    {
                        SetIndicDoneInOrder?.Invoke(order.OrderID, baseIngredientIndex);
                    }
                }
                return;
            }
            foreach (OrderVMData order in OrderIDToData.Values)
            {
                if (order.Items[order.CurrentItemIndex].ExtraData == startItemExtra && IsPizzaOrderItem(order.Items[order.CurrentItemIndex].ItemType))
                {
                    int ingredientIndex = pizzaCreator.GetPizzaConfigById(PizzeriaOrdersHandler.OrderItemTypeToPizzaId(order.Items[order.CurrentItemIndex].ItemType))
                        .GetIndexOfIngredient(ingredientType);
                    if (ingredientIndex > -1)
                    {
                        SetIndicDoneInOrder?.Invoke(order.OrderID, ingredientIndex + pizzaBaseIngredientAmount);
                    }
                }
            }
        }
        void ClearGatheringPizzasIndicsDones()
        {
            if (notReadyOrdersAmount < 1) return;
            foreach (OrderVMData order in OrderIDToData.Values)
            {
                if (order.Items[order.CurrentItemIndex].ExtraData == startItemExtra && IsPizzaOrderItem(order.Items[order.CurrentItemIndex].ItemType))
                {
                    ClearIndicsDonesInOrder?.Invoke(order.OrderID);
                }
            }
        }
        void HandlePizzaBake(int pizzaId)
        {
            if (notReadyOrdersAmount < 1) return;
            PizzeriaOrderItemType pizzaItemType = PizzeriaOrdersHandler.PizzaIdToOrderItemType(pizzaId);
            UpdateFindedItemExtra(pizzaItemType, startItemExtra, pizzaInBakeExtra);
            ClearGatheringPizzasIndicsDones();
        }
        void HandlePizzaInCut(int pizzaId)
        {
            if (notReadyOrdersAmount < 1) return;
            PizzeriaOrderItemType pizzaItemType = PizzeriaOrdersHandler.PizzaIdToOrderItemType(pizzaId);
            UpdateFindedItemExtra(pizzaItemType, pizzaInBakeExtra, pizzaInCutExtra);
        }
        void UpdateFindedItemExtra(PizzeriaOrderItemType itemTypeToFind, byte extraToFind, byte extraToSet)
        {
            bool itemFinded = false;
            int i;
            foreach (OrderVMData order in OrderIDToData.Values)
            {
                for (i = 0; i < order.Items.Length; i++)
                {
                    if (order.Items[i].ItemType == itemTypeToFind && order.Items[i].ExtraData == extraToFind)
                    {
                        order.Items[i].ExtraData = extraToSet;
                        if (i == order.CurrentItemIndex)
                        {
                            UpdateIndicatorsInOrder(order);
                        }
                        itemFinded = true;
                        break;
                    }
                }
                if (itemFinded)
                {
                    break;
                }
            }
        }
        bool IsPizzaOrderItem(PizzeriaOrderItemType orderItemType)
        {
            return orderItemType != PizzeriaOrderItemType.Soda;
        }
    }
}
