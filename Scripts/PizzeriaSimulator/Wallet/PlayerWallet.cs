using Game.PizzeriaSimulator.Currency;
using R3;

namespace Game.PizzeriaSimulator.Wallet
{
    public class PlayerWallet
    {
        readonly PlayerWalletData originData;
        readonly ReactiveProperty<MoneyQuantity> money;
        readonly ReactiveProperty<int> gems;
        public ReadOnlyReactiveProperty<MoneyQuantity> Money => money;
        public ReadOnlyReactiveProperty<int> Gems => gems;
        public PlayerWallet(PlayerWalletData _originData)
        {
            originData = _originData;
            money = new ReactiveProperty<MoneyQuantity>(originData.Money);
            gems = new ReactiveProperty<int>(originData.Gems);
        }
        public void AddMoney(MoneyQuantity moneyToAdd)
        {
            if (moneyToAdd.Dollars < 0 || moneyToAdd.Cents < 0) return;
            money.Value += moneyToAdd;
        }
        public bool TryTakeMoney(MoneyQuantity moneyToTake)
        {
            if (money.Value.AllCents < moneyToTake.AllCents) return false;
            money.Value -= moneyToTake;
            return true;
        }
        public void AddGems(int amount)
        {
            if(amount < 0) return;
            gems.Value += amount;
        }
        public bool TryTakeGems(int amount)
        {
            if(gems.Value <  amount) return false;
            gems.Value -= amount;
            return true;
        }
        public PlayerWalletData GetOriginData()
        {
            originData.Money = money.Value;
            originData.Gems = gems.Value;
            return originData.Clone();
        }
    }
}
