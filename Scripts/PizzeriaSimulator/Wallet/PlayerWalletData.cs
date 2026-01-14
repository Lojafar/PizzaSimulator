using Game.PizzeriaSimulator.Currency;
using System;

namespace Game.PizzeriaSimulator.Wallet
{
    [Serializable]
    public class PlayerWalletData 
    {
        public MoneyQuantity Money; 
        public PlayerWalletData()
        {

        }
        public PlayerWalletData(MoneyQuantity _money)
        {
            Money = _money;
        }
      
        public PlayerWalletData Clone()
        {
            return new PlayerWalletData(Money);
        }
    }
}
