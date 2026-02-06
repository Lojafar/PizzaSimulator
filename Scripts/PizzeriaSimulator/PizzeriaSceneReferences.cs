using Game.PizzeriaSimulator.Computer.App.ManagmentApp.Visual;
using Game.PizzeriaSimulator.Computer.App.Market.Visual;
using Game.PizzeriaSimulator.Computer.Visual;
using Game.PizzeriaSimulator.Customers.WaypointField;
using Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual;
using Game.PizzeriaSimulator.PaymentReceive.Visual;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold.Visual;
using Game.PizzeriaSimulator.PizzaCreation.Visual;
using Game.PizzeriaSimulator.PizzaHold.Visual;
using Game.PizzeriaSimulator.PizzaHold.Visual.PizzaContainer;
using Game.PizzeriaSimulator.Pizzeria.Furniture.Placement;
using Game.PizzeriaSimulator.Pizzeria.Managment.Expansion;
using Game.PizzeriaSimulator.Pizzeria.Managment.Visual;
using System;
using UnityEngine;

namespace Game.PizzeriaSimulator
{
    public class PizzeriaSceneReferences : MonoBehaviour
    {
        [field: SerializeField] public Light DirectionalLight { get; private set; }
        [field: SerializeField] public Transform DeliveryPoint { get; private set; }
        [field: SerializeField] public Transform PlayerSpawnPoint { get; private set; }
        [field: SerializeField] public Transform CustomersSpawnPoint { get; private set; }
        [field: SerializeField] public CustomersWaypointFieldBase CustomerWaitOrderField { get; private set; }
        [field: SerializeField] public Transform CustomersTakeOrderPoint { get; private set; }
        [field: SerializeField] public Transform[] CustomersPointsInLine { get; private set; }
        [field: SerializeField] public PizzeriaExpansionsContainer PizzeriaExpansionsContainer { get; private set; }
        [field: SerializeField] public PizzaObjsContainer RemovedPizzasContainer { get; private set; }
        [field: SerializeField] public FurniturePlaceAreaHolder FurniturePlaceAreaHolder { get; private set; }
        [field: SerializeField] public Canvas SceneCanvas { get; private set; }
        [field: SerializeField] public PaymentReceiveViewBase PaymentReceiverViewBase { get; private set; }
        [field: SerializeField] public CashPaymentProccesorViewBase CashPaymentViewBase { get; private set; }
        [field: SerializeField] public CardPaymentProccesorViewBase CardPaymentViewBase { get; private set; }
        [field: SerializeField] public PizzaCreatorViewBase PizzaCreatorViewBase { get; private set; }
        [field: SerializeField] public PizzaCutViewBase PizzaCutViewBase { get; private set; }
        [field: SerializeField] public PizzaHolderViewBase PizzaHolderViewBase { get; private set; }
        [field: SerializeField] public PizzaIngredientsHolderViewBase PizzaIngredientsHoldView { get; private set; }
        [field: SerializeField] public PizzeriaComputerViewBase PizzeriaComputerView { get; private set; }
        [field: SerializeField] public MarketCompAppViewBase MarketCompAppView { get; private set; }
        [field: SerializeField] public ManagCompAppViewBase ManagmentCompAppView { get; private set; }
        [field: SerializeField] public PizzeriaManagerViewBase PizzeriaManagerView { get; private set; }
        public event Action OnLeaveScene;
        private void OnDestroy()
        {
            OnLeaveScene?.Invoke();
            OnLeaveScene = null;
        }
    }
}
