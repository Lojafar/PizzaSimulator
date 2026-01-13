using Game.PizzeriaSimulator.BoxCarry.Box.Item;
using UnityEngine;
namespace Game.PizzeriaSimulator.BoxCarry.Box
{
    public interface ICarriableBox
    {
        public CarriableBoxType BoxType { get; }
        public int ItemsAmount { get; }
        public bool IsOpened { get; }
        public void Open();
        public void Close();
        public void OnPicked();
        public void Throw(Vector3 forceVector);
        public BoxItemBase RemoveAndGetItem();
    }
}
