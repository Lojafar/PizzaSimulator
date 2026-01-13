using Game.PizzeriaSimulator.PizzaCreation;

namespace Game.PizzeriaSimulator.BoxCarry.Box
{
   public interface IPizzaIngredientBox : ICarriableBox
   {
        public PizzaIngredientType IngredientType { get; }
   }
}
