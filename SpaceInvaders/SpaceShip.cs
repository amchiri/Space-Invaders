using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace SpaceInvaders
{
    class SpaceShip : SimpleObject
    {
        /// <summary>
        /// Ship speed
        /// </summary>
        protected double speedPixelPerSecond;

        /// <summary>
        /// Ship Missile
        /// </summary>
        private Missile missile;

        /// <summary>
        /// Ship score
        /// </summary>
        public int Score;

        /// <summary>
        /// First soundtrack
        /// </summary>
        private static MediaPlayer myPlayer = new MediaPlayer();

        /// <summary>
        /// Second Soundtrack
        /// </summary>
        private static MediaPlayer myPlayer2 = new MediaPlayer();

        /// <summary>
        /// timer of the stun
        /// </summary>
        private int cpt;

        /// <summary>
        /// speed of the missile
        /// </summary>
        private Vitesse SpeedMissile;

        /// <summary>
        /// Property of the missile variable
        /// </summary>
        public Missile Missile { get { return missile; } private set { missile = value; } }

        /// <summary>
        /// Ship creation
        /// </summary>
        /// <param name="vie">number of life</param>
        /// <param name="x">X position of the ship</param>
        /// <param name="y">Y position of the ship</param>
        /// <param name="s">side of the ship</param>
        /// <param name="score"></param>
        public SpaceShip(int vie, double x, double y, Side s, int score=0) : base(s)
        {
            SpeedMissile = new Vitesse(120.0);
            Position = new Vecteur2D(x, y);
            Lives = vie;
            Image = new Bitmap(SpaceInvaders.Properties.Resources.ship3);
            if (this.Camp == Side.PLayerTwo)
                Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
            speedPixelPerSecond = 700;
            Score = score;
        }

        /// <summary>
        /// Update of the missile
        /// </summary>
        /// <param name="graphics">Graphics to draw in</param>
        /// <param name="gameInstance">instance of the game</param>
        public override void Update(Game gameInstance, double deltaT)
        {
            
        }

        /// <summary>
        /// Creation of the Ship shoot
        /// </summary>
        /// <param name="gameInstance"></param>
        public void Shoot(Game gameInstance)
        {
            if (!Missile.missileAlive && this.Camp == GameObject.Side.Ally)
            {
                Play(Application.StartupPath + @"\Resources\shoot1.wav");
                missile = new Missile(Position.X + (Image.Width) / 2, Position.Y, 300, this.Camp, SpeedMissile);
                gameInstance.AddNewGameObject(missile);
                Missile.missileAlive = true;
                
            }
            if(this.Camp == GameObject.Side.Enemy)
            {
                Play2(Application.StartupPath + @"\Resources\shoot2.wav");
                missile = new Missile(Position.X + (Image.Width)/2, Position.Y + Image.Height, 300, this.Camp);
                gameInstance.AddNewGameObject(missile);
            }
            if (this.Camp != Side.PLayerOne && this.Camp != Side.PLayerTwo) return;

            if (Missile.missileAliveOne < 3 && this.Camp == GameObject.Side.PLayerOne)
            {
                Play(Application.StartupPath + @"\Resources\shoot1.wav");
                missile = new Missile(Position.X + (Image.Width) / 2, Position.Y, 300, this.Camp, SpeedMissile);
                gameInstance.AddNewGameObject(missile);
                Missile.missileAliveOne += 1;

            }

            if (Missile.missileAliveTwo < 3 && this.Camp == GameObject.Side.PLayerTwo)
            {
                Play2(Application.StartupPath + @"\Resources\shoot1.wav");
                missile = new Missile(Position.X + (Image.Width) / 2, Position.Y + Image.Height, 300, this.Camp, SpeedMissile);
                gameInstance.AddNewGameObject(missile);
                Missile.missileAliveTwo += 1;

            }

        }

        /// <summary>
        /// Ship collision
        /// </summary>
        /// <param name="m"></param>
        public override void Collision(Missile m)
        {
            Color black = Color.Black;
            int NumberOfPixel = 0;
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
                                if (m.Image.GetPixel(j, i) == Color.FromArgb(255, 128, 0, 128)) EnemyBlock.cpt = 500;
                                Play(Application.StartupPath + @"\Resources\explosion2.wav");
                                GameForm.monGif.Location = new Point((int)missileX, (int)missileY);
                                GameForm.monGif.Visible = true;
                                GameForm.gifTimer.Start();
                                Lives = 0;
                                m.Lives -= 232;
                                m.Image.SetPixel(j, i, Color.FromArgb(0, 255, 255, 255));
                                Image.SetPixel(relativeX, relativeY, Color.FromArgb(0, 255, 255, 255));
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Start the first sound track
        /// </summary>
        /// <param name="audioPath"></param>
        private void Play(string audioPath)
        {
            myPlayer.Open(new System.Uri(audioPath));
            myPlayer.Volume = Game.volume;
            myPlayer.Play();
        }

        /// <summary>
        /// Start the second sound track
        /// </summary>
        /// <param name="audioPath"></param>
        private void Play2(string audioPath)
        {
            myPlayer2.Open(new System.Uri(audioPath));
            myPlayer2.Volume = Game.volume;
            myPlayer2.Play();
        }

        public override void Oncollision(Missile m, int NumberOfPixel)
        {

        }
    }
}
