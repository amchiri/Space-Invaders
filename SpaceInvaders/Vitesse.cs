using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders
{
    class Vitesse
    {
        public double Speed;

        public bool BigMissile = false;

        public bool RootMissile = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="speed"></param>
        public Vitesse(double speed)
        {
            this.Speed = speed;
        }

        /// <summary>
        /// Used to increase the speed
        /// </summary>
        /// <param name="speed"></param>
        public void SetValueM(double nouvelleValeur)
        {
            this.Speed *= nouvelleValeur;
        }

        /// <summary>
        /// multiplication of two vitesse
        /// </summary>
        /// <param name="v1">vitesse one</param>
        /// <param name="v2">vitesse two</param>
        public static double operator +(Vitesse v1, Vitesse v2)
        {
            return v1.Speed + v2.Speed;
        }

        /// <summary>
        /// Scalar product of a vitesse with a constant
        /// </summary>
        /// <param name="i1">constant</param>
        /// <param name="v2">vitesse</param>
        public static double operator *(int i1, Vitesse v2)
        {
            return  (i1 * v2.Speed);
        }

        /// <summary>
        /// Scalar product of a vitesse with a constant
        /// </summary>
        /// <param name="d1">constant</param>
        /// <param name="v2">vitesse</param>
        public static double operator *(double d1, Vitesse v2)
        {
            return (d1 * v2.Speed);
        }

        /// <summary>
        /// Scalar product of a vitesse with a constant
        /// </summary>
        /// <param name="i1">constant</param>
        /// <param name="v1">vitesse</param>
        public static double operator *(Vitesse v1, int i1)
        {
            return (i1 * v1.Speed);
        }

        /// <summary>
        /// Scalar product of a vitesse with a constant
        /// </summary>
        /// <param name="d1">constant</param>
        /// <param name="v1">vitesse</param>
        public static double operator *(Vitesse v1, double d1)
        {
            return (d1 * v1.Speed);
        }

        /// <summary>
        /// Addtition of a constant and a vitesse
        /// </summary>
        /// <param name="i1">constant</param>
        /// <param name="v1">vitesse</param>
        public static double operator +(Vitesse v1, int i1)
        {
            return (i1 + v1.Speed);
        }

        /// <summary>
        /// Addtition of a constant and a vitesse
        /// </summary>
        /// <param name="i1">constant</param>
        /// <param name="v1">vitesse</param>
        public static double operator +(Vitesse v1, double i1)
        {
            return (i1 + v1.Speed);
        }

        /// <summary>
        /// Addtition of a constant and a vitesse
        /// </summary>
        /// <param name="i1">constant</param>
        /// <param name="v1">vitesse</param>
        public static double operator +(Vitesse v1, float i1)
        {
            return (i1 + v1.Speed);
        }

        /// <summary>
        /// Addtition of a constant and a vitesse
        /// </summary>
        /// <param name="i1">constant</param>
        /// <param name="v1">vitesse</param>
        public static double operator +(int i1, Vitesse v1)
        {
            return (i1 + v1.Speed);
        }

        /// <summary>
        /// Addtition of a constant and a vitesse
        /// </summary>
        /// <param name="i1">constant</param>
        /// <param name="v1">vitesse</param>
        public static double operator +(double i1, Vitesse v1)
        {
            return (i1 + v1.Speed);
        }

        /// <summary>
        /// Addtition of a constant and a vitesse
        /// </summary>
        /// <param name="i1">constant</param>
        /// <param name="v1">vitesse</param>
        public static double operator +(float i1, Vitesse v1)
        {
            return (i1 + v1.Speed);
        }

    }
}
