using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.PaymentReceive.Visual.Config
{
    [Serializable]
    class PaymentObjectWithType
    {
        [field: SerializeField] public PaymentObjectType PaymentObjectType { get; private set; }
        [field: SerializeField] public PaymentObject PaymentObject { get; private set; }
    }
    [Serializable]
    public class PaymentVisualConfig
    {
        [field: SerializeField] public Vector3 InHandOffset { get; private set; }
        [field: SerializeField] public Vector3 InHandLocalRot { get; private set; }
        [SerializeField] PaymentObjectWithType[] paymentObjectWithTypes;
        PaymentObject[] sortedPaymentObjects;
        bool inited;
        public void Init()
        {
            sortedPaymentObjects = new PaymentObject[Enum.GetValues(typeof(PaymentObjectType)).Length];
            foreach(PaymentObjectWithType paymentObjectWithType in paymentObjectWithTypes)
            {
                sortedPaymentObjects[(int)paymentObjectWithType.PaymentObjectType] = paymentObjectWithType.PaymentObject;
            }
            inited = true;
        }
        public PaymentObject GetPaymentObjectByType(PaymentObjectType paymentObjectType) 
        {
            if (!inited) Init();
           return sortedPaymentObjects[(int)paymentObjectType];
        }
    }
}
