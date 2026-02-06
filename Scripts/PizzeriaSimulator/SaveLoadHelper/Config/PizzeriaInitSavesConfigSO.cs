using UnityEngine;

namespace Game.PizzeriaSimulator.SaveLoadHelp.Config
{
    [CreateAssetMenu(fileName = "PizzeriaInitSavesConfig", menuName = "Configs/PizzeriaConfigs/InitSavesConfig")]
    class PizzeriaInitSavesConfigSO : ScriptableObject
    {
        [field: SerializeField] public PizzeriaInitSavesConfig InitSavesConfig { get; private set; }
    }
}
