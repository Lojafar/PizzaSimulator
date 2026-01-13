using UnityEngine;

namespace Game.PizzeriaSimulator.Delivery.Config
{
    [CreateAssetMenu(fileName = "PizzeriaDeliveryConfig", menuName = "Configs/PizzeriaConfigs/DeliveryConfig")]
     public class PizzeriaDeliveryConfigSO : ScriptableObject
     {
        [field: SerializeField] public PizzeriaDeliveryConfig DeliveryConfig { get; private set; }
     }
}
