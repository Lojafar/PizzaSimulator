using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PizzeriaSimulator.Delivery.Visual.SubView
{
    class ButtonsDeliverySubView : DeliverySubViewBase
    {
        public override event Action GemsSkipInput;
        public override event Action AdvSkipInput;
        [SerializeField] protected Button gemsSkipButton;
        [SerializeField] protected Button advSkipButton;
        protected virtual void Awake()
        {
            gemsSkipButton.onClick.AddListener(RaiseGemsSkipInput);
            advSkipButton.onClick.AddListener(RaiseAdvSkipInput);
        }
        protected virtual void OnDestroy()
        {
            gemsSkipButton.onClick.RemoveListener(RaiseGemsSkipInput);
            advSkipButton.onClick.RemoveListener(RaiseAdvSkipInput);
            GemsSkipInput = null;
            AdvSkipInput = null;
        }
        protected void RaiseGemsSkipInput()
        {
            GemsSkipInput?.Invoke();
        }
        protected void RaiseAdvSkipInput()
        {
            AdvSkipInput?.Invoke();
        }
    }
}
