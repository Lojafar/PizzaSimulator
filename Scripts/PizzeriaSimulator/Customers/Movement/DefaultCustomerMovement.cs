using UnityEngine;

namespace Game.PizzeriaSimulator.Customers.Movement
{
    class DefaultCustomerMovement : ICustomerMovement
    {
        public float MoveMagnitude { get; private set; }
        readonly Customer customer;
        readonly Transform customerTransform;
        readonly float speed;
        readonly float rotSpeed;
        Quaternion targetRot;

        public DefaultCustomerMovement(Customer _customer, Transform _customerTransform)
        {
            customer = _customer;
            customerTransform = _customerTransform;
            speed = customer.Speed;
            rotSpeed = customer.RotSpeed;
        }
        public void Move(Vector3 direction)
        {
            MoveMagnitude = direction.magnitude;
            customerTransform.position += direction * (speed * Time.deltaTime);
            customerTransform.rotation = Quaternion.Slerp(customerTransform.rotation, targetRot, rotSpeed * Time.deltaTime);
        }
        public void Rotate(Quaternion endRot)
        {
            targetRot = endRot;
        }
    }
}
