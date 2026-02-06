using Game.PizzeriaSimulator.Leevl.Reward;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Game.PizzeriaSimulator.Levels.Config
{
    [Serializable]
    public class LevelsConfig
    {
        [field: SerializeField] public int FirstLevel { get; private set; }
        [field: SerializeField] public int MaxLevel { get; private set; }
        [field: SerializeField] public int XPForCustomerOrder { get; private set; }
        [field: SerializeField] public AnimationCurve XPForLevelCurve { get; private set; }
        [SerializeReference] List<LevelRewardBase> rewards;
        [SerializeField] LevelDataConfig[] levelsData;
        public void AddReward(LevelRewardBase levelRewardBase)
        {
            if(levelRewardBase != null) rewards.Add(levelRewardBase);
        }
        public LevelDataConfig GetLevelData(int levelNum)
        {
            levelNum--;
            if (levelNum < 0 || levelNum >= levelsData.Length) return null;
            return levelsData[levelNum];
        }
        public LevelRewardBase GetReward(int rewardID)
        {
            if (rewardID < 0 || rewardID >= rewards.Count) return null;
            return rewards[rewardID];
        }
    }
}
