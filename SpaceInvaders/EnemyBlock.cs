using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Brush = System.Drawing.Brush;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;
using FontFamily = System.Drawing.FontFamily;

namespace SpaceInvaders
{
    class EnemyBlock : GameObject
    {
        /// <summary>
        /// List containing all enemy 
        /// </summary>
        private HashSet<SpaceShip> enemyShips = new HashSet<SpaceShip>();

        /// <summary>
        /// width of the EnemyBlock
        /// </summary>
        private int baseWidth;

        /// <summary>
        /// Size of the EnemyBlock block
        /// </summary>
        private Size size;

        /// <summary>
        /// speed of the block
        /// </summary>
        private double vitesse = 0.5;

        /// <summary>
        /// variable for the probability of enemy fire in EnemyBlock
        /// </summary>
        private double randomShootProbability = 0.01;

        /// <summary>
        /// generates a probability of enemy fire in EnemyBlock
        /// </summary>
        private Random random;

        /// <summary>
        /// EnemyBloc score
        /// </summary>
        public int Score;

        /// <summary>
        /// sound track
        /// </summary>
        private MediaPlayer myPlayer = new MediaPlayer();

        /// <summary>
        /// timer for stun the block
        /// </summary>
        public static int cpt =-1;

        /// <summary>
        /// to know if the wave is alive
        /// </summary>
        public bool Wave = false;

        /// <summary>
        /// Our private font style
        /// </summary>
        private PrivateFontCollection privateFonts = new PrivateFontCollection();

        /// <summary>
        /// Property of the variable size
        /// </summary>
        Size Size { get { return size; } set { size = value; } }

        /// <summary>
        /// position of the block
        /// </summary>
        Vecteur2D position;

        /// <summary>
        /// Property of the variable position
        /// </summary>
        Vecteur2D Position { get { return position; } set { position = value; } }

        /// <summary>
        /// Constructor
        /// <param name="x">x position of the block</param>
        /// <param name="y">y position of the block</param>
        /// <param name="width">width of the block</param>
        /// <param name="s">side of the block</param>
        /// </summary>
        public EnemyBlock(double x, double y,int width , Side s) : base(s)
        {
            random = new Random();
            Position = new Vecteur2D(x, y);
            baseWidth = width;
            size = new Size(0, 0);
        }

        /// <summary>
        /// Constructor
        /// <param name="nbShips">number of ship in the line</param>
        /// <param name="nbLives">number of lives for each ship in the line</param>
        /// <param name="shipImage">image of each ship in the line</param>
        /// <param name="score">score of the line</param>
        /// </summary>
        public void AddLine(int nbShips, int nbLives, Bitmap shipImage,int score)
        {
            int shipWidth = baseWidth / nbShips;
            Wave=true;
            for (int i = 0; i < nbShips; i++)
            {
                double startX = i * shipWidth + (shipWidth - shipImage.Width) / 2;
                SpaceShip enemy = new SpaceShip(nbLives, startX, Position.Y + size.Height + shipImage.Height, GameObject.Side.Enemy,score);
                enemy.Image = shipImage;
                enemyShips.Add(enemy);
            }
            UpdateSize();
        }

        /// <summary>
        /// update de size of the block
        /// </summary>
        public void UpdateSize()
        {
            int maxWidth = 0;
            int maxHeight = 0;
            int minWidth = (int)enemyShips.First().Position.X;
            int minHeight = (int)enemyShips.First().Position.Y;
            foreach (SpaceShip enemy in enemyShips)
            {
                maxWidth = (int)Math.Max(maxWidth, enemy.Position.X + enemy.Image.Width);
                maxHeight = (int)Math.Max(maxHeight, enemy.Position.Y + enemy.Image.Height);

                minWidth = (int)Math.Min(minWidth, enemy.Position.X);
                minHeight = (int)Math.Min(minHeight, enemy.Position.Y);
            }
            size = new Size(maxWidth, maxHeight);
            Position.X = minWidth;
            Position.Y = minHeight;
        }

        /// <summary>
        /// Update the block position
        /// </summary>
        public override void Update(Game gameInstance, double deltaT)
        {
            if (!Wave) return;
            UpdateSize();
            EnemyRooted();
            EnemyMove(gameInstance);
            EnemyShoot(gameInstance, deltaT);

        }

        /// <summary>
        /// to find out if there is a living enemy in the block and eliminates all dead enemies
        /// </summary>
        public override bool IsAlive()
        {
            enemyShips.RemoveWhere(enemy => {
                if (!enemy.IsAlive())
                {
                    Score += enemy.Score;
                    return true;
                }
                return false;
                   });

            foreach (SpaceShip enemy in enemyShips)
            {
                if (enemy.IsAlive()) return true;
            }

            Wave = false;
            Position = new Vecteur2D(0, 0);
            size = new Size(0, 0);
            vitesse = 0.5;

            return true;
        }

