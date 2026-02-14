using Game.PizzeriaSimulator.Pizzeria.Furniture.Config;
using Game.PizzeriaSimulator.Pizzeria.Managment;
using Game.PizzeriaSimulator.Pizzeria.Managment.Expansion;
using Game.Root.ServicesInterfaces;
using System;
using UnityEngine;
namespace Game.PizzeriaSimulator.Pizzeria.Furniture.Placement.Manager
{
    using Object = UnityEngine.Object;
    public sealed class PizzeriaFurnitureManager : IInittable, ISceneDisposable
    {
        public event Action<int> OnNewFurniturePlaced;
        public event Action<int, GameObject> OnFurnitureSpawned;
        public int InitPriority => 8;
        public PizzeriaFurnitureManagerData ManagerData => managerData.Clone();
        readonly PizzeriaFurnitureManagerData managerData;
        readonly PizzeriaManager pizzeriaManager;
        readonly FurniturePlaceAreaHolder placeAreaHolder;
        readonly PizzeriaExpansionsContainer expansionsContainer;
        readonly PizzeriaFurnitureConfig furnitureConfig;
        public PizzeriaFurnitureManager(PizzeriaFurnitureManagerData _managerData, PizzeriaManager _pizzeriaManager,
            FurniturePlaceAreaHolder _placeAreaHolder, PizzeriaExpansionsContainer _expansionsContainer, PizzeriaFurnitureConfig _furnitureConfig)
        {
            managerData = _managerData ?? new PizzeriaFurnitureManagerData();
            pizzeriaManager = _pizzeriaManager;
            placeAreaHolder = _placeAreaHolder;
            expansionsContainer = _expansionsContainer;
            furnitureConfig = _furnitureConfig;
        }
        public void Init()
        {
            PlacedFurnitureData currentData;
            for (int i = 0; i < managerData.PlacedFurniture.Count; i++) 
            {
                currentData = managerData.PlacedFurniture[i];
                if (furnitureConfig.GetFurnitureItem(currentData.FurnitureID) is PizzeriaFurnitureItemConfig furnitureItemConfig)
                {
                   GameObject spawnedFurniture = Object.Instantiate(furnitureItemConfig.FurniturePrefab, new Vector3(currentData.PosX, currentData.PosY,
                        currentData.PosZ), Quaternion.Euler(new Vector3(currentData.RotX, currentData.RotY, currentData.RotZ)));
                    OnFurnitureSpawned?.Invoke(currentData.FurnitureID, spawnedFurniture);
                }
            }
            pizzeriaManager.OnExpansionActivated += HandleExpansion;
        }
        public void Dispose()
        {
            pizzeriaManager.OnExpansionActivated -= HandleExpansion;
        }
        void HandleExpansion(int id)
        {
            if (expansionsContainer.GetExpansion(id) is PizzeriaExpansionBase expansion && 
                expansion.ExpansionFurnPlaceOverrides != null && expansion.ExpansionFurnPlaceOverrides.Length > 0)
            {
                for (int i = 0; i < expansion.ExpansionFurnPlaceOverrides.Length; i++) 
                {
                    placeAreaHolder.OverridePlaceArea(expansion.ExpansionFurnPlaceOverrides[i].OveridePlaceAreaId,
                        expansion.ExpansionFurnPlaceOverrides[i].OverriderPlaceArea);
                }
            }
        }
        public void PlaceFurniture(int id, Vector3 pos)
        {
            if (furnitureConfig.GetFurnitureItem(id) is PizzeriaFurnitureItemConfig furnitureItemConfig)
            {
                Vector3 eulerRot =  furnitureItemConfig.FurniturePrefab.transform.eulerAngles;
                GameObject spawnedFurniture = Object.Instantiate(furnitureItemConfig.FurniturePrefab, pos, furnitureItemConfig.FurniturePrefab.transform.rotation);
                managerData.PlacedFurniture.Add(new PlacedFurnitureData(id, pos.x, pos.y, pos.z, eulerRot.x, eulerRot.y, eulerRot.z));
                OnNewFurniturePlaced?.Invoke(id);
                OnFurnitureSpawned?.Invoke(id, spawnedFurniture);
            }
        }
        public FurniturePlaceArea GetPlaceAreaForItem(int furnitureID)
        {
            if (furnitureConfig.GetFurnitureItem(furnitureID) is PizzeriaFurnitureItemConfig furnitureItemConfig)
            {
                return placeAreaHolder.GetPlaceAreaById(furnitureItemConfig.PlaceAreaID);
            }
            return placeAreaHolder.GetPlaceAreaById(-1);
        }
    }
}
