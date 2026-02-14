using Game.PizzeriaSimulator.Customers.AI;
using Game.PizzeriaSimulator.Customers.Movement;
using Game.PizzeriaSimulator.Customers.Skin;
using Game.PizzeriaSimulator.Customers.Visual;
using UnityEngine;

namespace Game.PizzeriaSimulator.Customers
{
    public class Customer : MonoBehaviour
    {
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float RotSpeed { get; private set; }
        public ICustomerAI CustomerAI { get; private set; }
        ICustomerMovement movement;
        ICustomerVisualHandler visualHandler;
        public CustomerSkin Skin { get; private set; }
        public int Id { get; private set; }
        public int OrderId { get; private set; }

        float stopDelay;
        public void Construct(ICustomerAI _customerAI, CustomerSkin _skin)
        {
            CustomerAI = _customerAI;
            Skin = _skin;
            Id = CustomerAI.Id;
            movement = new DefaultCustomerMovement(this, transform);
            visualHandler = new CustomerVisualHandler(Skin);

            CustomerAI.OnStateChanged += HandleNewState;
        }
        private void OnDestroy()
        {
            CustomerAI.OnStateChanged -= HandleNewState;
        }
        void HandleNewState(CustomerState newState)
        {
            visualHandler.UpdateCustomerState(newState);
        }
        public void SetOrder(int _orderID)
        {
            OrderId = _orderID;
        }
        public void SetStopDelay(float newDelay)
        {
            stopDelay = newDelay;
        }
        private void Update()
        {
            CustomerAI.Update();
            if (stopDelay > 0)
            {
                visualHandler.UpdateMoveSpeed(0);
                stopDelay -= Time.deltaTime;
                return;
            }
            movement.Rotate(CustomerAI.GetRot());
            movement.Move(CustomerAI.GetMoveDir());
            visualHandler.UpdateMoveSpeed(movement.MoveMagnitude);
        }

    }
}
