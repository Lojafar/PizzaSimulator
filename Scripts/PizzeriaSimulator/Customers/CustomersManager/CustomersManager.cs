using Cysharp.Threading.Tasks;
using Game.PizzeriaSimulator.Customers.AI;
using Game.PizzeriaSimulator.Customers.Manager.StateManager;
using Game.PizzeriaSimulator.Customers.OrdersProcces;
using Game.PizzeriaSimulator.Customers.SettingsConfig;
using Game.PizzeriaSimulator.Customers.Skin;
using Game.PizzeriaSimulator.OrdersHandle;
using Game.Root.AssetsManagment;
using Game.Root.ServicesInterfaces;
using Game.Root.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PizzeriaSimulator.Customers.Manager
{
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;
    public class CustomersManager : ISceneDisposable
    {
        public event Action<Customer, int> OnCustomerStartOrder;
        public event Action<Customer, int> OnCustomerMadeOrder;
        public event Action<Customer, int> OnCustomerTakedOrder;
        readonly CustomersOrdersProccesor customersOrdersProccesor;
        readonly PizzeriaOrdersHandler ordersHandler;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly IAssetsProvider assetsProvider;
        readonly CustomerSkinCreator customerSkinCreator;
        readonly CustomersSettingsConfig settingsConfig;
        readonly int maxCustomersInLine;
        Customer customerPrefab;
        int customersInLineCount;
        int lastCustomerID = -1;
        readonly  Dictionary<CustomerState, ICustomerStateManager> statesManagers;
        float remainedTimeToSpawn;
        public CustomersManager(CustomersOrdersProccesor _customersOrdersProccesor, PizzeriaOrdersHandler _ordersHandler, 
            PizzeriaSceneReferences _sceneReferences, IAssetsProvider _assetsProvider, CustomersSettingsConfig _settingsConfig)
        {
            customersOrdersProccesor = _customersOrdersProccesor;
            ordersHandler = _ordersHandler;
            sceneReferences = _sceneReferences;
            assetsProvider = _assetsProvider;
            settingsConfig = _settingsConfig;
            customerSkinCreator = new CustomerSkinCreator(assetsProvider);
            maxCustomersInLine = sceneReferences.CustomersPointsInLine.Length;
            statesManagers = new Dictionary<CustomerState, ICustomerStateManager>();
        }
        public async UniTask Init()
        {
            customerPrefab = await assetsProvider.LoadAsset<Customer>(AssetsKeys.CustomerPrefab);
            await customerSkinCreator.Init();
            CreateStatesManagers();
            Ticks.Instance.OnTick += OnUpdate;
        }
        void CreateStatesManagers()
        {
            LineCustomerStateManager lineCustomerStateManager = new(this, customersOrdersProccesor, sceneReferences);
            TakeOrderCustStateManager takeOrderCustStateManager = new(this, ordersHandler, sceneReferences);
            statesManagers.Add(CustomerState.InLine, lineCustomerStateManager);
            statesManagers.Add(CustomerState.WaitesOrder, new WaitOrderCustStateManager(this, ordersHandler, sceneReferences));
            statesManagers.Add(CustomerState.TakesOrder, takeOrderCustStateManager);
            statesManagers.Add(CustomerState.Leaves, new LeaveCustStateManager(sceneReferences));

            lineCustomerStateManager.OnCustomerStartOrder += OnCustomeStartedOrder;
            lineCustomerStateManager.OnCustomerMadeOrder += OnCustomerDidOrder;
            takeOrderCustStateManager.OnCustomerTakedOrder += OnCustomerTakeOrder;
        }
        public void Dispose()
        {
            foreach (ICustomerStateManager customerStateManager in statesManagers.Values)
            {
                if (customerStateManager is IDisposable disposableStateManager)
                {
                    disposableStateManager.Dispose();
                }
            }
            LineCustomerStateManager lineCustomerStateManager = statesManagers[CustomerState.InLine] as LineCustomerStateManager;
            TakeOrderCustStateManager takeOrderCustStateManager = statesManagers[CustomerState.TakesOrder] as TakeOrderCustStateManager;

            lineCustomerStateManager.OnCustomerStartOrder -= OnCustomeStartedOrder;
            lineCustomerStateManager.OnCustomerMadeOrder -= OnCustomerDidOrder;
            takeOrderCustStateManager.OnCustomerTakedOrder -= OnCustomerTakeOrder;

            Ticks.Instance.OnTick -= OnUpdate;
        }
        void OnCustomeStartedOrder(Customer customer, int orderID)
        {
            OnCustomerStartOrder?.Invoke(customer, orderID);
        }
        void OnCustomerDidOrder(Customer customer, int orderID)
        {
            customersInLineCount--; 
            OnCustomerMadeOrder?.Invoke(customer, orderID);
        }
        void OnCustomerTakeOrder(Customer customer, int orderID)
        {
            OnCustomerTakedOrder?.Invoke(customer, orderID);
        }
        void OnUpdate()
        {
            remainedTimeToSpawn -= Time.deltaTime;
            if (remainedTimeToSpawn < 0)
            {
                remainedTimeToSpawn = Random.Range(settingsConfig.MinSpawnDelay, settingsConfig.MaxSpawnDelay);
                if (customersInLineCount < maxCustomersInLine)
                {
                    Customer spawnedCustomer = Object.Instantiate(customerPrefab, sceneReferences.CustomersSpawnPoint.position, sceneReferences.CustomersSpawnPoint.rotation);
                    ICustomerAI customerAI = new DefaultCustomerAI(++lastCustomerID, spawnedCustomer.transform);
                    CustomerSkin customerSkin = customerSkinCreator.CreateSkin(spawnedCustomer.transform);
                    spawnedCustomer.Construct(customerAI, customerSkin);
                    customersInLineCount++;
                    SwitchCustomerStateManager(spawnedCustomer, CustomerState.InLine);
                }
            }
        }
        public void SwitchCustomerStateManager(Customer customer, CustomerState customerState)
        {
            if(statesManagers.TryGetValue(customerState, out ICustomerStateManager customerStateManager))
            {
                customerStateManager.HandleCustomerOfState(customer);
            }
            else
            {
                UnityEngine.Debug.LogError($"CustomerStateManager of state: {customerState} isn't setted!");
            }
        }
    }
}