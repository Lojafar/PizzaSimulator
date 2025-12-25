using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.Config
{
    [CreateAssetMenu(fileName = "PizzaCreatorConfig", menuName = "Configs/PizzeriaConfigs/PizzaCreatorConfig")]
    class PizzaCreatorConfigSO : ScriptableObject
    {
        [field: SerializeField] public PizzaCreatorConfig PizzaCreatorConfig { get; private set; }
    }
}
