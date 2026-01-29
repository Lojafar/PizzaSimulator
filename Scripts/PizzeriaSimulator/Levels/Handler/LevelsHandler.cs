using Game.PizzeriaSimulator.Customers.Manager;
using Game.PizzeriaSimulator.Leevl.Reward;
using Game.PizzeriaSimulator.Levels.Config;
using Game.PizzeriaSimulator.Pizzeria.Manager;
using Game.PizzeriaSimulator.Wallet;
using Game.Root.ServicesInterfaces;
using R3;
using System;
using Zenject;

namespace Game.PizzeriaSimulator.Levels.Handler
{
    class LevelsHandler : IInittable, ISceneDisposable
    {
        public int InitPriority => 11;
        readonly PizzeriaManager pizzeriaManager;
        readonly LevelsConfig levelsConfig;
        readonly DiContainer diContainer;
        PlayerWallet playerWallet;
        CustomersManager customersManager;
        IDisposable levelSubscription;
        public LevelsHandler(PizzeriaManager _pizzeriaManager, LevelsConfig _levelsConfig, DiContainer _diContainer)
        {
            pizzeriaManager = _pizzeriaManager;
            levelsConfig = _levelsConfig; 
            diContainer = _diContainer;
        }
        public void Init()
        {
            playerWallet = diContainer.TryResolve<PlayerWallet>();
            customersManager = diContainer.TryResolve<CustomersManager>();
            levelSubscription = pizzeriaManager.CurrentLevel.Skip(1).Subscribe(HandleNewLevel);
            for (int i = pizzeriaManager.FirstLevel; i <= pizzeriaManager.CurrentLevel.CurrentValue; i++)
            {
                HandleLevelFromInit(i);
            }
        }
        void HandleLevelFromInit(int level)
        {
            float customersSpawnReduce = 0;
            if (levelsConfig.GetLevelData(level) is LevelDataConfig levelDataConfig)
            {
                for (int i = 0; i < levelDataConfig.RewardsId.Count; i++)
                {
                    if (levelsConfig.GetReward(levelDataConfig.RewardsId[i]) is LevelRewardBase levelRewardBase)
                    {
                        switch (levelRewardBase.RewardType)
                        {
                            case LevelRewardType.CustomersRate:
                                if (levelRewardBase is CustomersRateLevelReward customersRateReward)
                                {
                                    customersSpawnReduce += customersRateReward.SpawnTimeReduce;
                                }
                                break;
                        }

                    }
                }
            }
            if (customersSpawnReduce > 0) 
            {
                customersManager.ReduceSpawnTime(customersSpawnReduce);
            }
        }
        public void Dispose()
        {
            levelSubscription.Dispose();
        }
        void HandleNewLevel(int level)
        {
            if (levelsConfig.GetLevelData(level) is LevelDataConfig levelDataConfig)
            {
                for (int i = 0; i < levelDataConfig.RewardsId.Count; i++)
                {
                    if (levelsConfig.GetReward(levelDataConfig.RewardsId[i]) is LevelRewardBase levelRewardBase)
                    {
                        HandleReward(levelRewardBase);
                    }
                }
            }
        }
        void HandleReward(LevelRewardBase levelRewardBase)
        {
            switch (levelRewardBase.RewardType)
            {
                case LevelRewardType.CustomersRate:
                    if(levelRewardBase is CustomersRateLevelReward customersRateReward)
                    {
                        customersManager.ReduceSpawnTime(customersRateReward.SpawnTimeReduce);
                    }
                    break;
                case LevelRewardType.Money:
                    if (levelRewardBase is MoneyLevelReward moneyLevelReward )
                    {
                        playerWallet.AddMoney(moneyLevelReward.RewardMoney);
                    }
                    break;
            }
        }
    }
}
