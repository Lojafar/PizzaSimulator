using Game.PizzeriaSimulator.Pizzeria.Managment;
using Game.PizzeriaSimulator.Pizzeria.Managment.Config;
using Game.PizzeriaSimulator.Wallet;
using Game.Root.ServicesInterfaces;
using System;

namespace Game.PizzeriaSimulator.Computer.App.ManagmentApp
{
    public sealed class ManagmentCompApp : ComputerAppBase, IInittable
    {
        public int InitPriority => 10;
        public event Action OnExpansionPurchased;
        public event Action<PizzeriaExpansionConfig> OnNewExpansionAdded;
        public event Action<int> OnFailToPurchaseExpansion;
        public event Action<int> OnExpansionActivated;
        readonly PlayerWallet playerWallet;
        readonly PizzeriaManager pizzeriaManager;
        readonly PizzeriaManagmentConfig managmentConfig;
        public const int FailCodeMoneyEnough = 0;
        public const int FailCodeNotFound = 1;
        public ManagmentCompApp(PlayerWallet _playerWallet, PizzeriaManager _pizzeriaManager, PizzeriaManagmentConfig _managmentConfig)
        {
            playerWallet = _playerWallet;
            pizzeriaManager = _pizzeriaManager;
            managmentConfig = _managmentConfig;
        }
        public void Init()
        {
            PizzeriaExpansionConfig expansionConfig;
            for (int i = 0; i < managmentConfig.ExpansionsCount; i++) 
            {
                expansionConfig = managmentConfig.GetExpansionConfig(i);
                if (expansionConfig != null) 
                {
                    OnNewExpansionAdded?.Invoke(expansionConfig);
                }
            }
            for (int i = 0; i < managmentConfig.ExpansionsCount; i++)
            {
                if (pizzeriaManager.IsExpansionUnlocked(i))
                {
                    OnExpansionActivated?.Invoke(i);
                }
            }
        }
        public void PurchaseExpansionInput(int expansionId)
        {
           
            if (pizzeriaManager.IsExpansionUnlocked(expansionId)) return;
            if(managmentConfig.GetExpansionConfig(expansionId) is PizzeriaExpansionConfig expansionConfig)
            {
                if (playerWallet.TryTakeMoney(expansionConfig.Price))
                {
                    pizzeriaManager.UnlockExpansion(expansionId);
                    OnExpansionPurchased?.Invoke();
                    OnExpansionActivated?.Invoke(expansionId);
                    return;
                }
                else
                {
                    OnFailToPurchaseExpansion?.Invoke(FailCodeMoneyEnough);
                    return;
                }
            }
            OnFailToPurchaseExpansion?.Invoke(FailCodeNotFound);
        }

    }
}
