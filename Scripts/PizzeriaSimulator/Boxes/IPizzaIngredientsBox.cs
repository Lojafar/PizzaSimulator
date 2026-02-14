using Game.PizzeriaSimulator.Boxes.Item.PizzaIngredient;
using Game.PizzeriaSimulator.PizzaCreation;
namespace Game.PizzeriaSimulator.Boxes
{
    public interface IPizzaIngredientsBox
    {
        public PizzaIngredientType IngredientType { get; }
        public PizzaIngredientBoxItemBase RemoveAndGetItem();
    }
}
