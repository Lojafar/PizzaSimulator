using Game.PizzeriaSimulator.PizzaCreation;
using UnityEngine;
namespace Game.PizzeriaSimulator.Boxes
{
    public abstract class PizzaIngredientBoxBase : CarriableBoxBase
    {
       [field: SerializeField] public PizzaIngredientType IngredientType { get; protected set; }
    }
}
