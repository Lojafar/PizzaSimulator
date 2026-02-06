using Game.Root.ServicesInterfaces;
using Game.Root.User.Environment;
using R3;
using System;

namespace Game.PizzeriaSimulator.Delivery.Visual
{
    class PizzeriaDeliveryVM : ISceneDisposable
    {
        public event Action ShowDeliveryVisuals;
        public event Action HideDeliveryVisuals;
        public event Action OnDeliveryEnded;
        public event Action<string> UpdateDeliveryTime;
        public event Action<string> UpdateGemsSkipCostText;
        public event Action<string> ShowDeliveryAddTime;
        public event Action<string> ShowGemsSkipFail;
        public readonly Subject<Unit> GemsSkipInput;
        public readonly Subject<Unit> AdvSkipInput;
        public readonly DeviceType DeviceType;
        readonly PizzeriaDelivery pizzeriaDelivery;
        readonly CompositeDisposable inputSubscriptions;
        int lastDeliverySeconds = -1;
        const string deliveryTimePattern = "00:{0:D2}";
        const string deliveryAddPattern = "Added 00:{0:D2} for new items delivery";
        public PizzeriaDeliveryVM(PizzeriaDelivery _pizzeriaDelivery, DeviceType _deviceType)
        {
            pizzeriaDelivery = _pizzeriaDelivery;
            DeviceType = _deviceType;
            GemsSkipInput = new Subject<Unit>();
            AdvSkipInput = new Subject<Unit>();
            inputSubscriptions = new CompositeDisposable();
        }
        public void Init()
        {
            GemsSkipInput.ThrottleFirst(TimeSpan.FromSeconds(0.3)).Subscribe(_ => pizzeriaDelivery.SkipDeliveryForGems()).AddTo(inputSubscriptions);
            AdvSkipInput.ThrottleFirst(TimeSpan.FromSeconds(0.3)).Subscribe(_ => pizzeriaDelivery.SkipDeliveryForAdv()).AddTo(inputSubscriptions);
            pizzeriaDelivery.OnDeliveryStarted += HandleDeliveryStart;
            pizzeriaDelivery.OnDeliveryEnded += HandleDeliveryEnd;
            pizzeriaDelivery.OnDeliveryTimeChanged += HandleDeliveryTime;
            pizzeriaDelivery.OnDeliveryTimeAdded += HandleDeliveryTimeAdd;
            pizzeriaDelivery.OnGemsSkipPriceChanged += HandleNewGemsPrice;
            pizzeriaDelivery.OnGemsSkipFailed += HandleGemsSkipFail;
            HideDeliveryVisuals?.Invoke();
        }
        public void Dispose() 
        {
            inputSubscriptions.Dispose();
            GemsSkipInput.Dispose();
            AdvSkipInput.Dispose();
            pizzeriaDelivery.OnDeliveryStarted -= HandleDeliveryStart;
            pizzeriaDelivery.OnDeliveryEnded -= HandleDeliveryEnd;
            pizzeriaDelivery.OnDeliveryTimeChanged -= HandleDeliveryTime;
            pizzeriaDelivery.OnDeliveryTimeAdded -= HandleDeliveryTimeAdd;
            pizzeriaDelivery.OnGemsSkipPriceChanged -= HandleNewGemsPrice;
            pizzeriaDelivery.OnGemsSkipFailed -= HandleGemsSkipFail;
        }
        void HandleDeliveryStart()
        {
            lastDeliverySeconds = -1;
            ShowDeliveryVisuals?.Invoke();
        }
        void HandleDeliveryEnd()
        {
            HideDeliveryVisuals?.Invoke();
            OnDeliveryEnded?.Invoke();
        }
        void HandleDeliveryTime(float remainedTime)
        {
            int currentSeconds = (int)remainedTime;
            if (currentSeconds != lastDeliverySeconds)
            {
                lastDeliverySeconds = currentSeconds;
                UpdateDeliveryTime?.Invoke(string.Format(deliveryTimePattern, currentSeconds));
            }

        }
        void HandleDeliveryTimeAdd(float addTime)
        {
            ShowDeliveryAddTime?.Invoke(string.Format(deliveryAddPattern, (int)addTime));
        }
        void HandleNewGemsPrice(int gems)
        {
            UpdateGemsSkipCostText?.Invoke(gems.ToString());
        }
        void HandleGemsSkipFail()
        {
            ShowGemsSkipFail?.Invoke("Not enough Gems");
        }
    }
}
