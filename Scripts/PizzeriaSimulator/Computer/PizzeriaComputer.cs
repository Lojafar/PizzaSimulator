using Game.PizzeriaSimulator.Computer.App;
using Game.PizzeriaSimulator.Computer.App.Market;
using Game.PizzeriaSimulator.Delivery;
using Game.PizzeriaSimulator.Delivery.Config;
using Game.PizzeriaSimulator.Interactions;
using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.Wallet;
using Game.Root.ServicesInterfaces;
using System;
using System.Collections.Generic;
using Zenject;

namespace Game.PizzeriaSimulator.Computer 
{
    public class PizzeriaComputer : IInittable, ISceneDisposable
    {
        public event Action OnEnterComputer;
        public event Action OnExitComputer;
        readonly Interactor interactor;
        readonly Dictionary<ComputerAppType, ComputerAppBase> appsByTypes;
        readonly DiContainer diContainer;
        ComputerAppBase openedApp;
        public PizzeriaComputer(Interactor _interactor, DiContainer _diContainer)
        {
            interactor = _interactor;
            diContainer = _diContainer;
            appsByTypes = new Dictionary<ComputerAppType, ComputerAppBase>()
            {
                { ComputerAppType.Market, new MarketCompApp(diContainer.Resolve<PlayerWallet>(),
                diContainer.Resolve<PizzeriaDelivery>(), diContainer.Resolve<PizzeriaDeliveryConfig>()) }
            };
        }
        public ComputerAppBase GetAppByType(ComputerAppType type)
        {
            if(appsByTypes.TryGetValue(type, out ComputerAppBase app)) return app;
            return null;
        }
        public void Init()
        {
            interactor.OnInteract += HandleInteractor;
            foreach (ComputerAppBase app in appsByTypes.Values) 
            {
                if(app is IInittable inittableApp)
                {
                    inittableApp.Init();
                }
                app.Close();
            }
        }
        public void Dispose()
        {
            interactor.OnInteract -= HandleInteractor;
            if(openedApp != null) openedApp.OnClose -= OnOpenedAppClosed;
            foreach (ComputerAppBase app in appsByTypes.Values)
            {
                if (app is IDisposable disposableApp)
                {
                    disposableApp.Dispose();
                }
            }
        }
        void HandleInteractor(InteractableType interactableType)
        {
            if(interactableType == InteractableType.Computer)
            {
                EnterComputer();
            }
        }
        public void EnterComputer()
        {
            OnEnterComputer?.Invoke();
        }
        public void ExitComputer() 
        {
            OnExitComputer?.Invoke();
        }
        public void OpenApp(ComputerAppType appType)
        {
            if (appsByTypes.TryGetValue(appType, out ComputerAppBase app))
            {
                if(openedApp != null)
                {
                    openedApp.OnClose -= OnOpenedAppClosed;
                    openedApp.Close();
                }
                openedApp = app;
                openedApp.OnClose += OnOpenedAppClosed;
                openedApp.Open();
            }
        }
        void OnOpenedAppClosed()
        {
            openedApp.OnClose -= OnOpenedAppClosed;
            openedApp = null;
        }
    }
}