        /// <summary>
        /// draw the block
        /// </summary>
        public override void Draw(Game gameInstance, Graphics graphics)
        {
            privateFonts.AddFontFile("./Resources/ThaleahFat.ttf");
            Font menuFont = new Font(privateFonts.Families[0], 20, FontStyle.Bold); // Texte en gras
            Brush menuBrush = new SolidBrush(Color.GreenYellow);
            graphics.DrawString("Score : " + Score, menuFont, menuBrush, gameInstance.gameSize.Width - 10 * menuFont.Size, 0);
            foreach (SpaceShip enemy in enemyShips)
            {
                if (!enemy.IsAlive()) continue;
                graphics.DrawImage(enemy.Image, (float)enemy.Position.X, (float)enemy.Position.Y, enemy.Image.Width, enemy.Image.Height);
            }
        }

        /// <summary>
        /// return the score
        /// </summary>
        public int GetScore()
        {
            return Score;
        }

        /// <summary>
        /// collision of the block
        /// </summary>
        public override void Collision(Missile m)
        {
            foreach (SpaceShip enemy in enemyShips)
            {
                if (!enemy.IsAlive()) continue;
                enemy.Collision(m);

            }
        }

        /// <summary>
        /// Applies a rooting effect to enemy spaceships, changing their color at a specific interval and then reverting it. 
        /// The method uses a counter 'cpt' to control the duration and state of the effect.
        /// </summary>
        private void EnemyRooted()
        {
            // When the counter reaches 500, apply a purple color effect to each enemy spaceship
            if (cpt == 500)
            {
                foreach (SpaceShip ene in enemyShips)
                {
                    // Iterate through each pixel of the enemy spaceship image
                    for (int k = 0; k < ene.Image.Height; k++)
                    {
                        for (int l = 0; l < ene.Image.Width; l++)
                        {
                            // Change pixels with full alpha to a purple color
                            if (255 == ene.Image.GetPixel(l, k).A)
                            {
                                ene.Image.SetPixel(l, k, Color.FromArgb(255, 128, 0, 128));
                            }
                        }
                    }
                }
                cpt--; // Decrement the counter
                return;
            }
            else if (cpt > 0)
            {
                // Decrement the counter if it's greater than zero but not yet at the reset point
                cpt--;
                return;
            }
            else if (cpt == 0)
            {
                // When the counter reaches zero, revert the color change
                foreach (SpaceShip ene in enemyShips)
                {
                    // Iterate through each pixel of the enemy spaceship image
                    for (int k = 0; k < ene.Image.Height; k++)
                    {
                        for (int l = 0; l < ene.Image.Width; l++)
                        {
                            // Revert pixels with full alpha back to black
                            if (255 == ene.Image.GetPixel(l, k).A)
                            {
                                ene.Image.SetPixel(l, k, Color.FromArgb(255, 0, 0, 0));
                            }
                        }
                    }
                }
                cpt--; // Decrement the counter to indicate the end of the effect
            }
        }


        /// <summary>
        /// Handles the movement of enemy spaceships across the game screen. 
        /// It updates their horizontal and vertical positions and changes the game state to 'Lost' if they reach a certain height.
        /// </summary>
        /// <param name="gameInstance">The instance of the game containing game-related data.</param>
        private void EnemyMove(Game gameInstance)
        {
            // Check if the enemy has reached a critical height and end the game if so
            if (size.Height >= (gameInstance.gameSize.Height - SpaceInvaders.Properties.Resources.ship3.Width))
            {
                Game.state = Game.GameState.Lost;
                return;
            }

            // Move enemies horizontally and update their direction and speed when reaching screen boundaries
            if (vitesse + size.Width > gameInstance.gameSize.Width || vitesse + Position.X < 0)
            {
                vitesse = -vitesse * 1.009; // Reverse and slightly increase speed
                randomShootProbability += 0.0001; // Increase shooting probability

                // Move each enemy down
                foreach (SpaceShip enemy in enemyShips)
                {
                    enemy.Position.Y += 5;
                }
            }
        }


        /// <summary>
        /// Handles the shooting behavior of enemy spaceships. 
        /// Each enemy spaceship has a chance to shoot based on a probability, which is influenced by the time delta.
        /// </summary>
        /// <param name="gameInstance">The instance of the game for accessing game-related data.</param>
        /// <param name="deltaT">The time delta since the last update.</param>
        private void EnemyShoot(Game gameInstance, double deltaT)
        {
            // Update the position of each enemy and handle their shooting
            foreach (SpaceShip enemy in enemyShips)
            {
                enemy.Position.X += vitesse; // Move the enemy horizontally

                double r = random.NextDouble(); // Generate a random value

                // Check if the enemy should shoot
                if (r <= randomShootProbability * deltaT)
                {
                    enemy.Shoot(gameInstance); // Enemy shoots
                }
            }
        }

    }
}
