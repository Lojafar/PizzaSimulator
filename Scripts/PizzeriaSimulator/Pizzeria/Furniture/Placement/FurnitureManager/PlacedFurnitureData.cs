using System;

namespace Game.PizzeriaSimulator.Pizzeria.Furniture.Placement.Manager
{
    [Serializable]
    public struct PlacedFurnitureData
    {
        public int FurnitureID;
        public float PosX;
        public float PosY;
        public float PosZ;
        public float RotX;
        public float RotY;
        public float RotZ;
        public PlacedFurnitureData(int _id, float _posX, float _posY, float _posZ, float _rotX, float _rotY, float _rotZ)
        {
            FurnitureID = _id;
            PosX = _posX;
            PosY = _posY;
            PosZ = _posZ;
            RotX = _rotX;
            RotY = _rotY;
            RotZ = _rotZ;
        }

    }
}
