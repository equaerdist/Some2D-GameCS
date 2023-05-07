using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Object
    {
        private double _hp;
        public double HP
        {
            get
            { return _hp; }
            set
            {
                if (value < 0) _hp = 0;
                else _hp = value;
            }
        }
    }
}
