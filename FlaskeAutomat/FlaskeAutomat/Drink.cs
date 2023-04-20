using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaskeAutomat
{
    public abstract class Drink
    {
        protected string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        protected int pant;
    }
    class EnergiDrink : Drink
    {
        private int Caffeine;
        public EnergiDrink(string name)
        {
            _name = name;
        }
    }
    class Beer : Drink
    {
        private int alcoholProcent;
        public Beer(string name)
        {
            _name = name;
        }
    }
}
