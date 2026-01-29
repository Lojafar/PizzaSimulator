using Game.PizzeriaSimulator.Leevl.Reward;
using Game.Root.ServicesInterfaces;
using R3;
using System;
using UnityEngine;

namespace Assets.Game.Scripts.PizzeriaSimulator.UI.NewLevelPopUp.Visual
{
    class NewLevelPopUpVM : ISceneDisposable
    {
        public event Action<bool> Open;
        public event Action<bool> Close;
        public event Action ClearAllRewards;
        public event Action<Sprite, string> ShowNewReward;
        public event Action<string> UpdateLevelText;
        public Subject<Unit> ConfirmInput;
        readonly NewLevelPopUpModel newLevelPopUpModel;
        const string levelTextPattern = "LEVEL {0}";
        IDisposable confirmSubscription;
        public NewLevelPopUpVM(NewLevelPopUpModel _newLevelPopUpModel)
        {
            newLevelPopUpModel = _newLevelPopUpModel;
            ConfirmInput = new Subject<Unit>();
        }
        public void Init()
        {
            newLevelPopUpModel.OnOpen += HandleOpen;
            newLevelPopUpModel.OnClose += HandleClose;
            newLevelPopUpModel.OnAllRewardsCleared += HandleAllRewardsClear;
            newLevelPopUpModel.OnNewReward += HandleNewReward;
            newLevelPopUpModel.OnNewLevel += HandleNewLevel;
            confirmSubscription = ConfirmInput.ThrottleFirst(TimeSpan.FromSeconds(0.1)).Subscribe(_ => newLevelPopUpModel.CloseInput());
            Close?.Invoke(true);
        }
        public void Dispose() 
        {
            newLevelPopUpModel.OnOpen -= HandleOpen;
            newLevelPopUpModel.OnClose -= HandleClose;
            newLevelPopUpModel.OnAllRewardsCleared -= HandleAllRewardsClear;
            newLevelPopUpModel.OnNewReward -= HandleNewReward;
            newLevelPopUpModel.OnNewLevel -= HandleNewLevel;
            confirmSubscription.Dispose();
            ConfirmInput.Dispose();
        }
        void HandleOpen()
        {
            Open?.Invoke(false);
        }
        void HandleClose()
        {
            Close?.Invoke(false);
        }
        void HandleAllRewardsClear()
        {
            ClearAllRewards?.Invoke();
        }
        void HandleNewReward(LevelRewardBase levelReward)
        {
            ShowNewReward?.Invoke(levelReward.Icon, levelReward.Description);
        }
        void HandleNewLevel(int level)
        {
            UpdateLevelText?.Invoke(string.Format(levelTextPattern, level));
        }
    }
}
