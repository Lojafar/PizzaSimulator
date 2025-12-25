using Game.PizzeriaSimulator.OrdersHandle;
using UnityEngine;

namespace Assets.Game.Scripts.PizzeriaSimulator.OrdersHandler
{
    public class OrderGIver : MonoBehaviour
    {
        PizzeriaOrdersHandler ordersHandler;
        [SerializeField] int pizzaID;
        [SerializeField] int pizzaID2 = 1;
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.X)) ordersHandler.Order(pizzaID);
            if (Input.GetKeyUp(KeyCode.Z)) ordersHandler.Order(pizzaID2);
        }
        public void Init(PizzeriaOrdersHandler _ordersHandler)
        {
            ordersHandler = _ordersHandler;
        }
    }
}
