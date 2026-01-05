namespace Game.PizzeriaSimulator.Customers
{
    public enum CustomerState : byte
    {
        InLine = 0, MakesOrder = 1, WaitesOrder = 2, TakesOrder = 3, Leaves = 4
    }
}
