using Game.PizzeriaSimulator.Orders.ObjsContainer;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.PizzeriaSimulator.PizzaHold.Visual
{
    sealed class PizzaHolderView : PizzaHolderViewBase
    {
        [SerializeField] PizzaBox pizzaBoxPrefab;
        [SerializeField] float pizzaHeight;
        [SerializeField] Transform pizzaStartSpawnPos;
        OrderItemsObjsContainer orderItemsContainer;
        List<PizzaBox> spawnedPizzas;
        [Inject]
        void Construct(PizzeriaSceneReferences sceneReferences)
        {
            orderItemsContainer = sceneReferences.RemovedOrderItemsContainer;
        }
        public override void Bind(PizzaHolderVM _viewModel)
        {
            spawnedPizzas = new List<PizzaBox>();
            base.Bind(_viewModel);
            viewModel.AddPizza += AddPizza;
            viewModel.RemovePizza += RemovePizza;
        }
        private void OnDestroy()
        {
            if(viewModel != null)
            {
                viewModel.AddPizza -= AddPizza;
                viewModel.RemovePizza -= RemovePizza;
            }
        }
        void AddPizza(int pizzaId, Sprite pizzaIcon)
        {
            PizzaBox spawnedPizzaBox = Instantiate(pizzaBoxPrefab, pizzaStartSpawnPos.position + new Vector3(0, pizzaHeight * spawnedPizzas.Count, 0), pizzaBoxPrefab.transform.rotation);
            spawnedPizzaBox.SetPizzaId(pizzaId);
            spawnedPizzaBox.SetPizzaIcon(pizzaIcon);
            spawnedPizzas.Add(spawnedPizzaBox);
        }  
        void RemovePizza(int index)
        {
            if (spawnedPizzas.Count <= index || index < 0) return;
            orderItemsContainer.AddPizza(spawnedPizzas[index]);
            spawnedPizzas.RemoveAt(index);
            UpdatePizzasPos();
        }
        void UpdatePizzasPos()
        {
            for (int i = 0; i < spawnedPizzas.Count; i++) 
            {
                spawnedPizzas[i].transform.position = new Vector3(pizzaStartSpawnPos.position.x, pizzaStartSpawnPos.position.y + pizzaHeight * i, pizzaStartSpawnPos.position.z);
            }
        }
    }
}