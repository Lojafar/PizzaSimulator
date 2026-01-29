using Game.Helps.Attributes;
using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Leevl.Reward
{
    [Serializable]
    sealed class CustomersRateLevelReward : LevelRewardBase
    {
        [field: SerializeField, ReadOnly] public override LevelRewardType RewardType { get; protected set; } = LevelRewardType.CustomersRate;
        [field: SerializeField] public float SpawnTimeReduce { get; private set; }
    }
}