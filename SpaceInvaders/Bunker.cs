using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SpaceInvaders
{
    class Bunker : SimpleObject
    {
        /// <summary>
        /// Constructor of the Bunker
        /// </summary>
        /// <param name="position">position of the bunker</param>
        /// <param name="s">side of the bunker</param>
        public Bunker(Vecteur2D position ,  Side s) : base(s) {
            Image = new Bitmap(SpaceInvaders.Properties.Resources.bunker);
            Position = new Vecteur2D(position.X, position.Y);
            for (int i = 0; i < Image.Height; i++)
            {
                for (int j = 0; j < Image.Width; j++)
                {
                    if (Image.GetPixel(j, i).A == 255) Lives++;
                }
            }
            if (this.Camp == Side.Enemy)
                Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
        }

        /// <summary>
        /// update of the bunker
        /// </summary>
        public override void Update(Game gameInstance, double deltaT)
        {
           
        }  
        public override void Oncollision(Missile m, int NumberOfPixel)
        {
            m.Lives-= NumberOfPixel;
            this.Lives-= NumberOfPixel;
        }

    }
}
