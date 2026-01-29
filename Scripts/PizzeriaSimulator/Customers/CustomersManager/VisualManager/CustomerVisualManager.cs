using Game.PizzeriaSimulator.Currency;
using Game.PizzeriaSimulator.Customers.OrdersProcces;
using Game.PizzeriaSimulator.Customers.Visual;
using Game.PizzeriaSimulator.PaymentReceive;
using Game.PizzeriaSimulator.PaymentReceive.Visual;
using Game.PizzeriaSimulator.PaymentReceive.Visual.Config;
using Game.Root.AssetsManagment;
using Game.Root.ServicesInterfaces;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game.PizzeriaSimulator.Customers.Manager
{
 
    class CustomerVisualManager : IPrewarmable, IInittable, ISceneDisposable
    {
        public int InitPriority => 9;
        readonly IAssetsProvider assetsProvider;
        readonly CustomersManager customersManager;
        readonly CustomersOrdersProccesor customersOrdersProccesor;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly PaymentVisualConfig paymentVisualConfig;

        CustomerDreamBubble dreamBubblePrefab;
        CustomerDreamBubble dreamBubble;
        PaymentObject currentPaymentObj;
        const float orderTakeDelay = 0.4f;
        const float orderMoveHalfDur = 0.15f;
        const float orderMoveYMaxOffset = 0.3f;
        public CustomerVisualManager(IAssetsProvider _assetsProvider, CustomersManager _customersManager, CustomersOrdersProccesor _customersOrdersProccesor,
            PizzeriaSceneReferences _sceneReferences, PaymentVisualConfig _paymentVisualConfig)
        {
            assetsProvider = _assetsProvider;
            customersManager = _customersManager;
            customersOrdersProccesor = _customersOrdersProccesor;
            sceneReferences = _sceneReferences;
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
            for (int i = sceneReferences.RemovedPizzasContainer.PizzaCount - 1; i >= 0; i--)
            {
                Object.Destroy(sceneReferences.RemovedPizzasContainer.GetPizza());
            }
        }
        void OnCustomerStartOrder(Customer customer, int orderID)
        {
            dreamBubble.SetItemSprite(customersOrdersProccesor.GetOrderConfig(orderID).PizzaIcon);
            dreamBubble.transform.position = customer.Skin.HeadBone.position;
            dreamBubble.gameObject.SetActive(true);
        }
        void OnCustomerMadeOrder(Customer customer, int orderID)
        {
            dreamBubble.gameObject.SetActive(false);
            if (currentPaymentObj != null)
            {
                Object.Destroy(currentPaymentObj.gameObject);
            }
        }
        async void OnCustomerStartTakingOrder(Customer customer, int orderID) 
        {
            if (sceneReferences.RemovedPizzasContainer.PizzaCount < 1) return;
            await UniTask.WaitForSeconds(orderTakeDelay);
            if( sceneReferences.RemovedPizzasContainer.TryGetPizza(out GameObject pizzaObj))
            {
                if (customer == null)
                {
                    Object.Destroy(pizzaObj);
                    return;
                }
                DOTween.Sequence()
                    .Append(pizzaObj.transform.DOMove(new Vector3(pizzaObj.transform.position.x, pizzaObj.transform.position.y + orderMoveYMaxOffset, pizzaObj.transform.position.z) +
                    (customer.Skin.OrderPoint.position - pizzaObj.transform.position) / 2, orderMoveHalfDur).SetEase(Ease.Linear))
                    .Append(pizzaObj.transform.DOMove(customer.Skin.OrderPoint.position, orderMoveHalfDur).SetEase(Ease.Linear))
                    .OnComplete(() => pizzaObj.transform.SetParent(customer.Skin.HandBone, true)).Play();
            }
            
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
