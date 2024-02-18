using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static SpaceInvaders.Game;
using static System.Windows.Forms.AxHost;

namespace SpaceInvaders
{
    class PlayerSpaceShip : SpaceShip
    {
        /// <summary>
        /// Our private font style
        /// </summary>
        private readonly PrivateFontCollection privateFonts = new PrivateFontCollection();
        public HashSet<Keys> keyPressede = new HashSet<Keys>();
        /// <summary>
        /// timer of the stun
        /// </summary>
        int cpt =-1;

        /// <summary>
        /// Constructor
        /// </summary>
        public PlayerSpaceShip(int vie, double x, double y, Side s) : base(vie, x, y,s)
        {

        }

        /// <summary>
        /// Draws the Ship's life points and the Ship
        /// </summary>
        /// <param name="gameInstance"></param>
        /// <param name="graphics"></param>
        public override void Draw(Game gameInstance, Graphics graphics)
        {
            if (this.Camp == Side.Ally)
            {
                privateFonts.AddFontFile("./Resources/ThaleahFat.ttf");
                Font menuFont = new Font(privateFonts.Families[0], 20, System.Drawing.FontStyle.Bold); // Texte en gras
                Brush menuBrush = new SolidBrush(Color.GreenYellow);
                graphics.DrawImage(Image, (int)Position.X, (int)Position.Y);
                graphics.DrawString("Vie : " + Lives, menuFont, menuBrush, 0, 0);
            }
            if (this.Camp == Side.PLayerOne)
            {
                privateFonts.AddFontFile("./Resources/ThaleahFat.ttf");
                Font menuFont = new Font(privateFonts.Families[0], 20, System.Drawing.FontStyle.Bold); // Texte en gras
                Brush menuBrush = new SolidBrush(Color.GreenYellow);
                graphics.DrawImage(Image, (int)Position.X, (int)Position.Y);
                graphics.DrawString("Vie : " + Lives, menuFont, menuBrush, 0, 500);
            }
            if (this.Camp == Side.PLayerTwo)
            {
                privateFonts.AddFontFile("./Resources/ThaleahFat.ttf");
                Font menuFont = new Font(privateFonts.Families[0], 20, System.Drawing.FontStyle.Bold); // Texte en gras
                Brush menuBrush = new SolidBrush(Color.GreenYellow);
                graphics.DrawImage(Image, (int)Position.X, (int)Position.Y);
                graphics.DrawString("Vie : " + Lives, menuFont, menuBrush, 0, 50);
            }

        }

        /// <summary>
        /// Updates the missile's state including its movement and shooting behavior based on key presses.
        /// </summary>
        /// <param name="gameInstance">The instance of the game for accessing game-related data.</param>
        /// <param name="deltaT">The time delta since the last update.</param>
        public override void Update(Game gameInstance, double deltaT)
        {
            double x;

            // Handle the countdown for a certain action or state change
            if (cpt > 0)
            {
                cpt--;
                return;
            }
            else if (cpt == 0)
            {
                // Change the missile's color to black once the countdown is complete
                for (int k = 0; k < Image.Height; k++)
                {
                    for (int l = 0; l < Image.Width; l++)
                    {
                        if (255 == Image.GetPixel(l, k).A)
                            Image.SetPixel(l, k, Color.FromArgb(255, 0, 0, 0));
                    }
                }
                cpt--;
            }

            // Handle missile movement based on player's key presses
            x = CalculateMissileMovement(gameInstance.keyPressed);

            // Calculate new position
            Vecteur2D newX = new Vecteur2D(x * speedPixelPerSecond * deltaT, 0);

            // Update position if within game boundaries
            UpdatePositionWithinBounds(newX, gameInstance.gameSize);

            // Handle shooting based on player's key presses
            HandleShooting(gameInstance.keyPressed, gameInstance);
        }

