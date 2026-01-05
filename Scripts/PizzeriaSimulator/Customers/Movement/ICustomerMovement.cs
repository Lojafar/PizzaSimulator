using UnityEngine;

namespace Game.PizzeriaSimulator.Customers.Movement
{ 
    interface ICustomerMovement
    {
        public float MoveMagnitude { get; }
        public void Move(Vector3 direction);
        public void Rotate(Quaternion endRot);
    }
}
