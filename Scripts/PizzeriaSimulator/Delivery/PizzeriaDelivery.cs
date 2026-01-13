using Game.PizzeriaSimulator.Delivery.Config;
using UnityEngine;
namespace Game.PizzeriaSimulator.Delivery
{
    public class PizzeriaDelivery
    {
        readonly PizzeriaDeliveryConfig deliveryConfig;
        readonly PizzeriaSceneReferences sceneReferences;
        public PizzeriaDelivery(PizzeriaDeliveryConfig _deliveryConfig, PizzeriaSceneReferences _sceneReferences)
        {
            deliveryConfig = _deliveryConfig;
            sceneReferences = _sceneReferences;
        }
        public void Order(int itemID)
        {
            if(deliveryConfig.GetDeliveryItemConfig(itemID) is DeliveryItemConfig order)
            {
                Object.Instantiate(order.ItemPrefab, sceneReferences.DeliveryPoint.position, order.ItemPrefab.transform.rotation);
            }
        }
        public void Order(int itemID, int amount)
        {
            if (deliveryConfig.GetDeliveryItemConfig(itemID) is DeliveryItemConfig order)
            {
                for(int i = 0; i < amount; i++)
                {
                    Object.Instantiate(order.ItemPrefab, sceneReferences.DeliveryPoint.position, order.ItemPrefab.transform.rotation);
                }
            }
        }
    }
}
