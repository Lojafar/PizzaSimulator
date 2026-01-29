using Game.PizzeriaSimulator.Leevl.Reward;
using Game.PizzeriaSimulator.Levels.Config;
using Game.PizzeriaSimulator.Pizzeria.Manager;
using Game.PizzeriaSimulator.Player.Input;
using Game.Root.ServicesInterfaces;
using R3;
using System;
namespace Assets.Game.Scripts.PizzeriaSimulator.UI.NewLevelPopUp
{
    class NewLevelPopUpModel : IInittable, ISceneDisposable
    {
        public event Action OnOpen;
        public event Action OnClose;
        public event Action OnAllRewardsCleared;
        public event Action<int> OnNewLevel;
        public event Action<LevelRewardBase> OnNewReward;
        public int InitPriority => 10;
        readonly PizzeriaManager pizzeriaManager;
        readonly IPlayerInput playerInput;
        readonly LevelsConfig levelsConfig;
        IDisposable levelSubscription;
        bool opened;
        public NewLevelPopUpModel(PizzeriaManager _pizzeriaManager, IPlayerInput _playerInput, LevelsConfig _levelsConfig)
        {
            pizzeriaManager = _pizzeriaManager;
            playerInput = _playerInput;
            levelsConfig = _levelsConfig;
        }
        public void Init()
        {
            levelSubscription = pizzeriaManager.CurrentLevel.Skip(1).Subscribe(HandleNewPizzeriaLevel);
        }
        public void Dispose() 
        {
            levelSubscription.Dispose();
        }
        void HandleNewPizzeriaLevel(int newLevel)
        { 
            if(opened)
            {
                OnAllRewardsCleared?.Invoke();
            }
            if (levelsConfig.GetLevelData(newLevel) is LevelDataConfig levelConfig)
            {
                for (int i = 0; i < levelConfig.RewardsId.Count; i++) 
                {
                    if (levelsConfig.GetReward(levelConfig.RewardsId[i]) is LevelRewardBase levelRewardBase) 
                    {
                        OnNewReward?.Invoke(levelRewardBase);
                    }
                }
            }
            OnNewLevel?.Invoke(newLevel);
            Open();
        }
        void Open()
        {
            if (!opened)
            {
                playerInput.Activate(false);
            }
           
            opened = true;
            OnOpen?.Invoke();
        }
        void Close()
        {
            if (opened)
            {
                playerInput.Activate(true);
            }
            opened = false;
            OnClose?.Invoke();
            OnAllRewardsCleared?.Invoke();
        }
        public void CloseInput()
        {
            Close();
        }
    }
}
