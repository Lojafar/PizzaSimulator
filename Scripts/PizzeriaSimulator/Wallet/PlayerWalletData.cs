using Game.PizzeriaSimulator.Currency;
using System;

namespace Game.PizzeriaSimulator.Wallet
{
    [Serializable]
    public class PlayerWalletData 
    {
        public MoneyQuantity Money; 
        public int Gems; 
        public PlayerWalletData()
        {

        }
        public PlayerWalletData(MoneyQuantity _money, int _gems)
        {
            Money = _money;
            Gems = _gems;
        }
      
        public PlayerWalletData Clone()
        {
            return new PlayerWalletData(Money, Gems);
        }
    }
}
