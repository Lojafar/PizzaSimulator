using UnityEngine;

namespace Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual
{
    class CashChangeObject : MonoBehaviour
    {
        [field: SerializeField] public int Amount { get; private set; }
        [field: SerializeField] public GameObject SinglePrefab { get; private set; }
        [SerializeField] Outline selectionOutline;
        private void Awake()
        {
            Deselect();
        }
        public void Select()
        {
            selectionOutline.enabled = true;
        }
        public void Deselect()
        {
            selectionOutline.enabled = false;
        }
    }
}
