using Game.PizzeriaSimulator.Boxes.Manager;
using Game.PizzeriaSimulator.Delivery.Config;
using Game.PizzeriaSimulator.Pizzeria.Managment;
using Game.PizzeriaSimulator.Wallet;
using Game.Root.ServicesInterfaces;
using Game.Root.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Game.PizzeriaSimulator.Delivery
{
    using Object = UnityEngine.Object;
    public class PizzeriaDelivery : ISceneDisposable
    {
        public event Action OnDeliveryStarted;
        public event Action OnDeliveryEnded;
        public event Action OnGemsSkipFailed;
        public event Action<int> OnGemsSkipPriceChanged;
        public event Action<float> OnDeliveryTimeChanged;
        public event Action<float> OnDeliveryTimeAdded;
        readonly PizzeriaDeliveryConfig deliveryConfig;
        readonly BoxesManager boxesManager;
        readonly PizzeriaManager pizzeriaManager;
        readonly PlayerWallet playerWallet;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly int gemsChangeStepDur;
        readonly Dictionary<int, int> activeOrdersDict;
        float remainedDeliveryTime = -1;
        int lastGemsForSkipRawValue;
        int currentGemsSkipCost;
        const int averageOrdersCount = 5;
        public PizzeriaDelivery(PizzeriaDeliveryConfig _deliveryConfig, BoxesManager _boxesManager, PizzeriaManager _pizzeriaManager, PlayerWallet _playerWallet, PizzeriaSceneReferences _sceneReferences)
        {
            deliveryConfig = _deliveryConfig;
            boxesManager = _boxesManager;
            pizzeriaManager = _pizzeriaManager;
            playerWallet = _playerWallet;
            sceneReferences = _sceneReferences;
            gemsChangeStepDur = deliveryConfig.SkipStepDuration;
            activeOrdersDict = new Dictionary<int, int>(averageOrdersCount);
        }
        public void Dispose()
        {
            Ticks.Instance.OnTick -= UpdateOnWaitingOrder;
        }
        public void Order(int itemID, int amount = 1)
        {
            int orderAmount = amount;
            if (activeOrdersDict.TryGetValue(itemID, out int activeAmount)) amount += activeAmount;
            activeOrdersDict[itemID] = amount;
            StartDelivery(orderAmount);
        }
        public void Order(Dictionary<int, int> items)
        {
            if (items == null || items.Count == 0) return;
            int itemsAmount = 0;
            foreach (KeyValuePair<int, int> item in items) 
            {
                if (activeOrdersDict.ContainsKey(item.Key))  activeOrdersDict[item.Key] += item.Value;
                else activeOrdersDict.Add(item.Key, item.Value);
            }
            StartDelivery(itemsAmount);
        }
        public void SkipDeliveryForGems()
        {
            if (playerWallet.TryTakeGems(currentGemsSkipCost))
            {
                OnOrdersDelivered();
            }
            else
            {
                OnGemsSkipFailed?.Invoke(); 
            }
        }
        public void SkipDeliveryForAdv()
        {
            OnOrdersDelivered();
        }
        void StartDelivery(int itemsAmount)
        {
            if (remainedDeliveryTime < 0)
            {
                remainedDeliveryTime = Mathf.Clamp(deliveryConfig.MinDeliveryTime +  itemsAmount * 
                    deliveryConfig.DeliveryTimeСoeffCurve.Evaluate(pizzeriaManager.CurrentLevel.CurrentValue),
                    deliveryConfig.MinDeliveryTime, deliveryConfig.MaxDeliveryTime);

                lastGemsForSkipRawValue = -1;
                OnDeliveryStarted?.Invoke();
                OnDeliveryTimeChanged?.Invoke(remainedDeliveryTime);
                UpdateGemsSkipCost();
                Ticks.Instance.OnTick += UpdateOnWaitingOrder;
            }
            else 
            {
                float timeAdd = itemsAmount * deliveryConfig.DeliveryTimeСoeffCurve.Evaluate(pizzeriaManager.CurrentLevel.CurrentValue);
                if (timeAdd > deliveryConfig.MaxDeliveryTime - remainedDeliveryTime) timeAdd = deliveryConfig.MaxDeliveryTime - remainedDeliveryTime;
                remainedDeliveryTime += timeAdd;
                OnDeliveryTimeAdded?.Invoke(timeAdd);
            }
        }
        void UpdateOnWaitingOrder()
        {
            remainedDeliveryTime -= Time.deltaTime;
            OnDeliveryTimeChanged?.Invoke(remainedDeliveryTime);
            UpdateGemsSkipCost();
            if (remainedDeliveryTime < 0)
            {
                OnOrdersDelivered();
            }
        }
        void UpdateGemsSkipCost()
        {
            if(Mathf.CeilToInt(remainedDeliveryTime / gemsChangeStepDur) != lastGemsForSkipRawValue)
            {
                lastGemsForSkipRawValue = Mathf.CeilToInt(remainedDeliveryTime / gemsChangeStepDur);
                currentGemsSkipCost = lastGemsForSkipRawValue * deliveryConfig.SkipStepGemsChange;
                OnGemsSkipPriceChanged?.Invoke(currentGemsSkipCost);
            }
        }
        void OnOrdersDelivered()
        {
            Ticks.Instance.OnTick -= UpdateOnWaitingOrder;
            remainedDeliveryTime = -1;
            int i = 0;
            foreach(KeyValuePair<int, int> orderData in activeOrdersDict)
            {
                if (deliveryConfig.GetDeliveryItemConfig(orderData.Key) is DeliveryItemConfig order)
                {
                    for (i = 0; i < orderData.Value; i++)
                    {
                        boxesManager.AddNewBox(Object.Instantiate(order.BoxPrefab, sceneReferences.DeliveryPoint.position, order.BoxPrefab.transform.rotation));
                    }
                }
            }
            boxesManager.FinishAddBoxes();
            activeOrdersDict.Clear();
            OnDeliveryEnded?.Invoke();
        }
    }
}
