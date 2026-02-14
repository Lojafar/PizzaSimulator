using Game.Root.ServicesInterfaces;
using R3;
using System;

namespace Game.PizzeriaSimulator.SodaMachine.Visual
{
    sealed class PizzeriaSodaMachineVM : ISceneDisposable
    {
        public event Action EnterMachine;
        public event Action LeaveMachine;
        public event Action SpawnNewCup;
        public event Action<int, Action> FillCup;
        public event Action<int> RemoveFilledCup;
        public event Action<int> ForceCupFill;
        public event Action<int, bool> ActivateFillInput;
        public Subject<Unit> LeaveInput;
        public Subject<int> FillCupInput;
        readonly PizzeriaSodaMachine sodaMachine;
        readonly CompositeDisposable inputDisposables;
        bool nextFillForced;
        public const int FillableSlotsCount = PizzeriaSodaMachine.FillableSlotsCount;
        public PizzeriaSodaMachineVM(PizzeriaSodaMachine _sodaMachine)
        {
            sodaMachine = _sodaMachine;
            LeaveInput = new Subject<Unit>();
            FillCupInput = new Subject<int>();
            inputDisposables = new CompositeDisposable();
        }
        public void Init()
        {
            sodaMachine.OnEnterSodaMake += HandleEnter;
            sodaMachine.OnLeaveSodaMake += HandleLeave;
            sodaMachine.OnNewCupAdded += HandleNewCup;
            sodaMachine.OnCupFillStarted += HandleCupFillStart;
            sodaMachine.OnFilledCupRemoved += HandleFilledCupRemove;
            sodaMachine.OnNextCupForced += HandleCupFillForce;
            sodaMachine.OnCupFilled += HandleCupFill;
            LeaveInput.ThrottleFirst(TimeSpan.FromSeconds(0.2)).Subscribe(_ => sodaMachine.LeaveInput()).AddTo(inputDisposables);
            FillCupInput.ThrottleFirst(TimeSpan.FromSeconds(0.05)).Subscribe(sodaMachine.FillCupInput).AddTo(inputDisposables);
            for (int i = 0; i < FillableSlotsCount; i++) 
            {
                ActivateFillInput?.Invoke(i, true);
            }
        }
        public void Dispose()
        {
            sodaMachine.OnEnterSodaMake -= HandleEnter;
            sodaMachine.OnLeaveSodaMake -= HandleLeave;
            sodaMachine.OnNewCupAdded -= HandleNewCup;
            sodaMachine.OnCupFillStarted -= HandleCupFillStart;
            sodaMachine.OnFilledCupRemoved -= HandleFilledCupRemove;
            sodaMachine.OnNextCupForced -= HandleCupFillForce;
            sodaMachine.OnCupFilled -= HandleCupFill;
            inputDisposables.Dispose();
            LeaveInput.Dispose();
            FillCupInput.Dispose();
        }
        void HandleEnter()
        {
            EnterMachine?.Invoke();
        }
        void HandleLeave()
        {
            LeaveMachine?.Invoke();
        }
        void HandleNewCup()
        {
            SpawnNewCup?.Invoke();
        }
        void HandleCupFillStart(int slotId)
        {
            FillCup?.Invoke(slotId, () => sodaMachine.CupFilledInput(slotId));
            UpdateSlotsActivity();
        }
        void HandleFilledCupRemove(int slotId) 
        {
            RemoveFilledCup?.Invoke(slotId);
            UpdateSlotsActivity();
        }
        void HandleCupFillForce()
        {
            nextFillForced = true;
        }
        void HandleCupFill(int slotId)
        {
            if (!nextFillForced) return;
            nextFillForced = false;
            ForceCupFill?.Invoke(slotId);
            UpdateSlotsActivity();
        }
        void UpdateSlotsActivity()
        {
            for (int i = 0; i < FillableSlotsCount; i++)
            {
                ActivateFillInput?.Invoke(i, !sodaMachine.IsSlotBusy(i));
            }
        }
    }
}
