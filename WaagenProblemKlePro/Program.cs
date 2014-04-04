using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WaagenProblemKlePro
{
    class Program
    {
        static void Main()
        {
            Console.Write("Bitte Gewichte durch Leerzeichen getrennt eingeben und mit Enter bestätigen:");
#if DEBUG
            const string line = "1 3 7 15";
#else
            var line=Console.ReadLine();
#endif
            var gewichte = line.ParseGewichteInt();
            
            

#if DEBUG
            const string gewicht = "12";
#else
            var gewicht=Console.ReadLine();
#endif
            var w = new WaagenController<int>(gewichte, int.Parse(gewicht));
            var erg = w.Ergebnis;
            Console.WriteLine("\r\nErgebnis(se):");
            foreach (var ergebnisZeile in erg)
            {
                Console.WriteLine(ergebnisZeile);
            }

            Console.ReadLine();
        }
    }

    public static class Extenstions
    {

        public static int[] ParseGewichteInt(this string input)
        {
            return input.Split(' ').Select(x => int.Parse(x, CultureInfo.CurrentCulture)).ToArray();
        }
        public static double[] ParseGewichteDouble(this string input)
        {
            return input.Split(' ').Select(x => double.Parse(x, CultureInfo.CurrentCulture)).ToArray();
        }

        public static T[] FilterAndArray<T>(this IEnumerable<T> enumerable,T notIncluded) where T:IComparable
        {
            var enumerable1 = enumerable as T[] ?? enumerable.ToArray();
            var ret = new T[enumerable1.Count()-1];//da alle gewichte nur ein mal vorkommen dürfen
            var i = 0;
            foreach (var current in enumerable1.Where(current => !current.Equals(notIncluded)))
            {
                ret[i++] = current;
            }
            return ret;
        }
    }
}
