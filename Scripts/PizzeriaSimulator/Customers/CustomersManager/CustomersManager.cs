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
using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.PizzeriaSimulator.DayCycle.Manager;
using Game.PizzeriaSimulator.Pizzeria.Managment;

namespace Game.PizzeriaSimulator.Customers.Manager
{
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;
    public class CustomersManager : IPrewarmable, IInittable, ISceneDisposable
    {
        public int InitPriority => 10;
        public event Action OnAllCustomersDestroyed;
        public event Action<Customer> OnNewCustomer;
        public event Action<Customer, int> OnCustomerStartOrder;
        public event Action<Customer, int> OnCustomerMadeOrder;
        public event Action<Customer, int> OnCustomerTakedOrder;
        public int ActiveCustomersCount { get; private set; }
        readonly CustomersManagerData managerData;
        readonly DayCycleManager dayCycleManager;
        readonly PizzeriaManager pizzeriaManager;
        readonly CustomersOrdersProccesor customersOrdersProccesor;
        readonly PizzeriaOrdersHandler ordersHandler;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly IAssetsProvider assetsProvider;
        readonly CustomerSkinCreator customerSkinCreator;
        readonly CustomersSettingsConfig settingsConfig;
        readonly int maxCustomersInLine;
        readonly Dictionary<int, Customer> customersById;
        readonly Dictionary<CustomerState, ICustomerStateManager> statesManagers;
        public CustomersManagerData ManagerData => managerData.Clone();
        Customer customerPrefab;
        int customersInLineCount;
        int lastCustomerID = -1;
        float timeSpawnReduce;
        float remainedTimeToSpawn;
        public CustomersManager(CustomersManagerData _managerData, DayCycleManager _dayCycleManager, PizzeriaManager _pizzeriaManager,
            CustomersOrdersProccesor _customersOrdersProccesor,  PizzeriaOrdersHandler _ordersHandler, 
            PizzeriaSceneReferences _sceneReferences, IAssetsProvider _assetsProvider, CustomersSettingsConfig _settingsConfig)
        {
            managerData = _managerData ?? new CustomersManagerData();
            dayCycleManager = _dayCycleManager;
            pizzeriaManager = _pizzeriaManager;
            customersOrdersProccesor = _customersOrdersProccesor;
            ordersHandler = _ordersHandler;
            sceneReferences = _sceneReferences;
            assetsProvider = _assetsProvider;
            settingsConfig = _settingsConfig;
            customerSkinCreator = new CustomerSkinCreator(assetsProvider);
            maxCustomersInLine = sceneReferences.CustomersPointsInLine.Length;
            customersById = new Dictionary<int, Customer>();
            statesManagers = new Dictionary<CustomerState, ICustomerStateManager>();
        }
        public void ReduceSpawnTime(float reduce)
        {
            timeSpawnReduce += reduce;
            timeSpawnReduce = Mathf.Clamp(timeSpawnReduce, 0, settingsConfig.MinSpawnDelay);
        }
        public async UniTask Prewarm()
        {
            customerPrefab = await assetsProvider.LoadAsset<Customer>(AssetsKeys.CustomerPrefab);
            await customerSkinCreator.Prewarm();
        }
        public void Init()
        {
            CreateStatesManagers();
            HandleManagerData();
            Ticks.Instance.OnTick += OnUpdate;
        }
        void CreateStatesManagers()
        {
            LineCustomerStateManager lineCustomerStateManager = new(this, customersOrdersProccesor, sceneReferences);
            TakeOrderCustStateManager takeOrderCustStateManager = new(this, ordersHandler, sceneReferences);
            LeaveCustStateManager leaveCustStateManager = new(sceneReferences);
            statesManagers.Add(CustomerState.InLine, lineCustomerStateManager);
            statesManagers.Add(CustomerState.WaitesOrder, new WaitOrderCustStateManager(this, ordersHandler, sceneReferences));
            statesManagers.Add(CustomerState.TakesOrder, takeOrderCustStateManager);
            statesManagers.Add(CustomerState.Leaves, leaveCustStateManager);

            lineCustomerStateManager.OnCustomerStartOrder += OnCustomeStartedOrder;
            lineCustomerStateManager.OnCustomerMadeOrder += OnCustomerDidOrder;
            takeOrderCustStateManager.OnCustomerTakedOrder += OnCustomerTakeOrder;
            leaveCustStateManager.OnCustomerLeaved += OnCustomerLeaved;
        }
        void HandleManagerData()
        {
            for (int i = 0; i < managerData.CustomersOrders.Count; i++) 
            {
                ActiveCustomersCount++;
                Customer spawnedCustomer = SpawnNewCustomer();
                spawnedCustomer.SetOrder(managerData.CustomersOrders[i]);
                customersById.Add(spawnedCustomer.Id, spawnedCustomer);
                if (managerData.CustomersOrders[i] != -1) customersOrdersProccesor.ForceCustomerOrder(managerData.CustomersOrders[i]);
                else customersInLineCount++;

                if (statesManagers.TryGetValue(managerData.CustomersOrders[i] == -1 ? CustomerState.InLine : CustomerState.WaitesOrder, out ICustomerStateManager customerStateManager))
                {
                    customerStateManager.ForceCustomer(spawnedCustomer);
                }
            }
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
            LeaveCustStateManager leaveCustStateManager = statesManagers[CustomerState.Leaves] as LeaveCustStateManager;

            lineCustomerStateManager.OnCustomerStartOrder -= OnCustomeStartedOrder;
            lineCustomerStateManager.OnCustomerMadeOrder -= OnCustomerDidOrder;
            takeOrderCustStateManager.OnCustomerTakedOrder -= OnCustomerTakeOrder;
            leaveCustStateManager.OnCustomerLeaved -= OnCustomerLeaved;

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
            for (int i = 0; i < managerData.CustomersOrders.Count; i++)
            {
                if (managerData.CustomersOrders[i] == -1)
                {
                    managerData.CustomersOrders[i] = orderID;
                    break;
                }
            }
        }
        void OnCustomerTakeOrder(Customer customer, int orderID)
        {
            ActiveCustomersCount--;
            OnCustomerTakedOrder?.Invoke(customer, orderID);
            for (int i = 0; i < managerData.CustomersOrders.Count; i++) 
            {
                if(orderID == managerData.CustomersOrders[i])
                {
                    managerData.CustomersOrders.RemoveAt(i);
                    break;
                }
            }
        }
        void OnCustomerLeaved(Customer customer)
        {
            if (customersById.ContainsKey(customer.Id))
            {
                customersById.Remove(customer.Id);
                Object.Destroy(customer.gameObject);
            }
            else
            {
                Debug.LogError("Customer leaved, but customers dictionary isn't contains his id");
            }
        }
        void OnUpdate()
        {
            remainedTimeToSpawn -= Time.deltaTime;
            if (remainedTimeToSpawn < 0)
            {
                remainedTimeToSpawn = Random.Range(settingsConfig.MinSpawnDelay, settingsConfig.MaxSpawnDelay) - timeSpawnReduce;
                if (!dayCycleManager.IsDayEnded && pizzeriaManager.Opened.CurrentValue && customersInLineCount < maxCustomersInLine)
                {
                    ActiveCustomersCount++;
                    managerData.CustomersOrders.Add(-1);
                    Customer spawnedCustomer = SpawnNewCustomer();
                    customersInLineCount++;
                    customersById.Add(spawnedCustomer.Id, spawnedCustomer);
                    SwitchCustomerStateManager(spawnedCustomer, CustomerState.InLine);
                    OnNewCustomer?.Invoke(spawnedCustomer);
                }
            }
        }
        Customer SpawnNewCustomer()
        {
            Customer spawnedCustomer = Object.Instantiate(customerPrefab, sceneReferences.CustomersSpawnPoint.position, sceneReferences.CustomersSpawnPoint.rotation);
            ICustomerAI customerAI = new DefaultCustomerAI(++lastCustomerID, spawnedCustomer.transform);
            CustomerSkin customerSkin = customerSkinCreator.CreateSkin(spawnedCustomer.transform);
            spawnedCustomer.Construct(customerAI, customerSkin);
            return spawnedCustomer;
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
        public void DestroyAllCustomers()
        {
            ICustomerStateManager customerStateManager;
            foreach (Customer customer in customersById.Values) 
            {
                if (statesManagers.TryGetValue(customer.CustomerAI.CurrentState, out customerStateManager))
                {
                    customerStateManager.RemoveCustomer(customer);
                }
                else
                {
                    UnityEngine.Debug.LogError($"CustomerStateManager of state: {customer.CustomerAI.CurrentState} isn't setted!");
                }
                Object.Destroy(customer.gameObject);
            }
            lastCustomerID = -1;
            OnAllCustomersDestroyed?.Invoke();
            customersById.Clear();
        }
       
    }
}