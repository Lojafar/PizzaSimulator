using Game.Root.ServicesInterfaces;
using System;

namespace Game.PizzeriaSimulator.Delivery.Visual
{
    class PizzeriaDeliveryVM : ISceneDisposable
    {
        public event Action ShowDeliveryVisuals;
        public event Action HideDeliveryVisuals;
        public event Action<string> UpdateDeliveryTime;
        readonly PizzeriaDelivery pizzeriaDelivery;
        int lastDeliverySeconds = -1;
        const string deliveryTimePattern = "00:{0:D2}";
        public PizzeriaDeliveryVM(PizzeriaDelivery _pizzeriaDelivery)
        {
            pizzeriaDelivery = _pizzeriaDelivery;
        }
        public void Init()
        {
            pizzeriaDelivery.OnDeliveryStarted += HandleDeliveryStart;
            pizzeriaDelivery.OnDeliveryEnded += HandleDeliveryEnd;
            pizzeriaDelivery.OnDeliveryTimeChanged += HandleDeliveryTime;
            HideDeliveryVisuals?.Invoke();
        }
        public void Dispose() 
        {
            pizzeriaDelivery.OnDeliveryStarted -= HandleDeliveryStart;
            pizzeriaDelivery.OnDeliveryEnded -= HandleDeliveryEnd;
            pizzeriaDelivery.OnDeliveryTimeChanged -= HandleDeliveryTime;
        }
        public void HandleDeliveryStart()
        {
            lastDeliverySeconds = -1;
            ShowDeliveryVisuals?.Invoke();
        }
        public void HandleDeliveryEnd()
        {
            HideDeliveryVisuals?.Invoke();
        }
        public void HandleDeliveryTime(float remainedTime)
        {
            int currentSeconds = (int)remainedTime;
            if (currentSeconds != lastDeliverySeconds)
            {
                lastDeliverySeconds = currentSeconds;
                UpdateDeliveryTime?.Invoke(string.Format(deliveryTimePattern, currentSeconds));
            }

        }
    }
}
