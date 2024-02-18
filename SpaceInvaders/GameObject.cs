using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SpaceInvaders
{
    /// <summary>
    /// This is the generic abstact base class for any entity in the game
    /// </summary>
    abstract class GameObject
    {
        /// <summary>
        /// L'enumeration Side qui indique de quel coté est l'objet
        /// </summary>
        public enum Side
        {
            Ally,
            Enemy,
            Neutral,
            PLayerOne,
            PLayerTwo,
        }

        public enum BonusType
        {
            DoubleSpeed = 0,
            BigMissile = 1,
            RootMissile = 2,          
        }

        /// <summary>
        /// la variable camp propre a chaque objet 
        /// </summary>
        private Side camp;

        public Side Camp { get { return camp; } set { camp = value; } }
        public GameObject(Side s)
        {
            this.camp = s;
        }

        /// <summary>
        /// Update the state of a game objet
        /// </summary>
        /// <param name="gameInstance">instance of the current game</param>
        /// <param name="deltaT">time ellapsed in seconds since last call to Update</param>
        public abstract void Update(Game gameInstance, double deltaT);



        /// <summary>
        /// Render the game object
        /// </summary>
        /// <param name="gameInstance">instance of the current game</param>
        /// <param name="graphics">graphic object where to perform rendering</param>
        public abstract void Draw(Game gameInstance, Graphics graphics);

        /// <summary>
        /// Determines if object is alive. If false, the object will be removed automatically.
        /// </summary>
        /// <returns>Am I alive ?</returns>
        public abstract bool IsAlive();

        /// <summary>
        /// Determines if there's a colision's object. If true the live's object will be minus by one.
        /// </summary>
        public abstract void Collision(Missile m);

    }
}
