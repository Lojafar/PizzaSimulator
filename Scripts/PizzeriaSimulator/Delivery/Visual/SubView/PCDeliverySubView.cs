using UnityEngine;
using UnityEngine.EventSystems;
namespace Game.PizzeriaSimulator.Delivery.Visual.SubView
{
    class PCDeliverySubView : ButtonsDeliverySubView
    {
        [SerializeField] GameObject gemsSkipHint;
        [SerializeField] GameObject advSkipHint;
        const KeyCode gemsSkipKeyCode = KeyCode.X;
        const KeyCode advSkipKeyCode = KeyCode.C;
        protected override void Awake()
        {
            base.Awake();
            gemsSkipHint.SetActive(false);
            advSkipHint.SetActive(false);
        }
        public override void StartUse()
        {
            gemsSkipHint.SetActive(true);
            advSkipHint.SetActive(true);
        }
        private void Update()
        {
            if (Input.GetKeyDown(gemsSkipKeyCode))
            {
                gemsSkipButton.OnPointerDown(new PointerEventData(EventSystem.current));
            }
            else if (Input.GetKeyUp(gemsSkipKeyCode))
            {
                gemsSkipButton.OnPointerUp(new PointerEventData(EventSystem.current));
                RaiseGemsSkipInput();

            }
            if (Input.GetKeyDown(advSkipKeyCode))
            {
                advSkipButton.OnPointerDown(new PointerEventData(EventSystem.current));
            }
            else if (Input.GetKeyUp(advSkipKeyCode))
            {
                advSkipButton.OnPointerUp(new PointerEventData(EventSystem.current));
                RaiseAdvSkipInput();
            }
        }

    }
}
