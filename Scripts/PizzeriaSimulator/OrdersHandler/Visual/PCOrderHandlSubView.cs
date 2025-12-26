using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.OrdersHandle.Visual
{
    class PCOrderHandlSubView : OrderHandlSubViewBase
    {
        [SerializeField] GameObject expandHint;
        [SerializeField] GameObject compressHint;
        public override event Action OnExpandInput;
        public override event Action OnCompressInput;
        private void Awake()
        {
            expandHint.SetActive(false);
            compressHint.SetActive(false);
        }
        public override void OnExpanded()
        {
            expandHint.SetActive(false);
            compressHint.SetActive(true);
        }
        public override void OnCompressed()
        {
            expandHint.SetActive(true);
            compressHint.SetActive(false);
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
