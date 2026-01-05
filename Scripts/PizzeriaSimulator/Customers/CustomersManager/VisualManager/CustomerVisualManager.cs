using Game.Root.ServicesInterfaces;
using Game.PizzeriaSimulator.Customers.OrdersProcces;
using Game.PizzeriaSimulator.PaymentReceive;
using Game.PizzeriaSimulator.PaymentReceive.Visual;
using Game.PizzeriaSimulator.PaymentReceive.Visual.Config;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Game.PizzeriaSimulator.Customers.Manager
{
    class CustomerVisualManager : ISceneDisposable
    {
        readonly CustomersManager customersManager;
        readonly CustomersOrdersProccesor customersOrdersProccesor;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly PaymentVisualConfig paymentVisualConfig;

        PaymentObject currentPaymentObj;
        const float orderTakeDelay = 0.4f;
        const float orderMoveHalfDur = 0.15f;
        const float orderMoveYMaxOffset = 0.3f;
        public CustomerVisualManager(CustomersManager _customersManager, CustomersOrdersProccesor _customersOrdersProccesor,
            PizzeriaSceneReferences _sceneReferences, PaymentVisualConfig _paymentVisualConfig)
        {
            customersManager = _customersManager;
            customersOrdersProccesor = _customersOrdersProccesor;
            sceneReferences = _sceneReferences;
            paymentVisualConfig = _paymentVisualConfig;
        } 
        public void Init()
        {
            customersManager.OnCustomerStartOrder += OnCustomerStartOrder;
            customersManager.OnCustomerMadeOrder += OnCustomerMadeOrder;
            customersManager.OnCustomerTakedOrder += OnCustomerStartTakingOrder;
            customersOrdersProccesor.OnCustomerStartPaying += OnCustomerStartPaying;
        }
        public void Dispose()
        {
            customersManager.OnCustomerStartOrder -= OnCustomerStartOrder;
            customersManager.OnCustomerMadeOrder -= OnCustomerMadeOrder;
            customersManager.OnCustomerTakedOrder -= OnCustomerStartTakingOrder;
            customersOrdersProccesor.OnCustomerStartPaying -= OnCustomerStartPaying;
        }
        void OnCustomerStartOrder(Customer customer, int orderID)
        {
            sceneReferences.CustomerDreamBubble.SetItemSprite(customersOrdersProccesor.GetOrderConfig(orderID).PizzaIcon);
            sceneReferences.CustomerDreamBubble.transform.position = customer.Skin.HeadBone.position;
            sceneReferences.CustomerDreamBubble.gameObject.SetActive(true);
        }
        void OnCustomerMadeOrder(Customer customer, int orderID)
        {
            sceneReferences.CustomerDreamBubble.gameObject.SetActive(false);
            if (currentPaymentObj != null)
            {
                Object.Destroy(currentPaymentObj.gameObject);
            }
        }
        async void OnCustomerStartTakingOrder(Customer customer, int orderID) 
        {
            if (sceneReferences.RemovedPizzasContainer.childCount < 1) return;
            await UniTask.WaitForSeconds(orderTakeDelay);
            Transform pizza = sceneReferences.RemovedPizzasContainer.GetChild(0);
            DOTween.Sequence()
                .Append(pizza.DOMove(new Vector3(pizza.position.x, pizza.position.y + orderMoveYMaxOffset, pizza.position.z) + 
                (customer.Skin.OrderPoint.position - pizza.position) / 2, orderMoveHalfDur).SetEase(Ease.Linear))
                .Append(pizza.DOMove(customer.Skin.OrderPoint.position, orderMoveHalfDur).SetEase(Ease.Linear))
                .OnComplete(() => pizza.SetParent(customer.Skin.HandBone, true)).Play();
        }
        void OnCustomerStartPaying(Customer customer, PaymentType paymentType, PaymentPrice price)
        {
            PaymentObject paymentObjectPrefab = paymentVisualConfig.GetPaymentObjectByType(GetPaymentObjTypeByPrice(paymentType, price));
            currentPaymentObj = Object.Instantiate(paymentObjectPrefab, customer.Skin.HandBone);
            currentPaymentObj.transform.localEulerAngles = paymentVisualConfig.InHandLocalRot;
            currentPaymentObj.transform.localPosition = paymentVisualConfig.InHandOffset;
        }
        PaymentObjectType GetPaymentObjTypeByPrice(PaymentType paymentType, PaymentPrice price)
        {
            if(paymentType == PaymentType.Card) return PaymentObjectType.Card;
            return price.Dollars switch
            {
                < 5 => PaymentObjectType.OneDollar,
                < 10 => PaymentObjectType.FiveDollars,
                < 20 => PaymentObjectType.TenDollars,
                < 50 => PaymentObjectType.TwentyDollars,
                _ => PaymentObjectType.FiftyDollars,
            };
        }
    }
}
