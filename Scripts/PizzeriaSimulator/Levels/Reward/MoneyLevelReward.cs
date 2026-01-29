using Game.Helps.Attributes;
using Game.PizzeriaSimulator.Currency;
using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Leevl.Reward
{
    [Serializable]
    sealed class MoneyLevelReward : LevelRewardBase
    {
        [field: SerializeField, ReadOnly] public override LevelRewardType RewardType { get; protected set; } = LevelRewardType.Money;
        [field: SerializeField] public MoneyQuantity RewardMoney { get; private set; }
    }
}
