using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders
{
    class Vecteur2D
    {
        /// <summary>
        /// Position X 
        /// </summary>
        private double x;

        /// <summary>
        /// Position Y
        /// </summary>
        private double y;

        /// <summary>
        /// Vector norm from top left corner of screen (0,0)
        /// </summary>
        private double norme;

        /// <summary>
        /// property of the variable norme
        /// </summary>
        public double Norme { get { return norme; } private set {  norme = value; } }

        /// <summary>
        /// property of the variable x
        /// </summary>
        public double X { get { return x; } set { x = value; } }

        /// <summary>
        /// property of the variable y
        /// </summary>
        public double Y { get { return y; } set { y = value; } }

        /// <summary>
        /// Creation of the Vector2D for the object
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vecteur2D(double x = 0, double y = 0) 
        {
            this.x = x;
            this.y = y;
            Norme = Math.Sqrt(Math.Pow(this.x,2) + Math.Pow(this.y,2));
        }

        /// <summary>
        /// Addtition of two vectors 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vecteur2D operator+(Vecteur2D v1, Vecteur2D v2) 
        {
            return new Vecteur2D(v1.x + v2.x, v1.y + v2.y);
        }

        /// <summary>
        /// Subtraction of two vectors 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vecteur2D operator -(Vecteur2D v1, Vecteur2D v2)
        {
            return new Vecteur2D(v1.x - v2.x, v1.y - v1.y);
        }
        /// <summary>
        /// Opposite of a vector 
        /// </summary>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vecteur2D operator -(Vecteur2D v1)
        {
            return new Vecteur2D(-v1.x, -v1.y);
        }

        /// <summary>
        /// Scalar product of a vector with a constant k
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Vecteur2D operator *(Vecteur2D v1, double k)
        {
            return new Vecteur2D(v1.x * k, v1.y * k);
        }
        /// <summary>
        /// Scalar product of a vector with a constant k
        /// </summary>
        /// <param name="k"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vecteur2D operator *(double k, Vecteur2D v1)
        {
            return new Vecteur2D(v1.x * k, v1.y * k);
        }
        /// <summary>
        /// Division with a k
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Vecteur2D operator /(Vecteur2D v1, double k)
        {
            return new Vecteur2D(v1.x / k, v1.y / k);
        }
    }
}
