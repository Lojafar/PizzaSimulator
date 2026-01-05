using UnityEngine;

namespace Game.PizzeriaSimulator.PizzasConfig
{
    [CreateAssetMenu(fileName = "AllPizzaConfig", menuName = "Configs/PizzeriaConfigs/AllPizzaConfig")]
    class AllPizzaConfigSO : ScriptableObject
    {
        [field: SerializeField] public AllPizzaConfig AllPizzaConfig { get; private set; }
    }
}
