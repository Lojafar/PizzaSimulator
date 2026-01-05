using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.PizzeriaSimulator.PizzaHold.Visual
{
    class PizzaHolderView : PizzaHolderViewBase
    {
        [SerializeField] GameObject pizzaPrefab;
        [SerializeField] float pizzaHeight;
        [SerializeField] Transform pizzaStartSpawnPos;
        Transform removedPizzasContainer;
        List<GameObject> spawnedPizzas;
        [Inject]
        void Construct(PizzeriaSceneReferences sceneReferences)
        {
            removedPizzasContainer = sceneReferences.RemovedPizzasContainer;
        }
        public override void Bind(PizzaHolderVM _viewModel)
        {
            spawnedPizzas = new List<GameObject>();
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
        void AddPizza()
        {
            GameObject spawnedPizza = Instantiate(pizzaPrefab, pizzaStartSpawnPos.position + new Vector3(0, pizzaHeight * spawnedPizzas.Count, 0), pizzaPrefab.transform.rotation);
            spawnedPizzas.Add(spawnedPizza);
        }  
        void RemovePizza(int index)
        {
            if (spawnedPizzas.Count <= index || index < 0) return;
            spawnedPizzas[index].transform.parent = removedPizzasContainer;
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