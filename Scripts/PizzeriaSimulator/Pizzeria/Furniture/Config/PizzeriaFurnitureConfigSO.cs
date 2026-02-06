using UnityEngine;
namespace Game.PizzeriaSimulator.Pizzeria.Furniture.Config
{
    [CreateAssetMenu(fileName = "PizzeriaFurnitureConfig", menuName = "Configs/PizzeriaConfigs/FurnitureConfig")]
    sealed class PizzeriaFurnitureConfigSO : ScriptableObject
    {
        [field : SerializeField ] public PizzeriaFurnitureConfig FurnitureConfig { get; private set; }
    }
}
