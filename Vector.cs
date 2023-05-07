using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Vector
    {
        public override string ToString()
        {
            return $"X is {X}, Y is {Y}";
        }
        public double X; public double Y;
        public Vector(double x, double y)
        {
            X = x ;
            Y = y ;
        }
        public Vector()
        {
            X = 0;
            Y = 0 ;
        }
        public double Lenght { get { return Math.Sqrt(X * X + Y * Y); } }
        public static Vector operator+(Vector a, Vector b)
        {
            return new Vector() { X= a.X + b.X, Y= a.Y + b.Y };
        }
        public static Vector operator -(Vector a, Vector b)
        {
            return a + new Vector() { X = b.X * -1, Y = b.Y * -1 };
        }
        public static Vector operator*(Vector a, double value)
        {
            return new Vector() { X = a.X * value, Y = a.Y * value };
        }
        public override bool Equals(object obj)
        {
            var temporary = obj as Vector;
            if(temporary != null)
            {
                return X.Equals(temporary.X) && Y.Equals(temporary.Y);
            }
            return false;
        }
    }
}
