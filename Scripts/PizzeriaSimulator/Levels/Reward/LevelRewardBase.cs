using Game.Helps.Attributes;
using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Leevl.Reward
{
    [Serializable]
    public abstract class LevelRewardBase
    {
        public abstract LevelRewardType RewardType { get; protected set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}
