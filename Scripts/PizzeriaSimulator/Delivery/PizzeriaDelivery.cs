using Game.PizzeriaSimulator.Delivery.Config;
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
        public event Action<float> OnDeliveryTimeChanged;
        readonly PizzeriaDeliveryConfig deliveryConfig;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly Dictionary<int, int> activeOrdersDict;
        float remainedDeliveryTime = -1;
        const int averageOrdersCount = 5;
        public PizzeriaDelivery(PizzeriaDeliveryConfig _deliveryConfig, PizzeriaSceneReferences _sceneReferences)
        {
            deliveryConfig = _deliveryConfig;
            sceneReferences = _sceneReferences;
            activeOrdersDict = new Dictionary<int, int>(averageOrdersCount);
        }
        public void Dispose()
        {
            Ticks.Instance.OnTick -= UpdateOnWaitingOrder;
        }
        public void Order(int itemID, int amount = 1)
        {
            CheckForStartDelivery();
            if (activeOrdersDict.TryGetValue(itemID, out int activeAmount)) amount += activeAmount;
            activeOrdersDict[itemID] = amount;
        }
        void CheckForStartDelivery()
        {
            if (remainedDeliveryTime < 0)
            {
                remainedDeliveryTime = deliveryConfig.DeliveryDuration;
                OnDeliveryStarted?.Invoke();
                OnDeliveryTimeChanged?.Invoke(remainedDeliveryTime);
                Ticks.Instance.OnTick += UpdateOnWaitingOrder;
            }
        }
        void UpdateOnWaitingOrder()
        {
            remainedDeliveryTime -= Time.deltaTime;
            OnDeliveryTimeChanged?.Invoke(remainedDeliveryTime);
            if (remainedDeliveryTime < 0)
            {
                OnOrdersDelivered();
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
                        Object.Instantiate(order.ItemPrefab, sceneReferences.DeliveryPoint.position, order.ItemPrefab.transform.rotation);
                    }
                }
            }
            activeOrdersDict.Clear();
            OnDeliveryEnded?.Invoke();
        }
    }
}
