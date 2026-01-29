using Game.Root.ServicesInterfaces;
using Game.PizzeriaSimulator.Boxes.Carry;
using Game.PizzeriaSimulator.Delivery.Config;
using Game.PizzeriaSimulator.SaveLoadHelp;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Game.PizzeriaSimulator.Boxes.Manager
{
    using Object = UnityEngine.Object;
    public class BoxesManager : IPrewarmable, IInittable, IDisposable
    {
        public int InitPriority => 9;
        readonly PizzeriaSaveLoadHelper saveLoadHelper;
        readonly BoxesCarrier boxesCarrier;
        readonly PizzeriaDeliveryConfig deliveryConfig;
        readonly Dictionary<uint, CarriableBoxBase> boxesObjectsDict;
        BoxesManagerData boxesManagerData;
        uint lastAddedBoxId = 0;
        const float throwSaveDelay = 1f;
        public BoxesManager(PizzeriaSaveLoadHelper _saveLoadHelper, BoxesCarrier _boxesCarrier, PizzeriaDeliveryConfig _deliveryConfig)
        {
            saveLoadHelper = _saveLoadHelper;
            boxesCarrier = _boxesCarrier;
            deliveryConfig = _deliveryConfig;
            boxesObjectsDict = new Dictionary<uint, CarriableBoxBase>();
        }
        public async UniTask Prewarm()
        {
            boxesManagerData = await saveLoadHelper.LoadData<BoxesManagerData>();
            boxesManagerData ??= new BoxesManagerData();
            CarriableBoxData boxData;
            DeliveryItemConfig deliveryItemConfig;
            for (int i = 0; i < boxesManagerData.ActiveIDs.Count; i++)
            {
                boxData = await saveLoadHelper.LoadData<CarriableBoxData>(boxesManagerData.ActiveIDs[i].ToString());
                if (boxData == null)
                {
                    Debug.LogError($"Saved in manager boxID({boxesManagerData.ActiveIDs[i]}) isn't exist!!!!");
                    continue;
                }
                lastAddedBoxId = boxesManagerData.ActiveIDs[i];
                deliveryItemConfig = deliveryConfig.GetDeliveryItemConfig(boxData.DeliveryItemID);
                if (deliveryItemConfig != null)
                {
                    CarriableBoxBase spawnedBox = Object.Instantiate(deliveryItemConfig.BoxPrefab, new Vector3(boxData.PositionX, boxData.PositionY, boxData.PositionZ),
                        Quaternion.Euler(boxData.RotationX, boxData.RotationY, boxData.RotationZ));
                    spawnedBox.SetObjectId(lastAddedBoxId);
                    spawnedBox.SetBoxData(boxData);
                    boxesObjectsDict.Add(lastAddedBoxId, spawnedBox);
                }
                else
                {
                    Debug.LogError($"Saved box has incorrect id: {boxData.DeliveryItemID} !!!!");
                }
            }
        }
        public void Init()
        {
            boxesCarrier.OnBoxThrowed += HandleBoxThrow;
            boxesCarrier.OnBoxRemoved += HandleBoxDestroy;
        }
        public void Dispose()
        {
            boxesCarrier.OnBoxThrowed -= HandleBoxThrow;
            boxesCarrier.OnBoxRemoved -= HandleBoxDestroy;
        }
        public void AddNewBox(CarriableBoxBase carriableBox)
        {
            lastAddedBoxId++;
            boxesManagerData.ActiveIDs.Add(lastAddedBoxId);
            carriableBox.SetObjectId(lastAddedBoxId);
            boxesObjectsDict.Add(lastAddedBoxId, carriableBox);
            saveLoadHelper.SaveData(carriableBox.GetBoxData(), lastAddedBoxId.ToString()).Forget();
        }
        public void FinishAddBoxes()
        {
            saveLoadHelper.SaveData(boxesManagerData).Forget();
        }
        async void HandleBoxThrow(uint boxId)
        {
            if(boxesObjectsDict.TryGetValue(boxId, out CarriableBoxBase box))
            {
                await UniTask.WaitForSeconds(throwSaveDelay);
                saveLoadHelper.SaveData(box.GetBoxData(), boxId.ToString()).Forget();
            }
        }
        void HandleBoxDestroy(uint boxId)
        {
            boxesObjectsDict.Remove(boxId);
            boxesManagerData.ActiveIDs.Remove(boxId);
            saveLoadHelper.ClearData<CarriableBoxData>(boxId.ToString()).Forget();
            saveLoadHelper.SaveData(boxesManagerData).Forget();
        }
    }
}
