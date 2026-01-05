using UnityEngine;

namespace Game.PizzeriaSimulator.Customers.Skin.Config
{
    [CreateAssetMenu(fileName = "CustomerSkinsConfig", menuName = "Configs/PizzeriaConfigs/CustomerSkinsConfig")]
    class CustomerSkinsConfigSO : ScriptableObject
    {
        [field: SerializeField] public CustomerSkinsConfig CustomerSkinsConfig { get; private set; }
    }
}
