using System;
using Assets.Game.Scripts.PizzeriaSimulator.OrdersHandler;
using Game.PizzeriaSimulator.OrdersHandle.Visual;
using Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual;
using Game.PizzeriaSimulator.PaymentReceive.Visual;
using Game.PizzeriaSimulator.PizzaCreation.Visual;
using Game.PizzeriaSimulator.PizzaHold.Visual;
using UnityEngine;

namespace Game.PizzeriaSimulator
{
    public class PizzeriaSceneReferences : MonoBehaviour
    {
        [field: SerializeField] public Transform PlayerSpawnPoint { get; private set; }
        [field: SerializeField] public Canvas SceneCanvas { get; private set; }
        [field: SerializeField] public PaymentReceiveViewBase PaymentReceiverViewBase { get; private set; }
        [field: SerializeField] public CashPaymentProccesorViewBase CashPaymentViewBase { get; private set; }
        [field: SerializeField] public CardPaymentProccesorViewBase CardPaymentViewBase { get; private set; }
        [field: SerializeField] public PizzaCreatorViewBase PizzaCreatorViewBase { get; private set; }
        [field: SerializeField] public PizzaCutViewBase PizzaCutViewBase { get; private set; }
        [field: SerializeField] public PizzaHolderViewBase PizzaHolderViewBase { get; private set; }
        [field: SerializeField] public OrderGIver OrderGIver { get; private set; }
        public event Action OnLeaveScene;
        private void OnDestroy()
        {
            OnLeaveScene?.Invoke();
            OnLeaveScene = null;
        }
    }
}
