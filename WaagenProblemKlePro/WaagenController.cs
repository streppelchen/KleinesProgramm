using System;
using System.Collections.Generic;
using System.Linq;

namespace WaagenProblemKlePro
{
    class WaagenController<T> where T : IComparable<T>, IComparable
    {
        private readonly List<T> _gewichte;
        private readonly T _zuWiegen;
        public WaagenController(IEnumerable<T> gewichte,T zuWiegen)
        {
            _gewichte = gewichte.ToList();
            _zuWiegen = zuWiegen;
        }

        public string[] Ergebnis
        {
            get
            {
                return RecErgebnis(new List<Waage<T>>(), _zuWiegen, _gewichte.ToArray())
                    .Select(waage => waage.ToString())
                    .ToArray();
            }
        }

        private List<Waage<T>> RecErgebnis<T>(List<Waage<T>> waagen, T d, IList<T> gewichte,Waage<T> current=null) where T : IComparable<T>, IComparable
        {
            if (current == null) //falls wir keine waage übergeben bekommen haben sind wir am start der rekursion
            {
                for (var i = 0; i < gewichte.Count; i++)//für jedes gewicht eine neue waage erstellen, wo wir das gesuchte gewicht d auf die linke seite legen und das gewicht an der stelle i auf die linke oder rechte seite legen
                {
                    var w1 = new Waage<T>(new[] {d}, new []{gewichte[i]});
                    if(w1.Ausgeglichen())
                        waagen.Add(w1);//aktuelles ergebnis ist eine lösung. hinzufügen zur ergebnisliste
                    RecErgebnis(waagen, d, gewichte.FilterAndArray(gewichte[i]), w1);

                    var w2 = new Waage<T>(new[] { d,gewichte[i] }, new T[0]);
                    if (w2.Ausgeglichen())
                        waagen.Add(w2);//aktuelles ergebnis ist eine lösung. hinzufügen zur ergebnisliste
                    RecErgebnis(waagen, d, gewichte.FilterAndArray(gewichte[i]), w2);
                }
            }
            else
            {
                for (int i = 0; i < gewichte.Count; i++)
                {
                    var copy1 = current.Copy();//kopien erzeugen, da wir das originale ergebnis nicht verändern wollen (call by reference)
                    var copy2 = current.Copy();
                    copy1.LinkeSeite = copy1.LinkeSeite.Concat(new[] {gewichte[i]}).ToArray(); //linken seite das gewicht hinzufügen. zusätzlich aus der liste der gewichte entfernen
                    if (copy1.Ausgeglichen() && !waagen.Contains(copy1))//prüfen ob es ein ergebnis ist UND es noch nicht im ergebnis vorhanden ist (via Equals)
                        waagen.Add(copy1);//hinzufügen falls bedingung erfüllt
                    RecErgebnis(waagen, d, gewichte.FilterAndArray(gewichte[i]), copy1);//rekursiver aufruf, ohne das aktuelle gewicht in dem gewichte-array

                    copy2.RechteSeite = copy1.RechteSeite.Concat(new[] { gewichte[i] }).ToArray();//rechten seite das gewicht hinzufügen. zusätzlich aus der liste der gewichte entfernen. rest analog zu darüber
                    if (copy2.Ausgeglichen() && !waagen.Contains(copy2))
                        waagen.Add(copy2);
                    RecErgebnis(waagen, d, gewichte.FilterAndArray(gewichte[i]), copy2);

                }
            }
            return waagen;
        }


     

        internal class Waage<T> where T:IComparable<T>
        {
            public T[] LinkeSeite;
            public T[] RechteSeite;
            public Waage(T[] links,T[] rechts)
            {
                LinkeSeite = links;
                RechteSeite = rechts;
            }

            public bool Ausgeglichen()
            {
                if (typeof (T) == typeof (double))
                {
                    var tmplinks = (double[])((object)LinkeSeite);
                    var tmprechts = (double[])((object)RechteSeite); 
                    return tmplinks.Sum() == tmprechts.Sum();
                }

                if (typeof(T) == typeof(int))
                {
                    var tmplinks = (int[])((object)LinkeSeite);
                    var tmprechts = (int[])((object)RechteSeite);
                    return tmplinks.Sum() == tmprechts.Sum();
                }

                throw new Exception("Aktuell werden nur die Datentypen \"int\" und \"double\" unterstützt");
            }

            public override string ToString()
            {
                string links = "",rechts="";
                links = LinkeSeite.Aggregate(links, (current, d) => current + (d + " + "));
                rechts = RechteSeite.Aggregate(rechts, (current, d) => current + (d + " + "));
                return links.Substring(0,links.Length-3) + "= " + rechts.Substring(0,rechts.Length-3);
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                var newobj = obj as Waage<T>;
                if(newobj == null)
                    return false;
                return !LinkeSeite.Any(x => !newobj.LinkeSeite.Contains(x)) &&
                        RechteSeite.All(x => newobj.RechteSeite.Contains(x));
            }

            public Waage<T> Copy()
            {
                return new Waage<T>(LinkeSeite.ToArray(),RechteSeite.ToArray());
            }
        }
    }
}