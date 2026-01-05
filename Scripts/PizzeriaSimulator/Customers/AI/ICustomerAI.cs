using System;
using UnityEngine;
namespace Game.PizzeriaSimulator.Customers.AI
{
    public interface ICustomerAI
    {
        public event Action<int> OnTargetPointReached;
        public event Action<CustomerState> OnStateChanged;
        public int Id { get; }
        public CustomerState CurrentState { get; }
        public void SetState(CustomerState customerState);
        public void SetTargetPoint(Transform target);
        public Vector3 GetMoveDir();
        public Quaternion GetRot();
        public void Update();
    }
}
