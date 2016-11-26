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
    public class Brick : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Private Members
        private SpriteBatch spriteBatch;
        private ContentManager contentManager;

        public enum BrickColor
        {
            RED,
            BLUE,
            YELLOW,
            GREEN,
            GREY,
            ORANGE,
            PINK,
            BLACK
        };

        // Brick sprite
        private Texture2D brickSprite;

        // Brick location
        private Vector2 brickPosition;
        private BrickColor brickColor;
        public const int BRICK_GAP = 5;

        private bool isBroken = false;

        private float widthScale;
        private Vector2 scaleVector;
        private Color[,] brickColorArray;

        private int numOfBricks;
        private int rowNumber;


        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the X position of the brick.
        /// </summary>
        public float X
        {
            get { return brickPosition.X; }
            set { brickPosition.X = value; }
        }

        /// <summary>
        /// Gets or sets the Y position of the paddle.
        /// </summary>
        public float Y
        {
            get { return brickPosition.Y; }
            set { brickPosition.Y = value; }
        }

        public bool IsBroken
        {
            get { return isBroken; }
            set { isBroken = value; }
        }

        public Vector2 ScaleVector
        {
            get { return scaleVector;  }        
        }

        public Color[,] ColorArray
        {
            get { return brickColorArray;  }
        }

        public int Height
        {
            get { return (int)(brickSprite.Height * scaleVector.Y); }
        }
        public int Width
        {
            get { return (int)(brickSprite.Width * scaleVector.X); }
        }



        /// <summary>
        /// Gets the bounding rectangle of the paddle.
        /// </summary>
        public Rectangle Boundary
        {
            get
            {
                return new Rectangle((int)brickPosition.X, (int)brickPosition.Y,
                    Width, Height);
            }
        }


        #endregion

        public Brick(Game game, BrickColor color)
            : base(game)
        {
            contentManager = new ContentManager(game.Services);
            brickColor = color;
            
        }

        public Brick(Game game, BrickColor color, int numBricks, int rowNum)
            : base(game)
        {
            contentManager = new ContentManager(game.Services);
            brickColor = color;
            numOfBricks = numBricks;
            rowNumber = rowNum;

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // Make sure base.Initialize() is called before this or handSprite will be null
            

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            switch (brickColor)
            { 
                case BrickColor.RED:
                    brickSprite = contentManager.Load<Texture2D>(@"Content\brick_red");
                    break;
                case BrickColor.BLUE:
                    brickSprite = contentManager.Load<Texture2D>(@"Content\brick_blue");
                    break;
                case BrickColor.YELLOW:
                    brickSprite = contentManager.Load<Texture2D>(@"Content\brick_yellow");
                    break;
                case BrickColor.GREEN:
                    brickSprite = contentManager.Load<Texture2D>(@"Content\brick_green");
                    break;
            }

            brickColorArray = Collisions.TextureTo2DArray(brickSprite);

            widthScale = ((float)Game1.graphics.PreferredBackBufferWidth / (float)numOfBricks) / (float)brickSprite.Width;
            scaleVector = new Vector2((widthScale * 0.8f), 0.2f);

            int yPos = (BRICK_GAP * (rowNumber + 1) + ((int)Height * rowNumber));
                Y = yPos;

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {          
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            
            if (!isBroken)
            {
                
                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                spriteBatch.Draw(brickSprite, new Vector2(X, Y), null, Color.White, 0f, Vector2.Zero, scaleVector, SpriteEffects.None, 1);
                spriteBatch.End();

                base.Draw(gameTime);
            }
            
        }
    }
}
