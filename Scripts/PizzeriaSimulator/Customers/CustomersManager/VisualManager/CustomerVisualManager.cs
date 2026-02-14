using Game.PizzeriaSimulator.Currency;
using Game.PizzeriaSimulator.Customers.OrdersProcces;
using Game.PizzeriaSimulator.Customers.Visual;
using Game.PizzeriaSimulator.PaymentReceive;
using Game.PizzeriaSimulator.PaymentReceive.Visual;
using Game.PizzeriaSimulator.PaymentReceive.Visual.Config;
using Game.PizzeriaSimulator.Orders.Items;
using Game.Root.AssetsManagment;
using Game.Root.ServicesInterfaces;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using Game.PizzeriaSimulator.Orders.Items.Config;

namespace Game.PizzeriaSimulator.Customers.Manager
{
    class CustomerVisualManager : IPrewarmable, IInittable, ISceneDisposable
    {
        public int InitPriority => 9;
        readonly IAssetsProvider assetsProvider;
        readonly CustomersManager customersManager;
        readonly CustomersOrdersProccesor customersOrdersProccesor;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly OrdersConfig ordersConfig;
        readonly PaymentVisualConfig paymentVisualConfig;

        CustomerDreamBubble dreamBubblePrefab;
        CustomerDreamBubble dreamBubble;
        PaymentObject currentPaymentObj;
        const float orderMoveHalfDur = 0.15f;
        const float orderMoveYMaxOffset = 0.3f;
        public CustomerVisualManager(IAssetsProvider _assetsProvider, CustomersManager _customersManager, CustomersOrdersProccesor _customersOrdersProccesor,
            PizzeriaSceneReferences _sceneReferences, OrdersConfig _ordersConfig, PaymentVisualConfig _paymentVisualConfig)
        {
            assetsProvider = _assetsProvider;
            customersManager = _customersManager;
            customersOrdersProccesor = _customersOrdersProccesor;
            sceneReferences = _sceneReferences;
            ordersConfig = _ordersConfig;
            paymentVisualConfig = _paymentVisualConfig;
        } 
        public async UniTask Prewarm()
        {
            dreamBubblePrefab = await assetsProvider.LoadAsset<CustomerDreamBubble>(AssetsKeys.CustomerDreamBubble);
        }
        public void Init()
        {
            dreamBubble = Object.Instantiate(dreamBubblePrefab);
            dreamBubble.gameObject.SetActive(false);

            customersManager.OnAllCustomersDestroyed += OnAllCustomersDestroyed;
            customersManager.OnCustomerStartOrder += OnCustomerStartOrder;
            customersManager.OnCustomerMadeOrder += OnCustomerMadeOrder;
            customersManager.OnCustomerTakedOrder += OnCustomerStartTakingOrder;
            customersOrdersProccesor.OnCustomerStartPaying += OnCustomerStartPaying;
        }
        public void Dispose()
        {
            customersManager.OnAllCustomersDestroyed -= OnAllCustomersDestroyed;
            customersManager.OnCustomerStartOrder -= OnCustomerStartOrder;
            customersManager.OnCustomerMadeOrder -= OnCustomerMadeOrder;
            customersManager.OnCustomerTakedOrder -= OnCustomerStartTakingOrder;
            customersOrdersProccesor.OnCustomerStartPaying -= OnCustomerStartPaying;
        }
        void OnAllCustomersDestroyed()
        {
            sceneReferences.RemovedOrderItemsContainer.DestroyAllItems();
        }
        void OnCustomerStartOrder(Customer customer, int orderID)
        {
            if(customersOrdersProccesor.TryGetOrderItems(orderID, out List<PizzeriaOrderItemType> orderItems))
            {
                dreamBubble.ClearItemSprites();
                foreach (PizzeriaOrderItemType itemType in orderItems)
                {
                    dreamBubble.AddItemSprite(ordersConfig.GetOrderItemByType(itemType).Icon);
                }
                dreamBubble.transform.position = customer.Skin.HeadBone.position;
                dreamBubble.gameObject.SetActive(true);
            }
        }
        void OnCustomerMadeOrder(Customer customer, int orderID)
        {
            dreamBubble.gameObject.SetActive(false);
            if (currentPaymentObj != null)
            {
                Object.Destroy(currentPaymentObj.gameObject);
            }
        }
        void OnCustomerStartTakingOrder(Customer customer, int orderID)
        {
            const float orderTakeDelay = 0.4f;
            const float betweenItemsDelay = 0.2f;
            const float afterTakedDelay = 0.3f;
            if (!customersOrdersProccesor.TryGetOrderItems(orderID, out List<PizzeriaOrderItemType> orderItems) || orderItems.Count < 1) return;
            customer.SetStopDelay(orderTakeDelay + (betweenItemsDelay + orderMoveHalfDur * 2) * orderItems.Count + afterTakedDelay);
            Sequence orderItemsSequence = DOTween.Sequence().AppendInterval(orderTakeDelay);
            for (int i = 0; i < orderItems.Count; i++)
            {
                if (sceneReferences.RemovedOrderItemsContainer.TryGetAndRemoveItem(orderItems[i], out GameObject itemObj))
                {
                    if (customer == null)
                    {
                        Object.Destroy(itemObj);
                        continue;
                    }
                    orderItemsSequence
                         .Append(itemObj.transform.DOMove(new Vector3(itemObj.transform.position.x, itemObj.transform.position.y + orderMoveYMaxOffset, itemObj.transform.position.z) +
                        (customer.Skin.OrderPoint.position - itemObj.transform.position) / 2, orderMoveHalfDur).SetEase(Ease.Linear))
                        .Append(itemObj.transform.DOMove(customer.Skin.OrderPoint.position + Vector3.up * (i * 0.05f), orderMoveHalfDur).SetEase(Ease.Linear))
                        .AppendCallback(() => itemObj.transform.SetParent(customer.Skin.HandBone, true));
                    if (i != orderItems.Count - 1) orderItemsSequence.AppendInterval(betweenItemsDelay);
                }
            }
            orderItemsSequence.Play();
        }
        void OnCustomerStartPaying(Customer customer, PaymentType paymentType, MoneyQuantity price)
        {
            PaymentObject paymentObjectPrefab = paymentVisualConfig.GetPaymentObjectByType(GetPaymentObjTypeByPrice(paymentType, price));
            currentPaymentObj = Object.Instantiate(paymentObjectPrefab, customer.Skin.HandBone);
            currentPaymentObj.transform.localEulerAngles = paymentVisualConfig.InHandLocalRot;
            currentPaymentObj.transform.localPosition = paymentVisualConfig.InHandOffset;
        }
        PaymentObjectType GetPaymentObjTypeByPrice(PaymentType paymentType, MoneyQuantity price)
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
