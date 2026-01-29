using Game.PizzeriaSimulator.Leevl.Reward;
using UnityEngine;
namespace Game.PizzeriaSimulator.Levels.Config
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "Configs/PizzeriaConfigs/LevelsConfig")]
    public class LevelsConfigSO : ScriptableObject
    {
        [field: SerializeField] public LevelsConfig LevelsConfig { get; private set; }
        public void AddRewardToConfig(LevelRewardType levelRewardType)
        {
            switch (levelRewardType)
            {
                case LevelRewardType.CustomersRate:
                    LevelsConfig.AddReward(new CustomersRateLevelReward());
                    break;
                case LevelRewardType.Money:
                    LevelsConfig.AddReward(new MoneyLevelReward());
                    break;
            }
        }
    }
}
