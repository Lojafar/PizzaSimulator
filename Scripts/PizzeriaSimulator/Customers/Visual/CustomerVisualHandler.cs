using Game.PizzeriaSimulator.Customers.Skin;
using UnityEngine;

namespace Game.PizzeriaSimulator.Customers.Visual
{
     class CustomerVisualHandler : ICustomerVisualHandler
    {
        readonly Customer customer;
        readonly CustomerSkin skin;
        readonly Animator skinAnimator;
        CustomerState lastCustomerState;
        readonly static int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        readonly static int PayHash = Animator.StringToHash("IsPaying");
        readonly static int TakeOrderHash = Animator.StringToHash("IsOrderTaked");
        const float takeOrderDuration =1f;
        public CustomerVisualHandler(Customer _customer, CustomerSkin _skin)
        {
            customer = _customer;
            skin = _skin;
            skinAnimator = skin.Animator;
        }
        public void UpdateCustomerState(CustomerState customerState)
        {
            HandleLastState();
            switch (customerState)
            {
                case CustomerState.MakesOrder:
                    skinAnimator.SetBool(PayHash, true);
                    break;
                case CustomerState.TakesOrder:
                    customer.SetStopDelay(takeOrderDuration);
                    skinAnimator.SetBool(TakeOrderHash, true);
                    break;
            }
            lastCustomerState = customerState;
        }
        void HandleLastState()
        {
            switch (lastCustomerState)
            {
                case CustomerState.MakesOrder:
                    skinAnimator.SetBool(PayHash, false);
                    break;
                case CustomerState.Leaves:
                    skinAnimator.SetBool(TakeOrderHash, false);
                    break;
            }
        }
        public void UpdateMoveSpeed(float currentSpeed)
        {
            skinAnimator.SetFloat(MoveSpeedHash, currentSpeed);
        }
    }
}
