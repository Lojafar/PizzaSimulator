using Game.PizzeriaSimulator.Pizzeria.Furniture.Placement.Manager;
using Game.Root.ServicesInterfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.PizzeriaSimulator.Pizzeria.Furniture.Placement.Handler
{
    class PizzeriaFurnitureHandler : IInittable, ISceneDisposable
    {
        public int InitPriority => 7;
        readonly PizzeriaFurnitureManager furnitureManager;
        readonly DiContainer diContainer;
        readonly Dictionary<int, IFurnitureSubHandler> subHandlersDict;
        public PizzeriaFurnitureHandler(PizzeriaFurnitureManager _furnitureManager, DiContainer _diContainer)
        {
            furnitureManager = _furnitureManager;
            diContainer = _diContainer;
            subHandlersDict = new Dictionary<int, IFurnitureSubHandler>();
        }
        public void Init()
        {
            subHandlersDict.Add(SodaMachineSubHandler.SubHandlerTargetID, new SodaMachineSubHandler(diContainer));
            furnitureManager.OnFurnitureSpawned += HandleSpawnedFurniture;
        }
        public void Dispose()
        {
            foreach (IFurnitureSubHandler subHandler in subHandlersDict.Values)
            {
                if(subHandler is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            furnitureManager.OnFurnitureSpawned -= HandleSpawnedFurniture;
        }
        void HandleSpawnedFurniture(int furnitureId, GameObject furnitureObject)
        {
            if(subHandlersDict.TryGetValue(furnitureId, out IFurnitureSubHandler subHandler))
            {
                subHandler.HandleFurniture(furnitureObject);
            }
        }
    }
}
