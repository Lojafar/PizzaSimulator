using Game.PizzeriaSimulator.Currency;
using R3;

namespace Game.PizzeriaSimulator.Wallet
{
    public class PlayerWallet
    {
        readonly PlayerWalletData originData;
        readonly ReactiveProperty<MoneyQuantity> money;
        public ReadOnlyReactiveProperty<MoneyQuantity> Money => money;
        public PlayerWallet(PlayerWalletData _originData)
        {
            originData = _originData;
            money = new ReactiveProperty<MoneyQuantity>(originData.Money);
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
        public PlayerWalletData GetOriginData()
        {
            originData.Money = money.Value;
            return originData.Clone();
        }
    }
}
