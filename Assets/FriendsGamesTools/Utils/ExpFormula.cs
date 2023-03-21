using System;

namespace FriendsGamesTools
{
    [Serializable]
    public class ExpFormula
    {
        public double Default, Base, Coef;
        public double GetValue(double power) => Get(Default, Base, Coef, power);
        public static double Get(double Default, double Base, double Coef, double power)
            => Default + Base * Math.Pow(Coef, power);
        public static double Get(double startValue, double coef, int currLevel)
            => Get(0, startValue, coef, currLevel - 1);
    }
}
