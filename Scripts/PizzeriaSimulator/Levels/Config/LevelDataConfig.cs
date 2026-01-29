using System;
using System.Collections.Generic;
using UnityEngine;
namespace Game.PizzeriaSimulator.Levels.Config
{
    [Serializable]
    public class LevelDataConfig
    {
        [SerializeField] int[] rewardsId;
        public IReadOnlyList<int> RewardsId => rewardsId;
    }
}
