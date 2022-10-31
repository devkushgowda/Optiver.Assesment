using Optiver.Assesment;
using System;

namespace Optiver
{
    class Program : DaysBetween
    {
        static void Main(string[] args)
        {
            DaysBetween.Test();
            SExpressionValidator.Test();
            Console.ReadKey();
        }
    }
}
