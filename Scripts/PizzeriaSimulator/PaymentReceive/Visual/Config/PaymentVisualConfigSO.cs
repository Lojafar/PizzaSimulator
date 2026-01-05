using UnityEngine;
namespace Game.PizzeriaSimulator.PaymentReceive.Visual.Config
{
    [CreateAssetMenu(fileName = "PaymentVisualConfig", menuName = "Configs/PizzeriaConfigs/PaymentVisualConfig")]
    class PaymentVisualConfigSO : ScriptableObject
    {
        [field: SerializeField] public PaymentVisualConfig PaymentVisualConfig { get; private set; }
    }
}
