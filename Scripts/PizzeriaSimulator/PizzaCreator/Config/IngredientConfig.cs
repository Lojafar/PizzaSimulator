using System;
using Game.PizzeriaSimulator.PizzaCreation.Visual.IngredientOnPizza;
using UnityEngine;
namespace Game.PizzeriaSimulator.PizzaCreation.Config
{
    [Serializable]
    public class IngredientConfig
    {
        [field: SerializeField] public PizzaIngredientType IngredientType { get; private set; }
        [field: SerializeField] public int MaxAmountInHolder { get; private set; }
        [field: SerializeField] public GameObject InContainerPrefab { get; private set; }  
        [field: SerializeField] public Sprite IngredientIcon { get; private set; }  
        [field: SerializeField] public IngredientOnPizzaObjectBase OnPizzaPrefab { get; private set; }  
    }
}
