using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BakalariDesktop.Moduly.Tridy
{
    public class Znamka
    {
        private string _hodnoceni;
        private string _predmet;
        private string _tema;
        private DateTime _datum;

        public string Hodnoceni
        {
            get
            {
                return _hodnoceni;
            }
            
            set
            {
                _hodnoceni = value;
            }
        }
        public string Predmet
        {
            get
            {
                return _predmet;
            }

            set
            {
                _predmet = value;
            }
        }
        public string Tema
        {
            get
            {
                return _tema;
            }

            set
            {
                _tema = value;
            }
        }
        public DateTime Datum
        {
            get
            {
                return _datum;
            }

            set
            {
                _datum = value;
            }
        }

        public Znamka (string hodnoceni, string predmet, string tema, DateTime datum)
        {
            _hodnoceni = hodnoceni;
            _predmet = predmet;
            _tema = tema;
            _datum = datum;
        }

    }
}
