using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold;
using Game.PizzeriaSimulator.Pizzeria.Furniture.Placement.Manager;
using Game.PizzeriaSimulator.SodaMachine;
using Game.PizzeriaSimulator.Wallet;
using System;
using UnityEngine;
namespace Game.PizzeriaSimulator.SaveLoadHelp.Config
{
    [Serializable]
    public class PizzeriaInitSavesConfig
    {
        [SerializeField] PlayerWalletData initWallet;
        [SerializeField] IngredientsHolderData initIngredsHolderData;
        [SerializeField] PizzeriaFurnitureManagerData initFurnitureManagData;
        [SerializeField] PizzeriaSodaMachineData initSodaMachineData;

        // maybe dictionary instead if block?
        public T GetInitSaving<T>() where T : class
        {
            Type tType = typeof(T);
            if (tType == typeof(PlayerWalletData)) return initWallet as T;
            else if (tType == typeof(IngredientsHolderData)) return initIngredsHolderData as T;
            else if(tType == typeof(PizzeriaFurnitureManagerData)) return initFurnitureManagData as T;
            else if(tType == typeof(PizzeriaSodaMachineData)) return initSodaMachineData as T;
            return null;
        }
    }
}
