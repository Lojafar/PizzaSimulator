using UnityEngine;

namespace Game.PizzeriaSimulator.DayCycle.Config
{
    [CreateAssetMenu(fileName = "DayCycleConfig", menuName = "Configs/PizzeriaConfigs/DayCycle")]
    class DayCycleConfigSO : ScriptableObject
    {
        [field: SerializeField] public DayCycleConfig DayCycleConfig { get; private set; }
    }
}
