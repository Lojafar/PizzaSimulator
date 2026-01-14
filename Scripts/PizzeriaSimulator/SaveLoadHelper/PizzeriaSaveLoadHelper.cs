using Game.Root.ServicesInterfaces;
using Game.PizzeriaSimulator.Wallet;
using Game.PizzeriaSimulator.Currency;
using Cysharp.Threading.Tasks;
using Zenject;
using R3;

namespace Game.PizzeriaSimulator.SaveLoadHelp
{
    class PizzeriaSaveLoadHelper : ISceneDisposable
    {
        readonly DiContainer diContainer;
        readonly CompositeDisposable disposables;
        public PlayerWallet PlayerWallet { get; private set; }
        public PizzeriaSaveLoadHelper(DiContainer _diContainer)
        {
            diContainer = _diContainer;
            disposables = new CompositeDisposable();
        }
        public async UniTask LoadAndBindSaves()
        {
            await LoadWallet();
            diContainer.Bind<PlayerWallet>().FromInstance(PlayerWallet).AsSingle();
            HandleLoads();
        }
        async UniTask LoadWallet()
        {
            PlayerWallet = new PlayerWallet(new PlayerWalletData());
        }
        void HandleLoads()
        {
            PlayerWallet.Money.Skip(1).Subscribe(OnWalletMoneyChanged).AddTo(disposables);
        }
        public void Dispose() 
        {
            disposables.Dispose();
        }
        void OnWalletMoneyChanged(MoneyQuantity newMoney)
        {

        }
    }
}
