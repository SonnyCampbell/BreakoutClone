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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Ball : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Private Members
        private SpriteBatch spriteBatch;
        private ContentManager contentManager;

        // Default speed of ball
        private const float DEFAULT_X_SPEED = 150;
        private const float DEFAULT_Y_SPEED = 150;

        // Initial location of the ball
        private int initialXPos = 20;
        private int initialYPos = 0;

        private const float RESIZE_PERCENT = 0.2f;

        // Increase in speed each hit
        private const float INCREASE_SPEED = 15;

        // Ball image
        private Texture2D ballSprite;

        Color[,] ballColorArray;

        // Ball location
        Vector2 ballPosition;

        // Ball's motion
        Vector2 ballSpeed = new Vector2(DEFAULT_X_SPEED, DEFAULT_Y_SPEED);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the ball's horizontal speed.
        /// </summary>
        public float SpeedX
        {
            get { return ballSpeed.X; }
            set { ballSpeed.X = value; }
        }

        /// <summary>
        /// Gets or sets the ball's vertical speed.
        /// </summary>
        public float SpeedY
        {
            get { return ballSpeed.Y; }
            set { ballSpeed.Y = value; }
        }

        /// <summary>
        /// Gets or sets the X position of the ball.
        /// </summary>
        public float X
        {
            get { return ballPosition.X; }
            set { ballPosition.X = value; }
        }

        /// <summary>
        /// Gets or sets the Y position of the ball.
        /// </summary>
        public float Y
        {
            get { return ballPosition.Y; }
            set { ballPosition.Y = value; }
        }

        public float MidY
        {
            get { return ballPosition.Y + (float)Height / 2.0f; }
        }

        /// <summary>
        /// Gets the width of the ball's sprite.
        /// </summary>
        public int Width
        {
            get { return (int)(ballSprite.Width * RESIZE_PERCENT); }
        }

        /// <summary>
        /// Gets the height of the ball's sprite.
        /// </summary>
        public int Height
        {
            get { return (int)(ballSprite.Height * RESIZE_PERCENT); }
        }

        public Rectangle Boundary
        {
            get
            {
                return new Rectangle((int)ballPosition.X, (int)ballPosition.Y,
                    Width, Height);
            }
        }


        public float Scale
        {
            get { return RESIZE_PERCENT; }
        }

        public Color[,] ColorArray
        {
            get { return ballColorArray; }
        }

        public int InitialYPos
        {
            get { return initialYPos; }
            set { initialYPos = value; }
        }

        #endregion

        public Ball(Game game)
            : base(game)
        {
            contentManager = new ContentManager(game.Services);

        }

        /// <summary>
        /// Set the ball at the top of the screen with default speed.
        /// </summary>
        public void Reset()
        {
            ballSpeed.X = DEFAULT_X_SPEED;
            ballSpeed.Y = DEFAULT_Y_SPEED;

            ballPosition.Y = initialYPos;

            // Make sure ball is not positioned off the screen
            if (ballPosition.X < 0)
                ballPosition.X = 0;
            else if (ballPosition.X + Width > GraphicsDevice.Viewport.Width)
            {
                ballPosition.X = GraphicsDevice.Viewport.Width - Width;
                ballSpeed.Y *= -1;
            }
        }

        /// <summary>
        /// Increase the ball's speed in the X and Y directions.
        /// </summary>
        public void SpeedUp()
        {
            if (ballSpeed.Y < 0)
                ballSpeed.Y -= INCREASE_SPEED;
            else
                ballSpeed.Y += INCREASE_SPEED;

            if (ballSpeed.X < 0)
                ballSpeed.X -= INCREASE_SPEED;
            else
                ballSpeed.X += INCREASE_SPEED;
        }

        /// <summary>
        /// Invert the ball's horizontal direction
        /// </summary>
        public void ChangeHorzDirection()
        {
            ballSpeed.X *= -1;
        }

        /// <summary>
        /// Invert the ball's vertical direction
        /// </summary>
        public void ChangeVertDirection()
        {
            ballSpeed.Y *= -1;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the texture if it exists
            ballSprite = contentManager.Load<Texture2D>(@"Content\ball_black");

            ballColorArray = Collisions.TextureTo2DArray(ballSprite);
           
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Move the sprite by speed, scaled by elapsed time.
            ballPosition += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            spriteBatch.Draw(ballSprite, ballPosition, null, Color.White, 0f, Vector2.Zero, RESIZE_PERCENT, SpriteEffects.None, 1);
            spriteBatch.End();
        }
    }
}
