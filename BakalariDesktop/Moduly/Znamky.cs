using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BakalariDesktop.Moduly.Tridy;
using BakalariDesktop.Networking;
using BakalariDesktop.Properties;
using System.Globalization;

namespace BakalariDesktop.Moduly
{
    public class Znamky
    {
        private string token = null;
        private string url = null;

        public Znamky ()
        {
            token = Settings.Default.token;
            url = Settings.Default.url;

        }
        /// <summary>
        /// Vrátí posledních 5 známek.
        /// </summary>
        /// <param name="returnNonGraded">Vracet neoznámkované známky (s otazníkem)</param>
        /// <returns>List známek</returns>
        public Znamka[] GetNewGrades(bool returnNonGraded = false)
        {
            var timeWatch = System.Diagnostics.Stopwatch.StartNew();

            Networking.Networking networking = new Networking.Networking();
            dynamic response = networking.ParseXMLFromURL(url + "/login.aspx?hx=" + token + "&pm=znamky");

            if (response == null)
            {
                Console.Out.WriteLine("RESPONSE JE NULL");
                return null;
            }

            List<Znamka> allGrades = new List<Znamka>();

            dynamic predmety = response.results.predmety.predmet;

            for (int i = 0; i < predmety.Count; i++)
            {
                for (int z = 0; z < predmety[i].znamky.znamka.Count; z++)
                {
                    if (!returnNonGraded && predmety[i].znamky.znamka[z].znamka == "?")
                    {
                        continue;
                    }
                    Znamka currZnamka = new Znamka(predmety[i].znamky.znamka[z].znamka, predmety[i].nazev, predmety[i].znamky.znamka[z].caption,  DateTime.ParseExact(predmety[i].znamky.znamka[z].datum, "yyMd", System.Globalization.CultureInfo.InvariantCulture));
                    allGrades.Add(currZnamka);
                    
                    if (z >= 4) break;
                }
            }

            allGrades = allGrades.OrderByDescending(x => x.Datum).ToList();

            timeWatch.Stop();
            var elapsedMs = timeWatch.ElapsedMilliseconds;

            Console.Out.WriteLine("EXECUTED IN: " + elapsedMs);

            return allGrades.ToArray();
        }

        public double GetGradeAverage (string subject)
        {
            Networking.Networking networking = new Networking.Networking();
            dynamic response = networking.ParseXMLFromURL(url + "/login.aspx?hx=" + token + "&pm=znamky");

            if (response == null)
            {
                Console.Out.WriteLine("RESPONSE JE NULL");
                return 0; // chyba (pripojeni)
            }

            dynamic predmety = response.results.predmety.predmet;

            if (response.results == null)
            {
                return 0;
            }

            foreach (dynamic predmet in predmety)
            {
                if (predmet.nazev.ToLower() == subject.ToLower() || predmet.zkratka.ToLower() == subject.ToLower())
                {
                    return Convert.ToDouble(predmet.prumer, CultureInfo.GetCultureInfo("cs-CZ")); // desetinné tečky
                }
            }

            return 0; // chyba (predmet neexistuje)


        }

        /// <summary>
        /// Vrátí známky podle předmětu.
        /// </summary>
        /// <param name="subject">Předmět</param>
        /// <param name="maxGrades">Maximum vrácených známek</param>
        /// <param name="returnNonGraded">Vracet neoznámkované (?)</param>
        /// <returns></returns>
        public Znamka[] GetGradesBySubject(string subject, int maxGrades = 0, bool returnNonGraded = false)
        {
            Networking.Networking networking = new Networking.Networking();
            dynamic response = networking.ParseXMLFromURL(url + "/login.aspx?hx=" + token + "&pm=znamky");

            if (response == null)
            {
                Console.Out.WriteLine("RESPONSE JE NULL");
                return null;
            }

            List<Znamka> allGrades = new List<Znamka>();

            dynamic predmety = response.results.predmety.predmet;

            if (response.results == null)
            {
                return null;
            }

            foreach (dynamic predmet in predmety)
            {
                if ( predmet.nazev.ToLower() == subject.ToLower() || predmet.zkratka.ToLower() == subject.ToLower() )
                {
                    int gradeCount = 0;
                    foreach (dynamic znamka in predmet.znamky.znamka)
                    {
                        gradeCount++;

                        if (maxGrades > 0 && maxGrades < gradeCount)
                        {
                            if (!returnNonGraded && znamka.znamka == "?")
                            {

                            }
                            else
                            {
                                break;
                            }
                        }

                        if (!returnNonGraded && znamka.znamka == "?")
                        {
                            gradeCount--;
                            continue;
                        }
                        

                        Znamka currZnamka = new Znamka(znamka.znamka, predmet.nazev, znamka.caption, DateTime.ParseExact(znamka.datum, "yyMd", System.Globalization.CultureInfo.InvariantCulture));
                        allGrades.Add(currZnamka);
                    }
                }
            }

            return allGrades.ToArray();

        }
    }
}
