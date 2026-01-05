using System;
namespace Game.PizzeriaSimulator.PaymentReceive
{
    [Serializable]
    public struct PaymentPrice
    {
        public int Dollars;
        public int Cents;
        public PaymentPrice(int _dollars, int _cents)
        {
            Dollars = _dollars;
            Cents = _cents;
        }
    }
}
