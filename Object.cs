using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Object
    {
        protected double _hp;
        public virtual double HP
        {
            get
            { return _hp; }
            set
            {
                if (value < 0) _hp = 0;
                else if(value > 100) _hp = 100;
                else _hp = value;
            }
        }
    }
}
