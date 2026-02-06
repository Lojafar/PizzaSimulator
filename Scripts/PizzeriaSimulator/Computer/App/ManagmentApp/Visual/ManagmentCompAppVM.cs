using Game.PizzeriaSimulator.Pizzeria.Managment.Config;
using Game.Root.ServicesInterfaces;
using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Computer.App.ManagmentApp.Visual
{
    public sealed class ManagmentExpansViewData
    {
        public int Id;
        public string Name;
        public string Description;
        public string PriceText;
        public Sprite Icon;
        public ManagmentExpansViewData(int _id, string _name, string _description, string _priceText, Sprite _icon)
        {
            Id = _id;
            Name = _name;
            Description = _description;
            PriceText = _priceText;
            Icon = _icon;
        }
    }
    public sealed class ManagmentCompAppVM : ISceneDisposable
    {
        public event Action Open;
        public event Action Close;
        public event Action<ManagmentExpansViewData> AddNewExpansionItem;
        public event Action<int> ActivateExpansSlot;
        public event Action<int, int> ChangeExpansSlotOrder;
        public event Action<string> ShowExpansFailMessage;
        readonly ManagmentCompApp managmentCompApp;
        const string priceTextPattern = "{0}$";
        int expansionsCount;
        public ManagmentCompAppVM(ManagmentCompApp _managmentCompApp)
        {
            managmentCompApp = _managmentCompApp;
        }
        public void Init()
        {
            managmentCompApp.OnOpen += HandleOpen;
            managmentCompApp.OnClose += HandleClose;
            managmentCompApp.OnNewExpansionAdded += HandleNewExpansionItem;
            managmentCompApp.OnFailToPurchaseExpansion += HandleExpansPurchaseFail;
            managmentCompApp.OnExpansionActivated += HandleExpansActivation;
        }
        public void Dispose()
        {
            managmentCompApp.OnOpen -= HandleOpen;
            managmentCompApp.OnClose -= HandleClose;
            managmentCompApp.OnNewExpansionAdded -= HandleNewExpansionItem;
            managmentCompApp.OnFailToPurchaseExpansion -= HandleExpansPurchaseFail;
            managmentCompApp.OnExpansionActivated -= HandleExpansActivation;
        }
        public void CloseInput()
        {
            managmentCompApp.Close();
        }
        public void PurchaseExpansInput(int expansionId)
        {
            managmentCompApp.PurchaseExpansionInput(expansionId);
        }
        void HandleOpen()
        {
            Open?.Invoke();
        }
        void HandleClose()
        {
            Close?.Invoke();
        }
        void HandleNewExpansionItem(PizzeriaExpansionConfig expansionConfig)
        {
            expansionsCount++;
            AddNewExpansionItem?.Invoke(new ManagmentExpansViewData(expansionConfig.Id, expansionConfig.Name,
                expansionConfig.Description, string.Format(priceTextPattern, expansionConfig.Price.Dollars), expansionConfig.Icon));
        }
        void HandleExpansPurchaseFail(int code)
        {
            if (code == ManagmentCompApp.FailCodeMoneyEnough)
            {
                ShowExpansFailMessage?.Invoke("Not enough money");
            }
        }
        void HandleExpansActivation(int expansionId)
        {
            ActivateExpansSlot?.Invoke(expansionId);
            ChangeExpansSlotOrder?.Invoke(expansionId, expansionsCount);
        }
    }
}
