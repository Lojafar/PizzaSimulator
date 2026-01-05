using System;
using UnityEngine;
namespace Game.PizzeriaSimulator.Customers.AI
{
    class DefaultCustomerAI : ICustomerAI
    {
        public event Action<int> OnTargetPointReached;
        public event Action<CustomerState> OnStateChanged;
        public int Id => id;
        public CustomerState CurrentState { get; private set; }
        readonly int id;
        readonly Transform customerTransform;
        Transform targetTransform;
        Vector3 toTargetDir;
        Quaternion targetRot;
        bool targetPosReached;
        bool targetReached;
        const float targetPosThreshold = 0.05f;
        const float targetRotThreshold = 2f;
        public DefaultCustomerAI(int _id, Transform _customerTransform)
        {
            id = _id;
            customerTransform = _customerTransform;
        }
        public void SetState(CustomerState customerState)
        {
            CurrentState = customerState;
            OnStateChanged?.Invoke(customerState);
        }
        public void SetTargetPoint(Transform target)
        {
            targetTransform = target;
            targetPosReached = false;
            targetReached = false;
            targetRot = Quaternion.identity;
            toTargetDir = Vector3.zero;
        }
        public Vector3 GetMoveDir()
        {
            return toTargetDir.normalized;
        }
        public Quaternion GetRot()
        {
            return targetRot;
        }
        public void Update()
        {
            if (targetTransform != null)
            {
                if (!targetPosReached)
                {
                    toTargetDir = new Vector3(targetTransform.position.x - customerTransform.position.x, 0, targetTransform.position.z - customerTransform.position.z);
                    if (Mathf.Abs(toTargetDir.x) < targetPosThreshold && Mathf.Abs(toTargetDir.z) < targetPosThreshold)
                    {
                        targetPosReached = true;
                        targetReached = false;
                        toTargetDir = Vector3.zero;
                        targetRot = targetTransform.rotation;
                        return;
                    }
                    targetRot = Quaternion.LookRotation(toTargetDir);
                }
                else if(!targetReached)
                {
                    if (Quaternion.Angle(targetRot, customerTransform.rotation) < targetRotThreshold)
                    {
                        targetReached = true;
                        OnTargetPointReached?.Invoke(id);
                    }
                }
              
            }
        }

    }
}
