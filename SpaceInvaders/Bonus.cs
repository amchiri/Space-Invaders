using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Windows.Media;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;

namespace SpaceInvaders
{
    class Bonus : SimpleObject
    {
        /// <summary>
        /// variable indicating whether the bonus is shown or not
        /// </summary>
        bool show = false;

        /// <summary>
        /// duration of text display 1
        /// </summary>
        public static int cpt;

        /// <summary>
        /// duration of text display 2
        /// </summary>
        public static int cpt1;

        /// <summary>
        /// duration of text display 3
        /// </summary>
        public static int cpt2;

        /// <summary>
        /// normal game bonus
        /// </summary>
        public static BonusType typeBonus;

        /// <summary>
        /// player one bonus
        /// </summary>
        public static BonusType typeBonus1;

        /// <summary>
        /// player two bonus
        /// </summary>
        public static BonusType typeBonus2;

        /// <summary>
        /// variable indicating whether or not the bonus has been hit
        /// </summary>
        public bool touche = false;

        /// <summary>
        /// Bonus : Multiplicateur SpeedFire for the normal game
        /// </summary>
        public static int nbBonusTakenAlly = 1;

        /// <summary>
        /// Bonus : Multiplicateur SpeedFire for the player one
        /// </summary>
        public static int nbBonusTakenOne = 1;

        /// <summary>
        /// Bonus : Multiplicateur SpeedFire for the player two
        /// </summary>
        public static int nbBonusTakenTwo = 1;

        /// <summary>
        /// chance to match a bonus
        /// </summary>
        private int chance;

        /// <summary>
        /// type of bonus
        /// </summary>
        private BonusType BT;

        /// <summary>
        /// Sound track
        /// </summary>
        static private MediaPlayer myPlayer = new MediaPlayer();

        /// <summary>
        /// Our font style
        /// </summary>
        private PrivateFontCollection privateFonts = new PrivateFontCollection();

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="vie">number of lives </param>
        /// <param name="x">x position </param>
        /// <param name="y">y position </param>
        /// <param name="side">side of the bonus which is an enemy  </param>
        /// 
        /// <returns></returns>
        public Bonus(int vie, double x, double y, Side side) : base(side)
        {
            Position = new Vecteur2D(x, y);
            Lives = vie;
            Lives = 300;
            Image = SpaceInvaders.Properties.Resources.bonus3;
            privateFonts.AddFontFile("./Resources/ThaleahFat.ttf");
            Random random = new Random();
            int a = random.Next(0, 3);
            BT = (BonusType)a;
            chance = random.Next(0, 1000);
        }

        /// <summary>
        /// Draw the bonus
        /// </summary>
        /// <param name="graphics">Graphics to draw in</param>
        /// <param name="gameInstance">instance of the game</param>
        public override void Draw(Game gameInstance, Graphics graphics)
        {
            if (show)
            {
                graphics.DrawImage(Image, (int)Position.X, (int)Position.Y);
            }

        }

        /// <summary>
        /// Update of the bonus
        /// </summary>
        /// <param name="graphics">Graphics to draw in</param>
        /// <param name="gameInstance">instance of the game</param>
        public override void Update(Game gameInstance, double deltaT)
        {
            Random random = new Random();
            int r = random.Next(0, 1000);
            if ((r > chance - 10) && (r < chance + 10) && !show)
            {
                show = true;
                Play(Application.StartupPath + @"\Resources\bonus.wav");
            }
        }

        /// <summary>
        /// Collision of the bonus
        /// </summary>
        /// <param name="m">missile that hit the bonus</param>
        public override void Collision(Missile m)
        {
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
                    bool missileInBoundsX = (missileX + j > objectX && missileX + j < objectX + objectWidth * 2);
                    bool missileInBoundsY = (missileY + i > objectY && missileY + i < objectY + objectHeight);
                    bool missileAboveScreen = (missileY > 0);
                    if (missileInBoundsX && missileInBoundsY && missileAboveScreen)
                    {
                        if (m.Image.GetPixel(j, i).A == 255)
                        {
                            int relativeX = (int)(missileX + j - objectX);
                            int relativeY = (int)(missileY + i - objectY);
                            bool targetPixelOpaque = (black.A == this.Image.GetPixel(relativeX/2, relativeY).A);
                            if (targetPixelOpaque)
                            {
                                if (show)
                                {
                                    switch (m.Camp)
                                    {
                                        case Side.Ally:
                                            typeBonus = this.BT;
                                            cpt = 50;
                                            break;

                                        case Side.PLayerOne:
                                            typeBonus1 = this.BT;
                                            cpt1 = 50;
                                            break;

                                        case Side.PLayerTwo:
                                            typeBonus2 = this.BT;
                                            cpt2 = 50;
                                            break;
                                    }

                                    if (BT == BonusType.BigMissile)
                                    {
                                        m.Vitesse.BigMissile = true;
                                    }else if(BT == BonusType.RootMissile)
                                    {
                                        m.Vitesse.RootMissile = true;
                                    }
                                    else
                                    {
                                        m.Vitesse.SetValueM(2);
                                        switch (m.Camp)
                                        {
                                            case Side.Ally:
                                                nbBonusTakenAlly *= 2;
                                                break;

                                            case Side.PLayerOne:
                                                nbBonusTakenOne *= 2;
                                                break;

                                            case Side.PLayerTwo:
                                                nbBonusTakenTwo *= 2;
                                                break;
                                        }
                                    }
                                    show = false;
                                    touche = true;
                                    Play(Application.StartupPath + @"\Resources\bonus.wav");
                                    m.Lives -= 232;
                                    this.Lives = 0;
                                    m.Image.SetPixel(j, i, Color.FromArgb(0, 255, 255, 255));
                                    this.Image.SetPixel(relativeX / 2, relativeY, Color.FromArgb(0, 255, 255, 255));
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sound track player
        /// </summary>
        /// <param name="audioPath">path of the file</param>
        static void Play(string audioPath)
        {
            myPlayer.Open(new System.Uri(audioPath));
            myPlayer.Volume = Game.volume;
            myPlayer.Play();
        }

        /// <summary>
        /// To know if the bonus is alive
        /// </summary>
        public override bool IsAlive()
        {
            return true;
        }

        /// <summary>
        /// initialising bonuses
        /// </summary>
        public static void Init()
        {
            nbBonusTakenAlly = 1;
            nbBonusTakenOne = 1;
            nbBonusTakenTwo = 1;
        }

        /// <summary>
        /// Change the type of bonus
        /// </summary>
        public void ChangeType()
        {
            Random random = new Random();
            BT = (BonusType)random.Next(0, 3);
        }


        public override void Oncollision(Missile m, int NumberOfPixel)
        {

        }
    }
    
}
