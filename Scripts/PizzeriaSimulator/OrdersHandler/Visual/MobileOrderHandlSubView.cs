using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PizzeriaSimulator.OrdersHandle.Visual
{
    class MobileOrderHandlSubView : OrderHandlSubViewBase
    {
        [SerializeField] Button expandButton;
        public override event Action OnExpandInput;
        public override event Action OnCompressInput;
        bool expanded;
        private void Awake()
        {
            expandButton.onClick.AddListener(OnExpandBtn);
        }
        private void OnDestroy()
        {
            expandButton.onClick.RemoveListener(OnExpandBtn);
        }
        void OnExpandBtn()
        {
            if (expanded) OnCompressInput?.Invoke();
            else OnExpandInput?.Invoke();
        }
        public override void OnExpanded()
        {
            expanded = true;
        }
        public override void OnCompressed()
        {
            expanded = false;
        }
        public override void UpdateCompress(bool compressed)
        {
            expanded = !compressed;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                OnExpandInput?.Invoke();

            }
            else if (Input.GetKeyUp(KeyCode.F))
            {
                OnCompressInput?.Invoke();
            }
        }
    }
}
