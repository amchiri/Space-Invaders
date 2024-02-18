using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.Drawing.Text;
using System.Media;
using System.Windows.Media;
using Color = System.Drawing.Color;
using Brush = System.Drawing.Brush;
using Brushes = System.Drawing.Brushes;
using FontFamily = System.Drawing.FontFamily;
using System.Timers;
using static SpaceInvaders.GameObject;
using System.Windows.Media.TextFormatting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SpaceInvaders
{
    /// <summary>
    /// This class represents the entire game, it implements the singleton pattern
    /// </summary>
    class Game
    {
        /// <sumary>
        /// State of the game
        /// </sumary>
        public enum GameState { Play, Pause, Won, Lost, Menu, Leaderboards, OneVsOne, OneVsOneWinner }
        #region GameObjects management
        /// <summary>
        /// Set of all game objects currently in the game
        /// </summary>
        public HashSet<GameObject> gameObjects = new HashSet<GameObject>();

        /// <summary>
        /// Set of new game objects scheduled for addition to the game
        /// </summary>
        private HashSet<GameObject> pendingNewGameObjects = new HashSet<GameObject>();

        /// <summary>
        /// Schedule a new object for addition in the game.
        /// The new object will be added at the beginning of the next update loop
        /// </summary>
        /// <param name="gameObject">object to add</param>
        public void AddNewGameObject(GameObject gameObject)
        {
            pendingNewGameObjects.Add(gameObject);
        }
        #endregion

        #region game technical elements
        /// <summary>
        /// Size of the game area
        /// </summary>
        public Size gameSize;

        /// <summary>
        /// State of the keyboard
        /// </summary>
        public HashSet<Keys> keyPressed = new HashSet<Keys>();

        /// <summary>
        /// SpaceShip of player
        /// </summary>
        private PlayerSpaceShip playerShip;

        /// <summary>
        /// SpaceShip of Player One (OneVsOne)
        /// </summary>
        private PlayerSpaceShip playerShipOne;

        /// <summary>
        ///  SpaceShip of Player Two (OneVsOne) 
        /// </summary>
        private PlayerSpaceShip playerShipTwo;

        /// <summary>
        /// The enemy block
        /// </summary>
        private EnemyBlock enemyBlock;

        /// <sumary>
        /// Score of the game
        /// </sumary>
        public static int Score;

        /// <sumary>
        /// Username of the game
        /// </sumary>
        public static string Username = "";

        /// <summary>
        /// State of the game
        /// </summary>
        public static GameState state = GameState.Menu;

        /// <summary>
        /// Menu Index
        /// </summary>
        private int selectedMenuItem = 0;

        /// <summary>
        /// leaderboards Index
        /// </summary>
        private int pagelist = 0;

        /// <summary>
        /// pagemax du leaderboards
        /// </summary>
        private int pagemax = 0;

        /// <summary>
        /// OneVsone Mode Flag
        /// </summary>
        private bool OneVSOne = false;

        /// <summary>
        /// Font table for writing
        /// </summary>
        private PrivateFontCollection privateFonts = new PrivateFontCollection();

        /// <summary>
        /// First sound track
        /// </summary>
        MediaPlayer myPlayer = new MediaPlayer();

        /// <summary>
        /// Second sound track
        /// </summary>
        MediaPlayer myPlayer2 = new MediaPlayer();

        /// <summary>
        /// Flag that Find out if the track of sound 1 has been played
        /// </summary>
        private bool playedMedia1 = false;

        /// <summary>
        /// Game Volume
        /// </summary>
        public static float volume = 0.1f;

        /// <summary>
        /// Volume Addition Index
        /// </summary>
        private float volumeDecalage = 5f;

        /// <summary>
        /// Soundbar image
        /// </summary>
        readonly Image volumeBarBackground = Image.FromFile("./Resources/background_volume1.png");

        /// <summary>
        /// Image of the filled soundbar
        /// </summary>
        readonly Image filledVolumeImage = Image.FromFile("./Resources/frontend_volume1.png");
        #endregion

        #region static fields (helpers)

        /// <summary>
        /// Singleton for easy access
        /// </summary>
        public static Game game { get; private set; }

        /// <summary>
        /// A shared black brush
        /// </summary>
        private static Brush blackBrush = new SolidBrush(Color.Black);

        /// <summary>
        /// A shared simple font
        /// </summary>
        private static Font defaultFont = new Font("Times New Roman", 24, FontStyle.Bold, GraphicsUnit.Pixel);

        /// <summary>
        /// Tell if game started
        /// </summary>
        private static bool gameStart = false;

        /// <summary>
        /// Timer that moves players forward in OneVsOne mode
        /// </summary>
        static System.Timers.Timer timer;

        /// <summary>
        /// Flag QuitGame
        /// </summary>
        static bool QuitGame;

        /// <summary>
        /// Variables to store the background image of the menu
        /// </summary>
        private static Image menuBackground = Image.FromFile(@"Resources\background.png"), gameBackground = Image.FromFile("./Resources/background3.png");

        private static Bitmap[] ImageShip =
        {
            SpaceInvaders.Properties.Resources.ship7,
            SpaceInvaders.Properties.Resources.ship6,
            SpaceInvaders.Properties.Resources.ship5,
            SpaceInvaders.Properties.Resources.ship4,
            SpaceInvaders.Properties.Resources.ship4
        };

        #endregion


        #region constructors
        /// <summary>
        /// Singleton constructor
        /// </summary>
        /// <param name="gameSize">Size of the game area</param>
        /// 
        /// <returns></returns>
        public static Game CreateGame(Size gameSize)
        {
            if (game == null) { }
            game = new Game(gameSize);
            return game;
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="gameSize">Size of the game area</param>
        private Game(Size gameSize)
        {
            this.gameSize = gameSize;
            privateFonts.AddFontFile("./Resources/ThaleahFat.ttf");
            Play(Application.StartupPath + @"\Resources\Royale_8bit_menu.wav");
        }
        #endregion

        #region methods

        /// <summary>
        /// Force a given key to be ignored in following updates until the user
        /// explicitily retype it or the system autofires it again.
        /// </summary>
        /// <param name="key">key to ignore</param>
        public void ReleaseKey(Keys key)
        {
            keyPressed.Remove(key);
        }


        /// <summary>
        /// Draw the whole game
        /// </summary>
        /// <param name="g">Graphics to draw in</param>
        public void Draw(Graphics g)
        {

            switch (state)
            {
                case GameState.Menu:
                    DrawMenuState(g);
                    break;


                case GameState.Play:
                    DrawPlayState(g);
                    break;

                case GameState.Pause:
                    DrawPauseState(g);
                    break;

                case GameState.OneVsOne:
                    DrawOneVsOneState(g);
                    break;


                case Game.GameState.Leaderboards:
                    DrawLeaderboardsState(g, pagelist);
                    break;

                case Game.GameState.Won:
                case Game.GameState.Lost:
                    DrawEndGameState(g);
                    break;

                case GameState.OneVsOneWinner:
                    DrawOneVsOneWinnerState(g);
                    break;
            }
        }

        /// <summary>
        /// Update the game
        /// </summary>
        /// <param name="deltaT">The time delta for updates</param>
        public void Update(double deltaT)
        {
            // add new game objects
            gameObjects.UnionWith(pendingNewGameObjects);
            pendingNewGameObjects.Clear();


            switch (state)
            {
                case GameState.Menu:
                    HandleMenuState(keyPressed);
                    break;

                case GameState.Leaderboards:
                    HandleLeaderboardsState(keyPressed);
                    break;
                case GameState.OneVsOneWinner:
                    HandleOneVsOneWinnerState(keyPressed);
                    break;
                case GameState.Won:
                case GameState.Lost:
                    HandleEndGameState(keyPressed);
                    break;

                case GameState.Play:
                    if (!gameStart)
                    {
                        InitGame();
                        gameStart = true;
                        // release key space (no autofire)
                        ReleaseKey(Keys.Space);
                    }
                    break;

                case GameState.OneVsOne:
                    if (!gameStart)
                    {
                        state = GameState.OneVsOne;
                        InitOneVSOne();
                        ReleaseKey(Keys.Space);
                    }
                    break;
            }

            // update each game object
            foreach (GameObject gameObject in gameObjects)
            {
                // Special handling for Bonus objects
                HandleBonusObject(gameObject);

                // Update score and potentially add new wave for EnemyBlock objects
                HandleEnemyBlockObject(gameObject);

                // Handle game state changes and player actions based on key inputs
                HandleGameAndPlayerActions(gameObject, keyPressed);

                // Update the state of each game object
                UpdateGameObjectState(gameObject, deltaT, keyPressed);

            }
            // remove dead objects
            gameObjects.RemoveWhere(gameObject => !gameObject.IsAlive());
            if(QuitGame) Remove(); // Clear certain game states
        }


        /// <summary>
        /// Plays an audio file on track 1. This method opens the specified audio file and plays it using the first audio player.
        /// </summary>
        /// <param name="audioPath">The path to the audio file to be played.</param>
        void Play(string audioPath)
        {
            myPlayer.Open(new System.Uri(audioPath));
            myPlayer.Volume = Game.volume;
            myPlayer.Play();
        }

        /// <summary>
        /// Plays an audio file on track 2. This method opens the specified audio file and plays it using the second audio player.
        /// </summary>
        /// <param name="audioPath">The path to the audio file to be played.</param>
        void Play2(string audioPath)
        {
            myPlayer2.Open(new System.Uri(audioPath));
            myPlayer2.Volume = Game.volume;
            myPlayer2.Play();
        }

        /// <summary>
        /// Method that gradually moves both players towards the center of the game area. This function is called on each timer tick and adjusts the players' positions.
        /// </summary>
        /// <param name="sender">The source of the event, typically the timer.</param>
        /// <param name="e">Event data that contains information about the elapsed time.</param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (playerShipOne.Position.Y < gameSize.Height / 2 + 50 || playerShipTwo.Position.Y > gameSize.Height / 2 - 50)
            {
                playerShipOne.Position.Y = gameSize.Height - SpaceInvaders.Properties.Resources.ship3.Height - 35;
                playerShipTwo.Position.Y = 35;
            }
            else
            {
                playerShipOne.Position.Y -= 1;
                playerShipTwo.Position.Y += 1;
            }
        }

        /// <summary>
        /// Determines if the pressed key is a letter between A and Z.
        /// </summary>
        /// <param name="key">The key that is pressed.</param>
        /// <returns>Returns true if the key is a letter between A and Z; otherwise, returns false.</returns>
        private bool IsLetterKey(Keys key)
        {
            return key >= Keys.A && key <= Keys.Z;
        }

        /// <summary>
        /// Removes all live objects from the game. This function clears the game state by removing any object that is currently alive. It also plays a specific audio track and sets the game start flag to false.
        /// </summary>
        private void Remove()
        {
            gameObjects.RemoveWhere(gameObject => gameObject.IsAlive());
            Play(Application.StartupPath + @"\Resources\Royale_8bit_menu.wav");
            gameStart = false;
            QuitGame = false;
        }

        /// <summary>
        /// Initializes the game for the Play mode. This includes setting up the player's spaceship, enemy blocks, bunkers, and generating bonus objects.
        /// </summary>
        private void InitGame()
        {
            Missile.missileAlive = false;
            Bonus.Init();
            playerShip = new PlayerSpaceShip(3, gameSize.Width / 2 - SpaceInvaders.Properties.Resources.ship3.Width / 2, gameSize.Height - SpaceInvaders.Properties.Resources.ship3.Height, GameObject.Side.Ally);
            enemyBlock = new EnemyBlock(0, 0, gameSize.Width, GameObject.Side.Enemy);
            Vecteur2D gameLine = new Vecteur2D(this.gameSize.Width / 4, 0);
            Vecteur2D posbunker = new Vecteur2D(-(SpaceInvaders.Properties.Resources.bunker.Width) / 2, 350);
            Bunker bunker1 = new Bunker(posbunker + gameLine, GameObject.Side.Neutral);
            Bunker bunker2 = new Bunker(posbunker + 2 * gameLine, GameObject.Side.Neutral);
            Bunker bunker3 = new Bunker(posbunker + 3 * gameLine, GameObject.Side.Neutral);
            CreateBonus(5);
            AddNewGameObject(enemyBlock);
            AddNewGameObject(playerShip);
            AddNewGameObject(bunker1);
            AddNewGameObject(bunker2);
            AddNewGameObject(bunker3);
            enemyBlock.AddLine(1, 5, SpaceInvaders.Properties.Resources.ship7, 10);
            enemyBlock.AddLine(8, 5, SpaceInvaders.Properties.Resources.ship6, 10);
            enemyBlock.AddLine(4, 5, SpaceInvaders.Properties.Resources.ship5, 10);
            enemyBlock.AddLine(6, 5, SpaceInvaders.Properties.Resources.ship4, 10);
            enemyBlock.AddLine(6, 5, SpaceInvaders.Properties.Resources.ship4, 10);
        }

        /// <summary>
        /// Initializes the game environment for a One vs One mode. This method sets up two player ships, bunkers, and generates bonuses. It also configures and starts the game timer.
        /// </summary>
        private void InitOneVSOne()
        {
            Missile.missileAlive = false;
            Bonus.Init();
            playerShipOne = new PlayerSpaceShip(3, gameSize.Width / 2 - SpaceInvaders.Properties.Resources.ship3.Width / 2, gameSize.Height - SpaceInvaders.Properties.Resources.ship3.Height - 35, GameObject.Side.PLayerOne);
            playerShipTwo = new PlayerSpaceShip(3, gameSize.Width / 2 - SpaceInvaders.Properties.Resources.ship3.Width / 2, 35, GameObject.Side.PLayerTwo);
            Vecteur2D gameLine = new Vecteur2D(this.gameSize.Width / 4, 0);
            Vecteur2D posbunker = new Vecteur2D(-(SpaceInvaders.Properties.Resources.bunker.Width) / 2, gameSize.Height / 2 + 100);
            Bunker bunker1 = new Bunker(posbunker + gameLine, GameObject.Side.Neutral);
            Bunker bunker2 = new Bunker(posbunker + 2 * gameLine, GameObject.Side.Neutral);
            Bunker bunker3 = new Bunker(posbunker + 3 * gameLine, GameObject.Side.Neutral);
            Vecteur2D posbunker1 = new Vecteur2D(-(SpaceInvaders.Properties.Resources.bunker.Width) / 2, gameSize.Height / 2 - 100 - SpaceInvaders.Properties.Resources.bunker.Height);
            Bunker bunker4 = new Bunker(posbunker1 + gameLine, GameObject.Side.Enemy);
            Bunker bunker5 = new Bunker(posbunker1 + 2 * gameLine, GameObject.Side.Enemy);
            Bunker bunker6 = new Bunker(posbunker1 + 3 * gameLine, GameObject.Side.Enemy);
            Random random = new Random();
            double x = random.Next(0, gameSize.Width - 20);
            double y = random.Next(0, gameSize.Height - 100);
            Bonus bonus = new Bonus(1, x, y, GameObject.Side.Enemy);
            CreateBonus(5);
            AddNewGameObject(playerShipOne);
            AddNewGameObject(playerShipTwo);
            AddNewGameObject(bunker1);
            AddNewGameObject(bunker2);
            AddNewGameObject(bunker3);
            AddNewGameObject(bunker4);
            AddNewGameObject(bunker5);
            AddNewGameObject(bunker6);
            timer = new System.Timers.Timer(100);
            timer.Elapsed += TimerElapsed;
            timer.Start();
            gameStart = true;
        }


        /// <summary>
        /// Adds a randomly generated wave of enemy ships.
        /// </summary>
        private void AddWave()
        {
            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                int nbShip = random.Next(1, 8);
                enemyBlock.AddLine(nbShip, 5, ImageShip[i], 10);
            }
        }


        /// <summary>
        /// Generates a specified number of bonus objects at random positions within the game area.
        /// </summary>
        /// <param name="NbBonus">The number of bonus objects to create. This value determines how many bonus objects are generated and added to the game.</param>
        private void CreateBonus(int NbBonus)
        {
            Random random = new Random();
            Bonus bonus;
            for (int i = 0; i < NbBonus; i++)
            {
                double x = random.Next(0, gameSize.Width - 20);
                double y = random.Next(0, gameSize.Height - 100);
                bonus = new Bonus(1, x, y, GameObject.Side.Enemy);
                AddNewGameObject(bonus);
            }
        }

        #region DrawRegion

        /// <summary>
        /// Generates a descriptive text for a given bonus type and the number of bonuses taken.
        /// </summary>
        /// <param name="typeBonus">The type of bonus, represented by the BonusType enum.</param>
        /// <param name="nbBonusTaken">The number of bonuses taken, used for certain types of bonuses to generate appropriate descriptive text.</param>
        /// <returns>Returns a string describing the bonus. If the bonus type does not match any known types, an empty string is returned.</returns>
        private string GetBonusText(BonusType typeBonus, int nbBonusTaken)
        {
            switch (typeBonus)
            {
                case BonusType.BigMissile:
                    return "BIG MISSILE";

                case BonusType.RootMissile:
                    return "Root Missile";

                case BonusType.DoubleSpeed:
                    return $"{nbBonusTaken}x shoot speed activated";

                default:
                    return string.Empty;
            }
        }


        /// <summary>
        /// Draws text with an optional border and various positioning modes.
        /// </summary>
        /// <param name="g">The Graphics object used for drawing the text.</param>
        /// <param name="title">The text to be drawn.</param>
        /// <param name="titleFont">The font used to draw the text.</param>
        /// <param name="borderBrush">The brush used to draw the text border.</param>
        /// <param name="Posx">The horizontal (X) position of the text on the drawing surface.</param>
        /// <param name="Posy">The vertical (Y) position of the text on the drawing surface.</param>
        /// <param name="brushs">The brush used to draw the main text.</param>
        /// <param name="titlesize">An optional integer determining the text positioning mode (0 for default, 1 for above, 2 for center, 3 for below).</param>

        private void DrawText(Graphics g, string title, Font titleFont, Brush borderBrush, float Posx, float Posy, Brush brushs, int titlesize = 0)
        {
            int borderSize = 2;
            SizeF titleSize = g.MeasureString(title, titleFont);
            PointF titlePos = new PointF(Posx - titleSize.Width / 2, Posy); ;
            switch (titlesize)
            {
                case 1:
                    titlePos = new PointF(Posx - titleSize.Width / 2, Posy - titleSize.Height);
                    break;
                case 2:
                    titlePos = new PointF(Posx - titleSize.Width / 2, Posy - (titleSize.Height / 2));
                    break;
                case 3:
                    titlePos = new PointF(Posx - titleSize.Width / 2, Posy + titleSize.Height + 20);
                    break;
            }
            for (int dx = -borderSize; dx <= borderSize; dx++)
            {
                for (int dy = -borderSize; dy <= borderSize; dy++)
                {
                    if (dx != 0 || dy != 0)
                    {
                        g.DrawString(title, titleFont, borderBrush, new PointF(titlePos.X + dx, titlePos.Y + dy));
                    }
                }
            }

            g.DrawString(title, titleFont, brushs, titlePos);
        }

        /// <summary>
        /// Draws the game's menu screen, including the background, menu items, and a volume bar. The menu items are dynamically highlighted based on user selection.
        /// </summary>
        /// <param name="g">Graphics object used for drawing.</param>
        private void DrawMenuState(Graphics g)
        {
            // Initialize fonts and brushes
            Font menuFont = new Font(privateFonts.Families[0], 20, FontStyle.Bold); // Bold text for menu items
            SolidBrush menuBrush = new SolidBrush(Color.FloralWhite);
            Brush borderBrush = new SolidBrush(Color.Black); // Brush for borders
            Font titleFont = new Font(privateFonts.Families[0], 40, FontStyle.Bold); // Title font
            int yPos = gameSize.Height / 2 - 135; // Initial Y position for menu items

            // Draw background and volume icons
            g.DrawImage(menuBackground, new Rectangle(0, 0, gameSize.Width, gameSize.Height));
            g.DrawImage(Image.FromFile("./Resources/volume_right.png"), new Rectangle(gameSize.Width / 2 + 65, 330, 50, 50));
            g.DrawImage(Image.FromFile("./Resources/volume_left.png"), new Rectangle(gameSize.Width / 2 - 100, 330, 50, 50));

            // Draw title
            DrawText(g, "Space Invaders", titleFont, borderBrush, game.gameSize.Width / 2, 100, Brushes.GreenYellow);

            // Volume bar dimensions and positions
            float volumeBarWidth = 220; // Total width of the volume bar
            float volumeBarHeight = 100; // Total height of the volume bar
            PointF volumeBarPosition1 = new PointF((gameSize.Width - volumeBarWidth) / 2 - 10 + 15, 280);
            PointF volumeBarPosition2 = new PointF((gameSize.Width - volumeBarWidth) / 2 + volumeDecalage + 15, 280);

            // Draw the background of the volume bar (like a control panel)
            g.DrawImage(volumeBarBackground, volumeBarPosition1.X, volumeBarPosition1.Y, volumeBarWidth, volumeBarHeight);

            // Calculate and draw the filled part of the volume bar (like a laser beam)
            float filledVolumeWidth = volume * volumeBarWidth;
            g.DrawImage(filledVolumeImage, volumeBarPosition2.X, volumeBarPosition2.Y, filledVolumeWidth, volumeBarHeight);

            // Menu items
            string[] menuItems = { "Play", "Leaderboards", "OneVsOne", "Press M to mute", "Press Q to EXIT", "VOLUME" };

            // Draw menu items
            for (int i = 0; i < menuItems.Length; i++)
            {
                FontStyle style = (i == selectedMenuItem) ? FontStyle.Underline | FontStyle.Bold : FontStyle.Bold;
                if (i == 3)
                {
                    yPos += 200; // Adjust position for specific item
                    menuBrush.Color = Color.GreenYellow; // Change brush color for specific item
                }
                else if (i == 5)
                {
                    yPos -= 278; // Adjust position for specific item
                    menuBrush.Color = Color.White; // Change brush color for specific item
                }

                Font itemFont = new Font(menuFont.FontFamily, menuFont.Size, style);
                DrawText(g, menuItems[i], itemFont, borderBrush, (gameSize.Width / 2) + 5, yPos, menuBrush);

                yPos += (i == 3) ? 30 : 50; // Adjust yPos for next item
            }

        }


        /// <summary>
        /// Draws the game screen for the 'Play' state. This includes rendering the game background, all game objects, and any active bonuses.
        /// </summary>
        /// <param name="g">Graphics object used for drawing.</param>
        private void DrawPlayState(Graphics g)
        {
            // Initialize brush and font for text drawing
            Brush borderBrush;
            FontStyle style;
            Font titleFont;

            // Draw the game background
            g.DrawImage(gameBackground, new Rectangle(0, 0, gameSize.Width, gameSize.Height));

            // Draw each game object
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Draw(this, g);
            }

            // Draw bonus-related text if applicable
            if (Bonus.cpt > 0)
            {
                // Setup font and brush for drawing bonus text
                borderBrush = new SolidBrush(Color.Black);
                titleFont = new Font(privateFonts.Families[0], 30, FontStyle.Bold);

                // Determine the bonus text based on the bonus type
                string bonusText = GetBonusText(Bonus.typeBonus, Bonus.nbBonusTakenAlly);

                // Draw the bonus text
                DrawText(g, bonusText, titleFont, borderBrush, gameSize.Width / 2, 20, Brushes.Yellow);

                // Decrement the bonus counter
                Bonus.cpt--;
            }
        }

        /// <summary>
        /// Draws the game screen for the 'Pause' state. This includes the game background, the 'Pause' title, and the 'Press Q to EXIT' text.
        /// </summary>
        /// <param name="g">Graphics object used for drawing.</param>
        private void DrawPauseState(Graphics g)
        {
            // Initialize fonts and brushes
            Font menuFont;
            Brush borderBrush;
            FontStyle style;
            Font itemFont;
            Font titleFont;

            // Draw the game background
            g.DrawImage(gameBackground, new Rectangle(0, 0, gameSize.Width, gameSize.Height));

            // Draw "Pause" title
            titleFont = new Font(privateFonts.Families[0], 40, FontStyle.Bold);
            borderBrush = new SolidBrush(Color.White);
            DrawText(g, "Pause", titleFont, borderBrush, gameSize.Width / 2, 100, Brushes.GreenYellow);

            // Draw "Press Q to EXIT" text
            menuFont = new Font(privateFonts.Families[0], 20, FontStyle.Bold); // Bold text for menu options
            style = FontStyle.Bold;
            itemFont = new Font(menuFont.FontFamily, menuFont.Size, style);
            DrawText(g, "Press Q to EXIT", itemFont, borderBrush, gameSize.Width / 2 + 5, gameSize.Height / 2 - 135 + 250 + 100, Brushes.GreenYellow);
        }


        /// <summary>
        /// Draws the game screen for the 'One vs One' state. This includes the game background, all game objects, and any active bonuses for both players.
        /// </summary>
        /// <param name="g">Graphics object used for drawing.</param>
        private void DrawOneVsOneState(Graphics g)
        {
            // Initialize brush and font for text drawing
            Brush borderBrush;
            FontStyle style;
            Font titleFont;

            // Draw the game background
            g.DrawImage(gameBackground, new Rectangle(0, 0, gameSize.Width, gameSize.Height));

            // Draw all game objects
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Draw(this, g);
            }

            // Draw bonuses for player one
            if (Bonus.cpt1 > 0)
            {
                // Setup font and brush for drawing player one's bonus
                titleFont = new Font(privateFonts.Families[0], 20, FontStyle.Bold);
                borderBrush = new SolidBrush(Color.Black);

                // Get bonus text for player one and draw it
                string bonusText1 = GetBonusText(Bonus.typeBonus1, Bonus.nbBonusTakenOne);
                DrawText(g, bonusText1, titleFont, borderBrush, gameSize.Width / 2, gameSize.Height, Brushes.Yellow, 1);

                // Decrement bonus counter for player one
                Bonus.cpt1--;
            }

            // Draw bonuses for player two
            if (Bonus.cpt2 > 0)
            {
                // Setup font and brush for drawing player two's bonus
                titleFont = new Font(privateFonts.Families[0], 20, FontStyle.Bold);
                borderBrush = new SolidBrush(Color.Black);

                // Get bonus text for player two and draw it
                string bonusText2 = GetBonusText(Bonus.typeBonus2, Bonus.nbBonusTakenTwo);
                DrawText(g, bonusText2, titleFont, borderBrush, gameSize.Width / 2, 0, Brushes.Yellow);

                // Decrement bonus counter for player two
                Bonus.cpt2--;
            }
        }


        /// <summary>
        /// Draws the leaderboard screen, showing a list of users and their scores. This screen also includes pagination and an option to exit.
        /// </summary>
        /// <param name="g">Graphics object used for drawing.</param>
        /// <param name="pageList">The current page number in the leaderboard pagination.</param>
        private void DrawLeaderboardsState(Graphics g, int pageList)
        {
            // Initialize fonts and brushes
            Font menuFont;
            Brush borderBrush;
            int yPos;
            Font itemFont;
            Font titleFont;

            // Clear the screen and draw the background
            g.Clear(Color.White);
            g.DrawImage(menuBackground, new Rectangle(0, 0, gameSize.Width, gameSize.Height));

            // Setup fonts and brushes
            menuFont = new Font(privateFonts.Families[0], 20, FontStyle.Bold);
            titleFont = new Font(privateFonts.Families[0], 40, FontStyle.Bold);
            itemFont = new Font(menuFont.FontFamily, menuFont.Size, FontStyle.Bold);
            borderBrush = new SolidBrush(Color.Black);

            // Draw the 'Leaderboards' title
            DrawText(g, "LeaderBoards", titleFont, borderBrush, gameSize.Width / 2, 100, Brushes.White);

            // Fetch user list and calculate pagination details
            var userList = UserData.Get_users(@"Resources\file.json").OrderByDescending(x => x.Score);
            int pageMax = userList.Count() / 5 + ((userList.Count() % 5 >= 1) ? 1 : 0);
            yPos = gameSize.Height / 2 - 135;

            // Draw leaderboard entries
            for (int i = 5 * pageList; i < 5 * (pageList + 1) && i < userList.Count(); i++)
            {
                DrawText(g, userList.ElementAt(i).ToString(), itemFont, borderBrush, gameSize.Width / 2 + 5, yPos, Brushes.GreenYellow);
                yPos += 50;
            }

            // Draw pagination and exit option
            DrawText(g, $"{pageList + 1}/{pageMax}", itemFont, borderBrush, gameSize.Width / 2 + 5, gameSize.Height / 2 - 135 + 250, Brushes.White);
            DrawText(g, "Press Q to EXIT", itemFont, borderBrush, gameSize.Width / 2 + 5, gameSize.Height / 2 - 135 + 250 + 100, Brushes.GreenYellow);
        }


        /// <summary>
        /// Draws the game screen for the 'End Game' state, displaying either the win or loss message and prompts the user for their username.
        /// This method handles the rendering for both the 'Won' and 'Lost' states of the game.
        /// </summary>
        /// <param name="g">Graphics object used for drawing.</param>
        private void DrawEndGameState(Graphics g)
        {
            // Initialize the brush, style, and font for text drawing
            Brush borderBrush;
            FontStyle style;
            Font font;

            // Draw the game background for the end state
            g.DrawImage(gameBackground, new Rectangle(0, 0, gameSize.Width, gameSize.Height));

            // Set up the font for the game over text
            font = new Font(privateFonts.Families[0], 40, FontStyle.Bold);
            borderBrush = new SolidBrush(Color.Black);

            // Draw the 'Space Invaders' title
            DrawText(g, "Space Invaders", font, borderBrush, gameSize.Width / 2, 100, Brushes.GreenYellow);

            // Determine the game state text based on whether the player won or lost and draw it
            string gameStateText = state == Game.GameState.Won ? "You WIN !" : "Game OVER";
            DrawText(g, gameStateText, font, borderBrush, gameSize.Width / 2, gameSize.Height / 2, Brushes.White, 2);

            // Draw the prompt for the user's username
            FontStyle promptStyle = FontStyle.Regular;
            DrawText(g, "Ecrivez votre nom d'utilisateur", new Font(font.FontFamily, 24, promptStyle), borderBrush, gameSize.Width / 2, gameSize.Height / 2 + 50, Brushes.White, 2);

            // Draw the user's username if it's not null
            if (Username != null)
            {
                DrawText(g, Username, new Font(font.FontFamily, 40, promptStyle), borderBrush, gameSize.Width / 2, gameSize.Height / 2, Brushes.GreenYellow, 3);
            }
        }


        /// <summary>
        /// Draws the game screen for the 'One vs One Winner' state. This includes the background image, the game title, and the winner announcement.
        /// </summary>
        /// <param name="g">Graphics object used for drawing.</param>
        private void DrawOneVsOneWinnerState(Graphics g)
        {
            // Initialize brush and style for text drawing
            Brush borderBrush;
            FontStyle style;
            Font font;

            // Draw the game background
            g.DrawImage(gameBackground, new Rectangle(0, 0, gameSize.Width, gameSize.Height));

            // Set up the font for the title and winner text
            font = new Font(privateFonts.Families[0], 40, FontStyle.Bold);
            borderBrush = new SolidBrush(Color.Black);

            // Draw the game title
            DrawText(g, "Space Invaders", font, borderBrush, gameSize.Width / 2, 100, Brushes.GreenYellow);

            // Determine the winner and draw the winner text
            string winner = playerShipOne.IsAlive() ? "Player one WIN" : "Player two WIN";
            DrawText(g, winner, font, borderBrush, gameSize.Width / 2, 200, Brushes.Yellow);
        }
        #endregion

        #region UpdateRegion
        /// <summary>
        /// Handles the overall menu state. This includes adjusting the volume, navigating through the menu, and selecting menu items based on user input.
        /// </summary>
        /// <param name="keyPressed">Set of keys that are currently pressed.</param>
        private void HandleMenuState(HashSet<Keys> keyPressed)
        {
            AdjustVolume(keyPressed); // Handle volume adjustment
            NavigateMenu(keyPressed); // Navigate the menu
            SelectMenuItem(keyPressed); // Select menu item
        }

        /// <summary>
        /// Adjusts the game's volume based on user input. Allows increasing, decreasing, and muting the volume.
        /// </summary>
        /// <param name="keyPressed">Set of keys that are currently pressed.</param>
        private void AdjustVolume(HashSet<Keys> keyPressed)
        {
            if (keyPressed.Contains(Keys.Right))
            {
                // Increase volume
                if (volume < 1.0f)
                {
                    volumeDecalage -= 1.5f;
                }
                volume = Math.Min(1.0f, volume + 0.1f);
                myPlayer.Volume = volume;
                myPlayer2.Volume = volume;
                ReleaseKey(Keys.Right);
            }
            else if (keyPressed.Contains(Keys.Left))
            {
                // Decrease volume
                if (volume > 0.0f)
                {
                    volumeDecalage += 1.5f;
                }
                volume = Math.Max(0.0f, volume - 0.1f);
                myPlayer.Volume = volume;
                myPlayer2.Volume = volume;
                ReleaseKey(Keys.Left);
            }

            if (keyPressed.Contains(Keys.M))
            {
                // Mute volume
                volume = (volume != 0) ? 0 : volume; // Toggles mute on and off
                myPlayer.Volume = volume;
                myPlayer2.Volume = volume;
                volumeDecalage = (volume == 0) ? 5.0f : volumeDecalage; // Adjust volume indicator if muted
                ReleaseKey(Keys.M);
            }
        }


        /// <summary>
        /// Navigates through the menu items based on user input, allowing selection movement and exiting the application.
        /// </summary>
        /// <param name="keyPressed">Set of keys that are currently pressed.</param>
        private void NavigateMenu(HashSet<Keys> keyPressed)
        {
            if (keyPressed.Contains(Keys.Q))
            {
                Application.Exit();
                return;
            }
            if (keyPressed.Contains(Keys.Down))
            {
                Play2(Application.StartupPath + @"\Resources\menu_change.wav");
                selectedMenuItem = (selectedMenuItem + 1) % 3; // Index down
                ReleaseKey(Keys.Down);
            }
            else if (keyPressed.Contains(Keys.Up))
            {
                Play2(Application.StartupPath + @"\Resources\menu_change.wav");
                selectedMenuItem = (selectedMenuItem - 1 + 3) % 3; // Index up
                ReleaseKey(Keys.Up);
            }
        }

        /// <summary>
        /// Processes the selection of a menu item when the Space key is pressed. 
        /// It handles actions such as starting the game, showing the leaderboards, or entering the One vs One mode.
        /// </summary>
        /// <param name="keyPressed">Set of keys that are currently pressed.</param>
        private void SelectMenuItem(HashSet<Keys> keyPressed)
        {
            if (keyPressed.Contains(Keys.Space))
            {
                switch (selectedMenuItem)
                {
                    case 0: // Play
                            // Stop any currently playing audio and start the game audio
                        myPlayer.Stop();
                        Play(Application.StartupPath + @"\Resources\battle_8bit.wav");
                        // Change the game state to 'Play'
                        state = GameState.Play;
                        break;

                    case 1: // Leaderboards
                            // Check if there are any users in the leaderboard
                        if (UserData.Get_users(@"Resources\file.json").Count() != 0)
                        {
                            // If users exist, change the state to 'Leaderboards'
                            state = GameState.Leaderboards;
                        }
                        else
                        {
                            // If no users, play an error sound
                            Play2(Application.StartupPath + @"\Resources\error-sound-39539.wav");
                        }
                        break;

                    case 2: // One vs One
                            // Stop any currently playing audio and start the game audio for One vs One mode
                        myPlayer.Stop();
                        Play(Application.StartupPath + @"\Resources\battle_8bit.wav");
                        // Set One vs One mode to true and change the game state
                        OneVSOne = true;
                        state = GameState.OneVsOne;
                        break;
                }
                // Release the Space key to prevent repeated inputs
                ReleaseKey(Keys.Space);
            }
        }


        /// <summary>
        /// Handles the leaderboard state of the game, including navigation through the leaderboard pages and returning to the main menu.
        /// </summary>
        /// <param name="keyPressed">Set of keys that are currently pressed.</param>
        private void HandleLeaderboardsState(HashSet<Keys> keyPressed)
        {
            // Retrieve the user list and calculate the maximum number of pages
            var userList = UserData.Get_users(@"Resources\file.json").OrderByDescending(x => x.Score);
            pagemax = userList.Count() / 5 + ((userList.Count() % 5 >= 1) ? 1 : 0);

            // Navigate the leaderboard pages
            if (keyPressed.Contains(Keys.Down) || keyPressed.Contains(Keys.Right))
            {
                PlaySoundForMenuChange();
                pagelist = (pagelist + 1) % pagemax; // Move to the next page
                ReleaseKey(Keys.Down);
                ReleaseKey(Keys.Right);
            }
            else if (keyPressed.Contains(Keys.Up) || keyPressed.Contains(Keys.Left))
            {
                PlaySoundForMenuChange();
                pagelist = (pagelist - 1 + pagemax) % pagemax; // Move to the previous page
                ReleaseKey(Keys.Up);
                ReleaseKey(Keys.Left);
            }

            // Return to the main menu
            if (keyPressed.Contains(Keys.Q))
            {
                PlaySoundForMenuChange();
                state = GameState.Menu; // Change state to Menu
                ReleaseKey(Keys.Q);
            }
        }

        /// <summary>
        /// Plays a sound effect for menu navigation changes.
        /// </summary>
        private void PlaySoundForMenuChange()
        {
            Play2(Application.StartupPath + @"\Resources\menu_change.wav");
        }


        /// <summary>
        /// Handles the logic for the 'One vs One Winner' state. This includes stopping the timer, 
        /// resetting missile states, and handling the transition back to the menu upon pressing the Enter key.
        /// </summary>
        /// <param name="keyPressed">Set of keys that are currently pressed.</param>
        private void HandleOneVsOneWinnerState(HashSet<Keys> keyPressed)
        {
            // Stop the game timer and reset missile states
            timer.Stop();
            Missile.missileAliveOne = 0;
            Missile.missileAliveTwo = 0;
            OneVSOne = false; // Indicate that the One vs One mode has ended

            // Check if the Enter key is pressed
            if (keyPressed.Contains(Keys.Enter))
            {
                // Transition back to the main menu
                state = GameState.Menu;

                // Remove all game objects that are still alive
                gameObjects.RemoveWhere(gameObject => gameObject.IsAlive());

                // Play the menu soundtrack
                Play(Application.StartupPath + @"\Resources\Royale_8bit_menu.wav");

                // Indicate that the game has ended
                gameStart = false;

                // Release the Enter key to prevent repeated inputs
                ReleaseKey(Keys.Enter);
            }
        }


        /// <summary>
        /// Processes user key inputs for typing operations, such as entering a username. 
        /// It handles letter keys for character input and the backspace key for deletion.
        /// </summary>
        /// <param name="keyPressed">Set of keys that are currently pressed.</param>
        private void UserKey(HashSet<Keys> keyPressed)
        {
            // A list to keep track of keys that need to be released
            List<Keys> keysList = new List<Keys>();

            foreach (var key in keyPressed)
            {
                if (IsLetterKey(key))
                {
                    // Convert the key to a character and append it to the username
                    string keyChar = key.ToString();
                    Username += keyChar;
                    keysList.Add(key);
                }
                else if (key == Keys.Back && Username.Length > 0)
                {
                    // Remove the last character from the username if the backspace key is pressed
                    Username = Username.Remove(Username.Length - 1);
                    keysList.Add(key);
                }
            }

            // Release all processed keys
            foreach (var key in keysList)
            {
                ReleaseKey(key);
            }

            // Clear the list of keys to be released
            keysList.Clear();
        }


        /// <summary>
        /// Handles the end game state logic, including playing the game over sound, saving user data, and managing key inputs for transitioning states.
        /// </summary>
        /// <param name="keyPressed">Set of keys that are currently pressed.</param>
        private void HandleEndGameState(HashSet<Keys> keyPressed)
        {
            // Play the game over sound once if it hasn't been played already
            if (!playedMedia1)
            {
                myPlayer.Stop();
                Play(Application.StartupPath + @"\Resources\game_over.wav");
                playedMedia1 = true;
            }

            // Remove all game objects that are still alive
            gameObjects.RemoveWhere(gameObject => gameObject.IsAlive());

            // If the game has started and the Space key is pressed with a non-empty username
            if (gameStart && keyPressed.Contains(Keys.Space) && Username != "")
            {
                gameStart = false; // Indicate that the game has ended
                                   // Create and save a new user data record
                var newUser = new UserData { UserName = Username, Score = Score };
                newUser.AddUserData(@"Resources\file.json");

                // Reset the media play flag and transition back to the menu
                playedMedia1 = false;
                if (keyPressed.Contains(Keys.Space))
                {
                    myPlayer.Stop();
                    Play(Application.StartupPath + @"\Resources\Royale_8bit_menu.wav");
                    state = GameState.Menu;
                    ReleaseKey(Keys.Space);
                }
            }

            // Handle additional user key inputs
            UserKey(keyPressed);
        }

        /// <summary>
        /// Handles the behavior of Bonus game objects. 
        /// If a Bonus object is touched, it is repositioned randomly on the screen and its type is changed.
        /// </summary>
        /// <param name="gameObject">The game object to be handled, expected to be of type Bonus.</param>
        private void HandleBonusObject(GameObject gameObject)
        {
            if (gameObject is Bonus bonus && bonus.touche)
            {
                // Generate a new random position for the bonus
                Random random = new Random();
                double x = random.Next(0, gameSize.Width - 20);
                double y = random.Next(0, gameSize.Height - 20);

                // Set the new position and change the bonus type
                bonus.Position.X = x;
                bonus.Position.Y = y;
                bonus.ChangeType();

                // Reset the touched flag
                bonus.touche = false;
            }
        }


        /// <summary>
        /// Handles the behavior of EnemyBlock game objects.
        /// Updates the game's score based on the EnemyBlock's score and triggers the addition of a new wave if needed.
        /// </summary>
        /// <param name="gameObject">The game object to be handled, expected to be of type EnemyBlock.</param>
        private void HandleEnemyBlockObject(GameObject gameObject)
        {
            if (gameObject is EnemyBlock enemyBlock)
            {
                // Update the game score from the enemy block
                Score = enemyBlock.Score;

                // Check and add a new wave if there are no more enemies
                if (!enemyBlock.Wave) AddWave();
            }
        }


        /// <summary>
        /// Handles general game and player actions based on key presses. This includes pausing the game and exiting to the menu.
        /// </summary>
        /// <param name="keyPressed">Set of keys that are currently pressed.</param>
        private void HandleGameAndPlayerActions(GameObject gameObject, HashSet<Keys> keyPressed)
        {
            // Toggle pause state
            if (keyPressed.Contains(Keys.P) && (state == GameState.Play || state == GameState.OneVsOne || state == GameState.Pause))
            {
                state = (state == GameState.Play || state == GameState.OneVsOne) ? GameState.Pause : (OneVSOne ? GameState.OneVsOne : GameState.Play);
                ReleaseKey(Keys.P);
            }

            // Exit to the main menu from the pause state
            if (keyPressed.Contains(Keys.Q) && state == GameState.Pause)
            {
                QuitGame = true;
                OneVSOne = false;
                timer?.Stop(); // Stop the game timer
                state = GameState.Menu; // Change state to Menu
                ReleaseKey(Keys.Q);
            }
        }

        /// <summary>
        /// Updates the state of each game object based on the current game state and key presses.
        /// This includes handling specific actions like moving and shooting for different types of game objects.
        /// </summary>
        /// <param name="gameObject">The game object to update.</param>
        /// <param name="deltaT">The time delta for updates.</param>
        /// <param name="keyPressed">Set of keys that are currently pressed.</param>
        private void UpdateGameObjectState(GameObject gameObject, double deltaT, HashSet<Keys> keyPressed)
        {
            if (state != GameState.Play && state != GameState.OneVsOne) return;
                gameObject.Update(this, deltaT);

        }
        #endregion

        #endregion
    }

}
