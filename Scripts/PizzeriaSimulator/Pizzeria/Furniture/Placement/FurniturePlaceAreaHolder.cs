using UnityEngine;
namespace Game.PizzeriaSimulator.Pizzeria.Furniture.Placement
{
    public sealed class FurniturePlaceAreaHolder : MonoBehaviour
    {
        [SerializeField] FurniturePlaceArea extraPlaceArea;
        [SerializeField] FurniturePlaceArea[] placeAreas;
        private void Awake()
        {
            extraPlaceArea.gameObject.SetActive(false);
            for (int i = 0; i < placeAreas.Length; i++)
            {
                placeAreas[i].gameObject.SetActive(false);
            }
        }
        public FurniturePlaceArea GetPlaceAreaById(int id)
        {
            if (id < 0 || id >= placeAreas.Length) return extraPlaceArea;
            return placeAreas[id];
        }
        public void OverridePlaceArea(int id, FurniturePlaceArea area)
        {
            if (id >= placeAreas.Length)
            {
                UnityEngine.Debug.Log($"Id of override place area is bigger that array size! Make sure that you checked all expansions! ID is {id}");
                return;
            }
            if (id < 0)
            {
                bool extraAreaWasActive = placeAreas[id].gameObject.activeInHierarchy;
                extraPlaceArea.gameObject.SetActive(false);
                extraPlaceArea = area;
                extraPlaceArea.gameObject.SetActive(extraAreaWasActive);
                return;
            }
            bool areaWasActive = placeAreas[id].gameObject.activeInHierarchy;
            placeAreas[id].gameObject.SetActive(false);
            placeAreas[id] = area;
            placeAreas[id].gameObject.SetActive(areaWasActive);
        }
    }
}
