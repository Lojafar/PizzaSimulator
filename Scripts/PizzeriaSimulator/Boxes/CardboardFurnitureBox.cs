using Game.PizzeriaSimulator.Boxes.Item.Furniture;
using UnityEngine;
namespace Game.PizzeriaSimulator.Boxes
{
    sealed class CardboardFurnitureBox : CardboardCarriableBoxBase, IFurnitureBox
    {
        [field: SerializeField] public int FurnitureID { get; private set; }
        [SerializeField] FurnitureBoxItemBase furnitureItem;
        public override CarriableBoxType BoxType => CarriableBoxType.FurnitureBox;
        protected override void Awake()
        {
            base.Awake();
            if (furnitureItem != null)
            {
                furnitureItem.transform.parent = null;
                furnitureItem.gameObject.SetActive(false);
            }
            boxData.ItemsAmount = 1;
            boxData.IsOpened = false;
        }
        public override void SetBoxData(CarriableBoxData _boxData)
        {
            base.SetBoxData(_boxData);
            if (ItemsAmount == 0 ) 
            {
                if (furnitureItem != null) Destroy(furnitureItem.gameObject);
                if (openVisuals != null) Destroy(openVisuals);
            }
        }
        public override void Open()
        {
            base.Open();
            if (furnitureItem != null) furnitureItem.gameObject.SetActive(true);
        }
        public override void Close()
        {
            base.Close();
            if (furnitureItem != null) furnitureItem.gameObject.SetActive(false);
        }
        public override void OnPicked()
        {
            base.OnPicked();
            if (IsOpened && furnitureItem != null)
            {
                furnitureItem.gameObject.SetActive(true);
            }
        }
        public override void Throw(Vector3 forceVector)
        {
            base.Throw(forceVector);
            if (furnitureItem != null)
            {
                furnitureItem.gameObject.SetActive(false);
            }
        }
        public FurnitureBoxItemBase GetFurnitureItem()
        {
            return furnitureItem;
        }
        public void RemoveItem()
        {
            boxData.ItemsAmount--;
            if(furnitureItem != null ) Destroy(furnitureItem.gameObject);
            if(openVisuals != null ) Destroy(openVisuals);
        }
    }
}
