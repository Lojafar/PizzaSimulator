using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.PizzeriaSimulator.PizzaHold.Visual
{
    class PizzaHolderView : PizzaHolderViewBase
    {
        [SerializeField] PizzaBox pizzaBoxPrefab;
        [SerializeField] float pizzaHeight;
        [SerializeField] Transform pizzaStartSpawnPos;
        Transform removedPizzasContainer;
        List<PizzaBox> spawnedPizzas;
        [Inject]
        void Construct(PizzeriaSceneReferences sceneReferences)
        {
            removedPizzasContainer = sceneReferences.RemovedPizzasContainer;
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
        void AddPizza(Sprite pizzaIcon)
        {
            PizzaBox spawnedPizzaBox = Instantiate(pizzaBoxPrefab, pizzaStartSpawnPos.position + new Vector3(0, pizzaHeight * spawnedPizzas.Count, 0), pizzaBoxPrefab.transform.rotation);
            spawnedPizzaBox.SetPizzaIcon(pizzaIcon);
            spawnedPizzas.Add(spawnedPizzaBox);
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