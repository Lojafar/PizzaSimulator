using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Delivery.Visual.SubView
{
    abstract class DeliverySubViewBase : MonoBehaviour
    {
        public abstract event Action GemsSkipInput;
        public abstract event Action AdvSkipInput;
        public virtual void StartUse()
        {

        }
    }
}
