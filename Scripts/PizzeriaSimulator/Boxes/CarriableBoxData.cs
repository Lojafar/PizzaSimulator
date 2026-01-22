namespace Game.PizzeriaSimulator.Boxes
{
    [System.Serializable]
    public class CarriableBoxData
    {
        public int DeliveryItemID;
        public int ItemsAmount;
        public bool IsOpened;
        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public float RotationX;
        public float RotationY;
        public float RotationZ;
        public CarriableBoxData() { }
        public CarriableBoxData(int _deliveryItemID, int _itemsAmount, bool _isOpened, float _positionX, float _positionY, float _positionZ, 
            float _rotationX, float _rotationY, float _rotationZ) 
        {
            DeliveryItemID = _deliveryItemID;
            ItemsAmount = _itemsAmount;
            IsOpened = _isOpened;
            PositionX = _positionX;
            PositionY = _positionY;
            PositionZ = _positionZ;
            RotationX = _rotationX;
            RotationY = _rotationY;
            RotationZ = _rotationZ;
        }
        public CarriableBoxData Clone()
        {
            return new CarriableBoxData(DeliveryItemID, ItemsAmount, IsOpened, PositionX, PositionY, PositionZ, RotationX, RotationY, RotationZ);
        }
    }
}
