using UnityEngine;
namespace Game.PizzeriaSimulator.Boxes
{
    public abstract class CarriableBoxBase : MonoBehaviour
    {
        [SerializeField] protected CarriableBoxData boxData = new();
        public uint BoxObjectID { get; protected set; }
        public virtual CarriableBoxType BoxType { get; protected set; }
        public int ItemsAmount => boxData.ItemsAmount;
        public bool IsOpened => boxData.IsOpened;
        public virtual CarriableBoxData GetBoxData()
        {
            boxData.PositionX = transform.position.x;
            boxData.PositionY = transform.position.y;
            boxData.PositionZ = transform.position.z;
            boxData.RotationX = transform.eulerAngles.x;
            boxData.RotationY = transform.eulerAngles.y;
            boxData.RotationZ = transform.eulerAngles.z;
            return boxData.Clone();
        }
        public virtual void SetObjectId(uint boxObjectID)
        {
            BoxObjectID = boxObjectID;
        }
        public virtual void SetBoxData(CarriableBoxData _boxData)
        {
            boxData = _boxData;
        }
        public abstract void Open();
        public abstract void Close();
        public abstract void OnPicked();
        public abstract void Throw(Vector3 forceVector);
    }
}