        /// <summary>
        /// Calculates the horizontal movement of the missile based on player's key presses.
        /// </summary>
        /// <param name="keyPressed">Set of keys that are currently pressed.</param>
        /// <returns>The calculated horizontal movement distance.</returns>
        private double CalculateMissileMovement(HashSet<Keys> keyPressed)
        {
            double x = 0;

            // Adjust missile's horizontal movement based on key presses
            if (keyPressed.Contains(Keys.Right) && this.Camp != GameObject.Side.PLayerTwo)
            {
                x = 0.2; // Move right
            }
            if (keyPressed.Contains(Keys.Left) && this.Camp != GameObject.Side.PLayerTwo)
            {
                x = -0.2; // Move left
            }
            if (keyPressed.Contains(Keys.D) && this.Camp != GameObject.Side.PLayerOne && state == GameState.OneVsOne)
            {
                x = 0.2; // Move right
            }
            if (keyPressed.Contains(Keys.Q) && this.Camp != GameObject.Side.PLayerOne && state == GameState.OneVsOne)
            {
                x = -0.2; // Move left
            }

            return x;
        }


        /// <summary>
        /// Updates the position of the missile within the boundaries of the game area.
        /// </summary>
        /// <param name="newPosition">The new proposed position for the missile.</param>
        /// <param name="gameSize">The size of the game area to check boundaries against.</param>
        private void UpdatePositionWithinBounds(Vecteur2D newPosition, System.Drawing.Size gameSize)
        {
            // Check and update position only if within game boundaries
            if ((Position + newPosition).X < gameSize.Width - Image.Width && (Position + newPosition).X > 0)
            {
                Position += newPosition;
            }
            else if ((Position + newPosition).X == 0 && newPosition.X > 0)
            {
                Position += newPosition;
            }
            else if ((Position + newPosition).X >= gameSize.Width - Image.Width && newPosition.X < 0)
            {
                Position += newPosition;
            }
            // Additional conditions can be added to handle other boundaries or special cases
        }


        /// <summary>
        /// Handles the shooting mechanism of the missile based on player's key presses.
        /// </summary>
        /// <param name="keyPressed">Set of keys that are currently pressed.</param>
        /// <param name="gameInstance">The instance of the game for accessing game-related data.</param>
        private void HandleShooting(HashSet<Keys> keyPressed, Game gameInstance)
        {
            // Execute shooting actions based on key presses and game conditions
            if (keyPressed.Contains(Keys.Space) && !Missile.missileAlive && this.Camp == GameObject.Side.Ally)
            {
                Shoot(gameInstance);
                gameInstance.ReleaseKey(Keys.Space);
            }
            if (keyPressed.Contains(Keys.Insert) && this.Camp == GameObject.Side.PLayerOne)
            {
                Shoot(gameInstance);
                gameInstance.ReleaseKey(Keys.Insert);
            }
            if (keyPressed.Contains(Keys.Space) && this.Camp == GameObject.Side.PLayerTwo)
            {
                Shoot(gameInstance);
                gameInstance.ReleaseKey(Keys.Space);
            }
            // Additional conditions for shooting based on other keys or game states can be added here
        }





        /// <summary>
        /// Returns true if the player ship is alive 
        /// </summary>
        /// <returns></returns>
        public override bool IsAlive()
        {
            if (Lives<=0)
            {
                Game.state = (this.Camp == Side.PLayerOne || this.Camp == Side.PLayerTwo) ? Game.GameState.OneVsOneWinner : Game.GameState.Lost;
            }
            return Lives > 0;
        }

        /// <summary>
        /// Verifies the collision on the player
        /// </summary>
        /// <param name="m"></param>
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
                    bool missileInBoundsX = (missileX + j > objectX && missileX + j < objectX + objectWidth);
                    bool missileInBoundsY = (missileY + i > objectY && missileY + i < objectY + objectHeight);
                    bool missileAboveScreen = (missileY > 0);

                    if (missileInBoundsX && missileInBoundsY && missileAboveScreen)
                    {
                        if (m.Image.GetPixel(j, i).A == 255 && m.Camp !=this.Camp)
                        {
                            int relativeX = (int)(missileX + j - objectX);
                            int relativeY = (int)(missileY + i - objectY);
                            bool targetPixelOpaque = (254 == this.Image.GetPixel(relativeX, relativeY).A);
                            if (targetPixelOpaque)
                            {
                                if (m.Image.GetPixel(j, i) == Color.FromArgb(255, 128, 0, 128))
                                {
                                    for (int k = 0; k < Image.Height; k++)
                                    {
                                        for (int l = 0; l < Image.Width; l++)
                                        {
                                            if (255 == this.Image.GetPixel(l, k).A)
                                                Image.SetPixel(l, k, Color.FromArgb(255, 128, 0, 128));
                                        }
                                    }
                                    cpt = 350;
                                }
                                m.Lives = 0;
                                this.Lives--;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
