using UnityEngine;
namespace Game.PizzeriaSimulator.Pizzeria.Managment.Config
{
    [CreateAssetMenu(fileName = "PizzeriaManagmentConfig", menuName = "Configs/PizzeriaConfigs/PizzeriaManagment")]

    sealed class PizzeriaManagmentConfigSO : ScriptableObject
    {
        [field: SerializeField] public PizzeriaManagmentConfig ManagmentConfig {  get; private set; }
    }
}
