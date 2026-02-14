using Game.PizzeriaSimulator.Boxes.Carry;
using Game.PizzeriaSimulator.Interactions;
using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.SaveLoadHelp;
using Game.Root.ServicesInterfaces;
using Cysharp.Threading.Tasks;
using System;

namespace Game.PizzeriaSimulator.SodaMachine
{
    public class PizzeriaSodaMachine : ISceneDisposable
    {
        public Action OnEnterSodaMake;
        public Action OnLeaveSodaMake;
        public Action OnNewCupAdded;
        public Action OnNextCupForced;
        public Action<int> OnCupFillStarted;
        public Action<int> OnCupFilled;
        public Action<int> OnFilledCupRemoved;
        readonly Interactor interactor;
        readonly BoxesCarrier boxesCarrier;
        readonly PizzeriaSaveLoadHelper saveLoadHelper;
        PizzeriaSodaMachineData sodaMachineData;
        int fillingSlotsMask;
        int filledSlotsMask;
        int readySodaCupsCount;
        bool entered;
        public const int MaxEmptyCups = 12;
        public const int FillableSlotsCount = 4;
        public PizzeriaSodaMachine(Interactor _interactor, BoxesCarrier _boxesCarrier, PizzeriaSaveLoadHelper _saveLoadHelper)
        {
            interactor = _interactor;
            boxesCarrier = _boxesCarrier;
            saveLoadHelper = _saveLoadHelper;
        }
        public async UniTask Init()
        {
            sodaMachineData = await saveLoadHelper.LoadOrTryGetInitData<PizzeriaSodaMachineData>();
            filledSlotsMask = sodaMachineData.FilledSlotsMask;
            int i; 
            for (i = 0; i < FillableSlotsCount; i++)
            {
                if (CheckBitOfMask(filledSlotsMask, i))
                {
                    readySodaCupsCount++;
                    OnNewCupAdded?.Invoke();
                    OnNextCupForced?.Invoke();
                    OnCupFilled?.Invoke(i);
                }
            }
            for (i = 0; i < sodaMachineData.EmptyCupsAmount; i++)
            {
                OnNewCupAdded?.Invoke();
            }
            interactor.OnInteract += HandleInteraction;
        }
        public void Dispose()
        {
            interactor.OnInteract -= HandleInteraction;
        }
        void HandleInteraction(InteractableType interactableType)
        {
            if (boxesCarrier.IsCarryingBox) return;
            if (interactableType == InteractableType.SodaMachine)
            {
                Enter();
            }
        }
        public void Enter()
        {
            if (entered) return;
            entered = true;
            OnEnterSodaMake?.Invoke();
        }
        public void LeaveInput()
        {
            if (!entered) return;
            entered = false;
            OnLeaveSodaMake?.Invoke();
        }
        public void FillCupInput(int slotId)
        {
            if (sodaMachineData.EmptyCupsAmount < 1 || slotId < 0 || slotId >= FillableSlotsCount
                || CheckBitOfMask(filledSlotsMask, slotId) || CheckBitOfMask(fillingSlotsMask, slotId)) return;

            fillingSlotsMask |= 1 << slotId;
            sodaMachineData.EmptyCupsAmount--;
            OnCupFillStarted?.Invoke(slotId);
            SaveSodaMachineData();
        }
        public void CupFilledInput(int slotId)
        {
            if (slotId < 0 || slotId >= FillableSlotsCount || !CheckBitOfMask(fillingSlotsMask, slotId)) return;
            fillingSlotsMask &= ~(1 << slotId);
            filledSlotsMask |= 1 << slotId;
            readySodaCupsCount++;
            OnCupFilled?.Invoke(slotId);
            SaveSodaMachineData();
        }
        public bool TryAddEmptyCup()
        {
            if (sodaMachineData.EmptyCupsAmount + 1 <= MaxEmptyCups)
            {
                sodaMachineData.EmptyCupsAmount++;
                SaveSodaMachineData();
                return true;
            }
            return false;
        }
        public bool TryReserveSodaCup()
        {
            if (readySodaCupsCount > 0)
            {
                readySodaCupsCount--;
                return true;
            }
            return false;
        }
        public void RemoveReservedCup()
        {
            for(int i =0; i < FillableSlotsCount; i++)
            {
                if(CheckBitOfMask(filledSlotsMask, i))
                {
                    RemoveCup(i);
                    break;
                }
            }
        }
        void RemoveCup(int slotId)
        {
            filledSlotsMask &= ~(1 << slotId);
            OnFilledCupRemoved?.Invoke(slotId);
            SaveSodaMachineData();
        }
        public bool IsSlotBusy(int slotId)
        {
            if (slotId < 0 || slotId >= FillableSlotsCount) return true;
            return CheckBitOfMask(fillingSlotsMask, slotId) || CheckBitOfMask(filledSlotsMask, slotId);
        }
        bool CheckBitOfMask(int mask, int bitIndex)
        {
            return (mask & (1 << bitIndex)) != 0;
        }
        void SaveSodaMachineData()
        {
            sodaMachineData.FilledSlotsMask = filledSlotsMask | fillingSlotsMask;
            saveLoadHelper.SaveData(sodaMachineData).Forget();
        }
    }
}
