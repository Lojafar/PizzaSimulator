using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Currency
{
    [Serializable]
    public struct MoneyQuantity
    {
        public int Dollars;
        public int Cents;
        public readonly int AllCents => Dollars * CentsInDollar + Cents;
        public const int CentsInDollar = 100;
        public MoneyQuantity(int _dollars, int _cents)
        {
            Dollars = _dollars;
            if(_cents >= CentsInDollar)
            {
                _cents = 0;
            }
            Cents = _cents;
        }
        public static MoneyQuantity operator *(MoneyQuantity quantity, int amount)
        {
            int allCents = quantity.AllCents * amount;
            int dollars = Mathf.FloorToInt((float)allCents / CentsInDollar);
            return new MoneyQuantity(dollars, allCents - dollars * CentsInDollar);
        }
        public static MoneyQuantity operator +(MoneyQuantity quantityOne, MoneyQuantity quantityTwo)
        {
            int allCents = (quantityOne.Dollars + quantityTwo.Dollars) * CentsInDollar + quantityOne.Cents + quantityTwo.Cents;
            int dollars = Mathf.FloorToInt((float)allCents / CentsInDollar);
            return new MoneyQuantity(dollars, allCents - dollars * CentsInDollar);
        }
        public static MoneyQuantity operator -(MoneyQuantity quantityOne, MoneyQuantity quantityTwo)
        {
            int allCents = (quantityOne.Dollars - quantityTwo.Dollars) * CentsInDollar + quantityOne.Cents - quantityTwo.Cents;
            int dollars = Mathf.FloorToInt((float)allCents / CentsInDollar);
            return new MoneyQuantity(dollars, allCents - dollars * CentsInDollar);
        }
    }
}
