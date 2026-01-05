using UnityEngine;

namespace Game.PizzeriaSimulator.Customers.SettingsConfig
{
    [CreateAssetMenu(fileName = "CustomersSettingsConfig", menuName = "Configs/PizzeriaConfigs/CustomersSettingsConfig")]
    class CustomersSettingsConfigSO : ScriptableObject
    {
        [field: SerializeField] public CustomersSettingsConfig CustomersSettingsConfig { get; private set; }
    }
}
