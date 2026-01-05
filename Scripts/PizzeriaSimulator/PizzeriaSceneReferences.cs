using Game.PizzeriaSimulator.Customers.Visual;
using Game.PizzeriaSimulator.Customers.WaypointField;
using Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual;
using Game.PizzeriaSimulator.PaymentReceive.Visual;
using Game.PizzeriaSimulator.PizzaCreation.Visual;
using Game.PizzeriaSimulator.PizzaHold.Visual;
using System;
using UnityEngine;

namespace Game.PizzeriaSimulator
{
    public class PizzeriaSceneReferences : MonoBehaviour
    {
        [field: SerializeField] public Transform PlayerSpawnPoint { get; private set; }
        [field: SerializeField] public Transform RemovedPizzasContainer { get; private set; }
        [field: SerializeField] public Transform CustomersSpawnPoint { get; private set; }
        [field: SerializeField] public CustomersWaypointFieldBase CustomerWaitOrderField { get; private set; }
        [field: SerializeField] public Transform CustomersTakeOrderPoint { get; private set; }
        [field: SerializeField] public Transform[] CustomersPointsInLine { get; private set; }
        [field: SerializeField] public Canvas SceneCanvas { get; private set; }
        [field: SerializeField] public CustomerDreamBubble CustomerDreamBubble { get; private set; }
        [field: SerializeField] public PaymentReceiveViewBase PaymentReceiverViewBase { get; private set; }
        [field: SerializeField] public CashPaymentProccesorViewBase CashPaymentViewBase { get; private set; }
        [field: SerializeField] public CardPaymentProccesorViewBase CardPaymentViewBase { get; private set; }
        [field: SerializeField] public PizzaCreatorViewBase PizzaCreatorViewBase { get; private set; }
        [field: SerializeField] public PizzaCutViewBase PizzaCutViewBase { get; private set; }
        [field: SerializeField] public PizzaHolderViewBase PizzaHolderViewBase { get; private set; }
        public event Action OnLeaveScene;
        private void OnDestroy()
        {
            OnLeaveScene?.Invoke();
            OnLeaveScene = null;
        }
    }
}
