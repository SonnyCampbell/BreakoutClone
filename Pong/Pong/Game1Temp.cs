/*
 * Basketball Pong
 * by Frank McCown, Harding University
 * Spring 2012
 * 
 * Sounds: Creative Commons Sampling Plus 1.0 License.
 * http://www.freesound.org/samplesViewSingle.php?id=34201
 * http://www.freesound.org/samplesViewSingle.php?id=12658
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphics;
        private int screenWidth;
        private int screenHeight;

        

        private Ball ball;
        private Paddle paddle;
        public const int NUM_OF_ROWS = 4;
        private Brick[] bricks;
        private Brick[,] brickRows;
        private const int numOfBricks = 12;
       

        public static SoundEffect swishSound;
        public static SoundEffect crashSound;

        

        // Used to delay between rounds 
        private float delayTimer = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;

            Content.RootDirectory = "Content";

            

            SetUpBricks();

            ball = new Ball(this);
            paddle = new Paddle(this);

            Components.Add(ball);
            Components.Add(paddle);
            

            // Call Window_ClientSizeChanged when screen size is changed
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            // Move paddle back onto screen if it's off
            paddle.Y = GraphicsDevice.Viewport.Height - paddle.Height;
            if (paddle.X + paddle.Width > GraphicsDevice.Viewport.Width)
                paddle.X = GraphicsDevice.Viewport.Width - paddle.Width;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Make mouse visible
            IsMouseVisible = true;           

            
            

            // Set the window's title bar
            Window.Title = "Sonny's Pongy Game!";
            graphics.ApplyChanges();

            // Don't allow ball to move just yet
            ball.Enabled = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            

            swishSound = Content.Load<SoundEffect>(@"swish");
            crashSound = Content.Load<SoundEffect>(@"crash");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // Press F to toggle full-screen mode
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }

            if (delayTimer == 0)
            {
                ball.Y = 300;
                ball.InitialYPos = (int)ball.Y;
            }                
               
            // Wait until a second has passed before animating ball 
            delayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (delayTimer > 1)              
                ball.Enabled = true;

            CheckCollisions();
           

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            base.Draw(gameTime);
        }

        private void CheckCollisions()
        {
            if (Collisions.CheckBallScreenCollision(ball, ref delayTimer))
                return;

            // Collision?  Check rectangle intersection between ball and hand
            if (ball.Boundary.Intersects(paddle.Boundary))
            {
                Vector2 paddleColPoint = Collisions.CheckBallPaddleCollision(ball, paddle);
                if (paddleColPoint.X > -1 && ball.SpeedY > 0)
                {
                    //swishSound.Play();
                    Collisions.BallPaddleCollision(ball, paddle);
                    return;
                }
            }
            

            Vector2 brickColPoint;
            Brick theBrick;
            Collisions.CheckBallBrickCollision(ball, brickRows, out brickColPoint, out theBrick);
            if (brickColPoint.X > -1)
            {
                //swishSound.Play();
                Collisions.BallBrickCollision(ball, brickColPoint, theBrick);
                return;
            }
        }


        private void SetUpBricks()
        {
            brickRows = new Brick[NUM_OF_ROWS,numOfBricks];
            for (int j = 0; j < NUM_OF_ROWS; j++)
            {
                bricks = new Brick[numOfBricks];
                for (int i = 0; i < numOfBricks; i++)
                {
                    bricks[i] = new Brick(this, (Brick.BrickColor)j, numOfBricks, j);
                    bricks[i].X = (graphics.PreferredBackBufferWidth / (numOfBricks + 1) * (i + 0.6f));
                    bricks[i].Y = Brick.BRICK_GAP;
                    brickRows[j, i] = bricks[i];

                    Components.Add(bricks[i]);

                }

                
            }

            
        }

        


        
    }
}
