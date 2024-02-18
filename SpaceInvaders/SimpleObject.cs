using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceInvaders
{
    abstract class SimpleObject : GameObject
    {
        /// <summary>
        /// Life of the Object
        /// </summary>
        private int lives;

        /// <summary>
        /// Object position (in px)
        /// </summary>
        public Vecteur2D Position;

        /// <summary>
        /// Image of the object
        /// </summary>
        public Bitmap Image;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="s">side of the object</param>
        public SimpleObject(Side s):base(s)
        {
        }

        /// <summary>
        /// life number property
        /// </summary>
        public int Lives { get { return lives; }  set { lives = value; } }


        /// <summary>
        /// Draw the object 
        /// </summary>
        /// <param name="gameInstance">instance of the game</param>
        /// <param name="graphics">graphic where the image is drawn</param>
        public override void Draw(Game gameInstance, Graphics graphics)
        {
            graphics.DrawImage(Image, (int)Position.X, (int)Position.Y);
        }

        /// <summary>
        /// Returns True if the object is still alive 
        /// </summary>
        /// <returns></returns>
        public override bool IsAlive()
        {
            return Lives > 0;
        }

        /// <summary>
        /// Base Collision
        /// </summary>
        /// <param name="m"></param>
        public override void Collision(Missile m)
        {
            int NumberOfPixel = 0;
            Color black = Color.Black;
            int missileHeight = m.Image.Height;
            int missileWidth = m.Image.Width;
            double missileX = m.Position.X;
            double missileY = m.Position.Y;
            double objectX = this.Position.X;
            double objectY = this.Position.Y;
            int objectWidth = this.Image.Width;
            int objectHeight = this.Image.Height;
            bool side = this.Camp != m.Camp;

            if (!side) return;

            for (int i = 0; i < missileHeight && m.Lives > 0; i++)
            {
                for (int j = 0; j < missileWidth && m.Lives > 0; j++)
                {
                    bool missileInBoundsX = (missileX + j > objectX && missileX + j < objectX + objectWidth);
                    bool missileInBoundsY = (missileY + i > objectY && missileY + i < objectY + objectHeight);
                    bool missileAboveScreen = (missileY > 0);

                    if (missileInBoundsX && missileInBoundsY && missileAboveScreen)
                    {
                        if (m.Image.GetPixel(j, i).A == 255)
                        {
                            int relativeX = (int)(missileX + j - objectX);
                            int relativeY = (int)(missileY + i - objectY);
                            bool targetPixelOpaque = (black.A == this.Image.GetPixel(relativeX, relativeY).A);
                            if (targetPixelOpaque)
                            {
                                NumberOfPixel++;
                                m.Image.SetPixel(j, i, Color.FromArgb(0, 255, 255, 255));
                                this.Image.SetPixel(relativeX, relativeY, Color.FromArgb(0, 255, 255, 255));
                            }
                        }
                    }
                }
            }

            if (NumberOfPixel > 0) Oncollision(m,NumberOfPixel);
        }

        public abstract void Oncollision(Missile m,int NumberOfPixel);

    }
}

