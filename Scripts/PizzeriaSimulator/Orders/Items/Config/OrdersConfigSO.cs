using UnityEngine;

namespace Game.PizzeriaSimulator.Orders.Items.Config
{
    [CreateAssetMenu(fileName = "PizzeriaOrdersConfig", menuName = "Configs/PizzeriaConfigs/OrdersConfig")]
    sealed class OrdersConfigSO : ScriptableObject
    {
        [field: SerializeField] public OrdersConfig OrdersConfig { get; private set; }
    }
}
