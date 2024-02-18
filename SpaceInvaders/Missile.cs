using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace SpaceInvaders
{
     class Missile : SimpleObject
    {
        /// <summary>
        /// The player's missile may or may not still be alive (Play mode)
        /// </summary>
        public static bool missileAlive = false;

        /// <summary>
        /// The number of missiles the No.1 player has left alive (OneVsOne mode)
        /// </summary>
        public static int missileAliveOne = 0;
        /// <summary>
        /// The number of missiles of player #2 alive (OneVsOne mode)
        /// </summary>
        public static int missileAliveTwo = 0;

        /// <summary>
        /// Missile speed
        /// </summary>
        private Vitesse vitesse;

        /// <summary>
        /// Variable indicating whether the two missiles (Player 1 missile & Player 2 missile) hit each other 
        /// </summary>
        private bool pvptouch = false;

        /// <summary>
        /// Property of the vitesse variable
        /// </summary>
        public Vitesse Vitesse { get { return vitesse; }
            set { vitesse = value; } }

        /// <summary>
        /// Creating the missile 
        /// </summary>
        /// <param name="x"> X position of the missile</param>
        /// <param name="y"> Y position of the missile</param>
        /// <param name="vies"> Missile lifetime</param>
        /// <param name="s"> The du Missile camp</param>
        /// <param name="speed">  Missile speed</param>
        public Missile(double x, double y, int vies, Side s, Vitesse speed = null) : base(s)
        {
            Lives = 0;
            if (speed != null) 
                vitesse = speed;
            else
            {
                Vitesse = new Vitesse(120.0);
            }
            if (s != Side.Ally)
            {
                Image = new Bitmap(SpaceInvaders.Properties.Resources.shoot3);
            }
            else
            {
                Image = new Bitmap(SpaceInvaders.Properties.Resources.shoot3);
            }
            Position = new Vecteur2D(x - (Image.Width) / 2, y - Image.Height);
            for (int i = 0; i < Image.Height; i++)
            {
                for (int j = 0; j < Image.Width; j++)
                {
                    if (Image.GetPixel(j, i).A == 255) Lives++;
                }
            }
            if (Vitesse.BigMissile)
            {
                Image = new Bitmap(SpaceInvaders.Properties.Resources.Big_shoot);
                Position = new Vecteur2D(x - (Image.Width) / 2, y - 50);
                Vitesse.BigMissile = false;
                Lives = 352 * 100;

            }
            if (Vitesse.RootMissile)
            {
                for (int i = 0; i < Image.Height; i++)
                {
                    for (int j = 0; j < Image.Width; j++)
                    {
                        if (255 == this.Image.GetPixel(j, i).A)
                            Image.SetPixel(j, i, Color.FromArgb(255, 128, 0, 128));
                    }
                }
                Vitesse.RootMissile = false;
            }
        }

        /// <summary>
        /// Update of the missile
        /// </summary>
        /// <param name="graphics">Graphics to draw in</param>
        /// <param name="gameInstance">instance of the game</param>
        public override void Update(Game gameInstance, double deltaT)
        {
            foreach (GameObject gameObject in gameInstance.gameObjects)
            {
                gameObject.Collision(this);

            }
            if (Position.Y + Image.Height < 0 || Lives <= 0 || Position.Y + Image.Height > gameInstance.gameSize.Height)
            {
                Lives = 0;              
            }
            int Sens = ((this.Camp == GameObject.Side.Ally || this.Camp == GameObject.Side.PLayerOne) ? 1 : -1);
            Position.Y -= Sens * (deltaT * Vitesse  > Image.Height? Image.Height : deltaT * Vitesse );
        }

        /// <summary>
        /// Check if the Missile object is alive if so, depending on the game mode, they set the flags.
        /// </summary>
        /// <returns></returns>
        public override bool IsAlive()
        {
            if ( Lives <= 0)
            {
                if (this.Camp == GameObject.Side.Ally) Missile.missileAlive = false;
                if (!pvptouch)
                {
                    if (missileAliveOne > 0 && this.Camp == GameObject.Side.PLayerOne) Missile.missileAliveOne -= 1;
                    if (missileAliveTwo > 0 && this.Camp == GameObject.Side.PLayerTwo) Missile.missileAliveTwo -= 1;
                }

            }

            return Lives > 0;
        }

        /// <summary>
        /// Verifies the collision between two missiles
        /// </summary>
        /// <param name="m"></param>
        public override void Collision(Missile m)
        {
            Color black = Color.Black;
            Color white = Color.White;

            int missileHeight = m.Image.Height;
            int missileWidth = m.Image.Width;
            double missileX = m.Position.X;
            double missileY = m.Position.Y;
            double objectX = this.Position.X;
            double objectY = this.Position.Y;
            int objectWidth = this.Image.Width;
            int objectHeight = this.Image.Height;

            for (int i = 0; i < missileHeight && m.Lives > 0; i++)
            {
                for (int j = 0; j < missileWidth && m.Lives > 0; j++)
                {
                    bool missileInBoundsX = (missileX + j > objectX && missileX + j < objectX + objectWidth);
                
                    bool missileInBoundsY = (missileY + i > objectY && missileY + i < objectY + objectHeight);
                    bool isMissile = (this != m);
                    bool missileAboveScreen = (missileY > 0);

                    if (missileInBoundsX && missileInBoundsY && isMissile && missileAboveScreen)
                    {
                        if (m.Image.GetPixel(j, i).A == 255)
                        {
                            int relativeX = (int)(missileX + j - objectX);
                            int relativeY = (int)(missileY + i - objectY);
                            bool targetPixelOpaque = (black.A == this.Image.GetPixel(relativeX, relativeY).A);
                            if (targetPixelOpaque && this.Camp != m.Camp)
                            {
                                Play(Application.StartupPath + @"\Resources\explosion2.wav");
                                GameForm.monGif.Location = new Point((int)m.Position.X, (int)m.Position.Y);
                                GameForm.monGif.Visible = true;
                                GameForm.gifTimer.Start();
                                m.Lives -= 232;
                                this.Lives -= 232;
                               if (this.pvptouch == false && (this.Camp == GameObject.Side.PLayerOne && m.Camp == GameObject.Side.PLayerTwo || this.Camp == GameObject.Side.PLayerTwo && m.Camp == GameObject.Side.PLayerOne))
                                {
                                    Missile.missileAliveOne -= 1;
                                    Missile.missileAliveTwo -= 1;
                                    this.pvptouch = true;
                                    m.pvptouch = true;

                                }
                            }
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Starts the audio in the sound track
        /// </summary>
        /// <param name="audioPath"></param>
        private void Play(string audioPath)
        {
            MediaPlayer myPlayer = new MediaPlayer();
            myPlayer.Open(new System.Uri(audioPath));
            myPlayer.Volume = Game.volume;
            myPlayer.Play();
        }

        public override void Oncollision(Missile m, int NumberOfPixel)
        {

        }
    }
}
